using DxRating.Hosting.Smtp4dev;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresqlPassword = builder.AddParameter("postgresql-password", "dxrating", secret: true);
var valkeyPassword = builder.AddParameter("redis-password", "dxrating", secret: true);

var postgresql = builder
    .AddPostgres("dxrating-psql", password: postgresqlPassword)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithOtlpExporter()
    .WithImageTag("17.4")
    .WithDataVolume("dxrating-psql-data");

postgresql.AddDatabase("psql-db", "dxrating");

var valkey = builder
    .AddValkey("valkey", password: valkeyPassword)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithOtlpExporter()
    .WithImageTag("8.1.0-alpine")
    .WithDataVolume("dxrating-valkey-data")
    .WithPersistence(TimeSpan.FromMinutes(5), 100);

var smtp4dev = builder
    .AddSmtp4dev("smtp4dev")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithOtlpExporter()
    .WithImageTag("3.8.3");

builder.AddProject<DxRating_Api>("api")
    .WithReference(postgresql, "PostgreSQL")
    .WithReference(valkey, "Cache")
    .WithReference(smtp4dev, "Smtp")
    .WaitFor(postgresql)
    .WaitFor(valkey)
    .WaitFor(smtp4dev);

var app = builder.Build();

await app.RunAsync();
