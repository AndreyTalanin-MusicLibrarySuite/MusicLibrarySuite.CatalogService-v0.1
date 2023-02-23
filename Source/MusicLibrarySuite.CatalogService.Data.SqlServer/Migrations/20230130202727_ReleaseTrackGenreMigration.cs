using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ReleaseTrackGenreDto" /> entity.
/// </summary>
public partial class ReleaseTrackGenreMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ReleaseTrackGenre",
            schema: "dbo",
            columns: table => new
            {
                TrackNumber = table.Column<byte>(type: "tinyint", nullable: false),
                MediaNumber = table.Column<byte>(type: "tinyint", nullable: false),
                ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                GenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ReleaseTrackGenre", columns: x => new { x.TrackNumber, x.MediaNumber, x.ReleaseId, x.GenreId });
                table.ForeignKey(
                    name: "FK_ReleaseTrackGenre_ReleaseTrack_TrackNumber_MediaNumber_ReleaseId",
                    columns: x => new { x.TrackNumber, x.MediaNumber, x.ReleaseId },
                    principalSchema: "dbo",
                    principalTable: "ReleaseTrack",
                    principalColumns: new[] { "TrackNumber", "MediaNumber", "ReleaseId" },
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ReleaseTrackGenre_Genre_GenreId",
                    column: x => x.GenreId,
                    principalSchema: "dbo",
                    principalTable: "Genre",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseTrackGenre_TrackNumber_MediaNumber_ReleaseId",
            schema: "dbo",
            table: "ReleaseTrackGenre",
            columns: new[] { "TrackNumber", "MediaNumber", "ReleaseId" });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseTrackGenre_GenreId",
            schema: "dbo",
            table: "ReleaseTrackGenre",
            column: "GenreId");

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseTrackGenre_TrackNumber_MediaNumber_ReleaseId_Order",
            schema: "dbo",
            table: "ReleaseTrackGenre",
            columns: new[] { "TrackNumber", "MediaNumber", "ReleaseId", "Order" },
            unique: true);

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleaseTrackGenre_AfterInsertUpdateDelete_SetReleaseUpdatedOn]
            ON [dbo].[ReleaseTrackGenre]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedRelease] AS
                (
                    SELECT [insertedReleaseTrackGenre].[ReleaseId] AS [Id]
                    FROM [inserted] AS [insertedReleaseTrackGenre]
                    UNION
                    SELECT [deletedReleaseTrackGenre].[ReleaseId] AS [Id]
                    FROM [deleted] AS [deletedReleaseTrackGenre]
                )
                UPDATE [dbo].[Release]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Release] AS [release]
                INNER JOIN [UpdatedRelease] AS [updatedRelease] ON [updatedRelease].[Id] = [release].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleaseTrackGenre] AS TABLE
            (
                [TrackNumber] TINYINT NOT NULL,
                [MediaNumber] TINYINT NOT NULL,
                [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
                [GenreId] UNIQUEIDENTIFIER NOT NULL,
                [Order] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_Internal_MergeReleaseTrackGenres]
            (
                @Id UNIQUEIDENTIFIER,
                @ReleaseTrackGenres [dbo].[ReleaseTrackGenre] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [SourceReleaseTrackGenre] AS
                (
                    SELECT
                        [TrackNumber],
                        [MediaNumber],
                        @Id AS [ReleaseId],
                        [GenreId],
                        [Order]
                    FROM @ReleaseTrackGenres
                    WHERE [ReleaseId] = CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER)
                        OR [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseTrackGenre] AS [target]
                USING [SourceReleaseTrackGenre] AS [source]
                ON [target].[TrackNumber] = [source].[TrackNumber]
                    AND [target].[MediaNumber] = [source].[MediaNumber]
                    AND [target].[ReleaseId] = [source].[ReleaseId]
                    AND [target].[GenreId] = [source].[GenreId]
                WHEN MATCHED THEN UPDATE
                SET [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [TrackNumber],
                    [MediaNumber],
                    [ReleaseId],
                    [GenreId],
                    [Order]
                )
                VALUES
                (
                    [source].[TrackNumber],
                    [source].[MediaNumber],
                    [source].[ReleaseId],
                    [source].[GenreId],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;
            END;");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateRelease];");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_CreateRelease]
            (
                @Id UNIQUEIDENTIFIER,
                @Title NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
                @Barcode NVARCHAR(32),
                @CatalogNumber NVARCHAR(32),
                @MediaFormat NVARCHAR(256),
                @PublishFormat NVARCHAR(256),
                @ReleasedOn DATE,
                @ReleasedOnYearOnly BIT,
                @Enabled BIT,
                @ReleaseRelationships [dbo].[ReleaseRelationship] READONLY,
                @ReleaseToProductRelationships [dbo].[ReleaseToProductRelationship] READONLY,
                @ReleaseToReleaseGroupRelationships [dbo].[ReleaseToReleaseGroupRelationship] READONLY,
                @ReleaseArtists [dbo].[ReleaseArtist] READONLY,
                @ReleaseFeaturedArtists [dbo].[ReleaseFeaturedArtist] READONLY,
                @ReleasePerformers [dbo].[ReleasePerformer] READONLY,
                @ReleaseComposers [dbo].[ReleaseComposer] READONLY,
                @ReleaseGenres [dbo].[ReleaseGenre] READONLY,
                @ReleaseMediaCollection [dbo].[ReleaseMedia] READONLY,
                @ReleaseMediaToProductRelationships [dbo].[ReleaseMediaToProductRelationship] READONLY,
                @ReleaseTrackCollection [dbo].[ReleaseTrack] READONLY,
                @ReleaseTrackArtists [dbo].[ReleaseTrackArtist] READONLY,
                @ReleaseTrackFeaturedArtists [dbo].[ReleaseTrackFeaturedArtist] READONLY,
                @ReleaseTrackPerformers [dbo].[ReleaseTrackPerformer] READONLY,
                @ReleaseTrackComposers [dbo].[ReleaseTrackComposer] READONLY,
                @ReleaseTrackGenres [dbo].[ReleaseTrackGenre] READONLY,
                @ResultId UNIQUEIDENTIFIER OUTPUT,
                @ResultCreatedOn DATETIMEOFFSET OUTPUT,
                @ResultUpdatedOn DATETIMEOFFSET OUTPUT
            )
            AS
            BEGIN
                SET NOCOUNT ON;

                IF (@Id = CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER))
                BEGIN
                    SET @Id = NEWID();
                END;

                BEGIN TRANSACTION;

                EXEC [dbo].[sp_Internal_MergeRelease]
                    @Id,
                    @Title,
                    @Description,
                    @DisambiguationText,
                    @Barcode,
                    @CatalogNumber,
                    @MediaFormat,
                    @PublishFormat,
                    @ReleasedOn,
                    @ReleasedOnYearOnly,
                    @Enabled,
                    NULL;

                EXEC [dbo].[sp_Internal_MergeReleaseRelationships]
                    @Id,
                    @ReleaseRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseToProductRelationships]
                    @Id,
                    @ReleaseToProductRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseToReleaseGroupRelationships]
                    @Id,
                    @ReleaseToReleaseGroupRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseArtists]
                    @Id,
                    @ReleaseArtists;

                EXEC [dbo].[sp_Internal_MergeReleaseFeaturedArtists]
                    @Id,
                    @ReleaseFeaturedArtists;

                EXEC [dbo].[sp_Internal_MergeReleasePerformers]
                    @Id,
                    @ReleasePerformers;

                EXEC [dbo].[sp_Internal_MergeReleaseComposers]
                    @Id,
                    @ReleaseComposers;

                EXEC [dbo].[sp_Internal_MergeReleaseGenres]
                    @Id,
                    @ReleaseGenres;

                EXEC [dbo].[sp_Internal_MergeReleaseMediaCollection]
                    @Id,
                    @ReleaseMediaCollection;

                EXEC [dbo].[sp_Internal_MergeReleaseMediaToProductRelationships]
                    @Id,
                    @ReleaseMediaToProductRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackCollection]
                    @Id,
                    @ReleaseTrackCollection;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackArtists]
                    @Id,
                    @ReleaseTrackArtists;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackFeaturedArtists]
                    @Id,
                    @ReleaseTrackFeaturedArtists;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackPerformers]
                    @Id,
                    @ReleaseTrackPerformers;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackComposers]
                    @Id,
                    @ReleaseTrackComposers;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackGenres]
                    @Id,
                    @ReleaseTrackGenres;

                COMMIT TRANSACTION;

                SELECT TOP (1)
                    @ResultId = @Id,
                    @ResultCreatedOn = [release].[CreatedOn],
                    @ResultUpdatedOn = [release].[UpdatedOn]
                FROM [dbo].[Release] AS [release]
                WHERE [release].[Id] = @Id;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_UpdateRelease]
            (
                @Id UNIQUEIDENTIFIER,
                @Title NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
                @Barcode NVARCHAR(32),
                @CatalogNumber NVARCHAR(32),
                @MediaFormat NVARCHAR(256),
                @PublishFormat NVARCHAR(256),
                @ReleasedOn DATE,
                @ReleasedOnYearOnly BIT,
                @Enabled BIT,
                @ReleaseRelationships [dbo].[ReleaseRelationship] READONLY,
                @ReleaseToProductRelationships [dbo].[ReleaseToProductRelationship] READONLY,
                @ReleaseToReleaseGroupRelationships [dbo].[ReleaseToReleaseGroupRelationship] READONLY,
                @ReleaseArtists [dbo].[ReleaseArtist] READONLY,
                @ReleaseFeaturedArtists [dbo].[ReleaseFeaturedArtist] READONLY,
                @ReleasePerformers [dbo].[ReleasePerformer] READONLY,
                @ReleaseComposers [dbo].[ReleaseComposer] READONLY,
                @ReleaseGenres [dbo].[ReleaseGenre] READONLY,
                @ReleaseMediaCollection [dbo].[ReleaseMedia] READONLY,
                @ReleaseMediaToProductRelationships [dbo].[ReleaseMediaToProductRelationship] READONLY,
                @ReleaseTrackCollection [dbo].[ReleaseTrack] READONLY,
                @ReleaseTrackArtists [dbo].[ReleaseTrackArtist] READONLY,
                @ReleaseTrackFeaturedArtists [dbo].[ReleaseTrackFeaturedArtist] READONLY,
                @ReleaseTrackPerformers [dbo].[ReleaseTrackPerformer] READONLY,
                @ReleaseTrackComposers [dbo].[ReleaseTrackComposer] READONLY,
                @ReleaseTrackGenres [dbo].[ReleaseTrackGenre] READONLY,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                BEGIN TRANSACTION;

                EXEC [dbo].[sp_Internal_MergeRelease]
                    @Id,
                    @Title,
                    @Description,
                    @DisambiguationText,
                    @Barcode,
                    @CatalogNumber,
                    @MediaFormat,
                    @PublishFormat,
                    @ReleasedOn,
                    @ReleasedOnYearOnly,
                    @Enabled,
                    @ResultRowsUpdated OUTPUT;

                EXEC [dbo].[sp_Internal_MergeReleaseRelationships]
                    @Id,
                    @ReleaseRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseToProductRelationships]
                    @Id,
                    @ReleaseToProductRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseToReleaseGroupRelationships]
                    @Id,
                    @ReleaseToReleaseGroupRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseArtists]
                    @Id,
                    @ReleaseArtists;

                EXEC [dbo].[sp_Internal_MergeReleaseFeaturedArtists]
                    @Id,
                    @ReleaseFeaturedArtists;

                EXEC [dbo].[sp_Internal_MergeReleasePerformers]
                    @Id,
                    @ReleasePerformers;

                EXEC [dbo].[sp_Internal_MergeReleaseComposers]
                    @Id,
                    @ReleaseComposers;

                EXEC [dbo].[sp_Internal_MergeReleaseGenres]
                    @Id,
                    @ReleaseGenres;

                EXEC [dbo].[sp_Internal_MergeReleaseMediaCollection]
                    @Id,
                    @ReleaseMediaCollection;

                EXEC [dbo].[sp_Internal_MergeReleaseMediaToProductRelationships]
                    @Id,
                    @ReleaseMediaToProductRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackCollection]
                    @Id,
                    @ReleaseTrackCollection;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackArtists]
                    @Id,
                    @ReleaseTrackArtists;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackFeaturedArtists]
                    @Id,
                    @ReleaseTrackFeaturedArtists;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackPerformers]
                    @Id,
                    @ReleaseTrackPerformers;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackComposers]
                    @Id,
                    @ReleaseTrackComposers;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackGenres]
                    @Id,
                    @ReleaseTrackGenres;

                COMMIT TRANSACTION;
            END;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_Internal_MergeReleaseTrackGenres];");

        migrationBuilder.DropTable(name: "ReleaseTrackGenre", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleaseTrackGenre];");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_CreateRelease]
            (
                @Id UNIQUEIDENTIFIER,
                @Title NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
                @Barcode NVARCHAR(32),
                @CatalogNumber NVARCHAR(32),
                @MediaFormat NVARCHAR(256),
                @PublishFormat NVARCHAR(256),
                @ReleasedOn DATE,
                @ReleasedOnYearOnly BIT,
                @Enabled BIT,
                @ReleaseRelationships [dbo].[ReleaseRelationship] READONLY,
                @ReleaseToProductRelationships [dbo].[ReleaseToProductRelationship] READONLY,
                @ReleaseToReleaseGroupRelationships [dbo].[ReleaseToReleaseGroupRelationship] READONLY,
                @ReleaseArtists [dbo].[ReleaseArtist] READONLY,
                @ReleaseFeaturedArtists [dbo].[ReleaseFeaturedArtist] READONLY,
                @ReleasePerformers [dbo].[ReleasePerformer] READONLY,
                @ReleaseComposers [dbo].[ReleaseComposer] READONLY,
                @ReleaseGenres [dbo].[ReleaseGenre] READONLY,
                @ReleaseMediaCollection [dbo].[ReleaseMedia] READONLY,
                @ReleaseMediaToProductRelationships [dbo].[ReleaseMediaToProductRelationship] READONLY,
                @ReleaseTrackCollection [dbo].[ReleaseTrack] READONLY,
                @ReleaseTrackArtists [dbo].[ReleaseTrackArtist] READONLY,
                @ReleaseTrackFeaturedArtists [dbo].[ReleaseTrackFeaturedArtist] READONLY,
                @ReleaseTrackPerformers [dbo].[ReleaseTrackPerformer] READONLY,
                @ReleaseTrackComposers [dbo].[ReleaseTrackComposer] READONLY,
                @ResultId UNIQUEIDENTIFIER OUTPUT,
                @ResultCreatedOn DATETIMEOFFSET OUTPUT,
                @ResultUpdatedOn DATETIMEOFFSET OUTPUT
            )
            AS
            BEGIN
                SET NOCOUNT ON;

                IF (@Id = CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER))
                BEGIN
                    SET @Id = NEWID();
                END;

                BEGIN TRANSACTION;

                EXEC [dbo].[sp_Internal_MergeRelease]
                    @Id,
                    @Title,
                    @Description,
                    @DisambiguationText,
                    @Barcode,
                    @CatalogNumber,
                    @MediaFormat,
                    @PublishFormat,
                    @ReleasedOn,
                    @ReleasedOnYearOnly,
                    @Enabled,
                    NULL;

                EXEC [dbo].[sp_Internal_MergeReleaseRelationships]
                    @Id,
                    @ReleaseRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseToProductRelationships]
                    @Id,
                    @ReleaseToProductRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseToReleaseGroupRelationships]
                    @Id,
                    @ReleaseToReleaseGroupRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseArtists]
                    @Id,
                    @ReleaseArtists;

                EXEC [dbo].[sp_Internal_MergeReleaseFeaturedArtists]
                    @Id,
                    @ReleaseFeaturedArtists;

                EXEC [dbo].[sp_Internal_MergeReleasePerformers]
                    @Id,
                    @ReleasePerformers;

                EXEC [dbo].[sp_Internal_MergeReleaseComposers]
                    @Id,
                    @ReleaseComposers;

                EXEC [dbo].[sp_Internal_MergeReleaseGenres]
                    @Id,
                    @ReleaseGenres;

                EXEC [dbo].[sp_Internal_MergeReleaseMediaCollection]
                    @Id,
                    @ReleaseMediaCollection;

                EXEC [dbo].[sp_Internal_MergeReleaseMediaToProductRelationships]
                    @Id,
                    @ReleaseMediaToProductRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackCollection]
                    @Id,
                    @ReleaseTrackCollection;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackArtists]
                    @Id,
                    @ReleaseTrackArtists;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackFeaturedArtists]
                    @Id,
                    @ReleaseTrackFeaturedArtists;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackPerformers]
                    @Id,
                    @ReleaseTrackPerformers;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackComposers]
                    @Id,
                    @ReleaseTrackComposers;

                COMMIT TRANSACTION;

                SELECT TOP (1)
                    @ResultId = @Id,
                    @ResultCreatedOn = [release].[CreatedOn],
                    @ResultUpdatedOn = [release].[UpdatedOn]
                FROM [dbo].[Release] AS [release]
                WHERE [release].[Id] = @Id;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_UpdateRelease]
            (
                @Id UNIQUEIDENTIFIER,
                @Title NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
                @Barcode NVARCHAR(32),
                @CatalogNumber NVARCHAR(32),
                @MediaFormat NVARCHAR(256),
                @PublishFormat NVARCHAR(256),
                @ReleasedOn DATE,
                @ReleasedOnYearOnly BIT,
                @Enabled BIT,
                @ReleaseRelationships [dbo].[ReleaseRelationship] READONLY,
                @ReleaseToProductRelationships [dbo].[ReleaseToProductRelationship] READONLY,
                @ReleaseToReleaseGroupRelationships [dbo].[ReleaseToReleaseGroupRelationship] READONLY,
                @ReleaseArtists [dbo].[ReleaseArtist] READONLY,
                @ReleaseFeaturedArtists [dbo].[ReleaseFeaturedArtist] READONLY,
                @ReleasePerformers [dbo].[ReleasePerformer] READONLY,
                @ReleaseComposers [dbo].[ReleaseComposer] READONLY,
                @ReleaseGenres [dbo].[ReleaseGenre] READONLY,
                @ReleaseMediaCollection [dbo].[ReleaseMedia] READONLY,
                @ReleaseMediaToProductRelationships [dbo].[ReleaseMediaToProductRelationship] READONLY,
                @ReleaseTrackCollection [dbo].[ReleaseTrack] READONLY,
                @ReleaseTrackArtists [dbo].[ReleaseTrackArtist] READONLY,
                @ReleaseTrackFeaturedArtists [dbo].[ReleaseTrackFeaturedArtist] READONLY,
                @ReleaseTrackPerformers [dbo].[ReleaseTrackPerformer] READONLY,
                @ReleaseTrackComposers [dbo].[ReleaseTrackComposer] READONLY,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                BEGIN TRANSACTION;

                EXEC [dbo].[sp_Internal_MergeRelease]
                    @Id,
                    @Title,
                    @Description,
                    @DisambiguationText,
                    @Barcode,
                    @CatalogNumber,
                    @MediaFormat,
                    @PublishFormat,
                    @ReleasedOn,
                    @ReleasedOnYearOnly,
                    @Enabled,
                    @ResultRowsUpdated OUTPUT;

                EXEC [dbo].[sp_Internal_MergeReleaseRelationships]
                    @Id,
                    @ReleaseRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseToProductRelationships]
                    @Id,
                    @ReleaseToProductRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseToReleaseGroupRelationships]
                    @Id,
                    @ReleaseToReleaseGroupRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseArtists]
                    @Id,
                    @ReleaseArtists;

                EXEC [dbo].[sp_Internal_MergeReleaseFeaturedArtists]
                    @Id,
                    @ReleaseFeaturedArtists;

                EXEC [dbo].[sp_Internal_MergeReleasePerformers]
                    @Id,
                    @ReleasePerformers;

                EXEC [dbo].[sp_Internal_MergeReleaseComposers]
                    @Id,
                    @ReleaseComposers;

                EXEC [dbo].[sp_Internal_MergeReleaseGenres]
                    @Id,
                    @ReleaseGenres;

                EXEC [dbo].[sp_Internal_MergeReleaseMediaCollection]
                    @Id,
                    @ReleaseMediaCollection;

                EXEC [dbo].[sp_Internal_MergeReleaseMediaToProductRelationships]
                    @Id,
                    @ReleaseMediaToProductRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackCollection]
                    @Id,
                    @ReleaseTrackCollection;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackArtists]
                    @Id,
                    @ReleaseTrackArtists;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackFeaturedArtists]
                    @Id,
                    @ReleaseTrackFeaturedArtists;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackPerformers]
                    @Id,
                    @ReleaseTrackPerformers;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackComposers]
                    @Id,
                    @ReleaseTrackComposers;

                COMMIT TRANSACTION;
            END;");
    }
}
