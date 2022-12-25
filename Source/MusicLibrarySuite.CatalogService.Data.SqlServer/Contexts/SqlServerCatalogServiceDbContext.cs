using Microsoft.EntityFrameworkCore;

using MusicLibrarySuite.CatalogService.Data.Contexts;
using MusicLibrarySuite.CatalogService.Data.Entities;

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

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArtistDto>().ToTable("Artist", "dbo");
        modelBuilder.Entity<ArtistDto>().HasKey(entity => entity.Id);
        modelBuilder.Entity<ArtistDto>().HasCheckConstraint("CK_Artist_Name", "LEN(TRIM([Name])) > 0");
        modelBuilder.Entity<ArtistDto>().HasCheckConstraint("CK_Artist_Description", "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
        modelBuilder.Entity<ArtistDto>().HasCheckConstraint("CK_Artist_DisambiguationText", "[DisambiguationText] IS NULL OR LEN(TRIM([DisambiguationText])) > 0");

        modelBuilder.Entity<ArtistRelationshipDto>().ToTable("ArtistRelationship", "dbo");
        modelBuilder.Entity<ArtistRelationshipDto>().HasKey(entity => new { entity.ArtistId, entity.DependentArtistId });
        modelBuilder.Entity<ArtistRelationshipDto>()
            .HasOne<ArtistDto>(entity => entity.Artist)
            .WithMany(entity => entity.ArtistRelationships)
            .HasForeignKey(entity => entity.ArtistId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ArtistRelationshipDto>()
            .HasOne<ArtistDto>(entity => entity.DependentArtist)
            .WithMany()
            .HasForeignKey(entity => entity.DependentArtistId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ArtistRelationshipDto>()
            .HasIndex(entity => entity.ArtistId)
            .HasDatabaseName("IX_ArtistRelationship_ArtistId");
        modelBuilder.Entity<ArtistRelationshipDto>()
            .HasIndex(entity => entity.DependentArtistId)
            .HasDatabaseName("IX_ArtistRelationship_DependentArtistId");
        modelBuilder.Entity<ArtistRelationshipDto>()
            .HasIndex(entity => new { entity.ArtistId, entity.Order })
            .HasDatabaseName("UIX_ArtistRelationship_ArtistId_Order")
            .IsUnique();
        modelBuilder.Entity<ArtistRelationshipDto>().HasCheckConstraint("CK_ArtistRelationship_Name", "LEN(TRIM([Name])) > 0");
        modelBuilder.Entity<ArtistRelationshipDto>().HasCheckConstraint("CK_ArtistRelationship_Description", "[Description] IS NULL OR LEN(TRIM([Description])) > 0");

        modelBuilder.Entity<ArtistGenreDto>().ToTable("ArtistGenre", "dbo");
        modelBuilder.Entity<ArtistGenreDto>().HasKey(entity => new { entity.ArtistId, entity.GenreId });
        modelBuilder.Entity<ArtistGenreDto>()
            .HasOne(entity => entity.Artist)
            .WithMany(entity => entity.ArtistGenres)
            .HasForeignKey(entity => entity.ArtistId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ArtistGenreDto>()
            .HasOne(entity => entity.Genre)
            .WithMany()
            .HasForeignKey(entity => entity.GenreId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ArtistGenreDto>()
            .HasIndex(entity => entity.ArtistId)
            .HasDatabaseName("IX_ArtistGenre_ArtistId");
        modelBuilder.Entity<ArtistGenreDto>()
            .HasIndex(entity => entity.GenreId)
            .HasDatabaseName("IX_ArtistGenre_GenreId");
        modelBuilder.Entity<ArtistGenreDto>()
            .HasIndex(entity => new { entity.ArtistId, entity.Order })
            .HasDatabaseName("UIX_ArtistGenre_ArtistId_Order")
            .IsUnique();

        modelBuilder.Entity<GenreDto>().ToTable("Genre", "dbo");
        modelBuilder.Entity<GenreDto>().HasKey(entity => entity.Id);
        modelBuilder.Entity<GenreDto>().HasCheckConstraint("CK_Genre_Name", "LEN(TRIM([Name])) > 0");
        modelBuilder.Entity<GenreDto>().HasCheckConstraint("CK_Genre_Description", "[Description] IS NULL OR LEN(TRIM([Description])) > 0");

        modelBuilder.Entity<GenreRelationshipDto>().ToTable("GenreRelationship", "dbo");
        modelBuilder.Entity<GenreRelationshipDto>().HasKey(entity => new { entity.GenreId, entity.DependentGenreId });
        modelBuilder.Entity<GenreRelationshipDto>()
            .HasOne(entity => entity.Genre)
            .WithMany(entity => entity.GenreRelationships)
            .HasForeignKey(entity => entity.GenreId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<GenreRelationshipDto>()
            .HasOne(entity => entity.DependentGenre)
            .WithMany()
            .HasForeignKey(entity => entity.DependentGenreId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<GenreRelationshipDto>()
            .HasIndex(entity => entity.GenreId)
            .HasDatabaseName("IX_GenreRelationship_GenreId");
        modelBuilder.Entity<GenreRelationshipDto>()
            .HasIndex(entity => entity.DependentGenreId)
            .HasDatabaseName("IX_GenreRelationship_DependentGenreId");
        modelBuilder.Entity<GenreRelationshipDto>()
            .HasIndex(entity => new { entity.GenreId, entity.Order })
            .HasDatabaseName("UIX_GenreRelationship_GenreId_Order")
            .IsUnique();
        modelBuilder.Entity<GenreRelationshipDto>().HasCheckConstraint("CK_GenreRelationship_Name", "LEN(TRIM([Name])) > 0");
        modelBuilder.Entity<GenreRelationshipDto>().HasCheckConstraint("CK_GenreRelationship_Description", "[Description] IS NULL OR LEN(TRIM([Description])) > 0");

        modelBuilder.Entity<ProductDto>().ToTable("Product", "dbo");
        modelBuilder.Entity<ProductDto>().HasKey(entity => entity.Id);
        modelBuilder.Entity<ProductDto>().HasCheckConstraint("CK_Product_Title", "LEN(TRIM([Title])) > 0");
        modelBuilder.Entity<ProductDto>().HasCheckConstraint("CK_Product_Description", "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
        modelBuilder.Entity<ProductDto>().HasCheckConstraint("CK_Product_DisambiguationText", "[DisambiguationText] IS NULL OR LEN(TRIM([DisambiguationText])) > 0");
        modelBuilder.Entity<ProductDto>()
            .Property(entity => entity.ReleasedOn)
            .HasColumnType("date");

        modelBuilder.Entity<ProductRelationshipDto>().ToTable("ProductRelationship", "dbo");
        modelBuilder.Entity<ProductRelationshipDto>().HasKey(entity => new { entity.ProductId, entity.DependentProductId });
        modelBuilder.Entity<ProductRelationshipDto>()
            .HasOne(entity => entity.Product)
            .WithMany(entity => entity.ProductRelationships)
            .HasForeignKey(entity => entity.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ProductRelationshipDto>()
            .HasOne(entity => entity.DependentProduct)
            .WithMany()
            .HasForeignKey(entity => entity.DependentProductId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ProductRelationshipDto>()
            .HasIndex(entity => entity.ProductId)
            .HasDatabaseName("IX_ProductRelationship_ProductId");
        modelBuilder.Entity<ProductRelationshipDto>()
            .HasIndex(entity => entity.DependentProductId)
            .HasDatabaseName("IX_ProductRelationship_DependentProductId");
        modelBuilder.Entity<ProductRelationshipDto>()
            .HasIndex(entity => new { entity.ProductId, entity.Order })
            .HasDatabaseName("UIX_ProductRelationship_ProductId_Order")
            .IsUnique();
        modelBuilder.Entity<ProductRelationshipDto>().HasCheckConstraint("CK_ProductRelationship_Name", "LEN(TRIM([Name])) > 0");
        modelBuilder.Entity<ProductRelationshipDto>().HasCheckConstraint("CK_ProductRelationship_Description", "[Description] IS NULL OR LEN(TRIM([Description])) > 0");

        modelBuilder.Entity<WorkDto>().ToTable("Work", "dbo");
        modelBuilder.Entity<WorkDto>().HasKey(entity => entity.Id);
        modelBuilder.Entity<WorkDto>()
            .HasIndex(entity => entity.InternationalStandardMusicalWorkCode)
            .HasDatabaseName("IX_Work_InternationalStandardMusicalWorkCode");
        modelBuilder.Entity<WorkDto>().HasCheckConstraint("CK_Work_Title", "LEN(TRIM([Title])) > 0");
        modelBuilder.Entity<WorkDto>().HasCheckConstraint("CK_Work_Description", "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
        modelBuilder.Entity<WorkDto>().HasCheckConstraint("CK_Work_DisambiguationText", "[DisambiguationText] IS NULL OR LEN(TRIM([DisambiguationText])) > 0");
        modelBuilder.Entity<WorkDto>().HasCheckConstraint("CK_Work_InternationalStandardMusicalWorkCode", "[InternationalStandardMusicalWorkCode] IS NULL OR LEN(TRIM([InternationalStandardMusicalWorkCode])) > 0");
        modelBuilder.Entity<WorkDto>()
            .Property(entity => entity.ReleasedOn)
            .HasColumnType("date");

        base.OnModelCreating(modelBuilder);
    }
}
