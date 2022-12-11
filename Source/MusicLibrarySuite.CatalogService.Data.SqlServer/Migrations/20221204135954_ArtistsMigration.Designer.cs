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
    [Migration("20221204135954_ArtistsMigration")]
    partial class ArtistsMigration
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

            modelBuilder.Entity("MusicLibrarySuite.CatalogService.Data.Entities.GenreDto", b =>
                {
                    b.Navigation("GenreRelationships");
                });
#pragma warning restore 612, 618
        }
    }
}