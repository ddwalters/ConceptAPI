using ConceptAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Google.Apis.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ConceptAPI.Controllers;

public class GoogleAuthRequest
{
    public string IdToken { get; set; } = null!;
}

[ApiController]
[Route("/api/auth/google")]
public class AuthController : ControllerBase
{
    private readonly ConceptContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ConceptContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost()]
    public async Task<ActionResult<object>> Authenticate(GoogleAuthRequest request)
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            // TODO: @DW set Google:ClientId (from Google Cloud Console OAuth credentials) in config
            // before this actually verifies the token came from *your* app.
            var settings = new GoogleJsonWebSignature.ValidationSettings{ Audience = new[] { _configuration["Google:ClientId"] } };
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
        }
        catch (InvalidJwtException)
        {
            return Unauthorized(new { message = "Invalid Google token." });
        }

        var user = await _context.Users.FirstOrDefaultAsync(x => x.GoogleId == payload.Subject);

        if (user == null)
        {
            user = new User
            {
                GoogleId = payload.Subject,
                Email = payload.Email,
                DisplayName = payload.Name,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        return Ok(new { token = CreateToken(user) });
    }

    private string CreateToken(User user)
    {
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Convert.FromBase64String(key)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: "capi.hiileike.com",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
