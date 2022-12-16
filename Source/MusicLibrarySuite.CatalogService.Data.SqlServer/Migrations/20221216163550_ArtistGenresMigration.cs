using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ArtistGenreDto" /> entity.
/// </summary>
public partial class ArtistGenresMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ArtistGenre",
            schema: "dbo",
            columns: table => new
            {
                ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                GenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ArtistGenre", columns: x => new { x.ArtistId, x.GenreId });
                table.ForeignKey(
                    name: "FK_ArtistGenre_Artist_ArtistId",
                    column: x => x.ArtistId,
                    principalSchema: "dbo",
                    principalTable: "Artist",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ArtistGenre_Genre_GenreId",
                    column: x => x.GenreId,
                    principalSchema: "dbo",
                    principalTable: "Genre",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ArtistGenre_ArtistId",
            schema: "dbo",
            table: "ArtistGenre",
            column: "ArtistId");

        migrationBuilder.CreateIndex(
            name: "IX_ArtistGenre_GenreId",
            schema: "dbo",
            table: "ArtistGenre",
            column: "GenreId");

        migrationBuilder.CreateIndex(
            name: "UIX_ArtistGenre_ArtistId_Order",
            schema: "dbo",
            table: "ArtistGenre",
            columns: new[] { "ArtistId", "Order" },
            unique: true);

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ArtistGenre_AfterInsertUpdateDelete_SetArtistUpdatedOn]
            ON [dbo].[ArtistGenre]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedArtist] AS
                (
                    SELECT [insertedArtistGenre].[ArtistId] AS [Id]
                    FROM [inserted] AS [insertedArtistGenre]
                    UNION
                    SELECT [deletedArtistGenre].[ArtistId] AS [Id]
                    FROM [deleted] AS [deletedArtistGenre]
                )
                UPDATE [dbo].[Artist]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Artist] AS [artist]
                INNER JOIN [UpdatedArtist] AS [updatedArtist] ON [updatedArtist].[Id] = [artist].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ArtistGenre] AS TABLE
            (
                [ArtistId] UNIQUEIDENTIFIER NOT NULL,
                [GenreId] UNIQUEIDENTIFIER NOT NULL,
                [Order] INT NOT NULL
            );");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateArtist];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateArtist];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_DeleteArtist];");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_CreateArtist]
            (
                @Id UNIQUEIDENTIFIER,
                @Name NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
                @SystemEntity BIT,
                @Enabled BIT,
                @ArtistRelationships [dbo].[ArtistRelationship] READONLY,
                @ArtistGenres [dbo].[ArtistGenre] READONLY,
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

                WITH [SourceArtistRelationship] AS
                (
                    SELECT * FROM @ArtistRelationships WHERE [ArtistId] = @Id
                )
                MERGE INTO [dbo].[ArtistRelationship] AS [target]
                USING [SourceArtistRelationship] AS [source]
                ON [target].[ArtistId] = [source].[ArtistId] AND [target].[DependentArtistId] = [source].[DependentArtistId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ArtistId],
                    [DependentArtistId],
                    [Name],
                    [Description],
                    [Order]
                )
                VALUES
                (
                    [source].[ArtistId],
                    [source].[DependentArtistId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order]
                );

                WITH [SourceArtistGenre] AS
                (
                    SELECT * FROM @ArtistGenres WHERE [ArtistId] = @Id
                )
                MERGE INTO [dbo].[ArtistGenre] AS [target]
                USING [SourceArtistGenre] AS [source]
                ON [target].[ArtistId] = [source].[ArtistId] AND [target].[GenreId] = [source].[GenreId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ArtistId],
                    [GenreId],
                    [Order]
                )
                VALUES
                (
                    [source].[ArtistId],
                    [source].[GenreId],
                    [source].[Order]
                );

                COMMIT TRANSACTION;

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
                @ArtistRelationships [dbo].[ArtistRelationship] READONLY,
                @ArtistGenres [dbo].[ArtistGenre] READONLY,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                BEGIN TRANSACTION;

                UPDATE [dbo].[Artist]
                SET
                    [Name] = @Name,
                    [Description] = @Description,
                    [DisambiguationText] = @DisambiguationText,
                    [SystemEntity] = @SystemEntity,
                    [Enabled] = @Enabled
                WHERE [Id] = @Id;

                SET @ResultRowsUpdated = @@ROWCOUNT;

                WITH [SourceArtistRelationship] AS
                (
                    SELECT * FROM @ArtistRelationships WHERE [ArtistId] = @Id
                )
                MERGE INTO [dbo].[ArtistRelationship] AS [target]
                USING [SourceArtistRelationship] AS [source]
                ON [target].[ArtistId] = [source].[ArtistId] AND [target].[DependentArtistId] = [source].[DependentArtistId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Name] = [source].[Name],
                    [target].[Description] = [source].[Description],
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ArtistId],
                    [DependentArtistId],
                    [Name],
                    [Description],
                    [Order]
                )
                VALUES
                (
                    [source].[ArtistId],
                    [source].[DependentArtistId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ArtistId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceArtistGenre] AS
                (
                    SELECT * FROM @ArtistGenres WHERE [ArtistId] = @Id
                )
                MERGE INTO [dbo].[ArtistGenre] AS [target]
                USING [SourceArtistGenre] AS [source]
                ON [target].[ArtistId] = [source].[ArtistId] AND [target].[GenreId] = [source].[GenreId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ArtistId],
                    [GenreId],
                    [Order]
                )
                VALUES
                (
                    [source].[ArtistId],
                    [source].[GenreId],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ArtistId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                COMMIT TRANSACTION;
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
        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateArtist];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateArtist];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_DeleteArtist];");

        migrationBuilder.DropTable(name: "ArtistGenre", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[ArtistGenre];");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_CreateArtist]
            (
                @Id UNIQUEIDENTIFIER,
                @Name NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
                @SystemEntity BIT,
                @Enabled BIT,
                @ArtistRelationships [dbo].[ArtistRelationship] READONLY,
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

                WITH [SourceArtistRelationship] AS
                (
                    SELECT * FROM @ArtistRelationships WHERE [ArtistId] = @Id
                )
                MERGE INTO [dbo].[ArtistRelationship] AS [target]
                USING [SourceArtistRelationship] AS [source]
                ON [target].[ArtistId] = [source].[ArtistId] AND [target].[DependentArtistId] = [source].[DependentArtistId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ArtistId],
                    [DependentArtistId],
                    [Name],
                    [Description],
                    [Order]
                )
                VALUES
                (
                    [source].[ArtistId],
                    [source].[DependentArtistId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order]
                );

                COMMIT TRANSACTION;

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
                @ArtistRelationships [dbo].[ArtistRelationship] READONLY,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                BEGIN TRANSACTION;

                UPDATE [dbo].[Artist]
                SET
                    [Name] = @Name,
                    [Description] = @Description,
                    [DisambiguationText] = @DisambiguationText,
                    [SystemEntity] = @SystemEntity,
                    [Enabled] = @Enabled
                WHERE [Id] = @Id;

                SET @ResultRowsUpdated = @@ROWCOUNT;

                WITH [SourceArtistRelationship] AS
                (
                    SELECT * FROM @ArtistRelationships WHERE [ArtistId] = @Id
                )
                MERGE INTO [dbo].[ArtistRelationship] AS [target]
                USING [SourceArtistRelationship] AS [source]
                ON [target].[ArtistId] = [source].[ArtistId] AND [target].[DependentArtistId] = [source].[DependentArtistId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Name] = [source].[Name],
                    [target].[Description] = [source].[Description],
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ArtistId],
                    [DependentArtistId],
                    [Name],
                    [Description],
                    [Order]
                )
                VALUES
                (
                    [source].[ArtistId],
                    [source].[DependentArtistId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ArtistId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                COMMIT TRANSACTION;
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
}
