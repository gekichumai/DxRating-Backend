using DxRating.Database;
using DxRating.ServiceDefault.Extensions;
using DxRating.Worker.Migrator;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

if (EF.IsDesignTime is false)
{
    builder.AddServiceDefault();
    builder.Services.AddHostedService<Worker>();
}

builder.ConfigureNpgsql();
builder.Services.AddDbMigrator();

var app = builder.Build();

await app.RunAsync();
