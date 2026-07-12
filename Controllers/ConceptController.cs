using ConceptAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConceptAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/concepts")]
public class ConceptController : ControllerBase
{
    private readonly ConceptContext _context;

    public ConceptController(ConceptContext context)
    {
        _context = context;
    }

    [HttpPost()]
    public async Task<ActionResult<Concept>> PostConcept(Concept concept)
    {
        _context.Concepts.Add(concept);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(concept), new { concept.Id }, concept);

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