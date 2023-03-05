using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="WorkDto" /> entity.
/// </summary>
public partial class WorksMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Work",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                DisambiguationText = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                InternationalStandardMusicalWorkCode = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                ReleasedOn = table.Column<DateTime>(type: "date", nullable: false),
                ReleasedOnYearOnly = table.Column<bool>(type: "bit", nullable: false),
                SystemEntity = table.Column<bool>(type: "bit", nullable: false),
                Enabled = table.Column<bool>(type: "bit", nullable: false),
                CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_Work", columns: x => x.Id);
                table.CheckConstraint(name: "CK_Work_Title", sql: "LEN(TRIM([Title])) > 0");
                table.CheckConstraint(name: "CK_Work_Description", sql: "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
                table.CheckConstraint(name: "CK_Work_DisambiguationText", sql: "[DisambiguationText] IS NULL OR LEN(TRIM([DisambiguationText])) > 0");
                table.CheckConstraint(name: "CK_Work_InternationalStandardMusicalWorkCode", sql: "[InternationalStandardMusicalWorkCode] IS NULL OR LEN(TRIM([InternationalStandardMusicalWorkCode])) > 0");
            });

        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Work]
            ADD CONSTRAINT [DF_Work_Id] DEFAULT NEWID() FOR [Id];");

        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Work]
            ADD CONSTRAINT [DF_Work_CreatedOn] DEFAULT SYSDATETIMEOFFSET() FOR [CreatedOn];");

        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Work]
            ADD CONSTRAINT [DF_Work_UpdatedOn] DEFAULT SYSDATETIMEOFFSET() FOR [UpdatedOn];");

        migrationBuilder.CreateIndex(
            name: "IX_Work_InternationalStandardMusicalWorkCode",
            schema: "dbo",
            table: "Work",
            column: "InternationalStandardMusicalWorkCode");

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_Work_AfterUpdate_SetUpdatedOn]
            ON [dbo].[Work]
            AFTER UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;

                UPDATE [dbo].[Work]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Work] AS [work]
                INNER JOIN [inserted] AS [updatedWork] ON [updatedWork].[Id] = [work].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[Work] AS TABLE
            (
                [Id] UNIQUEIDENTIFIER NOT NULL,
                [Title] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
                [DisambiguationText] NVARCHAR(2048) NULL,
                [InternationalStandardMusicalWorkCode] NVARCHAR(32) NULL,
                [ReleasedOn] DATE NOT NULL,
                [ReleasedOnYearOnly] BIT NOT NULL,
                [SystemEntity] BIT NOT NULL,
                [Enabled] BIT NOT NULL,
                [CreatedOn] DATETIMEOFFSET NOT NULL,
                [UpdatedOn] DATETIMEOFFSET NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetWork] (@WorkId UNIQUEIDENTIFIER)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT TOP (1) [work].* FROM [dbo].[Work] AS [work] WHERE [work].[Id] = @WorkId
            );");

        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetWorks] (@WorkIds [dbo].[GuidArray] READONLY)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT [work].* FROM [dbo].[Work] AS [work] INNER JOIN @WorkIds AS [workId] ON [workId].[Value] = [work].[Id]
            );");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_Internal_MergeWork]
            (
                @Id UNIQUEIDENTIFIER,
                @Title NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
                @InternationalStandardMusicalWorkCode NVARCHAR(32),
                @ReleasedOn DATE,
                @ReleasedOnYearOnly BIT,
                @SystemEntity BIT,
                @Enabled BIT,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                WITH [SourceWork] AS
                (
                    SELECT
                        @Id AS [Id],
                        @Title AS [Title],
                        @Description AS [Description],
                        @DisambiguationText AS [DisambiguationText],
                        @InternationalStandardMusicalWorkCode AS [InternationalStandardMusicalWorkCode],
                        @ReleasedOn AS [ReleasedOn],
                        @ReleasedOnYearOnly AS [ReleasedOnYearOnly],
                        @SystemEntity AS [SystemEntity],
                        @Enabled AS [Enabled]
                )
                MERGE INTO [dbo].[Work] AS [target]
                USING [SourceWork] AS [source]
                ON [target].[Id] = [source].[Id]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Title] = [source].[Title],
                    [target].[Description] = [source].[Description],
                    [target].[DisambiguationText] = [source].[DisambiguationText],
                    [target].[InternationalStandardMusicalWorkCode] = [source].[InternationalStandardMusicalWorkCode],
                    [target].[ReleasedOn] = [source].[ReleasedOn],
                    [target].[ReleasedOnYearOnly] = [source].[ReleasedOnYearOnly],
                    [target].[SystemEntity] = [source].[SystemEntity],
                    [target].[Enabled] = [source].[Enabled]
                WHEN NOT MATCHED THEN INSERT
                (
                    [Id],
                    [Title],
                    [Description],
                    [DisambiguationText],
                    [InternationalStandardMusicalWorkCode],
                    [ReleasedOn],
                    [ReleasedOnYearOnly],
                    [SystemEntity],
                    [Enabled]
                )
                VALUES
                (
                    [source].[Id],
                    [source].[Title],
                    [source].[Description],
                    [source].[DisambiguationText],
                    [source].[InternationalStandardMusicalWorkCode],
                    [source].[ReleasedOn],
                    [source].[ReleasedOnYearOnly],
                    [source].[SystemEntity],
                    [source].[Enabled]
                );

                SET @ResultRowsUpdated = COALESCE(@ResultRowsUpdated, 0) + @@ROWCOUNT;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_CreateWork]
            (
                @Id UNIQUEIDENTIFIER,
                @Title NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
                @InternationalStandardMusicalWorkCode NVARCHAR(32),
                @ReleasedOn DATE,
                @ReleasedOnYearOnly BIT,
                @SystemEntity BIT,
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

                EXEC [dbo].[sp_Internal_MergeWork]
                    @Id,
                    @Title,
                    @Description,
                    @DisambiguationText,
                    @InternationalStandardMusicalWorkCode,
                    @ReleasedOn,
                    @ReleasedOnYearOnly,
                    @SystemEntity,
                    @Enabled,
                    NULL;

                SELECT TOP (1)
                    @ResultId = @Id,
                    @ResultCreatedOn = [work].[CreatedOn],
                    @ResultUpdatedOn = [work].[UpdatedOn]
                FROM [dbo].[Work] AS [work]
                WHERE [work].[Id] = @Id;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_UpdateWork]
            (
                @Id UNIQUEIDENTIFIER,
                @Title NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
                @InternationalStandardMusicalWorkCode NVARCHAR(32),
                @ReleasedOn DATE,
                @ReleasedOnYearOnly BIT,
                @SystemEntity BIT,
                @Enabled BIT,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                EXEC [dbo].[sp_Internal_MergeWork]
                    @Id,
                    @Title,
                    @Description,
                    @DisambiguationText,
                    @InternationalStandardMusicalWorkCode,
                    @ReleasedOn,
                    @ReleasedOnYearOnly,
                    @SystemEntity,
                    @Enabled,
                    @ResultRowsUpdated OUTPUT;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_DeleteWork]
            (
                @Id UNIQUEIDENTIFIER,
                @ResultRowsDeleted INT OUTPUT
            )
            AS
            BEGIN
                DELETE FROM [dbo].[Work] WHERE [Id] = @Id;

                SET @ResultRowsDeleted = @@ROWCOUNT;
            END;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetWork];");

        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetWorks];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateWork];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateWork];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_DeleteWork];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_Internal_MergeWork];");

        migrationBuilder.DropTable(name: "Work", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[Work];");
    }
}
