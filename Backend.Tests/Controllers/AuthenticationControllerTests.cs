using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Backend.Controllers;
using Backend.Data;
using Backend.Validators;
using Backend.Models.Dto;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
            {"ValidationSettings:Register:MaxPasswordLength", "4"},
            {"Jwt:Issuer", "TestIssuer"},
            {"Jwt:Audience", "TestAudience"},
            {"Jwt:Key", "eowuxZagltF68p9HdUqF2w4egNZ8AZtmsMdwxWiSZy4"},
        };
        IConfiguration mockConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        var registerValidator = new RegisterRequestDtoValidator(mockConfig);
        var loginValidator = new LoginRequestDtoValidator(); 
        var controller = new AuthenticationController(context, registerValidator, loginValidator, mockConfig);

        return (controller, context);
    }

    private JwtSecurityToken _AssertOkAndJwt(int userId, string username, ActionResult requestResult)
    {
        var okResult = requestResult.Should().BeOfType<OkObjectResult>().Which;
        var dto = okResult.Value.Should().NotBeNull().And.BeOfType<LoginResponseDto>().Which;
        string tokenString = dto.Token;
        tokenString.Should().NotBeNullOrEmpty();

        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        handler.CanReadToken(tokenString).Should().BeTrue();
        var decodedJwt = handler.ReadJwtToken(tokenString);
        decodedJwt.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == userId.ToString());
        decodedJwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == username.ToString());
        decodedJwt.ValidTo.Should().BeAfter(DateTime.UtcNow);

        return decodedJwt;
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
    public async Task Register_ValidDto_Created()
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
        result.Should().BeOfType<CreatedResult>();

        User? user = await context.Users
            .Where(u => u.Username == request.Username)
            .FirstOrDefaultAsync();
        user.Should().NotBeNull();
        user.Username.Should().Be(request.Username);
        user.PasswordHash.Should().NotBeNullOrEmpty().And.NotBe(request.Password);
        user.IsAdmin.Should().BeFalse();
    }

    [Fact]
    public async Task Register_ValidDtoAdmin_Ok()
    {
        // Arrange
        var (controller, context) = _Arrange();
        RegisterRequestDto request = new RegisterRequestDto
        {
            Username = "aaa",
            Password = "aaa",
            IsAdmin = true
        };

        // Act
        var result = await controller.Register(request);

        // Assert
        result.Should().BeOfType<CreatedResult>();

        User? user = await context.Users
            .Where(u => u.Username == request.Username)
            .FirstOrDefaultAsync();
        user.Should().NotBeNull();
        user.Username.Should().Be(request.Username);
        user.PasswordHash.Should().NotBeNullOrEmpty().And.NotBe(request.Password);
        user.IsAdmin.Should().BeTrue();
    }
    #endregion

    #region Login
    [Fact]
    public async Task Login_EmptyUsername_BadRequest()
    {
        // Arrange
        var (controller, _) = _Arrange();
        LoginRequestDto request = new LoginRequestDto
        {
            Username = "",
            Password = "aaa"
        };

        // Act
        var result = await controller.Login(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Login_EmptyPassword_BadRequest()
    {
        // Arrange
        var (controller, _) = _Arrange();
        LoginRequestDto request = new LoginRequestDto
        {
            Username = "aaa",
            Password = ""
        };

        // Act
        var result = await controller.Login(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Login_ValidDtoInvalidPassword_Unauthorized()
    {
        // Arrange
        var (controller, context) = _Arrange();
        string username = "aaa", password = "aaa";
        LoginRequestDto request = new LoginRequestDto
        {
            Username = username,
            Password = "bbb"
        };

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        await context.AddAsync(new User
        {
            Username = username,
            PasswordHash = passwordHash
        });
        await context.SaveChangesAsync();

        // Act
        var result = await controller.Login(request);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_ValidDtoInvalidUsername_Unauthorized()
    {
        // Arrange
        var (controller, _) = _Arrange();
        LoginRequestDto request = new LoginRequestDto
        {
            Username = "aaa",
            Password = "aaa"
        };

        // Act
        var result = await controller.Login(request);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_ValidDto_OkJwt()
    {
        // Arrange
        var (controller, context) = _Arrange();
        int id = 1; string username = "aaa", password = "aaa";
        LoginRequestDto request = new LoginRequestDto
        {
            Username = username,
            Password = password
        };

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        await context.AddAsync(new User
        {
            Id = id,
            Username = username,
            PasswordHash = passwordHash
        });
        await context.SaveChangesAsync();

        // Act
        var result = await controller.Login(request);

        // Assert
        var decodedJwt = _AssertOkAndJwt(id, username, result);
        decodedJwt.Claims.Should().NotContain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
    }

    [Fact]
    public async Task Login_ValidDtoAdmin_OkJwt()
    {
        // Arrange
        var (controller, context) = _Arrange();
        int id = 1; string username = "aaa", password = "aaa";
        LoginRequestDto request = new LoginRequestDto
        {
            Username = username,
            Password = password
        };

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        await context.AddAsync(new User
        {
            Id = id,
            Username = username,
            PasswordHash = passwordHash,
            IsAdmin = true
        });
        await context.SaveChangesAsync();

        // Act
        var result = await controller.Login(request);

        // Assert
        var decodedJwt = _AssertOkAndJwt(id, username, result);
        decodedJwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
    }
    #endregion
}