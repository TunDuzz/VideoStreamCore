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
            return BadRequest("Please select a video file!");

        var tempPath = Path.GetTempPath();
        var tempVideoFileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.File.FileName)}";
        var tempVideoPath = Path.Combine(tempPath, tempVideoFileName);

        using (var stream = new FileStream(tempVideoPath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        string minioVideoKey = string.Empty;
        string minioThumbKey = string.Empty;
        string tempThumbPath = string.Empty;

        try
        {
            tempThumbPath = await _processor.CreateThumbnailAsync(tempVideoPath);
            var thumbFileName = Path.GetFileName(tempThumbPath);

            using (var videoStream = new FileStream(tempVideoPath, FileMode.Open, FileAccess.Read))
            {
                minioVideoKey = await _storage.UploadFileAsync(videoStream, tempVideoFileName);
            }
            minioThumbKey = await _storage.UploadThumbnailAsync(tempThumbPath, thumbFileName);
        }
        catch (Exception ex)
        {
            return BadRequest($"processing error: {ex.Message}");
        }
        finally
        {
            if (System.IO.File.Exists(tempThumbPath)) System.IO.File.Delete(tempThumbPath);
        }
        double duration = 0;
        try 
        {
             duration = await _processor.GetDurationAsync(tempVideoPath);
        }
        catch
        {
             duration = 0;
        }

        var video = new Video
        {
            Title = dto.Title,
            OriginalFileName = dto.File.FileName,
            Size = dto.File.Length,
            Duration = duration,
            StoragePath = minioVideoKey, 
            ThumbnailPath = minioThumbKey, 
            Status = VideoStatus.Ready
        };

        if (System.IO.File.Exists(tempVideoPath)) System.IO.File.Delete(tempVideoPath);
        await _repo.AddAsync(video);
        await _repo.SaveChangesAsync();

        return Ok(new
        {
            Message = "Upload MinIO success!",
            VideoId = video.Id,
            VideoKey = minioVideoKey,
            ThumbnailKey = minioThumbKey,
            StreamUrl = $"{Request.Scheme}://{Request.Host}/api/Stream/{video.Id}",
        });
    }
}