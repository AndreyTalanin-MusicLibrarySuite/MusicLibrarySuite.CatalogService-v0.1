using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ArtistDto" /> entity.
/// </summary>
public partial class ArtistsMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Artist",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                DisambiguationText = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                SystemEntity = table.Column<bool>(type: "bit", nullable: false),
                Enabled = table.Column<bool>(type: "bit", nullable: false),
                CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_Artist", columns: x => x.Id);
                table.CheckConstraint(name: "CK_Artist_Name", sql: "LEN(TRIM([Name])) > 0");
                table.CheckConstraint(name: "CK_Artist_Description", sql: "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
                table.CheckConstraint(name: "CK_Artist_DisambiguationText", sql: "[DisambiguationText] IS NULL OR LEN(TRIM([DisambiguationText])) > 0");
            });

        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Artist]
            ADD CONSTRAINT [DF_Artist_Id] DEFAULT NEWID() FOR [Id];");

        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Artist]
            ADD CONSTRAINT [DF_Artist_CreatedOn] DEFAULT SYSDATETIMEOFFSET() FOR [CreatedOn];");

        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Artist]
            ADD CONSTRAINT [DF_Artist_UpdatedOn] DEFAULT SYSDATETIMEOFFSET() FOR [UpdatedOn];");

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_Artist_AfterUpdate_SetUpdatedOn]
            ON [dbo].[Artist]
            AFTER UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;

                UPDATE [dbo].[Artist]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Artist] AS [artist]
                INNER JOIN [inserted] AS [updatedArtist] ON [updatedArtist].[Id] = [artist].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[Artist] AS TABLE
            (
                [Id] UNIQUEIDENTIFIER NOT NULL,
                [Name] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
                [DisambiguationText] NVARCHAR(2048) NULL,
                [SystemEntity] BIT NOT NULL,
                [Enabled] BIT NOT NULL,
                [CreatedOn] DATETIMEOFFSET NOT NULL,
                [UpdatedOn] DATETIMEOFFSET NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetArtist] (@ArtistId UNIQUEIDENTIFIER)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT TOP (1) [artist].* FROM [dbo].[Artist] AS [artist] WHERE [artist].[Id] = @ArtistId
            );");

        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetArtists] (@ArtistIds [dbo].[GuidArray] READONLY)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT [artist].* FROM [dbo].[Artist] AS [artist] INNER JOIN @ArtistIds AS [artistId] ON [artistId].[Value] = [artist].[Id]
            );");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_CreateArtist]
            (
                @Id UNIQUEIDENTIFIER,
                @Name NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
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

                INSERT INTO [dbo].[Artist]
                (
                    [Id],
                    [Name],
                    [Description],
                    [DisambiguationText],
                    [SystemEntity],
                    [Enabled]
                )
                VALUES
                (
                    @Id,
                    @Name,
                    @Description,
                    @DisambiguationText,
                    @SystemEntity,
                    @Enabled
                );

                SELECT TOP (1)
                    @ResultId = @Id,
                    @ResultCreatedOn = [artist].[CreatedOn],
                    @ResultUpdatedOn = [artist].[UpdatedOn]
                FROM [dbo].[Artist] AS [artist]
                WHERE [artist].[Id] = @Id;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_UpdateArtist]
            (
                @Id UNIQUEIDENTIFIER,
                @Name NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
                @SystemEntity BIT,
                @Enabled BIT,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                UPDATE [dbo].[Artist]
                SET
                    [Name] = @Name,
                    [Description] = @Description,
                    [DisambiguationText] = @DisambiguationText,
                    [SystemEntity] = @SystemEntity,
                    [Enabled] = @Enabled
                WHERE [Id] = @Id;

                SET @ResultRowsUpdated = @@ROWCOUNT;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_DeleteArtist]
            (
                @Id UNIQUEIDENTIFIER,
                @ResultRowsDeleted INT OUTPUT
            )
            AS
            BEGIN
                DELETE FROM [dbo].[Artist] WHERE [Id] = @Id;

                SET @ResultRowsDeleted = @@ROWCOUNT;
            END;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetArtist];");

        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetArtists];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateArtist];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateArtist];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_DeleteArtist];");

        migrationBuilder.DropTable(name: "Artist", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[Artist];");
    }
}
