using Minio;
using Minio.DataModel.Args;
using VideoStreamCore.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace VideoStreamCore.Infrastructure.Storage;

public class MinioVideoStorage : IVideoStorage
{
    private readonly IMinioClient _minioClient;
    private const string VideoBucket = "videos";
    private const string ImageBucket = "images";

    private readonly IConfiguration _config;
    
    public MinioVideoStorage(IConfiguration config)
    {
        _config = config;
        _minioClient = new MinioClient()
            .WithEndpoint(_config["Minio:Endpoint"] ?? "localhost:9000")
            .WithCredentials(_config["Minio:AccessKey"], _config["Minio:SecretKey"])
            .WithSSL(false)
            .Build();
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {

        await EnsureBucketExistsAsync(VideoBucket);

        fileStream.Position = 0;
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(VideoBucket)
            .WithObject(fileName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType("video/mp4");

        await _minioClient.PutObjectAsync(putObjectArgs);

        return fileName;
    }

    public async Task<string> UploadThumbnailAsync(string filePath, string fileName)
    {
        await EnsureBucketExistsAsync(ImageBucket);

        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(ImageBucket)
            .WithObject(fileName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType("image/jpeg");

        await _minioClient.PutObjectAsync(putObjectArgs);

        return fileName;
    }

    public async Task<string> GetFileUrlAsync(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return string.Empty;

        var args = new PresignedGetObjectArgs()
       .WithBucket(VideoBucket)
       .WithObject(fileName)
       .WithExpiry(60 * 60);

        return await _minioClient.PresignedGetObjectAsync(args);
    }

    private async Task EnsureBucketExistsAsync(string bucketName)
    {
        var beArgs = new BucketExistsArgs().WithBucket(bucketName);
        bool found = await _minioClient.BucketExistsAsync(beArgs);

        if (!found)
        {
            var mbArgs = new MakeBucketArgs().WithBucket(bucketName);
            await _minioClient.MakeBucketAsync(mbArgs);
        }
    }
}