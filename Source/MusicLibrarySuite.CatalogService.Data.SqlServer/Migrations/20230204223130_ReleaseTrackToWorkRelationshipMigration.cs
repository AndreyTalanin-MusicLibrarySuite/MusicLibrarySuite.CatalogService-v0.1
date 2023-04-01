using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ReleaseTrackToWorkRelationshipDto" /> entity.
/// </summary>
public partial class ReleaseTrackToWorkRelationshipMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ReleaseTrackToWorkRelationship",
            schema: "dbo",
            columns: table => new
            {
                TrackNumber = table.Column<byte>(type: "tinyint", nullable: false),
                MediaNumber = table.Column<byte>(type: "tinyint", nullable: false),
                ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                WorkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                Order = table.Column<int>(type: "int", nullable: false),
                ReferenceOrder = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ReleaseTrackToWorkRelationship", columns: x => new { x.TrackNumber, x.MediaNumber, x.ReleaseId, x.WorkId });
                table.ForeignKey(
                    name: "FK_ReleaseTrackToWorkRelationship_ReleaseTrack_TrackNumber_MediaNumber_ReleaseId",
                    columns: x => new { x.TrackNumber, x.MediaNumber, x.ReleaseId },
                    principalSchema: "dbo",
                    principalTable: "ReleaseTrack",
                    principalColumns: new[] { "TrackNumber", "MediaNumber", "ReleaseId" },
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ReleaseTrackToWorkRelationship_Work_WorkId",
                    column: x => x.WorkId,
                    principalSchema: "dbo",
                    principalTable: "Work",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.CheckConstraint(name: "CK_ReleaseTrackToWorkRelationship_Name", sql: "LEN(TRIM([Name])) > 0");
                table.CheckConstraint(name: "CK_ReleaseTrackToWorkRelationship_Description", sql: "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseTrackToWorkRelationship_TrackNumber_MediaNumber_ReleaseId",
            schema: "dbo",
            table: "ReleaseTrackToWorkRelationship",
            columns: new[] { "TrackNumber", "MediaNumber", "ReleaseId" });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseTrackToWorkRelationship_WorkId",
            schema: "dbo",
            table: "ReleaseTrackToWorkRelationship",
            column: "WorkId");

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseTrackToWorkRelationship_TrackNumber_MediaNumber_ReleaseId_Order",
            schema: "dbo",
            table: "ReleaseTrackToWorkRelationship",
            columns: new[] { "TrackNumber", "MediaNumber", "ReleaseId", "Order" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseTrackToWorkRelationship_WorkId_ReferenceOrder",
            schema: "dbo",
            table: "ReleaseTrackToWorkRelationship",
            columns: new[] { "WorkId", "ReferenceOrder" },
            unique: true);

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleaseTrackToWorkRelationship_AfterInsertUpdateDelete_SetReleaseUpdatedOn]
            ON [dbo].[ReleaseTrackToWorkRelationship]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedRelease] AS
                (
                    SELECT [insertedReleaseTrackToWorkRelationship].[ReleaseId] AS [Id]
                    FROM [inserted] AS [insertedReleaseTrackToWorkRelationship]
                    UNION
                    SELECT [deletedReleaseTrackToWorkRelationship].[ReleaseId] AS [Id]
                    FROM [deleted] AS [deletedReleaseTrackToWorkRelationship]
                )
                UPDATE [dbo].[Release]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Release] AS [release]
                INNER JOIN [UpdatedRelease] AS [updatedRelease] ON [updatedRelease].[Id] = [release].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleaseTrackToWorkRelationship] AS TABLE
            (
                [TrackNumber] TINYINT NOT NULL,
                [MediaNumber] TINYINT NOT NULL,
                [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
                [WorkId] UNIQUEIDENTIFIER NOT NULL,
                [Name] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
                [Order] INT NOT NULL,
                [ReferenceOrder] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_Internal_MergeReleaseTrackToWorkRelationships]
            (
                @Id UNIQUEIDENTIFIER,
                @ReleaseTrackToWorkRelationships [dbo].[ReleaseTrackToWorkRelationship] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [SourceReleaseTrackToWorkRelationship] AS
                (
                    SELECT
                        [sourceReleaseTrackToWorkRelationship].[TrackNumber],
                        [sourceReleaseTrackToWorkRelationship].[MediaNumber],
                        @Id AS [ReleaseId],
                        [sourceReleaseTrackToWorkRelationship].[WorkId],
                        [sourceReleaseTrackToWorkRelationship].[Name],
                        [sourceReleaseTrackToWorkRelationship].[Description],
                        [sourceReleaseTrackToWorkRelationship].[Order],
                        COALESCE([targetReleaseTrackToWorkRelationship].[ReferenceOrder],
                            ROW_NUMBER() OVER (PARTITION BY [sourceReleaseTrackToWorkRelationship].[WorkId],
                                CASE WHEN [targetReleaseTrackToWorkRelationship].[ReferenceOrder] IS NOT NULL THEN 1 ELSE 0 END
                                ORDER BY [sourceReleaseTrackToWorkRelationship].[MediaNumber],
                                    [sourceReleaseTrackToWorkRelationship].[TrackNumber])
                                + COALESCE(MAX([workReleaseTrackToWorkRelationship].[ReferenceOrder]) + 1, 0) - 1) AS [ReferenceOrder]
                    FROM @ReleaseTrackToWorkRelationships AS [sourceReleaseTrackToWorkRelationship]
                    LEFT JOIN [dbo].[ReleaseTrackToWorkRelationship] AS [targetReleaseTrackToWorkRelationship]
                        ON [targetReleaseTrackToWorkRelationship].[TrackNumber] = [sourceReleaseTrackToWorkRelationship].[TrackNumber]
                        AND [targetReleaseTrackToWorkRelationship].[MediaNumber] = [sourceReleaseTrackToWorkRelationship].[MediaNumber]
                        AND [targetReleaseTrackToWorkRelationship].[ReleaseId] = [sourceReleaseTrackToWorkRelationship].[ReleaseId]
                        AND [targetReleaseTrackToWorkRelationship].[WorkId] = [sourceReleaseTrackToWorkRelationship].[WorkId]
                    LEFT JOIN [dbo].[ReleaseTrackToWorkRelationship] AS [workReleaseTrackToWorkRelationship]
                        ON [targetReleaseTrackToWorkRelationship].[ReferenceOrder] IS NULL
                        AND [workReleaseTrackToWorkRelationship].[WorkId] = [sourceReleaseTrackToWorkRelationship].[WorkId]
                    WHERE [sourceReleaseTrackToWorkRelationship].[ReleaseId] = CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER)
                        OR [sourceReleaseTrackToWorkRelationship].[ReleaseId] = @Id
                    GROUP BY
                        [sourceReleaseTrackToWorkRelationship].[TrackNumber],
                        [sourceReleaseTrackToWorkRelationship].[MediaNumber],
                        [sourceReleaseTrackToWorkRelationship].[ReleaseId],
                        [sourceReleaseTrackToWorkRelationship].[WorkId],
                        [sourceReleaseTrackToWorkRelationship].[Name],
                        [sourceReleaseTrackToWorkRelationship].[Description],
                        [sourceReleaseTrackToWorkRelationship].[Order],
                        [targetReleaseTrackToWorkRelationship].[ReferenceOrder]
                )
                MERGE INTO [dbo].[ReleaseTrackToWorkRelationship] AS [target]
                USING [SourceReleaseTrackToWorkRelationship] AS [source]
                ON [target].[TrackNumber] = [source].[TrackNumber]
                    AND [target].[MediaNumber] = [source].[MediaNumber]
                    AND [target].[ReleaseId] = [source].[ReleaseId]
                    AND [target].[WorkId] = [source].[WorkId]
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
                    [WorkId],
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
                    [source].[WorkId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order],
                    [source].[ReferenceOrder]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                WITH [UpdatedReleaseTrackToWorkRelationship] AS
                (
                    SELECT
                        [releaseTrackToWorkRelationship].[TrackNumber],
                        [releaseTrackToWorkRelationship].[MediaNumber],
                        [releaseTrackToWorkRelationship].[ReleaseId],
                        [releaseTrackToWorkRelationship].[WorkId],
                        ROW_NUMBER() OVER (PARTITION BY [releaseTrackToWorkRelationship].[WorkId]
                            ORDER BY [releaseTrackToWorkRelationship].[ReferenceOrder]) - 1 AS [UpdatedReferenceOrder]
                    FROM [dbo].[ReleaseTrackToWorkRelationship] [releaseTrackToWorkRelationship]
                )
                UPDATE [releaseTrackToWorkRelationship]
                SET [ReferenceOrder] = [updatedReleaseTrackToWorkRelationship].[UpdatedReferenceOrder]
                FROM [dbo].[ReleaseTrackToWorkRelationship] AS [releaseTrackToWorkRelationship]
                INNER JOIN [UpdatedReleaseTrackToWorkRelationship] AS [updatedReleaseTrackToWorkRelationship]
                    ON [updatedReleaseTrackToWorkRelationship].[TrackNumber] = [releaseTrackToWorkRelationship].[TrackNumber]
                    AND [updatedReleaseTrackToWorkRelationship].[MediaNumber] = [releaseTrackToWorkRelationship].[MediaNumber]
                    AND [updatedReleaseTrackToWorkRelationship].[ReleaseId] = [releaseTrackToWorkRelationship].[ReleaseId]
                    AND [updatedReleaseTrackToWorkRelationship].[WorkId] = [releaseTrackToWorkRelationship].[WorkId];
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
                @ReleaseTrackToWorkRelationships [dbo].[ReleaseTrackToWorkRelationship] READONLY,
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

                EXEC [dbo].[sp_Internal_MergeReleaseTrackToWorkRelationships]
                    @Id,
                    @ReleaseTrackToWorkRelationships;

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
                @ReleaseTrackToWorkRelationships [dbo].[ReleaseTrackToWorkRelationship] READONLY,
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

                EXEC [dbo].[sp_Internal_MergeReleaseTrackToWorkRelationships]
                    @Id,
                    @ReleaseTrackToWorkRelationships;

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
            CREATE PROCEDURE [dbo].[sp_UpdateReleaseTrackToWorkRelationshipsOrder]
            (
                @UseReferenceOrder BIT,
                @ReleaseTrackToWorkRelationships [dbo].[ReleaseTrackToWorkRelationship] READONLY,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                WITH [SourceReleaseTrackToWorkRelationship] AS
                (
                    SELECT * FROM @ReleaseTrackToWorkRelationships
                )
                MERGE INTO [dbo].[ReleaseTrackToWorkRelationship] AS [target]
                USING [SourceReleaseTrackToWorkRelationship] AS [source]
                ON [target].[TrackNumber] = [source].[TrackNumber]
                    AND [target].[MediaNumber] = [source].[MediaNumber]
                    AND [target].[ReleaseId] = [source].[ReleaseId]
                    AND [target].[WorkId] = [source].[WorkId]
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

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateReleaseTrackToWorkRelationshipsOrder];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_Internal_MergeReleaseTrackToWorkRelationships];");

        migrationBuilder.DropTable(name: "ReleaseTrackToWorkRelationship", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleaseTrackToWorkRelationship];");

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
    }
}
