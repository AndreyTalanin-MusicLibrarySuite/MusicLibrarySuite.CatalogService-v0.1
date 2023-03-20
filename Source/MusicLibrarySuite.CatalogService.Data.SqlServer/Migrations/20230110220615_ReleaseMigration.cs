using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ReleaseDto" /> entity.
/// </summary>
public partial class ReleaseMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Release",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                DisambiguationText = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                Barcode = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                CatalogNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                MediaFormat = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                PublishFormat = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                ReleasedOn = table.Column<DateTime>(type: "date", nullable: false),
                ReleasedOnYearOnly = table.Column<bool>(type: "bit", nullable: false),
                Enabled = table.Column<bool>(type: "bit", nullable: false),
                CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_Release", columns: x => x.Id);
                table.CheckConstraint(name: "CK_Release_Title", sql: "LEN(TRIM([Title])) > 0");
                table.CheckConstraint(name: "CK_Release_Description", sql: "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
                table.CheckConstraint(name: "CK_Release_DisambiguationText", sql: "[DisambiguationText] IS NULL OR LEN(TRIM([DisambiguationText])) > 0");
                table.CheckConstraint(name: "CK_Release_Barcode", sql: "[Barcode] IS NULL OR LEN(TRIM([Barcode])) > 0");
                table.CheckConstraint(name: "CK_Release_CatalogNumber", sql: "[CatalogNumber] IS NULL OR LEN(TRIM([CatalogNumber])) > 0");
                table.CheckConstraint(name: "CK_Release_MediaFormat", sql: "[MediaFormat] IS NULL OR LEN(TRIM([MediaFormat])) > 0");
                table.CheckConstraint(name: "CK_Release_PublishFormat", sql: "[PublishFormat] IS NULL OR LEN(TRIM([PublishFormat])) > 0");
            });

        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Release]
            ADD CONSTRAINT [DF_Release_Id] DEFAULT NEWID() FOR [Id];");

        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Release]
            ADD CONSTRAINT [DF_Release_CreatedOn] DEFAULT SYSDATETIMEOFFSET() FOR [CreatedOn];");

        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Release]
            ADD CONSTRAINT [DF_Release_UpdatedOn] DEFAULT SYSDATETIMEOFFSET() FOR [UpdatedOn];");

        migrationBuilder.CreateIndex(
            name: "IX_Release_Barcode",
            schema: "dbo",
            table: "Release",
            column: "Barcode");

        migrationBuilder.CreateIndex(
            name: "IX_Release_CatalogNumber",
            schema: "dbo",
            table: "Release",
            column: "CatalogNumber");

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_Release_AfterUpdate_SetUpdatedOn]
            ON [dbo].[Release]
            AFTER UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;

                UPDATE [dbo].[Release]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Release] AS [release]
                INNER JOIN [inserted] AS [updatedRelease] ON [updatedRelease].[Id] = [release].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[Release] AS TABLE
            (
                [Id] UNIQUEIDENTIFIER NOT NULL,
                [Title] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
                [DisambiguationText] NVARCHAR(2048) NULL,
                [Barcode] NVARCHAR(32) NULL,
                [CatalogNumber] NVARCHAR(32) NULL,
                [MediaFormat] NVARCHAR(256) NULL,
                [PublishFormat] NVARCHAR(256) NULL,
                [ReleasedOn] DATE NOT NULL,
                [ReleasedOnYearOnly] BIT NOT NULL,
                [Enabled] BIT NOT NULL,
                [CreatedOn] DATETIMEOFFSET NOT NULL,
                [UpdatedOn] DATETIMEOFFSET NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetRelease] (@ReleaseId UNIQUEIDENTIFIER)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT TOP (1) [release].* FROM [dbo].[Release] AS [release] WHERE [release].[Id] = @ReleaseId
            );");

        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetReleases] (@ReleaseIds [dbo].[GuidArray] READONLY)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT [release].* FROM [dbo].[Release] AS [release] INNER JOIN @ReleaseIds AS [releaseId] ON [releaseId].[Value] = [release].[Id]
            );");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_Internal_MergeRelease]
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
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                WITH [SourceRelease] AS
                (
                    SELECT
                        @Id AS [Id],
                        @Title AS [Title],
                        @Description AS [Description],
                        @DisambiguationText AS [DisambiguationText],
                        @Barcode AS [Barcode],
                        @CatalogNumber AS [CatalogNumber],
                        @MediaFormat AS [MediaFormat],
                        @PublishFormat AS [PublishFormat],
                        @ReleasedOn AS [ReleasedOn],
                        @ReleasedOnYearOnly AS [ReleasedOnYearOnly],
                        @Enabled AS [Enabled]
                )
                MERGE INTO [dbo].[Release] AS [target]
                USING [SourceRelease] AS [source]
                ON [target].[Id] = [source].[Id]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Title] = [source].[Title],
                    [target].[Description] = [source].[Description],
                    [target].[DisambiguationText] = [source].[DisambiguationText],
                    [target].[Barcode] = [source].[Barcode],
                    [target].[CatalogNumber] = [source].[CatalogNumber],
                    [target].[MediaFormat] = [source].[MediaFormat],
                    [target].[PublishFormat] = [source].[PublishFormat],
                    [target].[ReleasedOn] = [source].[ReleasedOn],
                    [target].[ReleasedOnYearOnly] = [source].[ReleasedOnYearOnly],
                    [target].[Enabled] = [source].[Enabled]
                WHEN NOT MATCHED THEN INSERT
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
                    [source].[Id],
                    [source].[Title],
                    [source].[Description],
                    [source].[DisambiguationText],
                    [source].[Barcode],
                    [source].[CatalogNumber],
                    [source].[MediaFormat],
                    [source].[PublishFormat],
                    [source].[ReleasedOn],
                    [source].[ReleasedOnYearOnly],
                    [source].[Enabled]
                );

                SET @ResultRowsUpdated = COALESCE(@ResultRowsUpdated, 0) + @@ROWCOUNT;
            END;");

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
                    @Barcode,
                    @CatalogNumber,
                    @MediaFormat,
                    @PublishFormat,
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
                @Barcode NVARCHAR(32),
                @CatalogNumber NVARCHAR(32),
                @MediaFormat NVARCHAR(256),
                @PublishFormat NVARCHAR(256),
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
                    @Barcode,
                    @CatalogNumber,
                    @MediaFormat,
                    @PublishFormat,
                    @ReleasedOn,
                    @ReleasedOnYearOnly,
                    @Enabled,
                    @ResultRowsUpdated OUTPUT;
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
        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetRelease];");

        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetReleases];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_DeleteRelease];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_Internal_MergeRelease];");

        migrationBuilder.DropTable(name: "Release", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[Release];");
    }
}
