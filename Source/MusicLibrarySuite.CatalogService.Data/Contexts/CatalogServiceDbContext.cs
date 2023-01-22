using Microsoft.EntityFrameworkCore;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.Contexts;

/// <summary>
/// Represents a base class for the catalog service database context.
/// </summary>
public abstract class CatalogServiceDbContext : DbContext
{
    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="ArtistDto" /> type.
    /// </summary>
    public DbSet<ArtistDto> Artists { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="ArtistRelationshipDto" /> type.
    /// </summary>
    public DbSet<ArtistRelationshipDto> ArtistRelationships { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="ArtistGenreDto" /> type.
    /// </summary>
    public DbSet<ArtistGenreDto> ArtistGenres { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="ReleaseDto" /> type.
    /// </summary>
    public DbSet<ReleaseDto> Releases { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="ReleaseRelationshipDto" /> type.
    /// </summary>
    public DbSet<ReleaseRelationshipDto> ReleaseRelationships { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="ReleaseMediaDto" /> type.
    /// </summary>
    public DbSet<ReleaseMediaDto> ReleaseMediaCollection { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="ReleaseTrackDto" /> type.
    /// </summary>
    public DbSet<ReleaseTrackDto> ReleaseTrackCollection { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="ReleaseGroupDto" /> type.
    /// </summary>
    public DbSet<ReleaseGroupDto> ReleaseGroups { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="ReleaseGroupRelationshipDto" /> type.
    /// </summary>
    public DbSet<ReleaseGroupRelationshipDto> ReleaseGroupRelationships { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="GenreDto" /> type.
    /// </summary>
    public DbSet<GenreDto> Genres { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="GenreRelationshipDto" /> type.
    /// </summary>
    public DbSet<GenreRelationshipDto> GenreRelationships { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="ProductDto" /> type.
    /// </summary>
    public DbSet<ProductDto> Products { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="ProductRelationshipDto" /> type.
    /// </summary>
    public DbSet<ProductRelationshipDto> ProductRelationships { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="WorkDto" /> type.
    /// </summary>
    public DbSet<WorkDto> Works { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="WorkRelationshipDto" /> type.
    /// </summary>
    public DbSet<WorkRelationshipDto> WorkRelationships { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="WorkArtistDto" /> type.
    /// </summary>
    public DbSet<WorkArtistDto> WorkArtists { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="WorkFeaturedArtistDto" /> type.
    /// </summary>
    public DbSet<WorkFeaturedArtistDto> WorkFeaturedArtists { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="WorkPerformerDto" /> type.
    /// </summary>
    public DbSet<WorkPerformerDto> WorkPerformers { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="WorkComposerDto" /> type.
    /// </summary>
    public DbSet<WorkComposerDto> WorkComposers { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="WorkGenreDto" /> type.
    /// </summary>
    public DbSet<WorkGenreDto> WorkGenres { get; }

    /// <summary>
    /// Gets a <see cref="DbSet{TEntity}" /> object for entities of the <see cref="WorkToProductRelationshipDto" /> type.
    /// </summary>
    public DbSet<WorkToProductRelationshipDto> WorkToProductRelationships { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CatalogServiceDbContext" /> type.
    /// </summary>
    protected CatalogServiceDbContext()
        : base()
    {
        Artists = Set<ArtistDto>();
        ArtistRelationships = Set<ArtistRelationshipDto>();
        ArtistGenres = Set<ArtistGenreDto>();
        Releases = Set<ReleaseDto>();
        ReleaseRelationships = Set<ReleaseRelationshipDto>();
        ReleaseMediaCollection = Set<ReleaseMediaDto>();
        ReleaseTrackCollection = Set<ReleaseTrackDto>();
        ReleaseGroups = Set<ReleaseGroupDto>();
        ReleaseGroupRelationships = Set<ReleaseGroupRelationshipDto>();
        Genres = Set<GenreDto>();
        GenreRelationships = Set<GenreRelationshipDto>();
        Products = Set<ProductDto>();
        ProductRelationships = Set<ProductRelationshipDto>();
        Works = Set<WorkDto>();
        WorkRelationships = Set<WorkRelationshipDto>();
        WorkArtists = Set<WorkArtistDto>();
        WorkFeaturedArtists = Set<WorkFeaturedArtistDto>();
        WorkPerformers = Set<WorkPerformerDto>();
        WorkComposers = Set<WorkComposerDto>();
        WorkGenres = Set<WorkGenreDto>();
        WorkToProductRelationships = Set<WorkToProductRelationshipDto>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CatalogServiceDbContext" /> type using the specified database context options.
    /// </summary>
    /// <param name="contextOptions">The database context options.</param>
    protected CatalogServiceDbContext(DbContextOptions contextOptions)
        : base(contextOptions)
    {
        Artists = Set<ArtistDto>();
        ArtistRelationships = Set<ArtistRelationshipDto>();
        ArtistGenres = Set<ArtistGenreDto>();
        Releases = Set<ReleaseDto>();
        ReleaseRelationships = Set<ReleaseRelationshipDto>();
        ReleaseMediaCollection = Set<ReleaseMediaDto>();
        ReleaseTrackCollection = Set<ReleaseTrackDto>();
        ReleaseGroups = Set<ReleaseGroupDto>();
        ReleaseGroupRelationships = Set<ReleaseGroupRelationshipDto>();
        Genres = Set<GenreDto>();
        GenreRelationships = Set<GenreRelationshipDto>();
        Products = Set<ProductDto>();
        ProductRelationships = Set<ProductRelationshipDto>();
        Works = Set<WorkDto>();
        WorkRelationships = Set<WorkRelationshipDto>();
        WorkArtists = Set<WorkArtistDto>();
        WorkFeaturedArtists = Set<WorkFeaturedArtistDto>();
        WorkPerformers = Set<WorkPerformerDto>();
        WorkComposers = Set<WorkComposerDto>();
        WorkGenres = Set<WorkGenreDto>();
        WorkToProductRelationships = Set<WorkToProductRelationshipDto>();
    }
}
