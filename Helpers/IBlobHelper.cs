namespace Garage88.Helpers
{
    public interface IBlobHelper
    {
        Task<Guid> UploadBlobAsync(IFormFile file, String containerName);

        Task<Guid> UploadBlobAsync(byte[] file, String containerName);

        Task<Guid> UploadBlobAsync(string image, String containerName);
    }
}