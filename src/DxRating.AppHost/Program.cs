using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresqlPassword = builder.AddParameter("postgresql-password", "dxrating", secret: true);


var postgresql = builder
    .AddPostgres("dxrating-psql", password: postgresqlPassword)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithOtlpExporter()
    .WithImageTag("17.4")
    .WithDataVolume("dxrating-psql-data");

postgresql.AddDatabase("dxrating", "dxrating");

builder.AddProject<DxRating_Api>("api")
    .WithReference(postgresql)
    .WaitFor(postgresql);

var app = builder.Build();

await app.RunAsync();
