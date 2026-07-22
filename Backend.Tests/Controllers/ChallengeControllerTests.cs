using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Backend.Data;
using Backend.Controllers;
using Backend.Models;

namespace Backend.Tests.Controllers;

public class ChallengeControllerTests
{
    #region Private Methods
    private (ChallengeController, BackendContext) _Arrange()
    {
        var options = new DbContextOptionsBuilder<BackendContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        BackendContext context = new BackendContext(options);
        var controller = new ChallengeController(context);  
        return (controller, context);
    }
    private T _AssertAndExtract<T>(ActionResult<T> result)
    {
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        okResult.Value.Should().BeOfType<T>().And.NotBeNull();
        return (T)okResult.Value!;
    }
    private Challenge _GetPartialChallenge()
    {
        return new Challenge 
            { 
                Id = 1, Title = "Challenge", 
                Category = "web", 
                Points = 250, 
                Description = "Description", 
                Flag = "flag{}", 
            };
    }
    private Challenge _GetFullChallenge()
    {
        ChallengeHint hint = new ChallengeHint {Id = 1, Content = "Hint", ChallengeId = 1};
        return new Challenge 
            { 
                Id = 1, Title = "Challenge", 
                Category = "web", 
                Points = 250, 
                Description = "Description", 
                Flag = "flag{}", 
                Date = DateOnly.FromDateTime(DateTime.Today),
                Hints = new List<ChallengeHint> { hint }
            };
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

        Challenge chall = _GetFullChallenge();
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

        Challenge chall = _GetPartialChallenge();
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

        Challenge chall = _GetFullChallenge();
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

        Challenge chall = _GetFullChallenge();
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

        Challenge chall = _GetPartialChallenge();
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
}
