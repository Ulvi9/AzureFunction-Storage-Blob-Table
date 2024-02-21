using Azure;
using Azure.Data.Tables;

namespace WebApp.Entities
{
    public class AzureTablePayload : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public ETag ETag { get; set; }
        DateTimeOffset? ITableEntity.Timestamp { get; set; }
    }
}
