
using Microsoft.AspNetCore.Mvc;
using WebApp.Entities;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzureController : ControllerBase
    {
        private readonly IAzureStorageService _azureStorageService;

        public AzureController(IAzureStorageService azureStorageService)
        {
            _azureStorageService = azureStorageService;
        }
        /// <summary>
        ///  endpoint to call from postman/swagger to fetch data(blob)
        /// </summary>
        /// <param name="blobName"></param>
        /// <returns>AzureBlobPayload </returns>
        [HttpGet("blob/{blobName}")]
        public AzureBlobPayload Get(string blobName)
        {
            return _azureStorageService.GetBlobData(blobName);
        }
        /// <summary>
        /// endpoint to call from postman/swagger to fetch data(table)
        /// </summary>
        /// <returns>IEnumerable<AzureTablePayload></returns>
        [HttpGet("table")]
        public IEnumerable<AzureTablePayload> Get(DateTime fromDate, DateTime toDate)
        {
            return _azureStorageService.GetTableData(fromDate, toDate);
        }

    }
}
