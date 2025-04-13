﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace DxRating.Database;

public static class Extensions
{
    public static void ConfigureNpgsql(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");

        builder.Services.AddDbContext<DxDbContext>(options =>
        {
            options.UseNpgsql(connectionString);

            if (builder.Environment.IsProduction())
            {
                return;
            }

            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        });

        builder.Services
            .AddHealthChecks()
            .AddDbContextCheck<DxDbContext>("npgsql");

        builder.Services.ConfigureOpenTelemetryMeterProvider(meter =>
        {
            meter.AddNpgsqlInstrumentation();
        });

        builder.Services.ConfigureOpenTelemetryTracerProvider(tracer =>
        {
            tracer.AddNpgsql();
        });
    }
}
