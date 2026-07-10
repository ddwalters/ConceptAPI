using ConceptAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConceptAPI.Controllers;

[ApiController]
[Route("api/concepts")]
public class ConceptController : ControllerBase
{
    private readonly ConceptContext _context;

    public ConceptController(ConceptContext context)
    {
        _context = context;
    }

    [HttpGet("{Id}")]
    public async Task<ActionResult<Concept>> GetConceptById(int id)
    {
        var concept = await _context.Concepts.FindAsync(id);

        if (concept == null)
            return NotFound(new { message = $"Concept with Id-{id} not found." });

        return Ok(concept);
    }
}