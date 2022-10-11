using Microsoft.EntityFrameworkCore;

namespace MusicLibrarySuite.CatalogService.Data.Helpers;

internal sealed class DbContextFactoryWrapper<TContextService, TContextImplementation> : IDbContextFactory<TContextService>
    where TContextService : DbContext
    where TContextImplementation : DbContext, TContextService
{
    private readonly IDbContextFactory<TContextImplementation> m_contextFactory;

    public DbContextFactoryWrapper(IDbContextFactory<TContextImplementation> contextFactory)
    {
        m_contextFactory = contextFactory;
    }

    public TContextService CreateDbContext()
    {
        return m_contextFactory.CreateDbContext();
    }
}
