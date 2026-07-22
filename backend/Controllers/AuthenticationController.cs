using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Models;
using FluentValidation;
using Backend.Models.Dto;
using BCrypt;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly BackendContext _context;
    private readonly IValidator<RegisterRequestDto> _registerValidator;
    private readonly IValidator<LoginRequestDto> _loginValidator;
    public AuthenticationController(
        BackendContext context, 
        IValidator<RegisterRequestDto> registerValidator, 
        IValidator<LoginRequestDto> loginValidator)
    {
        _context = context;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterRequestDto request)
    {
        var validationResult = await _registerValidator.ValidateAsync(request);
        if(!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        await _context.AddAsync(entity: new User
        {
            Username = request.Username,
            PasswordHash = passwordHash,
        });
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequestDto request)
    {
        return BadRequest();
    }
}

