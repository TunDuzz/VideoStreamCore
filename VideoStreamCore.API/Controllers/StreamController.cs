using Microsoft.AspNetCore.Mvc;
using VideoStreamCore.Application.Interfaces;

namespace VideoStreamCore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StreamController : ControllerBase
{
    private readonly IVideoRepository _repo;

    public StreamController(IVideoRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVideoStream(Guid id)
    {
        var video = await _repo.GetByIdAsync(id);
        if (video == null) return NotFound("Không tìm thấy video trong DB.");

        if (!System.IO.File.Exists(video.StoragePath))
            return NotFound("File gốc đã bị xóa hoặc lỗi đường dẫn.");

        return PhysicalFile(video.StoragePath, "video/mp4", enableRangeProcessing: true);
    }
}