using VideoStreamCore.Application.Interfaces;
using Xabe.FFmpeg;

namespace VideoStreamCore.Infrastructure.Processing;

public class FfmpegVideoProcessor : IVideoProcessor
{
    public FfmpegVideoProcessor()
    {
        // 1. Chỉ đường dẫn đến file FFmpeg bạn vừa cài (QUAN TRỌNG)
        FFmpeg.SetExecutablesPath(@"C:\ffmpeg");
    }

    public async Task<string> CreateThumbnailAsync(string videoPath)
    {
        // Tạo đường dẫn file ảnh đầu ra (cùng thư mục với video, đuôi .jpg)
        // Ví dụ: video.mp4 -> video_thumb.jpg
        string outputPath = Path.ChangeExtension(videoPath, "_thumb.jpg");

        // 2. Lấy thông tin video để biết độ dài
        IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(videoPath);

        // 3. Tính toán vị trí cắt ảnh (Lấy ở giây thứ 5, hoặc 1/2 video nếu video ngắn quá)
        // VideoStream là luồng hình ảnh (khác với AudioStream là luồng tiếng)
        IVideoStream videoStream = mediaInfo.VideoStreams.First();

        // Nếu video ngắn hơn 5s thì lấy ở giữa, ngược lại lấy ở giây thứ 5
        double seekTime = videoStream.Duration.TotalSeconds > 5 ? 5 : videoStream.Duration.TotalSeconds / 2;

        // 4. Ra lệnh FFmpeg chụp ảnh (Snapshot)
        var conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(
            videoPath,
            outputPath,
            TimeSpan.FromSeconds(seekTime)
        );

        // Chạy lệnh
        await conversion.Start();

        return outputPath;
    }
}