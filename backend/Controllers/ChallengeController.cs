using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Data;
using Backend.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChallengeController : ControllerBase
{
    private readonly BackendContext _context;
    public ChallengeController(BackendContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<ChallengeCardDto>>> GetChallenges()
    {
        List<ChallengeCardDto> cards = await _context.Challenges
            .Select(challenge => new ChallengeCardDto
            {
                Id = challenge.Id,
                Title = challenge.Title,
                Category = challenge.Category,
                Points = challenge.Points,
                Date = challenge.Date
            })
            .ToListAsync();

        return Ok(cards);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ChallengeDetailsDto>> GetChallenge([FromRoute] int id)
    {
        ChallengeDetailsDto? details = await _context.Challenges
        .Select(challenge => new ChallengeDetailsDto
        {
            Id = challenge.Id,
            Title = challenge.Title,
            Category = challenge.Category,
            Points = challenge.Points,
            Date = challenge.Date,
            Description = challenge.Description,
            Hints = challenge.Hints.Select(hint => new ChallengeHintDto { Id = hint.Id, Content = hint.Content }).ToList(),
        })
        .Where(challenge => challenge.Id == id)
        .FirstOrDefaultAsync();

        if(details == null)
        {
            return NotFound();
        }
        
        return Ok(details);
    }
}