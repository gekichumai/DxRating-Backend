using DxRating.ServiceDefault.Extensions;
using DxRating.Worker.Jobs;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefault();
builder.AddBackgroundJobs();

var app = builder.Build();

await app.RunAsync();
