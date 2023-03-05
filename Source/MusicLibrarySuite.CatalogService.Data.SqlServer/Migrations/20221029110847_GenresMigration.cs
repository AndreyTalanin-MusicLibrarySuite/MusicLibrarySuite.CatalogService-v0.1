using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="GenreDto" /> entity.
/// </summary>
public partial class GenresMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Genre",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                SystemEntity = table.Column<bool>(type: "bit", nullable: false),
                Enabled = table.Column<bool>(type: "bit", nullable: false),
                CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_Genre", columns: x => x.Id);
                table.CheckConstraint(name: "CK_Genre_Name", sql: "LEN(TRIM([Name])) > 0");
                table.CheckConstraint(name: "CK_Genre_Description", sql: "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
            });

        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Genre]
            ADD CONSTRAINT [DF_Genre_Id] DEFAULT NEWID() FOR [Id];");

        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Genre]
            ADD CONSTRAINT [DF_Genre_CreatedOn] DEFAULT SYSDATETIMEOFFSET() FOR [CreatedOn];");

        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Genre]
            ADD CONSTRAINT [DF_Genre_UpdatedOn] DEFAULT SYSDATETIMEOFFSET() FOR [UpdatedOn];");

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_Genre_AfterUpdate_SetUpdatedOn]
            ON [dbo].[Genre]
            AFTER UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;

                UPDATE [dbo].[Genre]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Genre] AS [genre]
                INNER JOIN [inserted] AS [updatedGenre] ON [updatedGenre].[Id] = [genre].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[Genre] AS TABLE
            (
                [Id] UNIQUEIDENTIFIER NOT NULL,
                [Name] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
                [SystemEntity] BIT NOT NULL,
                [Enabled] BIT NOT NULL,
                [CreatedOn] DATETIMEOFFSET NOT NULL,
                [UpdatedOn] DATETIMEOFFSET NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetGenre] (@GenreId UNIQUEIDENTIFIER)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT TOP (1) [genre].* FROM [dbo].[Genre] AS [genre] WHERE [genre].[Id] = @GenreId
            );");

        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetGenres] (@GenreIds [dbo].[GuidArray] READONLY)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT [genre].* FROM [dbo].[Genre] AS [genre] INNER JOIN @GenreIds AS [genreId] ON [genreId].[Value] = [genre].[Id]
            );");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_Internal_MergeGenre]
            (
                @Id UNIQUEIDENTIFIER,
                @Name NVARCHAR(256),
                @Description NVARCHAR(2048),
                @SystemEntity BIT,
                @Enabled BIT,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                WITH [SourceGenre] AS
                (
                    SELECT
                        @Id AS [Id],
                        @Name AS [Name],
                        @Description AS [Description],
                        @SystemEntity AS [SystemEntity],
                        @Enabled AS [Enabled]
                )
                MERGE INTO [dbo].[Genre] AS [target]
                USING [SourceGenre] AS [source]
                ON [target].[Id] = [source].[Id]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Name] = [source].[Name],
                    [target].[Description] = [source].[Description],
                    [target].[SystemEntity] = [source].[SystemEntity],
                    [target].[Enabled] = [source].[Enabled]
                WHEN NOT MATCHED THEN INSERT
                (
                    [Id],
                    [Name],
                    [Description],
                    [SystemEntity],
                    [Enabled]
                )
                VALUES
                (
                    [source].[Id],
                    [source].[Name],
                    [source].[Description],
                    [source].[SystemEntity],
                    [source].[Enabled]
                );

                SET @ResultRowsUpdated = COALESCE(@ResultRowsUpdated, 0) + @@ROWCOUNT;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_CreateGenre]
            (
                @Id UNIQUEIDENTIFIER,
                @Name NVARCHAR(256),
                @Description NVARCHAR(2048),
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

                EXEC [dbo].[sp_Internal_MergeGenre]
                    @Id,
                    @Name,
                    @Description,
                    @SystemEntity,
                    @Enabled,
                    NULL;

                SELECT TOP (1)
                    @ResultId = @Id,
                    @ResultCreatedOn = [genre].[CreatedOn],
                    @ResultUpdatedOn = [genre].[UpdatedOn]
                FROM [dbo].[Genre] AS [genre]
                WHERE [genre].[Id] = @Id;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_UpdateGenre]
            (
                @Id UNIQUEIDENTIFIER,
                @Name NVARCHAR(256),
                @Description NVARCHAR(2048),
                @SystemEntity BIT,
                @Enabled BIT,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                EXEC [dbo].[sp_Internal_MergeGenre]
                    @Id,
                    @Name,
                    @Description,
                    @SystemEntity,
                    @Enabled,
                    @ResultRowsUpdated OUTPUT;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_DeleteGenre]
            (
                @Id UNIQUEIDENTIFIER,
                @ResultRowsDeleted INT OUTPUT
            )
            AS
            BEGIN
                DELETE FROM [dbo].[Genre] WHERE [Id] = @Id;

                SET @ResultRowsDeleted = @@ROWCOUNT;
            END;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetGenre];");

        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetGenres];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateGenre];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateGenre];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_DeleteGenre];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_Internal_MergeGenre];");

        migrationBuilder.DropTable(name: "Genre", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[Genre];");
    }
}
