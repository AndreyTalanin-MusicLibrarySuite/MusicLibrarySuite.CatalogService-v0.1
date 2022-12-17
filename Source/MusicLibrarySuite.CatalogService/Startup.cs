using System;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using MusicLibrarySuite.CatalogService.Core.AutoMapper;
using MusicLibrarySuite.CatalogService.Core.Services;
using MusicLibrarySuite.CatalogService.Core.Services.Abstractions;
using MusicLibrarySuite.CatalogService.Data.Contexts;
using MusicLibrarySuite.CatalogService.Data.Extensions;
using MusicLibrarySuite.CatalogService.Data.Repositories.Abstractions;
using MusicLibrarySuite.CatalogService.Data.SqlServer.Contexts;
using MusicLibrarySuite.CatalogService.Data.SqlServer.Repositories;

namespace MusicLibrarySuite.CatalogService;

/// <summary>
/// Contains methods like <see cref="ConfigureServices(IServiceCollection)" /> or <see cref="Configure(IApplicationBuilder, IWebHostEnvironment)" />
/// that are called by the runtime to configure the generic host builder and the application's HTTP request pipeline.
/// </summary>
public class Startup
{
    private readonly IConfiguration m_configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="Startup" /> type using the specified application configuration.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    public Startup(IConfiguration configuration)
    {
        m_configuration = configuration;
    }

    /// <summary>
    /// Adds services to the container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> collection for adding service descriptors.</param>
    /// <remarks>This method gets called by the runtime. Use this method to add services to the container.</remarks>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAutoMapper(options =>
        {
            options.AddProfile<CommonDatabaseProfile>();
            options.AddProfile<ArtistDatabaseProfile>();
            options.AddProfile<GenreDatabaseProfile>();
        });

        services.AddDbContextFactory<CatalogServiceDbContext, SqlServerCatalogServiceDbContext>(contextOptionsBuilder =>
        {
            var connectionStringName = "CatalogServiceConnectionString";
            var connectionString = m_configuration.GetConnectionString(connectionStringName);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Unable to get the connection string using the \"{connectionStringName}\" configuration key.");
            }

            contextOptionsBuilder.UseSqlServer(connectionString, sqlServerOptionsBuilder =>
            {
                sqlServerOptionsBuilder
                    .MigrationsAssembly(typeof(SqlServerCatalogServiceDbContext).Assembly.FullName)
                    .MigrationsHistoryTable("MigrationsHistory", "dbo")
                    .CommandTimeout(120);
            });
        });

        IMvcBuilder mvcBuilder = services.AddControllers();
        mvcBuilder.AddJsonOptions(jsonOptions =>
        {
            jsonOptions.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SupportNonNullableReferenceTypes();

            var contact = new OpenApiContact()
            {
                Name = "Andrey Talanin",
                Email = "andrey.talanin@outlook.com",
                Url = new Uri("https://github.com/AndreyTalanin"),
            };
            var license = new OpenApiLicense()
            {
                Name = "The MIT License",
                Url = new Uri("https://github.com/AndreyTalanin-MusicLibrarySuite/MusicLibrarySuite.CatalogService/blob/main/LICENSE.md"),
            };

            options.SwaggerDoc("MusicLibrarySuite.CatalogService", new OpenApiInfo()
            {
                Title = "Music Library Suite - Catalog Service API v0.4.0-rc3",
                Description = "Initial pre-release (unstable) API version.",
                Version = "v0.4.0-rc3",
                Contact = contact,
                License = license,
            });
        });

        services.AddScoped<IArtistRepository, SqlServerArtistRepository>();
        services.AddScoped<IGenreRepository, SqlServerGenreRepository>();

        services.AddScoped<IArtistService, ArtistService>();
        services.AddScoped<IGenreService, GenreService>();
    }

    /// <summary>
    /// Configures the application's HTTP request pipeline.
    /// </summary>
    /// <param name="applicationBuilder">The request pipeline builder.</param>
    /// <param name="webHostEnvironment">The hosting environment information provider.</param>
    /// <remarks>This method gets called by the runtime. Use this method to configure the application's HTTP request pipeline.</remarks>
    public void Configure(IApplicationBuilder applicationBuilder, IWebHostEnvironment webHostEnvironment)
    {
        if (webHostEnvironment.IsDevelopment())
        {
            applicationBuilder.UseSwagger();
            applicationBuilder.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/MusicLibrarySuite.CatalogService/swagger.json", $"MusicLibrarySuite.CatalogService");
            });

            applicationBuilder.UseDeveloperExceptionPage();
        }
        else
        {
            applicationBuilder.UseHsts();
        }

        applicationBuilder.UseHttpsRedirection();

        applicationBuilder.UseRouting();

        applicationBuilder.UseAuthorization();

        applicationBuilder.UseEndpoints(endpointRouteBuilder =>
        {
            endpointRouteBuilder.MapControllers();
        });
    }
}
