namespace Backend.Models.Dto;

public class RegisterRequestDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}