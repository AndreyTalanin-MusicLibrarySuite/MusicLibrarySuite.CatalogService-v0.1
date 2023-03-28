using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ReleaseToProductRelationshipDto" /> entity.
/// </summary>
public partial class ReleaseToProductRelationshipMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ReleaseToProductRelationship",
            schema: "dbo",
            columns: table => new
            {
                ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                Order = table.Column<int>(type: "int", nullable: false),
                ReferenceOrder = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ReleaseToProductRelationship", columns: x => new { x.ReleaseId, x.ProductId });
                table.ForeignKey(
                    name: "FK_ReleaseToProductRelationship_Release_ReleaseId",
                    column: x => x.ReleaseId,
                    principalSchema: "dbo",
                    principalTable: "Release",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ReleaseToProductRelationship_Product_ProductId",
                    column: x => x.ProductId,
                    principalSchema: "dbo",
                    principalTable: "Product",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.CheckConstraint(name: "CK_ReleaseToProductRelationship_Name", sql: "LEN(TRIM([Name])) > 0");
                table.CheckConstraint(name: "CK_ReleaseToProductRelationship_Description", sql: "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseToProductRelationship_ReleaseId",
            schema: "dbo",
            table: "ReleaseToProductRelationship",
            column: "ReleaseId");

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseToProductRelationship_ProductId",
            schema: "dbo",
            table: "ReleaseToProductRelationship",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseToProductRelationship_ReleaseId_Order",
            schema: "dbo",
            table: "ReleaseToProductRelationship",
            columns: new[] { "ReleaseId", "Order" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseToProductRelationship_ProductId_ReferenceOrder",
            schema: "dbo",
            table: "ReleaseToProductRelationship",
            columns: new[] { "ProductId", "ReferenceOrder" },
            unique: true);

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleaseToProductRelationship_AfterInsertUpdateDelete_SetReleaseUpdatedOn]
            ON [dbo].[ReleaseToProductRelationship]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedRelease] AS
                (
                    SELECT [insertedReleaseToProductRelationship].[ReleaseId] AS [Id]
                    FROM [inserted] AS [insertedReleaseToProductRelationship]
                    UNION
                    SELECT [deletedReleaseToProductRelationship].[ReleaseId] AS [Id]
                    FROM [deleted] AS [deletedReleaseToProductRelationship]
                )
                UPDATE [dbo].[Release]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Release] AS [release]
                INNER JOIN [UpdatedRelease] AS [updatedRelease] ON [updatedRelease].[Id] = [release].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleaseToProductRelationship] AS TABLE
            (
                [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
                [ProductId] UNIQUEIDENTIFIER NOT NULL,
                [Name] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
                [Order] INT NOT NULL,
                [ReferenceOrder] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_Internal_MergeReleaseToProductRelationships]
            (
                @Id UNIQUEIDENTIFIER,
                @ReleaseToProductRelationships [dbo].[ReleaseToProductRelationship] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [SourceReleaseToProductRelationship] AS
                (
                    SELECT
                        @Id AS [ReleaseId],
                        [sourceReleaseToProductRelationship].[ProductId],
                        [sourceReleaseToProductRelationship].[Name],
                        [sourceReleaseToProductRelationship].[Description],
                        [sourceReleaseToProductRelationship].[Order],
                        COALESCE([targetReleaseToProductRelationship].[ReferenceOrder],
                            MAX([productReleaseToProductRelationship].[ReferenceOrder]) + 1,
                            0) AS [ReferenceOrder]
                    FROM @ReleaseToProductRelationships AS [sourceReleaseToProductRelationship]
                    LEFT JOIN [dbo].[ReleaseToProductRelationship] AS [targetReleaseToProductRelationship]
                        ON [targetReleaseToProductRelationship].[ReleaseId] = [sourceReleaseToProductRelationship].[ReleaseId]
                        AND [targetReleaseToProductRelationship].[ProductId] = [sourceReleaseToProductRelationship].[ProductId]
                    LEFT JOIN [dbo].[ReleaseToProductRelationship] AS [productReleaseToProductRelationship]
                        ON [targetReleaseToProductRelationship].[ReferenceOrder] IS NULL
                        AND [productReleaseToProductRelationship].[ProductId] = [sourceReleaseToProductRelationship].[ProductId]
                    WHERE [sourceReleaseToProductRelationship].[ReleaseId] = CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER)
                        OR [sourceReleaseToProductRelationship].[ReleaseId] = @Id
                    GROUP BY
                        [sourceReleaseToProductRelationship].[ReleaseId],
                        [sourceReleaseToProductRelationship].[ProductId],
                        [sourceReleaseToProductRelationship].[Name],
                        [sourceReleaseToProductRelationship].[Description],
                        [sourceReleaseToProductRelationship].[Order],
                        [targetReleaseToProductRelationship].[ReferenceOrder]
                )
                MERGE INTO [dbo].[ReleaseToProductRelationship] AS [target]
                USING [SourceReleaseToProductRelationship] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ProductId] = [source].[ProductId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Name] = [source].[Name],
                    [target].[Description] = [source].[Description],
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ProductId],
                    [Name],
                    [Description],
                    [Order],
                    [ReferenceOrder]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ProductId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order],
                    [source].[ReferenceOrder]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                WITH [UpdatedReleaseToProductRelationship] AS
                (
                    SELECT
                        [releaseToProductRelationship].[ReleaseId],
                        [releaseToProductRelationship].[ProductId],
                        ROW_NUMBER() OVER (PARTITION BY [releaseToProductRelationship].[ProductId]
                            ORDER BY [releaseToProductRelationship].[ReferenceOrder]) - 1 AS [UpdatedReferenceOrder]
                    FROM [dbo].[ReleaseToProductRelationship] [releaseToProductRelationship]
                )
                UPDATE [releaseToProductRelationship]
                SET [ReferenceOrder] = [updatedReleaseToProductRelationship].[UpdatedReferenceOrder]
                FROM [dbo].[ReleaseToProductRelationship] AS [releaseToProductRelationship]
                INNER JOIN [UpdatedReleaseToProductRelationship] AS [updatedReleaseToProductRelationship]
                    ON [updatedReleaseToProductRelationship].[ReleaseId] = [releaseToProductRelationship].[ReleaseId]
                    AND [updatedReleaseToProductRelationship].[ProductId] = [releaseToProductRelationship].[ProductId];
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
                @ReleaseArtists [dbo].[ReleaseArtist] READONLY,
                @ReleaseFeaturedArtists [dbo].[ReleaseFeaturedArtist] READONLY,
                @ReleasePerformers [dbo].[ReleasePerformer] READONLY,
                @ReleaseComposers [dbo].[ReleaseComposer] READONLY,
                @ReleaseGenres [dbo].[ReleaseGenre] READONLY,
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

                EXEC [dbo].[sp_Internal_MergeReleaseToProductRelationships]
                    @Id,
                    @ReleaseToProductRelationships;

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
                @ReleaseToProductRelationships [dbo].[ReleaseToProductRelationship] READONLY,
                @ReleaseArtists [dbo].[ReleaseArtist] READONLY,
                @ReleaseFeaturedArtists [dbo].[ReleaseFeaturedArtist] READONLY,
                @ReleasePerformers [dbo].[ReleasePerformer] READONLY,
                @ReleaseComposers [dbo].[ReleaseComposer] READONLY,
                @ReleaseGenres [dbo].[ReleaseGenre] READONLY,
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

                EXEC [dbo].[sp_Internal_MergeReleaseToProductRelationships]
                    @Id,
                    @ReleaseToProductRelationships;

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

                EXEC [dbo].[sp_Internal_MergeReleaseTrackCollection]
                    @Id,
                    @ReleaseTrackCollection;

                COMMIT TRANSACTION;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_UpdateReleaseToProductRelationshipsOrder]
            (
                @UseReferenceOrder BIT,
                @ReleaseToProductRelationships [dbo].[ReleaseToProductRelationship] READONLY,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                WITH [SourceReleaseToProductRelationship] AS
                (
                    SELECT * FROM @ReleaseToProductRelationships
                )
                MERGE INTO [dbo].[ReleaseToProductRelationship] AS [target]
                USING [SourceReleaseToProductRelationship] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ProductId] = [source].[ProductId]
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

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateReleaseToProductRelationshipsOrder];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_Internal_MergeReleaseToProductRelationships];");

        migrationBuilder.DropTable(name: "ReleaseToProductRelationship", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleaseToProductRelationship];");

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
