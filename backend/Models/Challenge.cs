namespace Backend.Models;

public class Challenge
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Category { get; set; }
    public required int Points { get; set; }
    public DateOnly? Date { get; set; }
    public required string Description { get; set; }
    public List<ChallengeHint> Hints { get; set; } = new();
    public required string Flag { get; set; }
}