using Microsoft.EntityFrameworkCore;

using MusicLibrarySuite.CatalogService.Data.Contexts;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Contexts;

/// <summary>
/// Represents a SQL Server - specific implementation of the catalog service database context.
/// </summary>
public class SqlServerCatalogServiceDbContext : CatalogServiceDbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerCatalogServiceDbContext" /> type.
    /// </summary>
    public SqlServerCatalogServiceDbContext()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerCatalogServiceDbContext" /> type using the specified database context options.
    /// </summary>
    /// <param name="contextOptions">The database context options.</param>
    public SqlServerCatalogServiceDbContext(DbContextOptions contextOptions)
        : base(contextOptions)
    {
    }
}
