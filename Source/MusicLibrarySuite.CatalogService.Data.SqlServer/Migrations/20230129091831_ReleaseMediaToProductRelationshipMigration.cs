using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ReleaseMediaToProductRelationshipDto" /> entity.
/// </summary>
public partial class ReleaseMediaToProductRelationshipMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ReleaseMediaToProductRelationship",
            schema: "dbo",
            columns: table => new
            {
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
                table.PrimaryKey(name: "PK_ReleaseMediaToProductRelationship", columns: x => new { x.MediaNumber, x.ReleaseId, x.ProductId });
                table.ForeignKey(
                    name: "FK_ReleaseMediaToProductRelationship_ReleaseMedia_MediaNumber_ReleaseId",
                    columns: x => new { x.MediaNumber, x.ReleaseId },
                    principalSchema: "dbo",
                    principalTable: "ReleaseMedia",
                    principalColumns: new[] { "MediaNumber", "ReleaseId" },
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ReleaseMediaToProductRelationship_Product_ProductId",
                    column: x => x.ProductId,
                    principalSchema: "dbo",
                    principalTable: "Product",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.CheckConstraint(name: "CK_ReleaseMediaToProductRelationship_Name", sql: "LEN(TRIM([Name])) > 0");
                table.CheckConstraint(name: "CK_ReleaseMediaToProductRelationship_Description", sql: "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseMediaToProductRelationship_MediaNumber_ReleaseId",
            schema: "dbo",
            table: "ReleaseMediaToProductRelationship",
            columns: new[] { "MediaNumber", "ReleaseId" });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseMediaToProductRelationship_ProductId",
            schema: "dbo",
            table: "ReleaseMediaToProductRelationship",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseMediaToProductRelationship_MediaNumber_ReleaseId_Order",
            schema: "dbo",
            table: "ReleaseMediaToProductRelationship",
            columns: new[] { "MediaNumber", "ReleaseId", "Order" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseMediaToProductRelationship_ProductId_ReferenceOrder",
            schema: "dbo",
            table: "ReleaseMediaToProductRelationship",
            columns: new[] { "ProductId", "ReferenceOrder" },
            unique: true);

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleaseMediaToProductRelationship_AfterInsertUpdateDelete_SetReleaseUpdatedOn]
            ON [dbo].[ReleaseMediaToProductRelationship]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedRelease] AS
                (
                    SELECT [insertedReleaseMediaToProductRelationship].[ReleaseId] AS [Id]
                    FROM [inserted] AS [insertedReleaseMediaToProductRelationship]
                    UNION
                    SELECT [deletedReleaseMediaToProductRelationship].[ReleaseId] AS [Id]
                    FROM [deleted] AS [deletedReleaseMediaToProductRelationship]
                )
                UPDATE [dbo].[Release]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Release] AS [release]
                INNER JOIN [UpdatedRelease] AS [updatedRelease] ON [updatedRelease].[Id] = [release].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleaseMediaToProductRelationship] AS TABLE
            (
                [MediaNumber] TINYINT NOT NULL,
                [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
                [ProductId] UNIQUEIDENTIFIER NOT NULL,
                [Name] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
                [Order] INT NOT NULL,
                [ReferenceOrder] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_Internal_MergeReleaseMediaToProductRelationships]
            (
                @Id UNIQUEIDENTIFIER,
                @ReleaseMediaToProductRelationships [dbo].[ReleaseMediaToProductRelationship] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [SourceReleaseMediaToProductRelationship] AS
                (
                    SELECT
                        [sourceReleaseMediaToProductRelationship].[MediaNumber],
                        @Id AS [ReleaseId],
                        [sourceReleaseMediaToProductRelationship].[ProductId],
                        [sourceReleaseMediaToProductRelationship].[Name],
                        [sourceReleaseMediaToProductRelationship].[Description],
                        [sourceReleaseMediaToProductRelationship].[Order],
                        COALESCE([targetReleaseMediaToProductRelationship].[ReferenceOrder],
                            MAX([productReleaseMediaToProductRelationship].[ReferenceOrder]) + 1,
                            0) AS [ReferenceOrder]
                    FROM @ReleaseMediaToProductRelationships AS [sourceReleaseMediaToProductRelationship]
                    LEFT JOIN [dbo].[ReleaseMediaToProductRelationship] AS [targetReleaseMediaToProductRelationship]
                        ON [targetReleaseMediaToProductRelationship].[MediaNumber] = [sourceReleaseMediaToProductRelationship].[MediaNumber]
                        AND [targetReleaseMediaToProductRelationship].[ReleaseId] = [sourceReleaseMediaToProductRelationship].[ReleaseId]
                        AND [targetReleaseMediaToProductRelationship].[ProductId] = [sourceReleaseMediaToProductRelationship].[ProductId]
                    LEFT JOIN [dbo].[ReleaseMediaToProductRelationship] AS [productReleaseMediaToProductRelationship]
                        ON [targetReleaseMediaToProductRelationship].[ReferenceOrder] IS NULL
                        AND [productReleaseMediaToProductRelationship].[ProductId] = [sourceReleaseMediaToProductRelationship].[ProductId]
                    WHERE [sourceReleaseMediaToProductRelationship].[ReleaseId] = CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER)
                        OR [sourceReleaseMediaToProductRelationship].[ReleaseId] = @Id
                    GROUP BY
                        [sourceReleaseMediaToProductRelationship].[MediaNumber],
                        [sourceReleaseMediaToProductRelationship].[ReleaseId],
                        [sourceReleaseMediaToProductRelationship].[ProductId],
                        [sourceReleaseMediaToProductRelationship].[Name],
                        [sourceReleaseMediaToProductRelationship].[Description],
                        [sourceReleaseMediaToProductRelationship].[Order],
                        [targetReleaseMediaToProductRelationship].[ReferenceOrder]
                )
                MERGE INTO [dbo].[ReleaseMediaToProductRelationship] AS [target]
                USING [SourceReleaseMediaToProductRelationship] AS [source]
                ON [target].[MediaNumber] = [source].[MediaNumber]
                    AND [target].[ReleaseId] = [source].[ReleaseId]
                    AND [target].[ProductId] = [source].[ProductId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Name] = [source].[Name],
                    [target].[Description] = [source].[Description],
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
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
                    [source].[MediaNumber],
                    [source].[ReleaseId],
                    [source].[ProductId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order],
                    [source].[ReferenceOrder]
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
                @ReleaseArtists [dbo].[ReleaseArtist] READONLY,
                @ReleaseFeaturedArtists [dbo].[ReleaseFeaturedArtist] READONLY,
                @ReleasePerformers [dbo].[ReleasePerformer] READONLY,
                @ReleaseComposers [dbo].[ReleaseComposer] READONLY,
                @ReleaseGenres [dbo].[ReleaseGenre] READONLY,
                @ReleaseToProductRelationships [dbo].[ReleaseToProductRelationship] READONLY,
                @ReleaseToReleaseGroupRelationships [dbo].[ReleaseToReleaseGroupRelationship] READONLY,
                @ReleaseMediaCollection [dbo].[ReleaseMedia] READONLY,
                @ReleaseMediaToProductRelationships [dbo].[ReleaseMediaToProductRelationship] READONLY,
                @ReleaseTrackCollection [dbo].[ReleaseTrack] READONLY,
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

                EXEC [dbo].[sp_Internal_MergeReleaseToProductRelationships]
                    @Id,
                    @ReleaseToProductRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseToReleaseGroupRelationships]
                    @Id,
                    @ReleaseToReleaseGroupRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseMediaCollection]
                    @Id,
                    @ReleaseMediaCollection;

                EXEC [dbo].[sp_Internal_MergeReleaseMediaToProductRelationships]
                    @Id,
                    @ReleaseMediaToProductRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackCollection]
                    @Id,
                    @ReleaseTrackCollection;

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
                @ReleaseArtists [dbo].[ReleaseArtist] READONLY,
                @ReleaseFeaturedArtists [dbo].[ReleaseFeaturedArtist] READONLY,
                @ReleasePerformers [dbo].[ReleasePerformer] READONLY,
                @ReleaseComposers [dbo].[ReleaseComposer] READONLY,
                @ReleaseGenres [dbo].[ReleaseGenre] READONLY,
                @ReleaseToProductRelationships [dbo].[ReleaseToProductRelationship] READONLY,
                @ReleaseToReleaseGroupRelationships [dbo].[ReleaseToReleaseGroupRelationship] READONLY,
                @ReleaseMediaCollection [dbo].[ReleaseMedia] READONLY,
                @ReleaseMediaToProductRelationships [dbo].[ReleaseMediaToProductRelationship] READONLY,
                @ReleaseTrackCollection [dbo].[ReleaseTrack] READONLY,
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

                EXEC [dbo].[sp_Internal_MergeReleaseToProductRelationships]
                    @Id,
                    @ReleaseToProductRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseToReleaseGroupRelationships]
                    @Id,
                    @ReleaseToReleaseGroupRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseMediaCollection]
                    @Id,
                    @ReleaseMediaCollection;

                EXEC [dbo].[sp_Internal_MergeReleaseMediaToProductRelationships]
                    @Id,
                    @ReleaseMediaToProductRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackCollection]
                    @Id,
                    @ReleaseTrackCollection;

                COMMIT TRANSACTION;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_UpdateReleaseMediaToProductRelationshipsOrder]
            (
                @UseReferenceOrder BIT,
                @ReleaseMediaToProductRelationships [dbo].[ReleaseMediaToProductRelationship] READONLY,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                WITH [SourceReleaseMediaToProductRelationship] AS
                (
                    SELECT * FROM @ReleaseMediaToProductRelationships
                )
                MERGE INTO [dbo].[ReleaseMediaToProductRelationship] AS [target]
                USING [SourceReleaseMediaToProductRelationship] AS [source]
                ON [target].[MediaNumber] = [source].[MediaNumber]
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

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateReleaseMediaToProductRelationshipsOrder];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_Internal_MergeReleaseMediaToProductRelationships];");

        migrationBuilder.DropTable(name: "ReleaseMediaToProductRelationship", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleaseMediaToProductRelationship];");

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
                @ReleaseArtists [dbo].[ReleaseArtist] READONLY,
                @ReleaseFeaturedArtists [dbo].[ReleaseFeaturedArtist] READONLY,
                @ReleasePerformers [dbo].[ReleasePerformer] READONLY,
                @ReleaseComposers [dbo].[ReleaseComposer] READONLY,
                @ReleaseGenres [dbo].[ReleaseGenre] READONLY,
                @ReleaseToProductRelationships [dbo].[ReleaseToProductRelationship] READONLY,
                @ReleaseToReleaseGroupRelationships [dbo].[ReleaseToReleaseGroupRelationship] READONLY,
                @ReleaseMediaCollection [dbo].[ReleaseMedia] READONLY,
                @ReleaseTrackCollection [dbo].[ReleaseTrack] READONLY,
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

                EXEC [dbo].[sp_Internal_MergeReleaseToProductRelationships]
                    @Id,
                    @ReleaseToProductRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseToReleaseGroupRelationships]
                    @Id,
                    @ReleaseToReleaseGroupRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseMediaCollection]
                    @Id,
                    @ReleaseMediaCollection;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackCollection]
                    @Id,
                    @ReleaseTrackCollection;

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
                @ReleaseArtists [dbo].[ReleaseArtist] READONLY,
                @ReleaseFeaturedArtists [dbo].[ReleaseFeaturedArtist] READONLY,
                @ReleasePerformers [dbo].[ReleasePerformer] READONLY,
                @ReleaseComposers [dbo].[ReleaseComposer] READONLY,
                @ReleaseGenres [dbo].[ReleaseGenre] READONLY,
                @ReleaseToProductRelationships [dbo].[ReleaseToProductRelationship] READONLY,
                @ReleaseToReleaseGroupRelationships [dbo].[ReleaseToReleaseGroupRelationship] READONLY,
                @ReleaseMediaCollection [dbo].[ReleaseMedia] READONLY,
                @ReleaseTrackCollection [dbo].[ReleaseTrack] READONLY,
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

                EXEC [dbo].[sp_Internal_MergeReleaseToProductRelationships]
                    @Id,
                    @ReleaseToProductRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseToReleaseGroupRelationships]
                    @Id,
                    @ReleaseToReleaseGroupRelationships;

                EXEC [dbo].[sp_Internal_MergeReleaseMediaCollection]
                    @Id,
                    @ReleaseMediaCollection;

                EXEC [dbo].[sp_Internal_MergeReleaseTrackCollection]
                    @Id,
                    @ReleaseTrackCollection;

                COMMIT TRANSACTION;
            END;");
    }
}
