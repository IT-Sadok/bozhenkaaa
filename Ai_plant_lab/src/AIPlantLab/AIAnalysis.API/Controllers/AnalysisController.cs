using AIAnalysis.Application.Commands.AnalyzePhoto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AIAnalysis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalysisController : ControllerBase
{
    private readonly IMediator _mediator;

    public AnalysisController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("vision")]
    public async Task<IActionResult> AnalyzeVision(
        IFormFile file, 
        [FromQuery] Guid experimentId)
    {
        if (file.Length == 0)
            return BadRequest("File was not chosen.");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var photoBytes = memoryStream.ToArray();

        var command = new AnalyzePhotoCommand(experimentId, photoBytes);
        var result = await _mediator.Send(command);

        return result.IsSuccess 
            ? Ok(new { DiagnosisId = result.Value }) 
            : BadRequest(new { result.Error });
    }
}