using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ReleaseMediaDto" /> entity.
/// </summary>
public partial class ReleaseMediaMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ReleaseMedia",
            schema: "dbo",
            columns: table => new
            {
                MediaNumber = table.Column<byte>(type: "tinyint", nullable: false),
                ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                DisambiguationText = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                MediaFormat = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                CatalogNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                FreeDbChecksum = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                MusicBrainzChecksum = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ReleaseMedia", columns: x => new { x.MediaNumber, x.ReleaseId });
                table.ForeignKey(
                    name: "FK_ReleaseMedia_Release_ReleaseId",
                    column: x => x.ReleaseId,
                    principalSchema: "dbo",
                    principalTable: "Release",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.CheckConstraint(name: "CK_ReleaseMedia_Title", sql: "LEN(TRIM([Title])) > 0");
                table.CheckConstraint(name: "CK_ReleaseMedia_Description", sql: "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
                table.CheckConstraint(name: "CK_ReleaseMedia_DisambiguationText", sql: "[DisambiguationText] IS NULL OR LEN(TRIM([DisambiguationText])) > 0");
                table.CheckConstraint(name: "CK_ReleaseMedia_MediaFormat", sql: "[MediaFormat] IS NULL OR LEN(TRIM([MediaFormat])) > 0");
                table.CheckConstraint(name: "CK_ReleaseMedia_CatalogNumber", sql: "[CatalogNumber] IS NULL OR LEN(TRIM([CatalogNumber])) > 0");
                table.CheckConstraint(name: "CK_ReleaseMedia_FreeDbChecksum", sql: "[FreeDbChecksum] IS NULL OR LEN(TRIM([FreeDbChecksum])) > 0");
                table.CheckConstraint(name: "CK_ReleaseMedia_MusicBrainzChecksum", sql: "[MusicBrainzChecksum] IS NULL OR LEN(TRIM([MusicBrainzChecksum])) > 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseMedia_ReleaseId",
            schema: "dbo",
            table: "ReleaseMedia",
            column: "ReleaseId");

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseMedia_CatalogNumber",
            schema: "dbo",
            table: "ReleaseMedia",
            column: "CatalogNumber");

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseMedia_FreeDbChecksum",
            schema: "dbo",
            table: "ReleaseMedia",
            column: "FreeDbChecksum");

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseMedia_MusicBrainzChecksum",
            schema: "dbo",
            table: "ReleaseMedia",
            column: "MusicBrainzChecksum");

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleaseMedia_AfterInsertUpdateDelete_SetReleaseUpdatedOn]
            ON [dbo].[ReleaseMedia]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedRelease] AS
                (
                    SELECT [insertedReleaseMedia].[ReleaseId] AS [Id]
                    FROM [inserted] AS [insertedReleaseMedia]
                    UNION
                    SELECT [deletedReleaseMedia].[ReleaseId] AS [Id]
                    FROM [deleted] AS [deletedReleaseMedia]
                )
                UPDATE [dbo].[Release]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Release] AS [release]
                INNER JOIN [UpdatedRelease] AS [updatedRelease] ON [updatedRelease].[Id] = [release].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleaseMedia] AS TABLE
            (
                [MediaNumber] TINYINT NOT NULL,
                [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
                [Title] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
                [DisambiguationText] NVARCHAR(2048) NULL,
                [MediaFormat] NVARCHAR(256) NULL,
                [CatalogNumber] NVARCHAR(32) NULL,
                [FreeDbChecksum] NVARCHAR(64) NULL,
                [MusicBrainzChecksum] NVARCHAR(64) NULL
            );");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_Internal_MergeReleaseMediaCollection]
            (
                @Id UNIQUEIDENTIFIER,
                @ReleaseMediaCollection [dbo].[ReleaseMedia] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [SourceReleaseMedia] AS
                (
                    SELECT
                        [MediaNumber],
                        @Id AS [ReleaseId],
                        [Title],
                        [Description],
                        [DisambiguationText],
                        [MediaFormat],
                        [CatalogNumber],
                        [FreeDbChecksum],
                        [MusicBrainzChecksum]
                    FROM @ReleaseMediaCollection
                    WHERE [ReleaseId] = CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER)
                        OR [ReleaseId] = @Id
                )
                MERGE INTO [dbo].[ReleaseMedia] AS [target]
                USING [SourceReleaseMedia] AS [source]
                ON [target].[MediaNumber] = [source].[MediaNumber]
                    AND [target].[ReleaseId] = [source].[ReleaseId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Title] = [source].[Title],
                    [target].[Description] = [source].[Description],
                    [target].[DisambiguationText] = [source].[DisambiguationText],
                    [target].[MediaFormat] = [source].[MediaFormat],
                    [target].[CatalogNumber] = [source].[CatalogNumber],
                    [target].[FreeDbChecksum] = [source].[FreeDbChecksum],
                    [target].[MusicBrainzChecksum] = [source].[MusicBrainzChecksum]
                WHEN NOT MATCHED THEN INSERT
                (
                    [MediaNumber],
                    [ReleaseId],
                    [Title],
                    [Description],
                    [DisambiguationText],
                    [MediaFormat],
                    [CatalogNumber],
                    [FreeDbChecksum],
                    [MusicBrainzChecksum]
                )
                VALUES
                (
                    [source].[MediaNumber],
                    [source].[ReleaseId],
                    [source].[Title],
                    [source].[Description],
                    [source].[DisambiguationText],
                    [source].[MediaFormat],
                    [source].[CatalogNumber],
                    [source].[FreeDbChecksum],
                    [source].[MusicBrainzChecksum]
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
                @MediaFormat NVARCHAR(256),
                @PublishFormat NVARCHAR(256),
                @CatalogNumber NVARCHAR(32),
                @Barcode NVARCHAR(32),
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

                EXEC [dbo].[sp_Internal_MergeReleaseMediaCollection]
                    @Id,
                    @ReleaseMediaCollection;

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
                @ReleaseMediaCollection [dbo].[ReleaseMedia] READONLY,
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

                EXEC [dbo].[sp_Internal_MergeReleaseMediaCollection]
                    @Id,
                    @ReleaseMediaCollection;

                COMMIT TRANSACTION;
            END;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_Internal_MergeReleaseMediaCollection];");

        migrationBuilder.DropTable(name: "ReleaseMedia", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleaseMedia];");

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
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
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
            END;");
    }
}
