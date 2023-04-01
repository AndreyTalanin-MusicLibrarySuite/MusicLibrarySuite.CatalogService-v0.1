using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ReleaseTrackToProductRelationshipDto" /> entity.
/// </summary>
public partial class ReleaseTrackToProductRelationshipMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ReleaseTrackToProductRelationship",
            schema: "dbo",
            columns: table => new
            {
                TrackNumber = table.Column<byte>(type: "tinyint", nullable: false),
                MediaNumber = table.Column<byte>(type: "tinyint", nullable: false),
                ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                Order = table.Column<int>(type: "int", nullable: false),
                ReferenceOrder = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ReleaseTrackToProductRelationship", columns: x => new { x.TrackNumber, x.MediaNumber, x.ReleaseId, x.ProductId });
                table.ForeignKey(
                    name: "FK_ReleaseTrackToProductRelationship_ReleaseTrack_TrackNumber_MediaNumber_ReleaseId",
                    columns: x => new { x.TrackNumber, x.MediaNumber, x.ReleaseId },
                    principalSchema: "dbo",
                    principalTable: "ReleaseTrack",
                    principalColumns: new[] { "TrackNumber", "MediaNumber", "ReleaseId" },
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ReleaseTrackToProductRelationship_Product_ProductId",
                    column: x => x.ProductId,
                    principalSchema: "dbo",
                    principalTable: "Product",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.CheckConstraint(name: "CK_ReleaseTrackToProductRelationship_Name", sql: "LEN(TRIM([Name])) > 0");
                table.CheckConstraint(name: "CK_ReleaseTrackToProductRelationship_Description", sql: "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseTrackToProductRelationship_TrackNumber_MediaNumber_ReleaseId",
            schema: "dbo",
            table: "ReleaseTrackToProductRelationship",
            columns: new[] { "TrackNumber", "MediaNumber", "ReleaseId" });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseTrackToProductRelationship_ProductId",
            schema: "dbo",
            table: "ReleaseTrackToProductRelationship",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseTrackToProductRelationship_TrackNumber_MediaNumber_ReleaseId_Order",
            schema: "dbo",
            table: "ReleaseTrackToProductRelationship",
            columns: new[] { "TrackNumber", "MediaNumber", "ReleaseId", "Order" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseTrackToProductRelationship_ProductId_ReferenceOrder",
            schema: "dbo",
            table: "ReleaseTrackToProductRelationship",
            columns: new[] { "ProductId", "ReferenceOrder" },
            unique: true);

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleaseTrackToProductRelationship_AfterInsertUpdateDelete_SetReleaseUpdatedOn]
            ON [dbo].[ReleaseTrackToProductRelationship]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedRelease] AS
                (
                    SELECT [insertedReleaseTrackToProductRelationship].[ReleaseId] AS [Id]
                    FROM [inserted] AS [insertedReleaseTrackToProductRelationship]
                    UNION
                    SELECT [deletedReleaseTrackToProductRelationship].[ReleaseId] AS [Id]
                    FROM [deleted] AS [deletedReleaseTrackToProductRelationship]
                )
                UPDATE [dbo].[Release]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Release] AS [release]
                INNER JOIN [UpdatedRelease] AS [updatedRelease] ON [updatedRelease].[Id] = [release].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleaseTrackToProductRelationship] AS TABLE
            (
                [TrackNumber] TINYINT NOT NULL,
                [MediaNumber] TINYINT NOT NULL,
                [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
                [ProductId] UNIQUEIDENTIFIER NOT NULL,
                [Name] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
                [Order] INT NOT NULL,
                [ReferenceOrder] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_Internal_MergeReleaseTrackToProductRelationships]
            (
                @Id UNIQUEIDENTIFIER,
                @ReleaseTrackToProductRelationships [dbo].[ReleaseTrackToProductRelationship] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [SourceReleaseTrackToProductRelationship] AS
                (
                    SELECT
                        [sourceReleaseTrackToProductRelationship].[TrackNumber],
                        [sourceReleaseTrackToProductRelationship].[MediaNumber],
                        @Id AS [ReleaseId],
                        [sourceReleaseTrackToProductRelationship].[ProductId],
                        [sourceReleaseTrackToProductRelationship].[Name],
                        [sourceReleaseTrackToProductRelationship].[Description],
                        [sourceReleaseTrackToProductRelationship].[Order],
                        COALESCE([targetReleaseTrackToProductRelationship].[ReferenceOrder],
                            ROW_NUMBER() OVER (PARTITION BY [sourceReleaseTrackToProductRelationship].[ProductId],
                                CASE WHEN [targetReleaseTrackToProductRelationship].[ReferenceOrder] IS NOT NULL THEN 1 ELSE 0 END
                                ORDER BY [sourceReleaseTrackToProductRelationship].[MediaNumber],
                                    [sourceReleaseTrackToProductRelationship].[TrackNumber])
                                + COALESCE(MAX([productReleaseTrackToProductRelationship].[ReferenceOrder]) + 1, 0) - 1) AS [ReferenceOrder]
                    FROM @ReleaseTrackToProductRelationships AS [sourceReleaseTrackToProductRelationship]
                    LEFT JOIN [dbo].[ReleaseTrackToProductRelationship] AS [targetReleaseTrackToProductRelationship]
                        ON [targetReleaseTrackToProductRelationship].[TrackNumber] = [sourceReleaseTrackToProductRelationship].[TrackNumber]
                        AND [targetReleaseTrackToProductRelationship].[MediaNumber] = [sourceReleaseTrackToProductRelationship].[MediaNumber]
                        AND [targetReleaseTrackToProductRelationship].[ReleaseId] = [sourceReleaseTrackToProductRelationship].[ReleaseId]
                        AND [targetReleaseTrackToProductRelationship].[ProductId] = [sourceReleaseTrackToProductRelationship].[ProductId]
                    LEFT JOIN [dbo].[ReleaseTrackToProductRelationship] AS [productReleaseTrackToProductRelationship]
                        ON [targetReleaseTrackToProductRelationship].[ReferenceOrder] IS NULL
                        AND [productReleaseTrackToProductRelationship].[ProductId] = [sourceReleaseTrackToProductRelationship].[ProductId]
                    WHERE [sourceReleaseTrackToProductRelationship].[ReleaseId] = CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER)
                        OR [sourceReleaseTrackToProductRelationship].[ReleaseId] = @Id
                    GROUP BY
                        [sourceReleaseTrackToProductRelationship].[TrackNumber],
                        [sourceReleaseTrackToProductRelationship].[MediaNumber],
                        [sourceReleaseTrackToProductRelationship].[ReleaseId],
                        [sourceReleaseTrackToProductRelationship].[ProductId],
                        [sourceReleaseTrackToProductRelationship].[Name],
                        [sourceReleaseTrackToProductRelationship].[Description],
                        [sourceReleaseTrackToProductRelationship].[Order],
                        [targetReleaseTrackToProductRelationship].[ReferenceOrder]
                )
                MERGE INTO [dbo].[ReleaseTrackToProductRelationship] AS [target]
                USING [SourceReleaseTrackToProductRelationship] AS [source]
                ON [target].[TrackNumber] = [source].[TrackNumber]
                    AND [target].[MediaNumber] = [source].[MediaNumber]
                    AND [target].[ReleaseId] = [source].[ReleaseId]
                    AND [target].[ProductId] = [source].[ProductId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Name] = [source].[Name],
                    [target].[Description] = [source].[Description],
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [TrackNumber],
                    [MediaNumber],
                    [ReleaseId],
                    [ProductId],
                    [Name],
                    [Description],
                    [Order],
                    [ReferenceOrder]
                )
                VALUES
                (
                    [source].[TrackNumber],
                    [source].[MediaNumber],
                    [source].[ReleaseId],
                    [source].[ProductId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order],
                    [source].[ReferenceOrder]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                WITH [UpdatedReleaseTrackToProductRelationship] AS
                (
                    SELECT
                        [releaseTrackToProductRelationship].[TrackNumber],
                        [releaseTrackToProductRelationship].[MediaNumber],
                        [releaseTrackToProductRelationship].[ReleaseId],
                        [releaseTrackToProductRelationship].[ProductId],
                        ROW_NUMBER() OVER (PARTITION BY [releaseTrackToProductRelationship].[ProductId]
                            ORDER BY [releaseTrackToProductRelationship].[ReferenceOrder]) - 1 AS [UpdatedReferenceOrder]
                    FROM [dbo].[ReleaseTrackToProductRelationship] [releaseTrackToProductRelationship]
                )
                UPDATE [releaseTrackToProductRelationship]
                SET [ReferenceOrder] = [updatedReleaseTrackToProductRelationship].[UpdatedReferenceOrder]
                FROM [dbo].[ReleaseTrackToProductRelationship] AS [releaseTrackToProductRelationship]
                INNER JOIN [UpdatedReleaseTrackToProductRelationship] AS [updatedReleaseTrackToProductRelationship]
                    ON [updatedReleaseTrackToProductRelationship].[TrackNumber] = [releaseTrackToProductRelationship].[TrackNumber]
                    AND [updatedReleaseTrackToProductRelationship].[MediaNumber] = [releaseTrackToProductRelationship].[MediaNumber]
                    AND [updatedReleaseTrackToProductRelationship].[ReleaseId] = [releaseTrackToProductRelationship].[ReleaseId]
                    AND [updatedReleaseTrackToProductRelationship].[ProductId] = [releaseTrackToProductRelationship].[ProductId];
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
                @MediaFormat NVARCHAR(256),
                @PublishFormat NVARCHAR(256),
                @CatalogNumber NVARCHAR(32),
                @Barcode NVARCHAR(32),
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
                @ReleaseTrackToProductRelationships [dbo].[ReleaseTrackToProductRelationship] READONLY,
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
                    @MediaFormat,
                    @PublishFormat,
                    @CatalogNumber,
                    @Barcode,
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

                EXEC [dbo].[sp_Internal_MergeReleaseTrackToProductRelationships]
                    @Id,
                    @ReleaseTrackToProductRelationships;

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
                @MediaFormat NVARCHAR(256),
                @PublishFormat NVARCHAR(256),
                @CatalogNumber NVARCHAR(32),
                @Barcode NVARCHAR(32),
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
                @ReleaseTrackToProductRelationships [dbo].[ReleaseTrackToProductRelationship] READONLY,
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
                    @MediaFormat,
                    @PublishFormat,
                    @CatalogNumber,
                    @Barcode,
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

                EXEC [dbo].[sp_Internal_MergeReleaseTrackToProductRelationships]
                    @Id,
                    @ReleaseTrackToProductRelationships;

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

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_UpdateReleaseTrackToProductRelationshipsOrder]
            (
                @UseReferenceOrder BIT,
                @ReleaseTrackToProductRelationships [dbo].[ReleaseTrackToProductRelationship] READONLY,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                WITH [SourceReleaseTrackToProductRelationship] AS
                (
                    SELECT * FROM @ReleaseTrackToProductRelationships
                )
                MERGE INTO [dbo].[ReleaseTrackToProductRelationship] AS [target]
                USING [SourceReleaseTrackToProductRelationship] AS [source]
                ON [target].[TrackNumber] = [source].[TrackNumber]
                    AND [target].[MediaNumber] = [source].[MediaNumber]
                    AND [target].[ReleaseId] = [source].[ReleaseId]
                    AND [target].[ProductId] = [source].[ProductId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Order] = CASE WHEN @UseReferenceOrder = 0 THEN [source].[Order] ELSE [target].[Order] END,
                    [target].[ReferenceOrder] = CASE WHEN @UseReferenceOrder = 1 THEN [source].[ReferenceOrder] ELSE [target].[ReferenceOrder] END;

                SET @ResultRowsUpdated = @@ROWCOUNT;
            END;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateReleaseTrackToProductRelationshipsOrder];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_Internal_MergeReleaseTrackToProductRelationships];");

        migrationBuilder.DropTable(name: "ReleaseTrackToProductRelationship", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleaseTrackToProductRelationship];");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_CreateRelease]
            (
                @Id UNIQUEIDENTIFIER,
                @Title NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
                @MediaFormat NVARCHAR(256),
                @PublishFormat NVARCHAR(256),
                @CatalogNumber NVARCHAR(32),
                @Barcode NVARCHAR(32),
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
                    @MediaFormat,
                    @PublishFormat,
                    @CatalogNumber,
                    @Barcode,
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
                @MediaFormat NVARCHAR(256),
                @PublishFormat NVARCHAR(256),
                @CatalogNumber NVARCHAR(32),
                @Barcode NVARCHAR(32),
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
                    @MediaFormat,
                    @PublishFormat,
                    @CatalogNumber,
                    @Barcode,
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
}
