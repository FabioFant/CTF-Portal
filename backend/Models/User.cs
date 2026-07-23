namespace Backend.Models;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public List<Challenge> SolvedChallenges { get; set; } = new();
    public bool IsAdmin { get; set; } = false;
}