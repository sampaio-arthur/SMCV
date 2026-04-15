namespace SMCV.Application.Interfaces;

public interface IFileStorageService
{
    /// <summary>
    /// Faz upload de um arquivo e retorna a object key (identificador único no storage).
    /// </summary>
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Faz download de um arquivo pelo object key e retorna os bytes.
    /// </summary>
    Task<byte[]> DownloadAsync(string objectKey, CancellationToken cancellationToken = default);
}
