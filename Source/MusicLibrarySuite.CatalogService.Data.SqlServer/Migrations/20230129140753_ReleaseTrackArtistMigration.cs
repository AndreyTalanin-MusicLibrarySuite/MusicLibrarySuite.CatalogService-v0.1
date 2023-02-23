using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ReleaseTrackArtistDto" />, <see cref="ReleaseTrackFeaturedArtistDto" />,
/// <see cref="ReleaseTrackPerformerDto" /> and <see cref="ReleaseTrackComposerDto" /> entities.
/// </summary>
public partial class ReleaseTrackArtistMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ReleaseTrackArtist",
            schema: "dbo",
            columns: table => new
            {
                TrackNumber = table.Column<byte>(type: "tinyint", nullable: false),
                MediaNumber = table.Column<byte>(type: "tinyint", nullable: false),
                ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ReleaseTrackArtist", columns: x => new { x.TrackNumber, x.MediaNumber, x.ReleaseId, x.ArtistId });
                table.ForeignKey(
                    name: "FK_ReleaseTrackArtist_ReleaseTrack_TrackNumber_MediaNumber_ReleaseId",
                    columns: x => new { x.TrackNumber, x.MediaNumber, x.ReleaseId },
                    principalSchema: "dbo",
                    principalTable: "ReleaseTrack",
                    principalColumns: new[] { "TrackNumber", "MediaNumber", "ReleaseId" },
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ReleaseTrackArtist_Artist_ArtistId",
                    column: x => x.ArtistId,
                    principalSchema: "dbo",
                    principalTable: "Artist",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ReleaseTrackFeaturedArtist",
            schema: "dbo",
            columns: table => new
            {
                TrackNumber = table.Column<byte>(type: "tinyint", nullable: false),
                MediaNumber = table.Column<byte>(type: "tinyint", nullable: false),
                ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ReleaseTrackFeaturedArtist", columns: x => new { x.TrackNumber, x.MediaNumber, x.ReleaseId, x.ArtistId });
                table.ForeignKey(
                    name: "FK_ReleaseTrackFeaturedArtist_ReleaseTrack_TrackNumber_MediaNumber_ReleaseId",
                    columns: x => new { x.TrackNumber, x.MediaNumber, x.ReleaseId },
                    principalSchema: "dbo",
                    principalTable: "ReleaseTrack",
                    principalColumns: new[] { "TrackNumber", "MediaNumber", "ReleaseId" },
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ReleaseTrackFeaturedArtist_Artist_ArtistId",
                    column: x => x.ArtistId,
                    principalSchema: "dbo",
                    principalTable: "Artist",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ReleaseTrackPerformer",
            schema: "dbo",
            columns: table => new
            {
                TrackNumber = table.Column<byte>(type: "tinyint", nullable: false),
                MediaNumber = table.Column<byte>(type: "tinyint", nullable: false),
                ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ReleaseTrackPerformer", columns: x => new { x.TrackNumber, x.MediaNumber, x.ReleaseId, x.ArtistId });
                table.ForeignKey(
                    name: "FK_ReleaseTrackPerformer_ReleaseTrack_TrackNumber_MediaNumber_ReleaseId",
                    columns: x => new { x.TrackNumber, x.MediaNumber, x.ReleaseId },
                    principalSchema: "dbo",
                    principalTable: "ReleaseTrack",
                    principalColumns: new[] { "TrackNumber", "MediaNumber", "ReleaseId" },
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ReleaseTrackPerformer_Artist_ArtistId",
                    column: x => x.ArtistId,
                    principalSchema: "dbo",
                    principalTable: "Artist",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ReleaseTrackComposer",
            schema: "dbo",
            columns: table => new
            {
                TrackNumber = table.Column<byte>(type: "tinyint", nullable: false),
                MediaNumber = table.Column<byte>(type: "tinyint", nullable: false),
                ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ReleaseTrackComposer", columns: x => new { x.TrackNumber, x.MediaNumber, x.ReleaseId, x.ArtistId });
                table.ForeignKey(
                    name: "FK_ReleaseTrackComposer_ReleaseTrack_TrackNumber_MediaNumber_ReleaseId",
                    columns: x => new { x.TrackNumber, x.MediaNumber, x.ReleaseId },
                    principalSchema: "dbo",
                    principalTable: "ReleaseTrack",
                    principalColumns: new[] { "TrackNumber", "MediaNumber", "ReleaseId" },
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ReleaseTrackComposer_Artist_ArtistId",
                    column: x => x.ArtistId,
                    principalSchema: "dbo",
                    principalTable: "Artist",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseTrackArtist_TrackNumber_MediaNumber_ReleaseId",
            schema: "dbo",
            table: "ReleaseTrackArtist",
            columns: new[] { "TrackNumber", "MediaNumber", "ReleaseId" });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseTrackArtist_ArtistId",
            schema: "dbo",
            table: "ReleaseTrackArtist",
            column: "ArtistId");

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseTrackArtist_TrackNumber_MediaNumber_ReleaseId_Order",
            schema: "dbo",
            table: "ReleaseTrackArtist",
            columns: new[] { "TrackNumber", "MediaNumber", "ReleaseId", "Order" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseTrackFeaturedArtist_TrackNumber_MediaNumber_ReleaseId",
            schema: "dbo",
            table: "ReleaseTrackFeaturedArtist",
            columns: new[] { "TrackNumber", "MediaNumber", "ReleaseId" });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseTrackFeaturedArtist_ArtistId",
            schema: "dbo",
            table: "ReleaseTrackFeaturedArtist",
            column: "ArtistId");

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseTrackFeaturedArtist_TrackNumber_MediaNumber_ReleaseId_Order",
            schema: "dbo",
            table: "ReleaseTrackFeaturedArtist",
            columns: new[] { "TrackNumber", "MediaNumber", "ReleaseId", "Order" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseTrackPerformer_TrackNumber_MediaNumber_ReleaseId",
            schema: "dbo",
            table: "ReleaseTrackPerformer",
            columns: new[] { "TrackNumber", "MediaNumber", "ReleaseId" });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseTrackPerformer_ArtistId",
            schema: "dbo",
            table: "ReleaseTrackPerformer",
            column: "ArtistId");

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseTrackPerformer_TrackNumber_MediaNumber_ReleaseId_Order",
            schema: "dbo",
            table: "ReleaseTrackPerformer",
            columns: new[] { "TrackNumber", "MediaNumber", "ReleaseId", "Order" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseTrackComposer_TrackNumber_MediaNumber_ReleaseId",
            schema: "dbo",
            table: "ReleaseTrackComposer",
            columns: new[] { "TrackNumber", "MediaNumber", "ReleaseId" });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseTrackComposer_ArtistId",
            schema: "dbo",
            table: "ReleaseTrackComposer",
            column: "ArtistId");

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseTrackComposer_TrackNumber_MediaNumber_ReleaseId_Order",
            schema: "dbo",
            table: "ReleaseTrackComposer",
            columns: new[] { "TrackNumber", "MediaNumber", "ReleaseId", "Order" },
            unique: true);

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleaseTrackArtist_AfterInsertUpdateDelete_SetReleaseUpdatedOn]
            ON [dbo].[ReleaseTrackArtist]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedRelease] AS
                (
                    SELECT [insertedReleaseTrackArtist].[ReleaseId] AS [Id]
                    FROM [inserted] AS [insertedReleaseTrackArtist]
                    UNION
                    SELECT [deletedReleaseTrackArtist].[ReleaseId] AS [Id]
                    FROM [deleted] AS [deletedReleaseTrackArtist]
                )
                UPDATE [dbo].[Release]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Release] AS [release]
                INNER JOIN [UpdatedRelease] AS [updatedRelease] ON [updatedRelease].[Id] = [release].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleaseTrackFeaturedArtist_AfterInsertUpdateDelete_SetReleaseUpdatedOn]
            ON [dbo].[ReleaseTrackFeaturedArtist]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedRelease] AS
                (
                    SELECT [insertedReleaseTrackFeaturedArtist].[ReleaseId] AS [Id]
                    FROM [inserted] AS [insertedReleaseTrackFeaturedArtist]
                    UNION
                    SELECT [deletedReleaseTrackFeaturedArtist].[ReleaseId] AS [Id]
                    FROM [deleted] AS [deletedReleaseTrackFeaturedArtist]
                )
                UPDATE [dbo].[Release]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Release] AS [release]
                INNER JOIN [UpdatedRelease] AS [updatedRelease] ON [updatedRelease].[Id] = [release].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleaseTrackPerformer_AfterInsertUpdateDelete_SetReleaseUpdatedOn]
            ON [dbo].[ReleaseTrackPerformer]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedRelease] AS
                (
                    SELECT [insertedReleaseTrackPerformer].[ReleaseId] AS [Id]
                    FROM [inserted] AS [insertedReleaseTrackPerformer]
                    UNION
                    SELECT [deletedReleaseTrackPerformer].[ReleaseId] AS [Id]
                    FROM [deleted] AS [deletedReleaseTrackPerformer]
                )
                UPDATE [dbo].[Release]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Release] AS [release]
                INNER JOIN [UpdatedRelease] AS [updatedRelease] ON [updatedRelease].[Id] = [release].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleaseTrackComposer_AfterInsertUpdateDelete_SetReleaseUpdatedOn]
            ON [dbo].[ReleaseTrackComposer]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedRelease] AS
                (
                    SELECT [insertedReleaseTrackComposer].[ReleaseId] AS [Id]
                    FROM [inserted] AS [insertedReleaseTrackComposer]
                    UNION
                    SELECT [deletedReleaseTrackComposer].[ReleaseId] AS [Id]
                    FROM [deleted] AS [deletedReleaseTrackComposer]
                )
                UPDATE [dbo].[Release]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Release] AS [release]
                INNER JOIN [UpdatedRelease] AS [updatedRelease] ON [updatedRelease].[Id] = [release].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleaseTrackArtist] AS TABLE
            (
                [TrackNumber] TINYINT NOT NULL,
                [MediaNumber] TINYINT NOT NULL,
                [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
                [ArtistId] UNIQUEIDENTIFIER NOT NULL,
                [Order] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleaseTrackFeaturedArtist] AS TABLE
            (
                [TrackNumber] TINYINT NOT NULL,
                [MediaNumber] TINYINT NOT NULL,
                [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
                [ArtistId] UNIQUEIDENTIFIER NOT NULL,
                [Order] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleaseTrackPerformer] AS TABLE
            (
                [TrackNumber] TINYINT NOT NULL,
                [MediaNumber] TINYINT NOT NULL,
                [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
                [ArtistId] UNIQUEIDENTIFIER NOT NULL,
                [Order] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleaseTrackComposer] AS TABLE
            (
                [TrackNumber] TINYINT NOT NULL,
                [MediaNumber] TINYINT NOT NULL,
                [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
                [ArtistId] UNIQUEIDENTIFIER NOT NULL,
                [Order] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_Internal_MergeReleaseTrackArtists]
            (
                @Id UNIQUEIDENTIFIER,
                @ReleaseTrackArtists [dbo].[ReleaseTrackArtist] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [SourceReleaseTrackArtist] AS
                (
                    SELECT
                        [TrackNumber],
                        [MediaNumber],
                        @Id AS [ReleaseId],
                        [ArtistId],
                        [Order]
                    FROM @ReleaseTrackArtists
                    WHERE [ReleaseId] = CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER)
                        OR [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseTrackArtist] AS [target]
                USING [SourceReleaseTrackArtist] AS [source]
                ON [target].[TrackNumber] = [source].[TrackNumber]
                    AND [target].[MediaNumber] = [source].[MediaNumber]
                    AND [target].[ReleaseId] = [source].[ReleaseId]
                    AND [target].[ArtistId] = [source].[ArtistId]
                WHEN MATCHED THEN UPDATE
                SET [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [TrackNumber],
                    [MediaNumber],
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[TrackNumber],
                    [source].[MediaNumber],
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_Internal_MergeReleaseTrackFeaturedArtists]
            (
                @Id UNIQUEIDENTIFIER,
                @ReleaseTrackFeaturedArtists [dbo].[ReleaseTrackFeaturedArtist] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [SourceReleaseTrackFeaturedArtist] AS
                (
                    SELECT
                        [TrackNumber],
                        [MediaNumber],
                        @Id AS [ReleaseId],
                        [ArtistId],
                        [Order]
                    FROM @ReleaseTrackFeaturedArtists
                    WHERE [ReleaseId] = CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER)
                        OR [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseTrackFeaturedArtist] AS [target]
                USING [SourceReleaseTrackFeaturedArtist] AS [source]
                ON [target].[TrackNumber] = [source].[TrackNumber]
                    AND [target].[MediaNumber] = [source].[MediaNumber]
                    AND [target].[ReleaseId] = [source].[ReleaseId]
                    AND [target].[ArtistId] = [source].[ArtistId]
                WHEN MATCHED THEN UPDATE
                SET [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [TrackNumber],
                    [MediaNumber],
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[TrackNumber],
                    [source].[MediaNumber],
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_Internal_MergeReleaseTrackPerformers]
            (
                @Id UNIQUEIDENTIFIER,
                @ReleaseTrackPerformers [dbo].[ReleaseTrackPerformer] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [SourceReleaseTrackPerformer] AS
                (
                    SELECT
                        [TrackNumber],
                        [MediaNumber],
                        @Id AS [ReleaseId],
                        [ArtistId],
                        [Order]
                    FROM @ReleaseTrackPerformers
                    WHERE [ReleaseId] = CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER)
                        OR [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseTrackPerformer] AS [target]
                USING [SourceReleaseTrackPerformer] AS [source]
                ON [target].[TrackNumber] = [source].[TrackNumber]
                    AND [target].[MediaNumber] = [source].[MediaNumber]
                    AND [target].[ReleaseId] = [source].[ReleaseId]
                    AND [target].[ArtistId] = [source].[ArtistId]
                WHEN MATCHED THEN UPDATE
                SET [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [TrackNumber],
                    [MediaNumber],
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[TrackNumber],
                    [source].[MediaNumber],
                    [source].[ReleaseId],
                    [source].[ArtistId],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseId] = @Id THEN DELETE;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_Internal_MergeReleaseTrackComposers]
            (
                @Id UNIQUEIDENTIFIER,
                @ReleaseTrackComposers [dbo].[ReleaseTrackComposer] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [SourceReleaseTrackComposer] AS
                (
                    SELECT
                        [TrackNumber],
                        [MediaNumber],
                        @Id AS [ReleaseId],
                        [ArtistId],
                        [Order]
                    FROM @ReleaseTrackComposers
                    WHERE [ReleaseId] = CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER)
                        OR [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseTrackComposer] AS [target]
                USING [SourceReleaseTrackComposer] AS [source]
                ON [target].[TrackNumber] = [source].[TrackNumber]
                    AND [target].[MediaNumber] = [source].[MediaNumber]
                    AND [target].[ReleaseId] = [source].[ReleaseId]
                    AND [target].[ArtistId] = [source].[ArtistId]
                WHEN MATCHED THEN UPDATE
                SET [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [TrackNumber],
                    [MediaNumber],
                    [ReleaseId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[TrackNumber],
                    [source].[MediaNumber],
                    [source].[ReleaseId],
                    [source].[ArtistId],
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

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_Internal_MergeReleaseTrackArtists];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_Internal_MergeReleaseTrackFeaturedArtists];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_Internal_MergeReleaseTrackPerformers];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_Internal_MergeReleaseTrackComposers];");

        migrationBuilder.DropTable(name: "ReleaseTrackArtist", schema: "dbo");

        migrationBuilder.DropTable(name: "ReleaseTrackComposer", schema: "dbo");

        migrationBuilder.DropTable(name: "ReleaseTrackFeaturedArtist", schema: "dbo");

        migrationBuilder.DropTable(name: "ReleaseTrackPerformer", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleaseTrackArtist];");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleaseTrackFeaturedArtist];");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleaseTrackPerformer];");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleaseTrackComposer];");

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
    }
}
