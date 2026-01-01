using Microsoft.AspNetCore.Http;

namespace VideoStreamCore.Application.DTO;

public class UploadVideoDto
{
    public string Title { get; set; }
    public IFormFile File { get; set; }
}
