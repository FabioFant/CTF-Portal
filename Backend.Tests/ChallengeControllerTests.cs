using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Controllers;
using Backend.Models.Dto;
using Backend.Models;

namespace Backend.Tests;

public class ChallengeControllerTests
{
    private (ChallengeController, BackendContext) _Arrange(string databaseName)
    {
        var options = new DbContextOptionsBuilder<BackendContext>().UseInMemoryDatabase(databaseName).Options;
        BackendContext context = new BackendContext(options);
        var controller = new ChallengeController(context);  
        return (controller, context);
    }

    private List<ChallengeCardDto> _AssertAndExtract(ActionResult<List<ChallengeCardDto>> result)
    {
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var challenges = Assert.IsType<List<ChallengeCardDto>>(okResult.Value);
        Assert.NotNull(challenges);
        return challenges;
    }

    [Fact]
    public async Task GetChallenges_EmptyDatabase_EmptyOutput()
    {
        // Arrange
        var (controller, _) = _Arrange("GetChallenges_Empty");

        // Act
        var result = await controller.GetChallenges();

        // Assert
        var challenges = _AssertAndExtract(result);
        Assert.Empty(challenges);
    }

    [Fact]
    public async Task GetChallenges_FilledDatabase_FilledOutput()
    {
        // Arrange
        var (controller, context) = _Arrange("GetChallenges_Filled");

        Challenge chall = new Challenge { Id = 1, Title = "Challenge", Category = "web", Points = 250, Description = "Description", Flag = "flag{}" }; // TODO : make this global
        context.Add(chall);
        context.SaveChanges();

        // Act
        var result = await controller.GetChallenges();

        // Assert
        var cards = _AssertAndExtract(result);
        Assert.NotEmpty(cards);

        var card = cards.First();
        Assert.Equal(card.Title, chall.Title);
        Assert.Equal(card.Category, chall.Category);
        Assert.Equal(card.Id, chall.Id);
        Assert.Equal(card.Points, chall.Points);
    }

    [Fact]
    public async Task GetChallenge_OneChallenge_OneOutput()
    {
        // Arrange
        var (controller, _) = _Arrange("GetChallenge_Empty");    
        // TODO : fill db    

        // Act
        var result = await controller.GetChallenge();

        // Assert : TODO

    }
}
