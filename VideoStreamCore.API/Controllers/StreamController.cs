using Microsoft.AspNetCore.Mvc;
using VideoStreamCore.Application.Interfaces;
using VideoStreamCore.Infrastructure.Storage; 

namespace VideoStreamCore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StreamController : ControllerBase
{
    private readonly IVideoRepository _repo;
    private readonly IVideoStorage _storage;

    public StreamController(IVideoRepository repo, IVideoStorage storage)
    {
        _repo = repo;
        _storage = storage;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVideoStream(Guid id)
    {
        var video = await _repo.GetByIdAsync(id);
        if (video == null) return NotFound("Video does not exist.");
        var fileUrl = await _storage.GetFileUrlAsync(video.StoragePath);
        if (_storage is MinioVideoStorage)
        {
            return Redirect(fileUrl); 
        }
        if (System.IO.File.Exists(fileUrl))
        {
            return PhysicalFile(fileUrl, video.ContentType, enableRangeProcessing: true);
        }
        return NotFound("Video file not found.");
    }
}