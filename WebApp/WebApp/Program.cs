using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using WebApp;
using WebApp.Services.Implementations;
using WebApp.Settings;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.Register(config);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
