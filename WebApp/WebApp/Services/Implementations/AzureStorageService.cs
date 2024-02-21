using Azure.Data.Tables;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using WebApp.Entities;
using WebApp.Services.Interfaces;
using WebApp.Settings;
using Newtonsoft.Json;

namespace WebApp.Services.Implementations
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _connectionString;
        private readonly string _tableName;
        private readonly string _containerName;

        public AzureStorageService(AzureSettings azureSettings, BlobServiceClient blobServiceClient) //IOption Pattern
        {
            _connectionString = azureSettings.AzureStorageConnectionString;
            _tableName = azureSettings.TableName;
            _containerName = azureSettings.ContainerName;
            _blobServiceClient = blobServiceClient;
        }
        /// <summary>
        /// get data from azure storage (table) and map to project entity
        /// </summary>
        /// <returns>IEnumerable<AzureTablePayload></returns>
        public IEnumerable<AzureTablePayload> GetTableData(DateTime fromDate, DateTime toDate)
        {
            var tableClient = new TableClient(_connectionString, _tableName);
            var response = tableClient.Query<AzureTablePayload>().Where(p => p.Timestamp > fromDate && p.Timestamp < toDate);
            var result = new List<AzureTablePayload>();
            foreach (var entity in response)
            {
                result.Add(entity);
            }
            return result;
        }
        /// <summary>
        /// get data from azure storage (blob) and map to project entity
        /// </summary>
        /// <param name="blobName"></param>
        /// <returns>AzureBlobPayload</returns>
        public AzureBlobPayload GetBlobData(string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            BlobDownloadInfo download = blobClient.Download();

            using (MemoryStream ms = new MemoryStream())
            {
                download.Content.CopyTo(ms);
                ms.Position = 0;

                using (StreamReader reader = new StreamReader(ms))
                {
                    string jsonContent = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<AzureBlobPayload>(jsonContent);
                }
            }
        }
    }


}
