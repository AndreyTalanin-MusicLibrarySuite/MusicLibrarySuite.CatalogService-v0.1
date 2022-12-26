using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="WorkRelationshipDto" /> entity.
/// </summary>
public partial class WorkRelationshipsMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "WorkRelationship",
            schema: "dbo",
            columns: table => new
            {
                WorkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DependentWorkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_WorkRelationship", columns: x => new { x.WorkId, x.DependentWorkId });
                table.ForeignKey(
                    name: "FK_WorkRelationship_Work_WorkId",
                    column: x => x.WorkId,
                    principalSchema: "dbo",
                    principalTable: "Work",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_WorkRelationship_Work_DependentWorkId",
                    column: x => x.DependentWorkId,
                    principalSchema: "dbo",
                    principalTable: "Work",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.CheckConstraint(name: "CK_WorkRelationship_Name", sql: "LEN(TRIM([Name])) > 0");
                table.CheckConstraint(name: "CK_WorkRelationship_Description", sql: "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_WorkRelationship_WorkId",
            schema: "dbo",
            table: "WorkRelationship",
            column: "WorkId");

        migrationBuilder.CreateIndex(
            name: "IX_WorkRelationship_DependentWorkId",
            schema: "dbo",
            table: "WorkRelationship",
            column: "DependentWorkId");

        migrationBuilder.CreateIndex(
            name: "UIX_WorkRelationship_WorkId_Order",
            schema: "dbo",
            table: "WorkRelationship",
            columns: new[] { "WorkId", "Order" },
            unique: true);

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_WorkRelationship_AfterInsertUpdateDelete_SetWorkUpdatedOn]
            ON [dbo].[WorkRelationship]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedWork] AS
                (
                    SELECT [insertedWorkRelationship].[WorkId] AS [Id]
                    FROM [inserted] AS [insertedWorkRelationship]
                    UNION
                    SELECT [deletedWorkRelationship].[WorkId] AS [Id]
                    FROM [deleted] AS [deletedWorkRelationship]
                )
                UPDATE [dbo].[Work]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Work] AS [work]
                INNER JOIN [UpdatedWork] AS [updatedWork] ON [updatedWork].[Id] = [work].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[WorkRelationship] AS TABLE
            (
                [WorkId] UNIQUEIDENTIFIER NOT NULL,
                [DependentWorkId] UNIQUEIDENTIFIER NOT NULL,
                [Name] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
                [Order] INT NOT NULL
            );");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateWork];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateWork];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_DeleteWork];");

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
                @WorkRelationships [dbo].[WorkRelationship] READONLY,
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

                INSERT INTO [dbo].[Work]
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
                    @Id,
                    @Title,
                    @Description,
                    @DisambiguationText,
                    @InternationalStandardMusicalWorkCode,
                    @ReleasedOn,
                    @ReleasedOnYearOnly,
                    @SystemEntity,
                    @Enabled
                );

                WITH [SourceWorkRelationship] AS
                (
                    SELECT * FROM @WorkRelationships WHERE [WorkId] = @Id
                )
                MERGE INTO [dbo].[WorkRelationship] AS [target]
                USING [SourceWorkRelationship] AS [source]
                ON [target].[WorkId] = [source].[WorkId] AND [target].[DependentWorkId] = [source].[DependentWorkId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [WorkId],
                    [DependentWorkId],
                    [Name],
                    [Description],
                    [Order]
                )
                VALUES
                (
                    [source].[WorkId],
                    [source].[DependentWorkId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order]
                );

                COMMIT TRANSACTION;

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
                @WorkRelationships [dbo].[WorkRelationship] READONLY,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                BEGIN TRANSACTION;

                UPDATE [dbo].[Work]
                SET
                    [Title] = @Title,
                    [Description] = @Description,
                    [DisambiguationText] = @DisambiguationText,
                    [InternationalStandardMusicalWorkCode] = @InternationalStandardMusicalWorkCode,
                    [ReleasedOn] = @ReleasedOn,
                    [ReleasedOnYearOnly] = @ReleasedOnYearOnly,
                    [SystemEntity] = @SystemEntity,
                    [Enabled] = @Enabled
                WHERE [Id] = @Id;

                SET @ResultRowsUpdated = @@ROWCOUNT;

                WITH [SourceWorkRelationship] AS
                (
                    SELECT * FROM @WorkRelationships WHERE [WorkId] = @Id
                )
                MERGE INTO [dbo].[WorkRelationship] AS [target]
                USING [SourceWorkRelationship] AS [source]
                ON [target].[WorkId] = [source].[WorkId] AND [target].[DependentWorkId] = [source].[DependentWorkId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Name] = [source].[Name],
                    [target].[Description] = [source].[Description],
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [WorkId],
                    [DependentWorkId],
                    [Name],
                    [Description],
                    [Order]
                )
                VALUES
                (
                    [source].[WorkId],
                    [source].[DependentWorkId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[WorkId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                COMMIT TRANSACTION;
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
        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateWork];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateWork];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_DeleteWork];");

        migrationBuilder.DropTable(name: "WorkRelationship", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[WorkRelationship];");

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

                INSERT INTO [dbo].[Work]
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
                    @Id,
                    @Title,
                    @Description,
                    @DisambiguationText,
                    @InternationalStandardMusicalWorkCode,
                    @ReleasedOn,
                    @ReleasedOnYearOnly,
                    @SystemEntity,
                    @Enabled
                );

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
                UPDATE [dbo].[Work]
                SET
                    [Title] = @Title,
                    [Description] = @Description,
                    [DisambiguationText] = @DisambiguationText,
                    [InternationalStandardMusicalWorkCode] = @InternationalStandardMusicalWorkCode,
                    [ReleasedOn] = @ReleasedOn,
                    [ReleasedOnYearOnly] = @ReleasedOnYearOnly,
                    [SystemEntity] = @SystemEntity,
                    [Enabled] = @Enabled
                WHERE [Id] = @Id;

                SET @ResultRowsUpdated = @@ROWCOUNT;
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
}
