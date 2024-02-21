using System;
using System.IO;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using DemoTask1.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Constant = DemoTask1.Constants.Constant;

namespace DemoTask1
{
    public class Function1
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public Function1(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        /// <summary>
        /// azure function app,work every minute,get data from url send azure storage(log=>table,data=>blob)
        /// </summary>
        /// <param name="myTimer"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("Function1")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
        {
            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                var response = await httpClient.GetAsync(Constant.Url);
                response.EnsureSuccessStatusCode();
                var payload = await response.Content.ReadAsStreamAsync();

                await StoreBlob(payload);
                await LogAttempt("Success", response.StatusCode.ToString());

                log.LogInformation("Success");
            }
            catch (Exception ex)
            {
                await LogAttempt("Failure", ex.Message);
                log.LogError(ex, "Failure");
            }
        }
        /// <summary>
        /// write payload to azure storage(blob)
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        private async Task StoreBlob(Stream payload)
        {
            var connectionString = Environment.GetEnvironmentVariable("StorageConnectionString");
            var containerName = Constant.ContainerName;
            var blobName = $"{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}_payload.json";

            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            var blob = container.GetBlockBlobReference(blobName);
            await blob.UploadFromStreamAsync(payload);
        }
        /// <summary>
        /// write success/failure error azure storage(table)
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task LogAttempt(string status, string message)
        {
            var connectionString = Environment.GetEnvironmentVariable("StorageConnectionString");
            var tableName = Constant.TableName;
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();

            var logEntity = new LogEntity
            {
                PartitionKey = DateTime.UtcNow.ToString("yyyyMMdd"),
                RowKey = Guid.NewGuid().ToString(),
                Status = status,
                Message = message
            };
            var insertOperation = TableOperation.Insert(logEntity);
            await table.ExecuteAsync(insertOperation);
        }
    }

}
