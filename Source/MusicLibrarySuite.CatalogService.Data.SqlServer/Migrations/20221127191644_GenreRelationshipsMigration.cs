using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="GenreRelationshipDto" /> entity.
/// </summary>
public partial class GenreRelationshipsMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "GenreRelationship",
            schema: "dbo",
            columns: table => new
            {
                GenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DependentGenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GenreRelationship", x => new { x.GenreId, x.DependentGenreId });
                table.ForeignKey(
                    name: "FK_GenreRelationship_Genre_GenreId",
                    column: x => x.GenreId,
                    principalSchema: "dbo",
                    principalTable: "Genre",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_GenreRelationship_Genre_DependentGenreId",
                    column: x => x.DependentGenreId,
                    principalSchema: "dbo",
                    principalTable: "Genre",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.CheckConstraint("CK_GenreRelationship_Name", "LEN(TRIM([Name])) > 0");
                table.CheckConstraint("CK_GenreRelationship_Description", "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_GenreRelationship_GenreId",
            schema: "dbo",
            table: "GenreRelationship",
            column: "GenreId");

        migrationBuilder.CreateIndex(
            name: "IX_GenreRelationship_DependentGenreId",
            schema: "dbo",
            table: "GenreRelationship",
            column: "DependentGenreId");

        migrationBuilder.CreateIndex(
            name: "UIX_GenreRelationship_GenreId_Order",
            schema: "dbo",
            table: "GenreRelationship",
            columns: new[] { "GenreId", "Order" },
            unique: true);

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_GenreRelationship_AfterInsertUpdateDelete_SetGenreUpdatedOn]
            ON [dbo].[GenreRelationship]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedGenre] AS
                (
                    SELECT [insertedGenreRelationship].[GenreId] AS [Id]
                    FROM [inserted] AS [insertedGenreRelationship]
                    UNION
                    SELECT [deletedGenreRelationship].[GenreId] AS [Id]
                    FROM [deleted] AS [deletedGenreRelationship]
                )
                UPDATE [dbo].[Genre]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Genre] AS [genre]
                INNER JOIN [UpdatedGenre] AS [updatedGenre] ON [updatedGenre].[Id] = [genre].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[GenreRelationship] AS TABLE
            (
                [GenreId] UNIQUEIDENTIFIER NOT NULL,
                [DependentGenreId] UNIQUEIDENTIFIER NOT NULL,
                [Name] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
                [Order] INT NOT NULL
            );");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateGenre];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateGenre];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_DeleteGenre];");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_CreateGenre]
            (
                @Id UNIQUEIDENTIFIER,
                @Name NVARCHAR(256),
                @Description NVARCHAR(2048),
                @SystemEntity BIT,
                @Enabled BIT,
                @GenreRelationships [dbo].[GenreRelationship] READONLY,
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

                INSERT INTO [dbo].[Genre]
                (
                    [Id],
                    [Name],
                    [Description],
                    [SystemEntity],
                    [Enabled]
                )
                VALUES
                (
                    @Id,
                    @Name,
                    @Description,
                    @SystemEntity,
                    @Enabled
                );

                WITH [SourceGenreRelationship] AS
                (
                    SELECT * FROM @GenreRelationships WHERE [GenreId] = @Id
                )
                MERGE INTO [dbo].[GenreRelationship] AS [target]
                USING [SourceGenreRelationship] AS [source]
                ON [target].[GenreId] = [source].[GenreId] AND [target].[DependentGenreId] = [source].[DependentGenreId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [GenreId],
                    [DependentGenreId],
                    [Name],
                    [Description],
                    [Order]
                )
                VALUES
                (
                    [source].[GenreId],
                    [source].[DependentGenreId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order]
                );

                COMMIT TRANSACTION;

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
                @GenreRelationships [dbo].[GenreRelationship] READONLY,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                BEGIN TRANSACTION;

                UPDATE [dbo].[Genre]
                SET
                    [Name] = @Name,
                    [Description] = @Description,
                    [SystemEntity] = @SystemEntity,
                    [Enabled] = @Enabled
                WHERE [Id] = @Id;

                SET @ResultRowsUpdated = @@ROWCOUNT;

                WITH [SourceGenreRelationship] AS
                (
                    SELECT * FROM @GenreRelationships WHERE [GenreId] = @Id
                )
                MERGE INTO [dbo].[GenreRelationship] AS [target]
                USING [SourceGenreRelationship] AS [source]
                ON [target].[GenreId] = [source].[GenreId] AND [target].[DependentGenreId] = [source].[DependentGenreId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Name] = [source].[Name],
                    [target].[Description] = [source].[Description],
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [GenreId],
                    [DependentGenreId],
                    [Name],
                    [Description],
                    [Order]
                )
                VALUES
                (
                    [source].[GenreId],
                    [source].[DependentGenreId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[GenreId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                COMMIT TRANSACTION;
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
        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateGenre];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateGenre];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_DeleteGenre];");

        migrationBuilder.DropTable(name: "GenreRelationship", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[GenreRelationship];");

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

                INSERT INTO [dbo].[Genre]
                (
                    [Id],
                    [Name],
                    [Description],
                    [SystemEntity],
                    [Enabled]
                )
                VALUES
                (
                    @Id,
                    @Name,
                    @Description,
                    @SystemEntity,
                    @Enabled
                );

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
                UPDATE [dbo].[Genre]
                SET
                    [Name] = @Name,
                    [Description] = @Description,
                    [SystemEntity] = @SystemEntity,
                    [Enabled] = @Enabled
                WHERE [Id] = @Id;

                SET @ResultRowsUpdated = @@ROWCOUNT;
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
}
