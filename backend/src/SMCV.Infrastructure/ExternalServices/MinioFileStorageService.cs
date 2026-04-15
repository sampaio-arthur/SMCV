using Minio;
using Minio.DataModel.Args;
using SMCV.Application.Interfaces;

namespace SMCV.Infrastructure.ExternalServices;

public class MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;

    public MinioFileStorageService(IMinioClient minioClient, MinioSettings settings)
    {
        _minioClient = minioClient;
        _bucketName = settings.BucketName;
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        await EnsureBucketExistsAsync(cancellationToken);

        var objectKey = $"resumes/{Guid.NewGuid()}_{fileName}";

        var args = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectKey)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType);

        await _minioClient.PutObjectAsync(args, cancellationToken);

        return objectKey;
    }

    public async Task<byte[]> DownloadAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        using var memoryStream = new MemoryStream();

        var args = new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectKey)
            .WithCallbackStream(stream => stream.CopyTo(memoryStream));

        await _minioClient.GetObjectAsync(args, cancellationToken);

        return memoryStream.ToArray();
    }

    public async Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectKey);

        await _minioClient.RemoveObjectAsync(args, cancellationToken);
    }

    private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
    {
        var exists = await _minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_bucketName), cancellationToken);

        if (!exists)
            await _minioClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_bucketName), cancellationToken);
    }
}
