using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Backend.Data;
using Backend.Models;
using Backend.Models.Dto;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly BackendContext _context;
    private readonly IValidator<RegisterRequestDto> _registerValidator;
    private readonly IValidator<LoginRequestDto> _loginValidator;
    private readonly IConfiguration _config;
    public AuthenticationController(
        BackendContext context, 
        IValidator<RegisterRequestDto> registerValidator, 
        IValidator<LoginRequestDto> loginValidator,
        IConfiguration config)
    {
        _context = context;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _config = config;
    }

    private string _GenerateJwtToken(User user) // There should be a external service for this
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
        };
        if(user.IsAdmin)
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterRequestDto request)
    {
        var validationResult = _registerValidator.Validate(request);
        if(!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        await _context.AddAsync(entity: new User
        {
            Username = request.Username,
            PasswordHash = passwordHash,
            IsAdmin = request.IsAdmin ?? false
        });
        await _context.SaveChangesAsync();

        return Created();
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequestDto request)
    {
        var validationResult = _loginValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        User? user = await _context.Users
        .Where(u => u.Username == request.Username)
        .FirstOrDefaultAsync();

        if(user == null) return Unauthorized("Invalid credentials");
        if(!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) return Unauthorized("Invalid credentials");

        string token = _GenerateJwtToken(user);
        var response = new LoginResponseDto { Token = token };
        return Ok(response);
    }
}

