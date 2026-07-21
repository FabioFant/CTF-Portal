using Microsoft.AspNetCore.Mvc;
using Backend.Models.Dto;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChallengeController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<List<ChallengeCardDto>> GetChallenges()
    {
        return null;
    }
}