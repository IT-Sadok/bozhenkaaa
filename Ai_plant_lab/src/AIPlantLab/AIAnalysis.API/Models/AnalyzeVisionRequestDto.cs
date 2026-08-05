namespace AIAnalysis.API.Models;

public class AnalyzeVisionRequestDto
{
    public Guid ExperimentId { get; set; }
    public IFormFile File { get; set; } = null!;
}