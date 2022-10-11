using Microsoft.EntityFrameworkCore;

namespace MusicLibrarySuite.CatalogService.Data.Contexts;

/// <summary>
/// Represents a base class for the catalog service database context.
/// </summary>
public abstract class CatalogServiceDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CatalogServiceDbContext" /> type.
    /// </summary>
    protected CatalogServiceDbContext()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CatalogServiceDbContext" /> type using the specified database context options.
    /// </summary>
    /// <param name="contextOptions">The database context options.</param>
    protected CatalogServiceDbContext(DbContextOptions contextOptions)
        : base(contextOptions)
    {
    }
}
