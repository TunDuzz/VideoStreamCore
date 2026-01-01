using VideoStreamCore.Application.Interfaces;
using Xabe.FFmpeg;

namespace VideoStreamCore.Infrastructure.Processing;

public class FfmpegVideoProcessor : IVideoProcessor
{
    public FfmpegVideoProcessor()
    {
        FFmpeg.SetExecutablesPath(@"C:\ffmpeg");
    }

    public async Task<string> CreateThumbnailAsync(string videoPath)
    {
        string outputPath = Path.ChangeExtension(videoPath, "_thumb.jpg");

        IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(videoPath);

        IVideoStream videoStream = mediaInfo.VideoStreams.First();

        double seekTime = videoStream.Duration.TotalSeconds > 5 ? 5 : videoStream.Duration.TotalSeconds / 2;

        var conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(
            videoPath,
            outputPath,
            TimeSpan.FromSeconds(seekTime)
        );


        await conversion.Start();

        return outputPath;
    }
}