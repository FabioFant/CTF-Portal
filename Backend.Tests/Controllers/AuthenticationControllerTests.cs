using Microsoft.EntityFrameworkCore;
using Backend.Controllers;
using Backend.Data;
using Backend.Validators;
using Backend.Models.Dto;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Tests.Helper;

namespace Backend.Tests.Controllers;

public class AuthenticationControllerTests : AuthTestBase
{
    #region Private methods
    private (AuthenticationController, BackendContext) _Arrange()
    {
        BackendContext context = GetContext();

        var registerValidator = new RegisterRequestDtoValidator(MockConfig);
        var loginValidator = new LoginRequestDtoValidator(); 
        var controller = new AuthenticationController(
            context,
            registerValidator, 
            loginValidator, 
            MockConfig
        );

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
    private async Task<User> _AssertCreatedAndUser(ActionResult result, RegisterRequestDto request, BackendContext context)
    {
        result.Should().BeOfType<CreatedResult>();

        User? user = await context.Users
            .Where(u => u.Username == request.Username)
            .FirstOrDefaultAsync();
        user.Should().NotBeNull();
        user.Username.Should().Be(request.Username);
        user.PasswordHash.Should().NotBeNullOrEmpty().And.NotBe(request.Password);
        return user;
    }
    #endregion

    #region Register
    [Fact]
    public async Task Register_InvalidDto_BadRequest()
    {
        // Arrange
        var (controller, _) = _Arrange();
        var request = GetValidRegisterRequestDto();
        request.Username = "";

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
        var request = GetValidRegisterRequestDto();

        // Act
        var result = await controller.Register(request);

        // Assert
        User user = await _AssertCreatedAndUser(result, request, context);
        user.IsAdmin.Should().BeFalse();
    }

    [Fact]
    public async Task Register_ValidDtoAdmin_Ok()
    {
        // Arrange
        var (controller, context) = _Arrange();
        var request = GetValidRegisterRequestDto();
        request.IsAdmin = true;

        // Act
        var result = await controller.Register(request);

        // Assert
        User user = await _AssertCreatedAndUser(result, request, context);
        user.IsAdmin.Should().BeTrue();
    }
    #endregion

    #region Login
    [Fact]
    public async Task Login_InvalidDto_BadRequest()
    {
        // Arrange
        var (controller, _) = _Arrange();
        var request = GetValidLoginRequestDto();
        request.Username = "";

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
        var request = GetValidLoginRequestDto();
        string username = request.Username, password = request.Password;
        request.Password = "bbb";

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
    public async Task Login_UserDoesNotExist_ReturnsUnauthorized()
    {
        // Arrange
        var (controller, _) = _Arrange();
        var request = GetValidLoginRequestDto();

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
        var request = GetValidLoginRequestDto();
        int id = 1; string username = request.Username, password = request.Password;

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
        var request = GetValidLoginRequestDto();
        int id = 1; string username = request.Username, password = request.Password;

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