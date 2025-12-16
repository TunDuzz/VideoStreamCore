using Microsoft.AspNetCore.Mvc;
using VideoStreamCore.Application.DTO;
using VideoStreamCore.Application.Interfaces;
using VideoStreamCore.Domain.Entities;
using VideoStreamCore.Domain.Enums;

namespace VideoStreamCore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VideosController : ControllerBase
{
    private readonly IVideoStorage _storage;
    private readonly IVideoRepository _repo;
    private readonly IVideoProcessor _processor;

    public VideosController(IVideoStorage storage, IVideoRepository repo, IVideoProcessor processor)
    {
        _storage = storage;
        _repo = repo;
        _processor = processor;
    }

    [HttpPost("upload")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> UploadVideo([FromForm] UploadVideoDto dto)
    {
        if (dto.File == null || dto.File.Length == 0)
            return BadRequest("Vui lòng chọn file video!");

        // 1. Lưu file gốc
        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.File.FileName)}";
        using var stream = dto.File.OpenReadStream();
        var savedPath = await _storage.UploadFileAsync(stream, uniqueFileName);

        // 2. Tạo Thumbnail (An toàn)
        string thumbPath = string.Empty;
        try
        {
            thumbPath = await _processor.CreateThumbnailAsync(savedPath);
        }
        catch
        {
            // Nếu lỗi tạo thumbnail thì bỏ qua, vẫn cho lưu video thành công
            // (Sau này có thể dùng Background Job để thử lại sau)
        }

        // 3. Lưu DB
        var video = new Video
        {
            Title = dto.Title,
            OriginalFileName = dto.File.FileName,
            Size = dto.File.Length,
            StoragePath = savedPath,
            ThumbnailPath = thumbPath,
            Status = VideoStatus.Ready
        };

        await _repo.AddAsync(video);
        await _repo.SaveChangesAsync();

        return Ok(new
        {
            Message = "Upload thành công!",
            VideoId = video.Id,
            Thumbnail = thumbPath,
            StreamUrl = $"{Request.Scheme}://{Request.Host}/api/Stream/{video.Id}"
        });
    }
}