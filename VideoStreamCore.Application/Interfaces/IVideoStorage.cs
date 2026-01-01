namespace VideoStreamCore.Application.Interfaces;

public interface IVideoStorage
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName);
    Task<string> UploadThumbnailAsync(string tempThumbPath, string thumbFileName);
    Task<string> GetFileUrlAsync(string fileName);
}