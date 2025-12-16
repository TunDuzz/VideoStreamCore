namespace VideoStreamCore.Application.Interfaces;

public interface IVideoStorage
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName);
}