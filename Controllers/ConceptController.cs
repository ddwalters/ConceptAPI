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

    [HttpPost()]
    public async Task<ActionResult<Concept>> PostConcept(Concept concept)
    {
        _context.Concepts.Add(concept);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(concept), new { Id = concept.Id }, concept);

    }

    [HttpDelete("{Id}")]
    public async Task<ActionResult<Concept>> DeleteConcept(int id)
    {
        var concept = await _context.Concepts.FindAsync(id);

        if (concept == null)
            return NotFound(new { message = $"Concept with Id-{id} not found." });

        _context.Concepts.Remove(concept);
        await _context.SaveChangesAsync();

        return NoContent();

    }
}