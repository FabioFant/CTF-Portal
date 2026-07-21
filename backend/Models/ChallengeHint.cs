namespace Backend.Models;

public class ChallengeHint
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public int ChallengeId { get; set; } 
}