using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ReleaseGroupRelationshipDto" /> entity.
/// </summary>
public partial class ReleaseGroupRelationshipsMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ReleaseGroupRelationship",
            schema: "dbo",
            columns: table => new
            {
                ReleaseGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DependentReleaseGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ReleaseGroupRelationship", columns: x => new { x.ReleaseGroupId, x.DependentReleaseGroupId });
                table.ForeignKey(
                    name: "FK_ReleaseGroupRelationship_ReleaseGroup_ReleaseGroupId",
                    column: x => x.ReleaseGroupId,
                    principalSchema: "dbo",
                    principalTable: "ReleaseGroup",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ReleaseGroupRelationship_ReleaseGroup_DependentReleaseGroupId",
                    column: x => x.DependentReleaseGroupId,
                    principalSchema: "dbo",
                    principalTable: "ReleaseGroup",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.CheckConstraint(name: "CK_ReleaseGroupRelationship_Name", sql: "LEN(TRIM([Name])) > 0");
                table.CheckConstraint(name: "CK_ReleaseGroupRelationship_Description", sql: "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseGroupRelationship_ReleaseGroupId",
            schema: "dbo",
            table: "ReleaseGroupRelationship",
            column: "ReleaseGroupId");

        migrationBuilder.CreateIndex(
            name: "IX_ReleaseGroupRelationship_DependentReleaseGroupId",
            schema: "dbo",
            table: "ReleaseGroupRelationship",
            column: "DependentReleaseGroupId");

        migrationBuilder.CreateIndex(
            name: "UIX_ReleaseGroupRelationship_ReleaseGroupId_Order",
            schema: "dbo",
            table: "ReleaseGroupRelationship",
            columns: new[] { "ReleaseGroupId", "Order" },
            unique: true);

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ReleaseGroupRelationship_AfterInsertUpdateDelete_SetReleaseGroupUpdatedOn]
            ON [dbo].[ReleaseGroupRelationship]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedReleaseGroup] AS
                (
                    SELECT [insertedReleaseGroupRelationship].[ReleaseGroupId] AS [Id]
                    FROM [inserted] AS [insertedReleaseGroupRelationship]
                    UNION
                    SELECT [deletedReleaseGroupRelationship].[ReleaseGroupId] AS [Id]
                    FROM [deleted] AS [deletedReleaseGroupRelationship]
                )
                UPDATE [dbo].[ReleaseGroup]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[ReleaseGroup] AS [releaseGroup]
                INNER JOIN [UpdatedReleaseGroup] AS [updatedReleaseGroup] ON [updatedReleaseGroup].[Id] = [releaseGroup].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ReleaseGroupRelationship] AS TABLE
            (
                [ReleaseGroupId] UNIQUEIDENTIFIER NOT NULL,
                [DependentReleaseGroupId] UNIQUEIDENTIFIER NOT NULL,
                [Name] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
                [Order] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_Internal_MergeReleaseGroupRelationships]
            (
                @Id UNIQUEIDENTIFIER,
                @ReleaseGroupRelationships [dbo].[ReleaseGroupRelationship] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [SourceReleaseGroupRelationship] AS
                (
                    SELECT
                        @Id AS [ReleaseGroupId],
                        [DependentReleaseGroupId],
                        [Name],
                        [Description],
                        [Order]
                    FROM @ReleaseGroupRelationships
                    WHERE [ReleaseGroupId] = CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER)
                        OR [ReleaseGroupId] = @Id
                )
                MERGE INTO [dbo].[ReleaseGroupRelationship] AS [target]
                USING [SourceReleaseGroupRelationship] AS [source]
                ON [target].[ReleaseGroupId] = [source].[ReleaseGroupId] AND [target].[DependentReleaseGroupId] = [source].[DependentReleaseGroupId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Name] = [source].[Name],
                    [target].[Description] = [source].[Description],
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ReleaseGroupId],
                    [DependentReleaseGroupId],
                    [Name],
                    [Description],
                    [Order]
                )
                VALUES
                (
                    [source].[ReleaseGroupId],
                    [source].[DependentReleaseGroupId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ReleaseGroupId] = @Id THEN DELETE;
            END;");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateReleaseGroup];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateReleaseGroup];");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_CreateReleaseGroup]
            (
                @Id UNIQUEIDENTIFIER,
                @Title NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
                @Enabled BIT,
                @ReleaseGroupRelationships [dbo].[ReleaseGroupRelationship] READONLY,
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

                EXEC [dbo].[sp_Internal_MergeReleaseGroup]
                    @Id,
                    @Title,
                    @Description,
                    @DisambiguationText,
                    @Enabled,
                    NULL;

                EXEC [dbo].[sp_Internal_MergeReleaseGroupRelationships]
                    @Id,
                    @ReleaseGroupRelationships;

                COMMIT TRANSACTION;

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
                @ReleaseGroupRelationships [dbo].[ReleaseGroupRelationship] READONLY,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                BEGIN TRANSACTION;

                EXEC [dbo].[sp_Internal_MergeReleaseGroup]
                    @Id,
                    @Title,
                    @Description,
                    @DisambiguationText,
                    @Enabled,
                    @ResultRowsUpdated OUTPUT;

                EXEC [dbo].[sp_Internal_MergeReleaseGroupRelationships]
                    @Id,
                    @ReleaseGroupRelationships;

                COMMIT TRANSACTION;
            END;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateReleaseGroup];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateReleaseGroup];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_Internal_MergeReleaseGroupRelationships];");

        migrationBuilder.DropTable(name: "ReleaseGroupRelationship", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[ReleaseGroupRelationship];");

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
    }
}
