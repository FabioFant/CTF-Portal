namespace Backend.Models;

public class Challenge
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Category { get; set; }
    public int Points { get; set; }
    public DateOnly? Date { get; set; }
    public required string Description { get; set; }
    public List<ChallengeHint>? Hint { get; set; }
    public required string Flag { get; set; }
}