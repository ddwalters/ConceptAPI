using ConceptAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FirebaseAdmin.Auth;
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
        FirebaseToken decodedToken;
        try
        {
            decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.IdToken);
        }
        catch (FirebaseAuthException)
        {
            return Unauthorized(new { message = "Invalid Firebase token." });
        }

        var firebaseUid = decodedToken.Uid;
        var email = decodedToken.Claims.TryGetValue("email", out var emailClaim) ? emailClaim?.ToString() : null;
        var name = decodedToken.Claims.TryGetValue("name", out var nameClaim) ? nameClaim?.ToString() : null;

        var user = await _context.Users.FirstOrDefaultAsync(x => x.FirebaseUid == firebaseUid);

        if (user == null)
        {
            user = new User
            {
                FirebaseUid = firebaseUid,
                Email = email ?? string.Empty,
                DisplayName = name ?? string.Empty,
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
