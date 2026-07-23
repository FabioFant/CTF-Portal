using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FluentAssertions;
using Backend.Data;
using Backend.Controllers;
using Backend.Models;
using Backend.Models.Dto;

namespace Backend.Tests.Controllers;

public class ChallengeControllerTests
{
    #region Private Methods
    private (ChallengeController, BackendContext) _Arrange(bool isLogged = false, bool isAdmin = false)
    {
        var options = new DbContextOptionsBuilder<BackendContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        BackendContext context = new BackendContext(options);
        var controller = new ChallengeController(context);  

        if (isLogged)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "UtenteTest")
            };

            if (isAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var mockUser = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = mockUser }
            };
        }

        return (controller, context);
    }
    private T _AssertAndExtract<T>(ActionResult<T> result)
    {
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        okResult.Value.Should().BeOfType<T>().And.NotBeNull();
        return (T)okResult.Value!;
    }
    private async Task<Challenge> _AssertCreatedAndEquivalence(
        AddChallengeRequestDto request, 
        ActionResult result, 
        BackendContext context)
    {
        result.Should().BeOfType<CreatedResult>();
        Challenge? challenge = await context.Challenges
            .Where(c => c.Title == request.Title)
            .FirstOrDefaultAsync();

        challenge.Should().NotBeNull();
        request.Should().BeEquivalentTo(challenge, options => options.ExcludingMissingMembers());
        return challenge;
    }
    private Challenge _GetChallenge(bool isFull = true)
    {
        var challenge = new Challenge 
        { 
            Id = 1, 
            Title = "Challenge", 
            Category = "web", 
            Points = 250, 
            Description = "Description", 
            Flag = "flag{}" 
        };

        if (isFull)
        {
            challenge.Date = DateOnly.FromDateTime(DateTime.Today);
            challenge.Hints = new List<ChallengeHint> 
            { 
                new ChallengeHint { Id = 1, Content = "Hint", ChallengeId = 1 } 
            };
        }

        return challenge;
    }
    private AddChallengeRequestDto _GetAddChallengeRequestDto(bool isFull = true)
    {
        var request = new AddChallengeRequestDto
        {
            Title = "Challenge",
            Category = "web",
            Points = 250,
            Description = "Description",
            Flag = "flag{}"
        };

        if (isFull)
        {
            request.Date = DateOnly.FromDateTime(DateTime.Today);
            request.Hints = new List<AddChallengeHintRequestDto> 
            { 
                new AddChallengeHintRequestDto { Content = "Hint" } 
            };
        }

        return request;
    }
    #endregion

    #region GetChallenges
    [Fact]
    public async Task GetChallenges_EmptyDatabase_OkEmptyOutput()
    {
        // Arrange
        var (controller, _) = _Arrange();

        // Act
        var result = await controller.GetChallenges();

        // Assert
        var challenges = _AssertAndExtract(result);
        challenges.Should().BeEmpty();
    }

    [Fact]
    public async Task GetChallenges_FilledDatabase_OkFullChallenge()
    {
        // Arrange
        var (controller, context) = _Arrange();

        Challenge chall = _GetChallenge();
        context.Add(chall);
        context.SaveChanges();

        // Act
        var result = await controller.GetChallenges();

        // Assert
        var cards = _AssertAndExtract(result);
        cards.Should().NotBeEmpty();

        var card = cards.First();
        card.Should().BeEquivalentTo(chall, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetChallenges_FilledDatabase_OkPartialChallenge()
    {
        // Arrange
        var (controller, context) = _Arrange();

        Challenge chall = _GetChallenge(isFull: false);
        context.Add(chall);
        context.SaveChanges();

        // Act
        var result = await controller.GetChallenges();

        // Assert
        var cards = _AssertAndExtract(result);
        cards.Should().NotBeEmpty();

        var card = cards.First();
        card.Should().BeEquivalentTo(chall, options => options.ExcludingMissingMembers());
        card.Date.Should().BeNull();
    }
    #endregion

    #region GetChallenge
    [Fact]
    public async Task GetChallenge_FullChallenge_Ok()
    {
        // Arrange
        var (controller, context) = _Arrange();   

        Challenge chall = _GetChallenge();
        context.Add(chall);
        context.SaveChanges();

        // Act
        var result = await controller.GetChallenge(chall.Id);

        // Assert
        var details = _AssertAndExtract(result);
        details.Should().BeEquivalentTo(chall, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetChallenge_InvalidId_NotFound()
    {
        // Arrange
        var (controller, context) = _Arrange();   

        Challenge chall = _GetChallenge();
        context.Add(chall);
        context.SaveChanges();

        // Act
        var result = await controller.GetChallenge(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetChallenge_PartialChallenge_Ok()
    {
        // Arrange
        var (controller, context) = _Arrange();   

        Challenge chall = _GetChallenge(isFull: false);
        context.Add(chall);
        context.SaveChanges();

        // Act
        var result = await controller.GetChallenge(chall.Id);

        // Assert
        var details = _AssertAndExtract(result);
        details.Should().BeEquivalentTo(chall, options => options.ExcludingMissingMembers());
        details.Date.Should().BeNull();
        details.Hints.Should().NotBeNull().And.BeEmpty();
    }
    #endregion

    #region AddChallenge
    /* // Authorization already guranteed by the framework --> [Authorize(Roles="Admin")]
    [Fact]
    public async Task AddChallenge_NotAdmin_Unathorized(){ }
    */

    [Fact]
    public async Task AddChallenge_FullChallenge_Created()
    {
        // Arrange
        var (controller, context) = _Arrange(isLogged: true, isAdmin: true);
        var hint = new AddChallengeHintRequestDto { Content = "hint" };
        var challengeRequest = _GetAddChallengeRequestDto();

        // Act
        var result = await controller.AddChallenge(challengeRequest);

        // Assert
        await _AssertCreatedAndEquivalence(challengeRequest, result, context);
    }

    [Fact]
    public async Task AddChallenge_PartialChallenge_Created()
    {
        // Arrange
        var (controller, context) = _Arrange(isLogged: true, isAdmin: true);
        var challengeRequest = _GetAddChallengeRequestDto(isFull: false);

        // Act
        var result = await controller.AddChallenge(challengeRequest);

        // Assert
        var challenge = await _AssertCreatedAndEquivalence(challengeRequest, result, context);
        challenge.Date.Should().BeNull();
        challenge.Hints.Should().NotBeNull().And.BeEmpty();
    }
    #endregion
}
