using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ReleaseGroupDto" /> entity.
/// </summary>
public partial class ReleaseGroupsMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ReleaseGroup",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                DisambiguationText = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                Enabled = table.Column<bool>(type: "bit", nullable: false),
                CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ReleaseGroup", columns: x => x.Id);
                table.CheckConstraint(name: "CK_ReleaseGroup_Title", sql: "LEN(TRIM([Title])) > 0");
                table.CheckConstraint(name: "CK_ReleaseGroup_Description", sql: "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
                table.CheckConstraint(name: "CK_ReleaseGroup_DisambiguationText", sql: "[DisambiguationText] IS NULL OR LEN(TRIM([DisambiguationText])) > 0");
            });

        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[ReleaseGroup]
            ADD CONSTRAINT [DF_ReleaseGroup_Id] DEFAULT NEWID() FOR [Id];");

        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[ReleaseGroup]
            ADD CONSTRAINT [DF_ReleaseGroup_CreatedOn] DEFAULT SYSDATETIMEOFFSET() FOR [CreatedOn];");

        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[ReleaseGroup]
            ADD CONSTRAINT [DF_ReleaseGroup_UpdatedOn] DEFAULT SYSDATETIMEOFFSET() FOR [UpdatedOn];");

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleaseGroup_AfterUpdate_SetUpdatedOn]
            ON [dbo].[ReleaseGroup]
            AFTER UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;

                UPDATE [dbo].[ReleaseGroup]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[ReleaseGroup] AS [releaseGroup]
                INNER JOIN [inserted] AS [updatedReleaseGroup] ON [updatedReleaseGroup].[Id] = [releaseGroup].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleaseGroup] AS TABLE
            (
                [Id] UNIQUEIDENTIFIER NOT NULL,
                [Title] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
                [DisambiguationText] NVARCHAR(2048) NULL,
                [Enabled] BIT NOT NULL,
                [CreatedOn] DATETIMEOFFSET NOT NULL,
                [UpdatedOn] DATETIMEOFFSET NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetReleaseGroup] (@ReleaseGroupId UNIQUEIDENTIFIER)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT TOP (1) [releaseGroup].*
                FROM [dbo].[ReleaseGroup] AS [releaseGroup]
                WHERE [releaseGroup].[Id] = @ReleaseGroupId
            );");

        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetReleaseGroups] (@ReleaseGroupIds [dbo].[GuidArray] READONLY)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT [releaseGroup].*
                FROM [dbo].[ReleaseGroup] AS [releaseGroup]
                INNER JOIN @ReleaseGroupIds AS [releaseGroupId] ON [releaseGroupId].[Value] = [releaseGroup].[Id]
            );");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_Internal_MergeReleaseGroup]
            (
                @Id UNIQUEIDENTIFIER,
                @Title NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
                @Enabled BIT,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                WITH [SourceReleaseGroup] AS
                (
                    SELECT
                        @Id AS [Id],
                        @Title AS [Title],
                        @Description AS [Description],
                        @DisambiguationText AS [DisambiguationText],
                        @Enabled AS [Enabled]
                )
                MERGE INTO [dbo].[ReleaseGroup] AS [target]
                USING [SourceReleaseGroup] AS [source]
                ON [target].[Id] = [source].[Id]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Title] = [source].[Title],
                    [target].[Description] = [source].[Description],
                    [target].[DisambiguationText] = [source].[DisambiguationText],
                    [target].[Enabled] = [source].[Enabled]
                WHEN NOT MATCHED THEN INSERT
                (
                    [Id],
                    [Title],
                    [Description],
                    [DisambiguationText],
                    [Enabled]
                )
                VALUES
                (
                    [source].[Id],
                    [source].[Title],
                    [source].[Description],
                    [source].[DisambiguationText],
                    [source].[Enabled]
                );

                SET @ResultRowsUpdated = COALESCE(@ResultRowsUpdated, 0) + @@ROWCOUNT;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_CreateReleaseGroup]
            (
                @Id UNIQUEIDENTIFIER,
                @Title NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
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

                EXEC [dbo].[sp_Internal_MergeReleaseGroup]
                    @Id,
                    @Title,
                    @Description,
                    @DisambiguationText,
                    @Enabled,
                    NULL;

                SELECT TOP (1)
                    @ResultId = @Id,
                    @ResultCreatedOn = [releaseGroup].[CreatedOn],
                    @ResultUpdatedOn = [releaseGroup].[UpdatedOn]
                FROM [dbo].[ReleaseGroup] AS [releaseGroup]
                WHERE [releaseGroup].[Id] = @Id;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_UpdateReleaseGroup]
            (
                @Id UNIQUEIDENTIFIER,
                @Title NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
                @Enabled BIT,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                EXEC [dbo].[sp_Internal_MergeReleaseGroup]
                    @Id,
                    @Title,
                    @Description,
                    @DisambiguationText,
                    @Enabled,
                    @ResultRowsUpdated OUTPUT;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_DeleteReleaseGroup]
            (
                @Id UNIQUEIDENTIFIER,
                @ResultRowsDeleted INT OUTPUT
            )
            AS
            BEGIN
                DELETE FROM [dbo].[ReleaseGroup] WHERE [Id] = @Id;

                SET @ResultRowsDeleted = @@ROWCOUNT;
            END;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetReleaseGroup];");

        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetReleaseGroups];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateReleaseGroup];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateReleaseGroup];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_DeleteReleaseGroup];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_Internal_MergeReleaseGroup];");

        migrationBuilder.DropTable(name: "ReleaseGroup", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleaseGroup];");
    }
}
