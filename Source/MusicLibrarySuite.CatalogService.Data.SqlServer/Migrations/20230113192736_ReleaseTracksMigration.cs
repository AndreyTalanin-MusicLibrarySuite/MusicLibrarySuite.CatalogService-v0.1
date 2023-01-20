using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ReleaseTrackDto" /> entity.
/// </summary>
public partial class ReleaseTracksMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ReleaseTrack",
            schema: "dbo",
            columns: table => new
            {
                TrackNumber = table.Column<byte>(type: "tinyint", nullable: false),
                MediaNumber = table.Column<byte>(type: "tinyint", nullable: false),
                ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                DisambiguationText = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                InternationalStandardRecordingCode = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ReleaseTrack", columns: x => new { x.TrackNumber, x.MediaNumber, x.ReleaseId });
                table.ForeignKey(
                    name: "FK_ReleaseTrack_ReleaseMedia_MediaNumber_ReleaseId",
                    columns: x => new { x.MediaNumber, x.ReleaseId },
                    principalSchema: "dbo",
                    principalTable: "ReleaseMedia",
                    principalColumns: new[] { "MediaNumber", "ReleaseId" },
                    onDelete: ReferentialAction.Cascade);
                table.CheckConstraint(name: "CK_ReleaseTrack_Title", sql: "LEN(TRIM([Title])) > 0");
                table.CheckConstraint(name: "CK_ReleaseTrack_Description", sql: "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
                table.CheckConstraint(name: "CK_ReleaseTrack_DisambiguationText", sql: "[DisambiguationText] IS NULL OR LEN(TRIM([DisambiguationText])) > 0");
                table.CheckConstraint(name: "CK_ReleaseTrack_InternationalStandardRecordingCode", sql: "[InternationalStandardRecordingCode] IS NULL OR LEN(TRIM([InternationalStandardRecordingCode])) > 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseTrack_ReleaseId",
            schema: "dbo",
            table: "ReleaseTrack",
            column: "ReleaseId");

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseTrack_MediaNumber_ReleaseId",
            schema: "dbo",
            table: "ReleaseTrack",
            columns: new[] { "MediaNumber", "ReleaseId" });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseTrack_InternationalStandardRecordingCode",
            schema: "dbo",
            table: "ReleaseTrack",
            column: "InternationalStandardRecordingCode");

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleaseTrack_AfterInsertUpdateDelete_SetReleaseUpdatedOn]
            ON [dbo].[ReleaseTrack]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedRelease] AS
                (
                    SELECT [insertedReleaseTrack].[ReleaseId] AS [Id]
                    FROM [inserted] AS [insertedReleaseTrack]
                    UNION
                    SELECT [deletedReleaseTrack].[ReleaseId] AS [Id]
                    FROM [deleted] AS [deletedReleaseTrack]
                )
                UPDATE [dbo].[Release]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Release] AS [release]
                INNER JOIN [UpdatedRelease] AS [updatedRelease] ON [updatedRelease].[Id] = [release].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleaseTrack] AS TABLE
            (
                [TrackNumber] TINYINT NOT NULL,
                [MediaNumber] TINYINT NOT NULL,
                [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
                [Title] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
                [DisambiguationText] NVARCHAR(2048) NULL,
                [InternationalStandardRecordingCode] NVARCHAR(32) NULL
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

        migrationBuilder.DropTable(name: "ReleaseTrack", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleaseTrack];");

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
                @ReleaseMediaCollection [dbo].[ReleaseMedia] READONLY,
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

                WITH [SourceReleaseMedia] AS
                (
                    SELECT * FROM @ReleaseMediaCollection WHERE [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseMedia] AS [target]
                USING [SourceReleaseMedia] AS [source]
                ON [target].[MediaNumber] = [source].[MediaNumber] AND [target].[ReleaseId] = [source].[ReleaseId]
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
                @ReleaseMediaCollection [dbo].[ReleaseMedia] READONLY,
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
