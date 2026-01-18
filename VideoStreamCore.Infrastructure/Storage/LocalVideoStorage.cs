using VideoStreamCore.Application.Interfaces;

namespace VideoStreamCore.Infrastructure.Storage;

public class LocalVideoStorage : IVideoStorage
{
    private readonly string _uploadFolder;

    public LocalVideoStorage()
    {
        _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        if (!Directory.Exists(_uploadFolder))
        {
            Directory.CreateDirectory(_uploadFolder);
        }
    }
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        var filePath = Path.Combine(_uploadFolder, fileName);
        using (var output = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(output);
        }
        return filePath;
    }
    public async Task<string> UploadThumbnailAsync(string sourcePath, string fileName)
    {
        var destPath = Path.Combine(_uploadFolder, fileName);

        using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
        using (var destStream = new FileStream(destPath, FileMode.Create))
        {
            await sourceStream.CopyToAsync(destStream);
        }

        return destPath; 
    }
    public Task<string> GetFileUrlAsync(string fileName)
    {
        var filePath = Path.Combine(_uploadFolder, fileName);
        return Task.FromResult(filePath);
    }
}