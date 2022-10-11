using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MusicLibrarySuite.CatalogService;

/// <summary>
/// Contains static runtime-called methods like <see cref="Main(string[])" /> or <see cref="CreateHostBuilder(string[])" />.
/// </summary>
public class Program
{
    /// <summary>
    /// Represents the entry point for the application.
    /// </summary>
    /// <param name="args">The array containing command line arguments.</param>
    public static void Main(string[] args)
    {
        CreateHostBuilder(args)
            .Build()
            .Run();
    }

    /// <summary>
    /// Creates a generic host builder for the application.
    /// </summary>
    /// <param name="args">The array containing command line arguments.</param>
    /// <remarks>This method gets called by the runtime. Use this method to configure the application host builder.</remarks>
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webHostBuilder => webHostBuilder.UseStartup<Startup>());

        return hostBuilder;
    }
}
