using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MusicLibrarySuite.CatalogService.Data.Helpers;

namespace MusicLibrarySuite.CatalogService.Data.Extensions;

/// <summary>
/// Provides a set of extension methods for the <see cref="IServiceCollection" /> interface.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the specified context as a service in the specified <see cref="IServiceCollection" /> collection.
    /// Also registers the implementation context factory injected by the service type.
    /// </summary>
    /// <typeparam name="TContextService">The class that will be used to resolve the context from the container.</typeparam>
    /// <typeparam name="TContextImplementation">The concrete implementation type to create.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection" /> collection to add services to.</param>
    /// <param name="optionsAction">The optional action to configure the <see cref="DbContextOptions" /> options for the context.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddDbContextFactory<TContextService, TContextImplementation>(this IServiceCollection services, Action<DbContextOptionsBuilder>? optionsAction = null)
        where TContextService : DbContext
        where TContextImplementation : DbContext, TContextService
    {
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped;
        ServiceLifetime contextFactoryLifetime = ServiceLifetime.Singleton;
        ServiceLifetime optionsLifetime = ServiceLifetime.Singleton;

        services.AddDbContext<TContextImplementation>(optionsAction, contextLifetime, optionsLifetime);
        services.AddDbContextFactory<TContextImplementation>(optionsAction, contextFactoryLifetime);

        services.AddScoped<TContextService, TContextImplementation>();
        services.AddSingleton<IDbContextFactory<TContextService>, DbContextFactoryWrapper<TContextService, TContextImplementation>>();

        return services;
    }
}
