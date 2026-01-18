using VideoStreamCore.Domain.Common;
using VideoStreamCore.Domain.Enums;

namespace VideoStreamCore.Domain.Entities;

public class Video : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public double Duration { get; set; }
    public string ContentType { get; set; } = "video/mp4";
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public string ThumbnailPath { get; set; } = string.Empty;
    public long Size { get; set; }
    public VideoStatus Status { get; set; } = VideoStatus.Uploaded;

}
