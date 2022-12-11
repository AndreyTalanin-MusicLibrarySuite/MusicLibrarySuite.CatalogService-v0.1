using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ArtistRelationshipDto" /> entity.
/// </summary>
public partial class ArtistRelationshipsMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ArtistRelationship",
            schema: "dbo",
            columns: table => new
            {
                ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DependentArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ArtistRelationship", columns: x => new { x.ArtistId, x.DependentArtistId });
                table.ForeignKey(
                    name: "FK_ArtistRelationship_Artist_ArtistId",
                    column: x => x.ArtistId,
                    principalSchema: "dbo",
                    principalTable: "Artist",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ArtistRelationship_Artist_DependentArtistId",
                    column: x => x.DependentArtistId,
                    principalSchema: "dbo",
                    principalTable: "Artist",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.CheckConstraint(name: "CK_ArtistRelationship_Name", sql: "LEN(TRIM([Name])) > 0");
                table.CheckConstraint(name: "CK_ArtistRelationship_Description", sql: "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ArtistRelationship_ArtistId",
            schema: "dbo",
            table: "ArtistRelationship",
            column: "ArtistId");

        migrationBuilder.CreateIndex(
            name: "IX_ArtistRelationship_DependentArtistId",
            schema: "dbo",
            table: "ArtistRelationship",
            column: "DependentArtistId");

        migrationBuilder.CreateIndex(
            name: "UIX_ArtistRelationship_ArtistId_Order",
            schema: "dbo",
            table: "ArtistRelationship",
            columns: new[] { "ArtistId", "Order" },
            unique: true);

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ArtistRelationship_AfterInsertUpdateDelete_SetArtistUpdatedOn]
            ON [dbo].[ArtistRelationship]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedArtist] AS
                (
                    SELECT [insertedArtistRelationship].[ArtistId] AS [Id]
                    FROM [inserted] AS [insertedArtistRelationship]
                    UNION
                    SELECT [deletedArtistRelationship].[ArtistId] AS [Id]
                    FROM [deleted] AS [deletedArtistRelationship]
                )
                UPDATE [dbo].[Artist]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Artist] AS [artist]
                INNER JOIN [UpdatedArtist] AS [updatedArtist] ON [updatedArtist].[Id] = [artist].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ArtistRelationship] AS TABLE
            (
                [ArtistId] UNIQUEIDENTIFIER NOT NULL,
                [DependentArtistId] UNIQUEIDENTIFIER NOT NULL,
                [Name] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
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

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateArtist];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateArtist];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_DeleteArtist];");

        migrationBuilder.DropTable(name: "ArtistRelationship", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[ArtistRelationship];");

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
}
