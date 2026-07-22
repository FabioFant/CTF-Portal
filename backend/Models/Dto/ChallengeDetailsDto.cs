namespace Backend.Models.Dto;

public class ChallengeDetailsDto
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string Category { get; set; }
    public required int Points { get; set; }
    public DateOnly? Date { get; set; }
    public required string Description { get; set; }
    public List<ChallengeHintDto> Hints { get; set; } = new();
}