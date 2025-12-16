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
}