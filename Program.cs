using Microsoft.EntityFrameworkCore;
using ConceptAPI.Models;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<ConceptContext>(opts => opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var firebaseServiceAccountJson = builder.Configuration["Firebase:ServiceAccountJson"]
    ?? throw new InvalidOperationException("Firebase:ServiceAccountJson is not configured.");

var serviceAccountParameters = NewtonsoftJsonSerializer.Instance.Deserialize<JsonCredentialParameters>(firebaseServiceAccountJson);
var serviceAccountCredential = CredentialFactory.FromJsonParameters<ServiceAccountCredential>(serviceAccountParameters);

FirebaseApp.Create(new AppOptions
{
    Credential = serviceAccountCredential.ToGoogleCredential()
});

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "capi.hiileike.com",
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtKey)),
            ValidateLifetime = true
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ConceptContext>();
    dbContext.Database.Migrate();
}

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
