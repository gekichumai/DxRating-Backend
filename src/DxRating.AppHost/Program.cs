using DxRating.Hosting.Smtp4dev;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

#region External Services

var postgresqlPassword = builder.AddParameter("postgresql-password", "dxrating", secret: true);
var redisPassword = builder.AddParameter("redis-password", "dxrating", secret: true);

var postgresql = builder
    .AddPostgres("dxrating-psql", password: postgresqlPassword)
    .WithOtlpExporter()
    .WithImageTag("17.4")
    .WithDataVolume("dxrating-psql-data")
    .AddDatabase("psql-db", "dxrating");

var redis = builder
    .AddRedis("redis", password: redisPassword)
    .WithOtlpExporter()
    .WithImageTag("7.4.2-alpine")
    .WithDataVolume("dxrating-redis-data")
    .WithPersistence(TimeSpan.FromMinutes(5), 100);

var smtp4dev = builder
    .AddSmtp4dev("smtp4dev")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithOtlpExporter()
    .WithImageTag("3.8.3")
    .WithDataVolume("dxrating-smtp-data");

#endregion

var migrator = builder.AddProject<DxRating_Worker_Migrator>("migrator")
    .WithReference(postgresql, "PostgreSQL")
    .WaitFor(postgresql);

builder.AddProject<DxRating_Api>("api")
    .WithReference(postgresql, "PostgreSQL")
    .WithReference(redis, "Cache")
    .WithReference(smtp4dev, "Smtp")
    .WaitForCompletion(migrator)
    .WaitFor(postgresql)
    .WaitFor(redis)
    .WaitFor(smtp4dev);

var app = builder.Build();

await app.RunAsync();
