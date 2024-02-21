using WebApp.Entities;

namespace WebApp.Services.Interfaces
{
    public interface IAzureStorageService
    {
        IEnumerable<AzureTablePayload> GetTableData(DateTime fromDate, DateTime toDate);
        AzureBlobPayload GetBlobData(string blobName);
    }
}