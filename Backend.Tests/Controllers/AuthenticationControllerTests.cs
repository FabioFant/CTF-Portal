using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Backend.Controllers;
using Backend.Data;
using Backend.Validators;
using Backend.Models.Dto;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
namespace Backend.Tests.Controllers;

public class AuthenticationControllerTests
{
    #region Private methods
    private (AuthenticationController, BackendContext) _Arrange()
    {
        string dbName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<BackendContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        var context = new BackendContext(options);

        var inMemorySettings = new Dictionary<string, string> {
            {"ValidationSettings:Register:MinUsernameLength", "2"},
            {"ValidationSettings:Register:MaxUsernameLength", "4"},
            {"ValidationSettings:Register:MinPasswordLength", "2"},
            {"ValidationSettings:Register:MaxPasswordLength", "4"}
        };
        IConfiguration mockConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        var registerValidator = new RegisterRequestDtoValidator(mockConfig);
        var loginValidator = new LoginRequestDtoValidator(); 
        var controller = new AuthenticationController(context, registerValidator, loginValidator);

        return (controller, context);
    }
    #endregion

    #region Register
    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("aaaaa") ]
    public async Task Register_InvalidUsername_BadRequest(string username)
    {
        // Arrange
        var (controller, _) = _Arrange();
        RegisterRequestDto request = new RegisterRequestDto
        {
            Username = username,
            Password = "aaa"
        };

        // Act
        var result = await controller.Register(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("aaaaa") ]
    public async Task Register_InvalidPassword_BadRequest(string password)
    {
        // Arrange
        var (controller, _) = _Arrange();
        RegisterRequestDto request = new RegisterRequestDto
        {
            Username = "aaa",
            Password = password
        };

        // Act
        var result = await controller.Register(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Register_ValidDto_Ok()
    {
        // Arrange
        var (controller, context) = _Arrange();
        RegisterRequestDto request = new RegisterRequestDto
        {
            Username = "aaa",
            Password = "aaa"
        };

        // Act
        var result = await controller.Register(request);

        // Assert
        result.Should().BeOfType<OkResult>();

        User? user = await context.Users
            .Where(u => u.Username == request.Username)
            .FirstOrDefaultAsync();
        user.Should().NotBeNull();
        user.Username.Should().Be(request.Username);
        user.PasswordHash.Should().NotBeNullOrEmpty().And.NotBe(request.Password);
    }
    #endregion

    [Fact]
    public async Task Login_NoValues_BadRequest()
    {
        // Arrange
        var (controller, _) = _Arrange();
        LoginRequestDto request = new LoginRequestDto
        {
            Username = null,
            Password = null
        };

        // Act
        var result = await controller.Login(request);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }
}