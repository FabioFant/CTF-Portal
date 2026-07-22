using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Backend.Data;
using Backend.Controllers;
using Backend.Models;

namespace Backend.Tests;

public class ChallengeControllerTests
{
    #region Private Methods
    private (ChallengeController, BackendContext) _Arrange(string databaseName)
    {
        var options = new DbContextOptionsBuilder<BackendContext>().UseInMemoryDatabase(databaseName).Options;
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
    public async Task GetChallenges_EmptyDatabase_EmptyOutput()
    {
        // Arrange
        var (controller, _) = _Arrange("GetChallenges_Empty");

        // Act
        var result = await controller.GetChallenges();

        // Assert
        var challenges = _AssertAndExtract(result);
        challenges.Should().BeEmpty();
    }

    [Fact]
    public async Task GetChallenges_FilledDatabase_FullChallenge()
    {
        // Arrange
        var (controller, context) = _Arrange("GetChallenges_Filled_Full");

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
    public async Task GetChallenges_FilledDatabase_PartialChallenge()
    {
        // Arrange
        var (controller, context) = _Arrange("GetChallenges_Filled_Partial");

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
    public async Task GetChallenge_OneFullChallenge_OneOutput()
    {
        // Arrange
        var (controller, context) = _Arrange("GetChallenge_Full_OneOutput");   

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
        var (controller, context) = _Arrange("GetChallenge_InvalidId");   

        Challenge chall = _GetFullChallenge();
        context.Add(chall);
        context.SaveChanges();

        // Act
        var result = await controller.GetChallenge(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetChallenge_OnePartialChallenge_OneOutput()
    {
        // Arrange
        var (controller, context) = _Arrange("GetChallenge_Partial_OneOutput");   

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
