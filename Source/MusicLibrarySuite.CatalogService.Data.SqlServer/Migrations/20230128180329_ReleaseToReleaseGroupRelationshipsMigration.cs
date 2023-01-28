using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ReleaseToReleaseGroupRelationshipsMigration" /> entity.
/// </summary>
public partial class ReleaseToReleaseGroupRelationshipsMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ReleaseToReleaseGroupRelationship",
            schema: "dbo",
            columns: table => new
            {
                ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ReleaseGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                Order = table.Column<int>(type: "int", nullable: false),
                ReferenceOrder = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ReleaseToReleaseGroupRelationship", columns: x => new { x.ReleaseId, x.ReleaseGroupId });
                table.ForeignKey(
                    name: "FK_ReleaseToReleaseGroupRelationship_Release_ReleaseId",
                    column: x => x.ReleaseId,
                    principalSchema: "dbo",
                    principalTable: "Release",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ReleaseToReleaseGroupRelationship_ReleaseGroup_ReleaseGroupId",
                    column: x => x.ReleaseGroupId,
                    principalSchema: "dbo",
                    principalTable: "ReleaseGroup",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.CheckConstraint(name: "CK_ReleaseToReleaseGroupRelationship_Name", sql: "LEN(TRIM([Name])) > 0");
                table.CheckConstraint(name: "CK_ReleaseToReleaseGroupRelationship_Description", sql: "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseToReleaseGroupRelationship_ReleaseId",
            schema: "dbo",
            table: "ReleaseToReleaseGroupRelationship",
            column: "ReleaseId");

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseToReleaseGroupRelationship_ReleaseGroupId",
            schema: "dbo",
            table: "ReleaseToReleaseGroupRelationship",
            column: "ReleaseGroupId");

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseToReleaseGroupRelationship_ReleaseId_Order",
            schema: "dbo",
            table: "ReleaseToReleaseGroupRelationship",
            columns: new[] { "ReleaseId", "Order" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseToReleaseGroupRelationship_ReleaseGroupId_ReferenceOrder",
            schema: "dbo",
            table: "ReleaseToReleaseGroupRelationship",
            columns: new[] { "ReleaseGroupId", "ReferenceOrder" },
            unique: true);

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleaseToReleaseGroupRelationship_AfterInsertUpdateDelete_SetReleaseUpdatedOn]
            ON [dbo].[ReleaseToReleaseGroupRelationship]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedRelease] AS
                (
                    SELECT [insertedReleaseToReleaseGroupRelationship].[ReleaseId] AS [Id]
                    FROM [inserted] AS [insertedReleaseToReleaseGroupRelationship]
                    UNION
                    SELECT [deletedReleaseToReleaseGroupRelationship].[ReleaseId] AS [Id]
                    FROM [deleted] AS [deletedReleaseToReleaseGroupRelationship]
                )
                UPDATE [dbo].[Release]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Release] AS [release]
                INNER JOIN [UpdatedRelease] AS [updatedRelease] ON [updatedRelease].[Id] = [release].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleaseToReleaseGroupRelationship] AS TABLE
            (
                [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
                [ReleaseGroupId] UNIQUEIDENTIFIER NOT NULL,
                [Name] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
                [Order] INT NOT NULL,
                [ReferenceOrder] INT NOT NULL
            );");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_DeleteRelease];");

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

                INSERT INTO [dbo].[Release]
                (
                    [Id],
                    [Title],
                    [Description],
                    [DisambiguationText],
                    [Barcode],
                    [CatalogNumber],
                    [MediaFormat],
                    [PublishFormat],
                    [ReleasedOn],
                    [ReleasedOnYearOnly],
                    [Enabled]
                )
                VALUES
                (
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
                    @Enabled
                );

                WITH [SourceReleaseRelationship] AS
                (
                    SELECT * FROM @ReleaseRelationships WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseRelationship] AS [target]
                USING [SourceReleaseRelationship] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[DependentReleaseId] = [source].[DependentReleaseId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [DependentReleaseId],
                    [Name],
                    [Description],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[DependentReleaseId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order]
                );

                WITH [SourceReleaseArtist] AS
                (
                    SELECT * FROM @ReleaseArtists WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseArtist] AS [target]
                USING [SourceReleaseArtist] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                );

                WITH [SourceReleaseFeaturedArtist] AS
                (
                    SELECT * FROM @ReleaseFeaturedArtists WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseFeaturedArtist] AS [target]
                USING [SourceReleaseFeaturedArtist] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                );

                WITH [SourceReleasePerformer] AS
                (
                    SELECT * FROM @ReleasePerformers WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleasePerformer] AS [target]
                USING [SourceReleasePerformer] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                );

                WITH [SourceReleaseComposer] AS
                (
                    SELECT * FROM @ReleaseComposers WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseComposer] AS [target]
                USING [SourceReleaseComposer] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                );

                WITH [SourceReleaseGenre] AS
                (
                    SELECT * FROM @ReleaseGenres WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseGenre] AS [target]
                USING [SourceReleaseGenre] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[GenreId] = [source].[GenreId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [GenreId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[GenreId],
                    [source].[Order]
                );

                WITH [SourceReleaseToProductRelationship] AS
                (
                    SELECT
                        [sourceReleaseToProductRelationship].[ReleaseId],
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
                    WHERE [sourceReleaseToProductRelationship].[ReleaseId] = @Id
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
                );

                WITH [SourceReleaseToReleaseGroupRelationship] AS
                (
                    SELECT
                        [sourceReleaseToReleaseGroupRelationship].[ReleaseId],
                        [sourceReleaseToReleaseGroupRelationship].[ReleaseGroupId],
                        [sourceReleaseToReleaseGroupRelationship].[Name],
                        [sourceReleaseToReleaseGroupRelationship].[Description],
                        [sourceReleaseToReleaseGroupRelationship].[Order],
                        COALESCE([targetReleaseToReleaseGroupRelationship].[ReferenceOrder],
                            MAX([releaseGroupReleaseToReleaseGroupRelationship].[ReferenceOrder]) + 1,
                            0) AS [ReferenceOrder]
                    FROM @ReleaseToReleaseGroupRelationships AS [sourceReleaseToReleaseGroupRelationship]
                    LEFT JOIN [dbo].[ReleaseToReleaseGroupRelationship] AS [targetReleaseToReleaseGroupRelationship]
                        ON [targetReleaseToReleaseGroupRelationship].[ReleaseId] = [sourceReleaseToReleaseGroupRelationship].[ReleaseId]
                        AND [targetReleaseToReleaseGroupRelationship].[ReleaseGroupId] = [sourceReleaseToReleaseGroupRelationship].[ReleaseGroupId]
                    LEFT JOIN [dbo].[ReleaseToReleaseGroupRelationship] AS [releaseGroupReleaseToReleaseGroupRelationship]
                        ON [targetReleaseToReleaseGroupRelationship].[ReferenceOrder] IS NULL
                        AND [releaseGroupReleaseToReleaseGroupRelationship].[ReleaseGroupId] = [sourceReleaseToReleaseGroupRelationship].[ReleaseGroupId]
                    WHERE [sourceReleaseToReleaseGroupRelationship].[ReleaseId] = @Id
                    GROUP BY
                        [sourceReleaseToReleaseGroupRelationship].[ReleaseId],
                        [sourceReleaseToReleaseGroupRelationship].[ReleaseGroupId],
                        [sourceReleaseToReleaseGroupRelationship].[Name],
                        [sourceReleaseToReleaseGroupRelationship].[Description],
                        [sourceReleaseToReleaseGroupRelationship].[Order],
                        [targetReleaseToReleaseGroupRelationship].[ReferenceOrder]
                )
                MERGE INTO [dbo].[ReleaseToReleaseGroupRelationship] AS [target]
                USING [SourceReleaseToReleaseGroupRelationship] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ReleaseGroupId] = [source].[ReleaseGroupId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ReleaseGroupId],
                    [Name],
                    [Description],
                    [Order],
                    [ReferenceOrder]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ReleaseGroupId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order],
                    [source].[ReferenceOrder]
                );

                WITH [SourceReleaseMedia] AS
                (
                    SELECT * FROM @ReleaseMediaCollection WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseMedia] AS [target]
                USING [SourceReleaseMedia] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId]
                    AND [target].[MediaNumber] = [source].[MediaNumber]
                WHEN NOT MATCHED THEN INSERT
                (
                    [MediaNumber],
                    [ReleaseId],
                    [Title],
                    [Description],
                    [DisambiguationText],
                    [CatalogNumber],
                    [MediaFormat],
                    [TableOfContentsChecksum],
                    [TableOfContentsChecksumLong]
                )
                VALUES
                (
                    [source].[MediaNumber],
                    [source].[ReleaseId],
                    [source].[Title],
                    [source].[Description],
                    [source].[DisambiguationText],
                    [source].[CatalogNumber],
                    [source].[MediaFormat],
                    [source].[TableOfContentsChecksum],
                    [source].[TableOfContentsChecksumLong]
                );

                WITH [SourceReleaseTrack] AS
                (
                    SELECT * FROM @ReleaseTrackCollection WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseTrack] AS [target]
                USING [SourceReleaseTrack] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId]
                    AND [target].[TrackNumber] = [source].[TrackNumber]
                    AND [target].[MediaNumber] = [source].[MediaNumber]
                WHEN NOT MATCHED THEN INSERT
                (
                    [TrackNumber],
                    [MediaNumber],
                    [ReleaseId],
                    [Title],
                    [Description],
                    [DisambiguationText],
                    [InternationalStandardRecordingCode]
                )
                VALUES
                (
                    [source].[TrackNumber],
                    [source].[MediaNumber],
                    [source].[ReleaseId],
                    [source].[Title],
                    [source].[Description],
                    [source].[DisambiguationText],
                    [source].[InternationalStandardRecordingCode]
                );

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

                UPDATE [dbo].[Release]
                SET
                    [Title] = @Title,
                    [Description] = @Description,
                    [DisambiguationText] = @DisambiguationText,
                    [Barcode] = @Barcode,
                    [CatalogNumber] = @CatalogNumber,
                    [MediaFormat] = @MediaFormat,
                    [PublishFormat] = @PublishFormat,
                    [ReleasedOn] = @ReleasedOn,
                    [ReleasedOnYearOnly] = @ReleasedOnYearOnly,
                    [Enabled] = @Enabled
                WHERE [Id] = @Id;

                SET @ResultRowsUpdated = @@ROWCOUNT;

                WITH [SourceReleaseRelationship] AS
                (
                    SELECT * FROM @ReleaseRelationships WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseRelationship] AS [target]
                USING [SourceReleaseRelationship] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[DependentReleaseId] = [source].[DependentReleaseId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Name] = [source].[Name],
                    [target].[Description] = [source].[Description],
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [DependentReleaseId],
                    [Name],
                    [Description],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[DependentReleaseId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceReleaseArtist] AS
                (
                    SELECT * FROM @ReleaseArtists WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseArtist] AS [target]
                USING [SourceReleaseArtist] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceReleaseFeaturedArtist] AS
                (
                    SELECT * FROM @ReleaseFeaturedArtists WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseFeaturedArtist] AS [target]
                USING [SourceReleaseFeaturedArtist] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceReleasePerformer] AS
                (
                    SELECT * FROM @ReleasePerformers WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleasePerformer] AS [target]
                USING [SourceReleasePerformer] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceReleaseComposer] AS
                (
                    SELECT * FROM @ReleaseComposers WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseComposer] AS [target]
                USING [SourceReleaseComposer] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceReleaseGenre] AS
                (
                    SELECT * FROM @ReleaseGenres WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseGenre] AS [target]
                USING [SourceReleaseGenre] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[GenreId] = [source].[GenreId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [GenreId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[GenreId],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceReleaseToProductRelationship] AS
                (
                    SELECT
                        [sourceReleaseToProductRelationship].[ReleaseId],
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
                    WHERE [sourceReleaseToProductRelationship].[ReleaseId] = @Id
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

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceReleaseToReleaseGroupRelationship] AS
                (
                    SELECT
                        [sourceReleaseToReleaseGroupRelationship].[ReleaseId],
                        [sourceReleaseToReleaseGroupRelationship].[ReleaseGroupId],
                        [sourceReleaseToReleaseGroupRelationship].[Name],
                        [sourceReleaseToReleaseGroupRelationship].[Description],
                        [sourceReleaseToReleaseGroupRelationship].[Order],
                        COALESCE([targetReleaseToReleaseGroupRelationship].[ReferenceOrder],
                            MAX([releaseGroupReleaseToReleaseGroupRelationship].[ReferenceOrder]) + 1,
                            0) AS [ReferenceOrder]
                    FROM @ReleaseToReleaseGroupRelationships AS [sourceReleaseToReleaseGroupRelationship]
                    LEFT JOIN [dbo].[ReleaseToReleaseGroupRelationship] AS [targetReleaseToReleaseGroupRelationship]
                        ON [targetReleaseToReleaseGroupRelationship].[ReleaseId] = [sourceReleaseToReleaseGroupRelationship].[ReleaseId]
                        AND [targetReleaseToReleaseGroupRelationship].[ReleaseGroupId] = [sourceReleaseToReleaseGroupRelationship].[ReleaseGroupId]
                    LEFT JOIN [dbo].[ReleaseToReleaseGroupRelationship] AS [releaseGroupReleaseToReleaseGroupRelationship]
                        ON [targetReleaseToReleaseGroupRelationship].[ReferenceOrder] IS NULL
                        AND [releaseGroupReleaseToReleaseGroupRelationship].[ReleaseGroupId] = [sourceReleaseToReleaseGroupRelationship].[ReleaseGroupId]
                    WHERE [sourceReleaseToReleaseGroupRelationship].[ReleaseId] = @Id
                    GROUP BY
                        [sourceReleaseToReleaseGroupRelationship].[ReleaseId],
                        [sourceReleaseToReleaseGroupRelationship].[ReleaseGroupId],
                        [sourceReleaseToReleaseGroupRelationship].[Name],
                        [sourceReleaseToReleaseGroupRelationship].[Description],
                        [sourceReleaseToReleaseGroupRelationship].[Order],
                        [targetReleaseToReleaseGroupRelationship].[ReferenceOrder]
                )
                MERGE INTO [dbo].[ReleaseToReleaseGroupRelationship] AS [target]
                USING [SourceReleaseToReleaseGroupRelationship] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ReleaseGroupId] = [source].[ReleaseGroupId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Name] = [source].[Name],
                    [target].[Description] = [source].[Description],
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ReleaseGroupId],
                    [Name],
                    [Description],
                    [Order],
                    [ReferenceOrder]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ReleaseGroupId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order],
                    [source].[ReferenceOrder]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceReleaseMedia] AS
                (
                    SELECT * FROM @ReleaseMediaCollection WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseMedia] AS [target]
                USING [SourceReleaseMedia] AS [source]
                ON [target].[MediaNumber] = [source].[MediaNumber] AND [target].[ReleaseId] = [source].[ReleaseId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Title] = [source].[Title],
                    [target].[Description] = [source].[Description],
                    [target].[DisambiguationText] = [source].[DisambiguationText],
                    [target].[CatalogNumber] = [source].[CatalogNumber],
                    [target].[MediaFormat] = [source].[MediaFormat],
                    [target].[TableOfContentsChecksum] = [source].[TableOfContentsChecksum],
                    [target].[TableOfContentsChecksumLong] = [source].[TableOfContentsChecksumLong]
                WHEN NOT MATCHED THEN INSERT
                (
                    [MediaNumber],
                    [ReleaseId],
                    [Title],
                    [Description],
                    [DisambiguationText],
                    [CatalogNumber],
                    [MediaFormat],
                    [TableOfContentsChecksum],
                    [TableOfContentsChecksumLong]
                )
                VALUES
                (
                    [source].[MediaNumber],
                    [source].[ReleaseId],
                    [source].[Title],
                    [source].[Description],
                    [source].[DisambiguationText],
                    [source].[CatalogNumber],
                    [source].[MediaFormat],
                    [source].[TableOfContentsChecksum],
                    [source].[TableOfContentsChecksumLong]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceReleaseTrack] AS
                (
                    SELECT * FROM @ReleaseTrackCollection WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseTrack] AS [target]
                USING [SourceReleaseTrack] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId]
                    AND [target].[TrackNumber] = [source].[TrackNumber]
                    AND [target].[MediaNumber] = [source].[MediaNumber]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Title] = [source].[Title],
                    [target].[Description] = [source].[Description],
                    [target].[DisambiguationText] = [source].[DisambiguationText],
                    [target].[InternationalStandardRecordingCode] = [source].[InternationalStandardRecordingCode]
                WHEN NOT MATCHED THEN INSERT
                (
                    [TrackNumber],
                    [MediaNumber],
                    [ReleaseId],
                    [Title],
                    [Description],
                    [DisambiguationText],
                    [InternationalStandardRecordingCode]
                )
                VALUES
                (
                    [source].[TrackNumber],
                    [source].[MediaNumber],
                    [source].[ReleaseId],
                    [source].[Title],
                    [source].[Description],
                    [source].[DisambiguationText],
                    [source].[InternationalStandardRecordingCode]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                COMMIT TRANSACTION;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_UpdateReleaseToReleaseGroupRelationshipsOrder]
            (
                @UseReferenceOrder BIT,
                @ReleaseToReleaseGroupRelationships [dbo].[ReleaseToReleaseGroupRelationship] READONLY,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                WITH [SourceReleaseToReleaseGroupRelationship] AS
                (
                    SELECT * FROM @ReleaseToReleaseGroupRelationships
                )
                MERGE INTO [dbo].[ReleaseToReleaseGroupRelationship] AS [target]
                USING [SourceReleaseToReleaseGroupRelationship] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ReleaseGroupId] = [source].[ReleaseGroupId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Order] = CASE WHEN @UseReferenceOrder = 0 THEN [source].[Order] ELSE [target].[Order] END,
                    [target].[ReferenceOrder] = CASE WHEN @UseReferenceOrder = 1 THEN [source].[ReferenceOrder] ELSE [target].[ReferenceOrder] END;

                SET @ResultRowsUpdated = @@ROWCOUNT;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_DeleteRelease]
            (
                @Id UNIQUEIDENTIFIER,
                @ResultRowsDeleted INT OUTPUT
            )
            AS
            BEGIN
                DELETE FROM [dbo].[Release] WHERE [Id] = @Id;

                SET @ResultRowsDeleted = @@ROWCOUNT;
            END;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateReleaseToReleaseGroupRelationshipsOrder];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_DeleteRelease];");

        migrationBuilder.DropTable(name: "ReleaseToReleaseGroupRelationship", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleaseToReleaseGroupRelationship];");

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

                INSERT INTO [dbo].[Release]
                (
                    [Id],
                    [Title],
                    [Description],
                    [DisambiguationText],
                    [Barcode],
                    [CatalogNumber],
                    [MediaFormat],
                    [PublishFormat],
                    [ReleasedOn],
                    [ReleasedOnYearOnly],
                    [Enabled]
                )
                VALUES
                (
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
                    @Enabled
                );

                WITH [SourceReleaseRelationship] AS
                (
                    SELECT * FROM @ReleaseRelationships WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseRelationship] AS [target]
                USING [SourceReleaseRelationship] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[DependentReleaseId] = [source].[DependentReleaseId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [DependentReleaseId],
                    [Name],
                    [Description],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[DependentReleaseId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order]
                );

                WITH [SourceReleaseArtist] AS
                (
                    SELECT * FROM @ReleaseArtists WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseArtist] AS [target]
                USING [SourceReleaseArtist] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                );

                WITH [SourceReleaseFeaturedArtist] AS
                (
                    SELECT * FROM @ReleaseFeaturedArtists WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseFeaturedArtist] AS [target]
                USING [SourceReleaseFeaturedArtist] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                );

                WITH [SourceReleasePerformer] AS
                (
                    SELECT * FROM @ReleasePerformers WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleasePerformer] AS [target]
                USING [SourceReleasePerformer] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                );

                WITH [SourceReleaseComposer] AS
                (
                    SELECT * FROM @ReleaseComposers WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseComposer] AS [target]
                USING [SourceReleaseComposer] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                );

                WITH [SourceReleaseGenre] AS
                (
                    SELECT * FROM @ReleaseGenres WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseGenre] AS [target]
                USING [SourceReleaseGenre] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[GenreId] = [source].[GenreId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [GenreId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[GenreId],
                    [source].[Order]
                );

                WITH [SourceReleaseToProductRelationship] AS
                (
                    SELECT
                        [sourceReleaseToProductRelationship].[ReleaseId],
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
                    WHERE [sourceReleaseToProductRelationship].[ReleaseId] = @Id
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
                );

                WITH [SourceReleaseMedia] AS
                (
                    SELECT * FROM @ReleaseMediaCollection WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseMedia] AS [target]
                USING [SourceReleaseMedia] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId]
                    AND [target].[MediaNumber] = [source].[MediaNumber]
                WHEN NOT MATCHED THEN INSERT
                (
                    [MediaNumber],
                    [ReleaseId],
                    [Title],
                    [Description],
                    [DisambiguationText],
                    [CatalogNumber],
                    [MediaFormat],
                    [TableOfContentsChecksum],
                    [TableOfContentsChecksumLong]
                )
                VALUES
                (
                    [source].[MediaNumber],
                    [source].[ReleaseId],
                    [source].[Title],
                    [source].[Description],
                    [source].[DisambiguationText],
                    [source].[CatalogNumber],
                    [source].[MediaFormat],
                    [source].[TableOfContentsChecksum],
                    [source].[TableOfContentsChecksumLong]
                );

                WITH [SourceReleaseTrack] AS
                (
                    SELECT * FROM @ReleaseTrackCollection WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseTrack] AS [target]
                USING [SourceReleaseTrack] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId]
                    AND [target].[TrackNumber] = [source].[TrackNumber]
                    AND [target].[MediaNumber] = [source].[MediaNumber]
                WHEN NOT MATCHED THEN INSERT
                (
                    [TrackNumber],
                    [MediaNumber],
                    [ReleaseId],
                    [Title],
                    [Description],
                    [DisambiguationText],
                    [InternationalStandardRecordingCode]
                )
                VALUES
                (
                    [source].[TrackNumber],
                    [source].[MediaNumber],
                    [source].[ReleaseId],
                    [source].[Title],
                    [source].[Description],
                    [source].[DisambiguationText],
                    [source].[InternationalStandardRecordingCode]
                );

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
                @ReleaseMediaCollection [dbo].[ReleaseMedia] READONLY,
                @ReleaseTrackCollection [dbo].[ReleaseTrack] READONLY,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                BEGIN TRANSACTION;

                UPDATE [dbo].[Release]
                SET
                    [Title] = @Title,
                    [Description] = @Description,
                    [DisambiguationText] = @DisambiguationText,
                    [Barcode] = @Barcode,
                    [CatalogNumber] = @CatalogNumber,
                    [MediaFormat] = @MediaFormat,
                    [PublishFormat] = @PublishFormat,
                    [ReleasedOn] = @ReleasedOn,
                    [ReleasedOnYearOnly] = @ReleasedOnYearOnly,
                    [Enabled] = @Enabled
                WHERE [Id] = @Id;

                SET @ResultRowsUpdated = @@ROWCOUNT;

                WITH [SourceReleaseRelationship] AS
                (
                    SELECT * FROM @ReleaseRelationships WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseRelationship] AS [target]
                USING [SourceReleaseRelationship] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[DependentReleaseId] = [source].[DependentReleaseId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Name] = [source].[Name],
                    [target].[Description] = [source].[Description],
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [DependentReleaseId],
                    [Name],
                    [Description],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[DependentReleaseId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceReleaseArtist] AS
                (
                    SELECT * FROM @ReleaseArtists WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseArtist] AS [target]
                USING [SourceReleaseArtist] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceReleaseFeaturedArtist] AS
                (
                    SELECT * FROM @ReleaseFeaturedArtists WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseFeaturedArtist] AS [target]
                USING [SourceReleaseFeaturedArtist] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceReleasePerformer] AS
                (
                    SELECT * FROM @ReleasePerformers WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleasePerformer] AS [target]
                USING [SourceReleasePerformer] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceReleaseComposer] AS
                (
                    SELECT * FROM @ReleaseComposers WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseComposer] AS [target]
                USING [SourceReleaseComposer] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceReleaseGenre] AS
                (
                    SELECT * FROM @ReleaseGenres WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseGenre] AS [target]
                USING [SourceReleaseGenre] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId] AND [target].[GenreId] = [source].[GenreId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseId],
                    [GenreId],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseId],
                    [source].[GenreId],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceReleaseToProductRelationship] AS
                (
                    SELECT
                        [sourceReleaseToProductRelationship].[ReleaseId],
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
                    WHERE [sourceReleaseToProductRelationship].[ReleaseId] = @Id
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

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceReleaseMedia] AS
                (
                    SELECT * FROM @ReleaseMediaCollection WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseMedia] AS [target]
                USING [SourceReleaseMedia] AS [source]
                ON [target].[MediaNumber] = [source].[MediaNumber] AND [target].[ReleaseId] = [source].[ReleaseId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Title] = [source].[Title],
                    [target].[Description] = [source].[Description],
                    [target].[DisambiguationText] = [source].[DisambiguationText],
                    [target].[CatalogNumber] = [source].[CatalogNumber],
                    [target].[MediaFormat] = [source].[MediaFormat],
                    [target].[TableOfContentsChecksum] = [source].[TableOfContentsChecksum],
                    [target].[TableOfContentsChecksumLong] = [source].[TableOfContentsChecksumLong]
                WHEN NOT MATCHED THEN INSERT
                (
                    [MediaNumber],
                    [ReleaseId],
                    [Title],
                    [Description],
                    [DisambiguationText],
                    [CatalogNumber],
                    [MediaFormat],
                    [TableOfContentsChecksum],
                    [TableOfContentsChecksumLong]
                )
                VALUES
                (
                    [source].[MediaNumber],
                    [source].[ReleaseId],
                    [source].[Title],
                    [source].[Description],
                    [source].[DisambiguationText],
                    [source].[CatalogNumber],
                    [source].[MediaFormat],
                    [source].[TableOfContentsChecksum],
                    [source].[TableOfContentsChecksumLong]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceReleaseTrack] AS
                (
                    SELECT * FROM @ReleaseTrackCollection WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseTrack] AS [target]
                USING [SourceReleaseTrack] AS [source]
                ON [target].[ReleaseId] = [source].[ReleaseId]
                    AND [target].[TrackNumber] = [source].[TrackNumber]
                    AND [target].[MediaNumber] = [source].[MediaNumber]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Title] = [source].[Title],
                    [target].[Description] = [source].[Description],
                    [target].[DisambiguationText] = [source].[DisambiguationText],
                    [target].[InternationalStandardRecordingCode] = [source].[InternationalStandardRecordingCode]
                WHEN NOT MATCHED THEN INSERT
                (
                    [TrackNumber],
                    [MediaNumber],
                    [ReleaseId],
                    [Title],
                    [Description],
                    [DisambiguationText],
                    [InternationalStandardRecordingCode]
                )
                VALUES
                (
                    [source].[TrackNumber],
                    [source].[MediaNumber],
                    [source].[ReleaseId],
                    [source].[Title],
                    [source].[Description],
                    [source].[DisambiguationText],
                    [source].[InternationalStandardRecordingCode]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                COMMIT TRANSACTION;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_DeleteRelease]
            (
                @Id UNIQUEIDENTIFIER,
                @ResultRowsDeleted INT OUTPUT
            )
            AS
            BEGIN
                DELETE FROM [dbo].[Release] WHERE [Id] = @Id;

                SET @ResultRowsDeleted = @@ROWCOUNT;
            END;");
    }
}
