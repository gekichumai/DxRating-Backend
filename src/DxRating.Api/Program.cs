using DxRating.ServiceDefault.Extensions;
using DxRating.Services.Api;
using DxRating.Services.Api.Configurator;
using DxRating.Services.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefault();
builder.AddApiServices();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapApiEndpoints();

await app.RunAsync();
