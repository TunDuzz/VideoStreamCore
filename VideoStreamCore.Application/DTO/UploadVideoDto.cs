using Microsoft.AspNetCore.Http; // Cần dùng IFormFile

namespace VideoStreamCore.Application.DTO;

public class UploadVideoDto
{
    public string Title { get; set; }
    public IFormFile File { get; set; } // IFormFile là kiểu dữ liệu file của ASP.NET Core
}