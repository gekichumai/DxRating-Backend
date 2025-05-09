using DxRating.ServiceDefault.Extensions;
using DxRating.Services.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefault();
builder.AddApiServices();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapApiEndpoints();

await app.RunAsync();
