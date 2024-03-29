// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MusicLibrarySuite.CatalogService.Data.SqlServer.Contexts;

#nullable disable

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations
{
    [DbContext(typeof(SqlServerCatalogServiceDbContext))]
    [Migration($"20221216163550_{nameof(ArtistGenreMigration)}")]
    partial class ArtistGenreMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("MusicLibrarySuite.CatalogService.Data.Entities.ArtistDto", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<string>("DisambiguationText")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("SystemEntity")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("UpdatedOn")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.ToTable("Artist", "dbo");

                    b.HasCheckConstraint("CK_Artist_Description", "[Description] IS NULL OR LEN(TRIM([Description])) > 0");

                    b.HasCheckConstraint("CK_Artist_DisambiguationText", "[DisambiguationText] IS NULL OR LEN(TRIM([DisambiguationText])) > 0");

                    b.HasCheckConstraint("CK_Artist_Name", "LEN(TRIM([Name])) > 0");
                });

            modelBuilder.Entity("MusicLibrarySuite.CatalogService.Data.Entities.ArtistGenreDto", b =>
                {
                    b.Property<Guid>("ArtistId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GenreId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.HasKey("ArtistId", "GenreId");

                    b.HasIndex("ArtistId")
                        .HasDatabaseName("IX_ArtistGenre_ArtistId");

                    b.HasIndex("GenreId")
                        .HasDatabaseName("IX_ArtistGenre_GenreId");

                    b.HasIndex("ArtistId", "Order")
                        .IsUnique()
                        .HasDatabaseName("UIX_ArtistGenre_ArtistId_Order");

                    b.ToTable("ArtistGenre", "dbo");
                });

            modelBuilder.Entity("MusicLibrarySuite.CatalogService.Data.Entities.ArtistRelationshipDto", b =>
                {
                    b.Property<Guid>("ArtistId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DependentArtistId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.HasKey("ArtistId", "DependentArtistId");

                    b.HasIndex("ArtistId")
                        .HasDatabaseName("IX_ArtistRelationship_ArtistId");

                    b.HasIndex("DependentArtistId")
                        .HasDatabaseName("IX_ArtistRelationship_DependentArtistId");

                    b.HasIndex("ArtistId", "Order")
                        .IsUnique()
                        .HasDatabaseName("UIX_ArtistRelationship_ArtistId_Order");

                    b.ToTable("ArtistRelationship", "dbo");

                    b.HasCheckConstraint("CK_ArtistRelationship_Description", "[Description] IS NULL OR LEN(TRIM([Description])) > 0");

                    b.HasCheckConstraint("CK_ArtistRelationship_Name", "LEN(TRIM([Name])) > 0");
                });

            modelBuilder.Entity("MusicLibrarySuite.CatalogService.Data.Entities.GenreDto", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("SystemEntity")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("UpdatedOn")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.ToTable("Genre", "dbo");

                    b.HasCheckConstraint("CK_Genre_Description", "[Description] IS NULL OR LEN(TRIM([Description])) > 0");

                    b.HasCheckConstraint("CK_Genre_Name", "LEN(TRIM([Name])) > 0");
                });

            modelBuilder.Entity("MusicLibrarySuite.CatalogService.Data.Entities.GenreRelationshipDto", b =>
                {
                    b.Property<Guid>("GenreId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DependentGenreId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.HasKey("GenreId", "DependentGenreId");

                    b.HasIndex("DependentGenreId")
                        .HasDatabaseName("IX_GenreRelationship_DependentGenreId");

                    b.HasIndex("GenreId")
                        .HasDatabaseName("IX_GenreRelationship_GenreId");

                    b.HasIndex("GenreId", "Order")
                        .IsUnique()
                        .HasDatabaseName("UIX_GenreRelationship_GenreId_Order");

                    b.ToTable("GenreRelationship", "dbo");

                    b.HasCheckConstraint("CK_GenreRelationship_Description", "[Description] IS NULL OR LEN(TRIM([Description])) > 0");

                    b.HasCheckConstraint("CK_GenreRelationship_Name", "LEN(TRIM([Name])) > 0");
                });

            modelBuilder.Entity("MusicLibrarySuite.CatalogService.Data.Entities.ArtistGenreDto", b =>
                {
                    b.HasOne("MusicLibrarySuite.CatalogService.Data.Entities.ArtistDto", "Artist")
                        .WithMany("ArtistGenres")
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicLibrarySuite.CatalogService.Data.Entities.GenreDto", "Genre")
                        .WithMany()
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artist");

                    b.Navigation("Genre");
                });

            modelBuilder.Entity("MusicLibrarySuite.CatalogService.Data.Entities.ArtistRelationshipDto", b =>
                {
                    b.HasOne("MusicLibrarySuite.CatalogService.Data.Entities.ArtistDto", "Artist")
                        .WithMany("ArtistRelationships")
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicLibrarySuite.CatalogService.Data.Entities.ArtistDto", "DependentArtist")
                        .WithMany()
                        .HasForeignKey("DependentArtistId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Artist");

                    b.Navigation("DependentArtist");
                });

            modelBuilder.Entity("MusicLibrarySuite.CatalogService.Data.Entities.GenreRelationshipDto", b =>
                {
                    b.HasOne("MusicLibrarySuite.CatalogService.Data.Entities.GenreDto", "DependentGenre")
                        .WithMany()
                        .HasForeignKey("DependentGenreId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MusicLibrarySuite.CatalogService.Data.Entities.GenreDto", "Genre")
                        .WithMany("GenreRelationships")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DependentGenre");

                    b.Navigation("Genre");
                });

            modelBuilder.Entity("MusicLibrarySuite.CatalogService.Data.Entities.ArtistDto", b =>
                {
                    b.Navigation("ArtistGenres");

                    b.Navigation("ArtistRelationships");
                });

            modelBuilder.Entity("MusicLibrarySuite.CatalogService.Data.Entities.GenreDto", b =>
                {
                    b.Navigation("GenreRelationships");
                });
#pragma warning restore 612, 618
        }
    }
}
