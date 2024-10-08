using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Garage88.Helpers
{
    public class BlobHelper : IBlobHelper
    {
        private readonly CloudBlobClient _blobClient;

        public BlobHelper(IConfiguration configuration)
        {
            string keys = configuration["Blob:ConnectionString"];

            // Verifica se a chave de conexão está vazia ou nula
            if (string.IsNullOrEmpty(keys))
            {
                _blobClient = null; // Não inicializa o CloudBlobClient
                Console.WriteLine("Blob connection string is missing. BlobHelper will not be functional.");
                return; // Retorna do construtor
            }

            // Se a chave de conexão estiver presente, inicializa o CloudBlobClient
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(keys);
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task<Guid> UploadBlobAsync(IFormFile file, string containerName)
        {
            if (_blobClient == null)
            {
                throw new InvalidOperationException("BlobClient is not initialized. Please check the connection string.");
            }

            Stream stream = file.OpenReadStream();
            return await UploadStreamAsync(stream, containerName);
        }

        public async Task<Guid> UploadBlobAsync(byte[] file, string containerName)
        {
            if (_blobClient == null)
            {
                throw new InvalidOperationException("BlobClient is not initialized. Please check the connection string.");
            }

            MemoryStream stream = new MemoryStream(file);
            return await UploadStreamAsync(stream, containerName);
        }

        public async Task<Guid> UploadBlobAsync(string image, string containerName)
        {
            if (_blobClient == null)
            {
                throw new InvalidOperationException("BlobClient is not initialized. Please check the connection string.");
            }

            Stream stream = File.OpenRead(image);
            return await UploadStreamAsync(stream, containerName);
        }

        private async Task<Guid> UploadStreamAsync(Stream stream, string containerName)
        {
            if (_blobClient == null)
            {
                throw new InvalidOperationException("BlobClient is not initialized. Please check the connection string.");
            }

            Guid name = Guid.NewGuid();
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{name}");
            await blockBlob.UploadFromStreamAsync(stream);
            return name;
        }
    }
}