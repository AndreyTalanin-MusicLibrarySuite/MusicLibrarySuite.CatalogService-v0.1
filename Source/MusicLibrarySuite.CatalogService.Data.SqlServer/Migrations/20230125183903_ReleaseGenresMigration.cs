using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ReleaseGenreDto" /> entity.
/// </summary>
public partial class ReleaseGenresMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ReleaseGenre",
            schema: "dbo",
            columns: table => new
            {
                ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                GenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ReleaseGenre", columns: x => new { x.ReleaseId, x.GenreId });
                table.ForeignKey(
                    name: "FK_ReleaseGenre_Release_ReleaseId",
                    column: x => x.ReleaseId,
                    principalSchema: "dbo",
                    principalTable: "Release",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ReleaseGenre_Genre_GenreId",
                    column: x => x.GenreId,
                    principalSchema: "dbo",
                    principalTable: "Genre",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseGenre_ReleaseId",
            schema: "dbo",
            table: "ReleaseGenre",
            column: "ReleaseId");

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseGenre_GenreId",
            schema: "dbo",
            table: "ReleaseGenre",
            column: "GenreId");

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseGenre_ReleaseId_Order",
            schema: "dbo",
            table: "ReleaseGenre",
            columns: new[] { "ReleaseId", "Order" },
            unique: true);

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleaseGenre_AfterInsertUpdateDelete_SetReleaseUpdatedOn]
            ON [dbo].[ReleaseGenre]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedRelease] AS
                (
                    SELECT [insertedReleaseGenre].[ReleaseId] AS [Id]
                    FROM [inserted] AS [insertedReleaseGenre]
                    UNION
                    SELECT [deletedReleaseGenre].[ReleaseId] AS [Id]
                    FROM [deleted] AS [deletedReleaseGenre]
                )
                UPDATE [dbo].[Release]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Release] AS [release]
                INNER JOIN [UpdatedRelease] AS [updatedRelease] ON [updatedRelease].[Id] = [release].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleaseGenre] AS TABLE
            (
                [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
                [GenreId] UNIQUEIDENTIFIER NOT NULL,
                [Order] INT NOT NULL
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

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_DeleteRelease];");

        migrationBuilder.DropTable(name: "ReleaseGenre", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleaseGenre];");

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
