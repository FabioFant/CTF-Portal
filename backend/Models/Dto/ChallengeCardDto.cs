namespace Backend.Models.Dto;

public class ChallengeCardDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Category { get; set; }
    public required int Points { get; set; }
    public DateOnly? Date { get; set; }
}