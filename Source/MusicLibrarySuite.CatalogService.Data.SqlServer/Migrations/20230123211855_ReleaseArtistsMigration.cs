using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ReleaseArtistDto" />, <see cref="ReleaseFeaturedArtistDto" />,
/// <see cref="ReleasePerformerDto" /> and <see cref="ReleaseComposerDto" /> entities.
/// </summary>
public partial class ReleaseArtistsMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ReleaseArtist",
            schema: "dbo",
            columns: table => new
            {
                ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ReleaseArtist", columns: x => new { x.ReleaseId, x.ArtistId });
                table.ForeignKey(
                    name: "FK_ReleaseArtist_Release_ReleaseId",
                    column: x => x.ReleaseId,
                    principalSchema: "dbo",
                    principalTable: "Release",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ReleaseArtist_Artist_ArtistId",
                    column: x => x.ArtistId,
                    principalSchema: "dbo",
                    principalTable: "Artist",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ReleaseFeaturedArtist",
            schema: "dbo",
            columns: table => new
            {
                ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ReleaseFeaturedArtist", columns: x => new { x.ReleaseId, x.ArtistId });
                table.ForeignKey(
                    name: "FK_ReleaseFeaturedArtist_Release_ReleaseId",
                    column: x => x.ReleaseId,
                    principalSchema: "dbo",
                    principalTable: "Release",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ReleaseFeaturedArtist_Artist_ArtistId",
                    column: x => x.ArtistId,
                    principalSchema: "dbo",
                    principalTable: "Artist",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ReleasePerformer",
            schema: "dbo",
            columns: table => new
            {
                ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ReleasePerformer", columns: x => new { x.ReleaseId, x.ArtistId });
                table.ForeignKey(
                    name: "FK_ReleasePerformer_Release_ReleaseId",
                    column: x => x.ReleaseId,
                    principalSchema: "dbo",
                    principalTable: "Release",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ReleasePerformer_Artist_ArtistId",
                    column: x => x.ArtistId,
                    principalSchema: "dbo",
                    principalTable: "Artist",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ReleaseComposer",
            schema: "dbo",
            columns: table => new
            {
                ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ReleaseComposer", columns: x => new { x.ReleaseId, x.ArtistId });
                table.ForeignKey(
                    name: "FK_ReleaseComposer_Release_ReleaseId",
                    column: x => x.ReleaseId,
                    principalSchema: "dbo",
                    principalTable: "Release",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ReleaseComposer_Artist_ArtistId",
                    column: x => x.ArtistId,
                    principalSchema: "dbo",
                    principalTable: "Artist",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseArtist_ReleaseId",
            schema: "dbo",
            table: "ReleaseArtist",
            column: "ReleaseId");

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseArtist_ArtistId",
            schema: "dbo",
            table: "ReleaseArtist",
            column: "ArtistId");

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseArtist_ReleaseId_Order",
            schema: "dbo",
            table: "ReleaseArtist",
            columns: new[] { "ReleaseId", "Order" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseFeaturedArtist_ReleaseId",
            schema: "dbo",
            table: "ReleaseFeaturedArtist",
            column: "ReleaseId");

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseFeaturedArtist_ArtistId",
            schema: "dbo",
            table: "ReleaseFeaturedArtist",
            column: "ArtistId");

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseFeaturedArtist_ReleaseId_Order",
            schema: "dbo",
            table: "ReleaseFeaturedArtist",
            columns: new[] { "ReleaseId", "Order" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ReleasePerformer_ReleaseId",
            schema: "dbo",
            table: "ReleasePerformer",
            column: "ReleaseId");

        migrationBuilder.CreateIndex(
            name: "IX_ReleasePerformer_ArtistId",
            schema: "dbo",
            table: "ReleasePerformer",
            column: "ArtistId");

        migrationBuilder.CreateIndex(
            name: "UIX_ReleasePerformer_ReleaseId_Order",
            schema: "dbo",
            table: "ReleasePerformer",
            columns: new[] { "ReleaseId", "Order" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseComposer_ReleaseId",
            schema: "dbo",
            table: "ReleaseComposer",
            column: "ReleaseId");

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseComposer_ArtistId",
            schema: "dbo",
            table: "ReleaseComposer",
            column: "ArtistId");

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseComposer_ReleaseId_Order",
            schema: "dbo",
            table: "ReleaseComposer",
            columns: new[] { "ReleaseId", "Order" },
            unique: true);

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleaseArtist_AfterInsertUpdateDelete_SetReleaseUpdatedOn]
            ON [dbo].[ReleaseArtist]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedRelease] AS
                (
                    SELECT [insertedReleaseArtist].[ReleaseId] AS [Id]
                    FROM [inserted] AS [insertedReleaseArtist]
                    UNION
                    SELECT [deletedReleaseArtist].[ReleaseId] AS [Id]
                    FROM [deleted] AS [deletedReleaseArtist]
                )
                UPDATE [dbo].[Release]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Release] AS [release]
                INNER JOIN [UpdatedRelease] AS [updatedRelease] ON [updatedRelease].[Id] = [release].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleaseFeaturedArtist_AfterInsertUpdateDelete_SetReleaseUpdatedOn]
            ON [dbo].[ReleaseFeaturedArtist]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedRelease] AS
                (
                    SELECT [insertedReleaseFeaturedArtist].[ReleaseId] AS [Id]
                    FROM [inserted] AS [insertedReleaseFeaturedArtist]
                    UNION
                    SELECT [deletedReleaseFeaturedArtist].[ReleaseId] AS [Id]
                    FROM [deleted] AS [deletedReleaseFeaturedArtist]
                )
                UPDATE [dbo].[Release]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Release] AS [release]
                INNER JOIN [UpdatedRelease] AS [updatedRelease] ON [updatedRelease].[Id] = [release].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleasePerformer_AfterInsertUpdateDelete_SetReleaseUpdatedOn]
            ON [dbo].[ReleasePerformer]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedRelease] AS
                (
                    SELECT [insertedReleasePerformer].[ReleaseId] AS [Id]
                    FROM [inserted] AS [insertedReleasePerformer]
                    UNION
                    SELECT [deletedReleasePerformer].[ReleaseId] AS [Id]
                    FROM [deleted] AS [deletedReleasePerformer]
                )
                UPDATE [dbo].[Release]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Release] AS [release]
                INNER JOIN [UpdatedRelease] AS [updatedRelease] ON [updatedRelease].[Id] = [release].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleaseComposer_AfterInsertUpdateDelete_SetReleaseUpdatedOn]
            ON [dbo].[ReleaseComposer]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedRelease] AS
                (
                    SELECT [insertedReleaseComposer].[ReleaseId] AS [Id]
                    FROM [inserted] AS [insertedReleaseComposer]
                    UNION
                    SELECT [deletedReleaseComposer].[ReleaseId] AS [Id]
                    FROM [deleted] AS [deletedReleaseComposer]
                )
                UPDATE [dbo].[Release]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Release] AS [release]
                INNER JOIN [UpdatedRelease] AS [updatedRelease] ON [updatedRelease].[Id] = [release].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleaseArtist] AS TABLE
            (
                [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
                [ArtistId] UNIQUEIDENTIFIER NOT NULL,
                [Order] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleaseFeaturedArtist] AS TABLE
            (
                [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
                [ArtistId] UNIQUEIDENTIFIER NOT NULL,
                [Order] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleasePerformer] AS TABLE
            (
                [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
                [ArtistId] UNIQUEIDENTIFIER NOT NULL,
                [Order] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleaseComposer] AS TABLE
            (
                [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
                [ArtistId] UNIQUEIDENTIFIER NOT NULL,
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

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_DeleteRelease];");

        migrationBuilder.DropTable(name: "ReleaseArtist", schema: "dbo");

        migrationBuilder.DropTable(name: "ReleaseFeaturedArtist", schema: "dbo");

        migrationBuilder.DropTable(name: "ReleasePerformer", schema: "dbo");

        migrationBuilder.DropTable(name: "ReleaseComposer", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleaseArtist];");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleaseFeaturedArtist];");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleasePerformer];");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleaseComposer];");

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
