using ConceptAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace ConceptAPI.Controllers;

public class GachaPullRequest
{
    public int Count { get; set; } = 1;
}

[Authorize]
[ApiController]
[Route("api/conceptownerships")]
public class ConceptOwnershipController : ControllerBase
{
    private const int GachaPullCost = 5;

    private readonly ConceptContext _context;

    public ConceptOwnershipController(ConceptContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<ConceptOwnership>>> GetOwnedConcepts()
    {
        var userId = GetCurrentUserId();

        var owned = await _context.ConceptOwnerships
            .Where(x => x.UserId == userId)
            .ToListAsync();

        return Ok(owned);
    }

    [HttpGet("{Id}")]
    public async Task<ActionResult<ConceptOwnership>> GetOwnedConceptById(int id)
    {
        var userId = GetCurrentUserId();

        var owned = await _context.ConceptOwnerships.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (owned == null)
            return NotFound(new { message = $"ConceptOwnership with Id-{id} not owned by user." });

        return Ok(owned);
    }

    [HttpPost("gacha")]
    public async Task<ActionResult<List<ConceptOwnership>>> GachaPull(GachaPullRequest request)
    {
        var userId = GetCurrentUserId();

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return Unauthorized();

        if (user.Shards < GachaPullCost)
            return BadRequest(new { message = $"Not enough shards. Need {GachaPullCost}, have {user.Shards}." });

        var conceptsAvailable = await _context.Concepts.ToListAsync();
        if (conceptsAvailable.Count == 0)
            return StatusCode(500, new { message = "No concepts exist to pull from." });

        user.Shards -= GachaPullCost;

        var results = new List<ConceptOwnership>();
        for (int i = 0; i < 10; i++)
        {
            var conceptIndex = Random.Shared.Next(conceptsAvailable.Count);
            var concept = conceptsAvailable[conceptIndex];

            var ownership = new ConceptOwnership
            {
                ConceptId = concept.Id,
                UserId = userId,
                Level = 1,
                Rank = 1,
                Experiance = 0
            };

            _context.ConceptOwnerships.Add(ownership);
            results.Add(ownership);
        }

        await _context.SaveChangesAsync();

        return Ok(results);
    }

    private int GetCurrentUserId()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        return int.Parse(sub!);
    }
}