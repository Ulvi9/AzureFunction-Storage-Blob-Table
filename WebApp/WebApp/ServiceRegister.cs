using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using WebApp.Services.Implementations;
using WebApp.Services.Interfaces;
using WebApp.Settings;

namespace WebApp
{
    public static class ServiceRegister
    {
        public static void Register(this IServiceCollection services, IConfiguration config)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.Configure<AzureSettings>(config.GetSection("AzureSettings"));
            services.AddSingleton(sp =>
            {
                var azureSettings = sp.GetService<IOptions<AzureSettings>>().Value;
                return new BlobServiceClient(azureSettings.AzureStorageConnectionString);
            });
            services.AddSingleton<AzureSettings>(sp =>
            {
                return sp.GetRequiredService<IOptions<AzureSettings>>().Value;
            });
            services.AddScoped<IAzureStorageService,AzureStorageService>();
        }
    }
}
