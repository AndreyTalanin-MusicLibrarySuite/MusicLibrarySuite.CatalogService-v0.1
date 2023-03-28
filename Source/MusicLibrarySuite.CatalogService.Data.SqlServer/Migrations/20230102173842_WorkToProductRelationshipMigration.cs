using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="WorkToProductRelationshipDto" /> entity.
/// </summary>
public partial class WorkToProductRelationshipMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "WorkToProductRelationship",
            schema: "dbo",
            columns: table => new
            {
                WorkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                Order = table.Column<int>(type: "int", nullable: false),
                ReferenceOrder = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_WorkToProductRelationship", columns: x => new { x.WorkId, x.ProductId });
                table.ForeignKey(
                    name: "FK_WorkToProductRelationship_Work_WorkId",
                    column: x => x.WorkId,
                    principalSchema: "dbo",
                    principalTable: "Work",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_WorkToProductRelationship_Product_ProductId",
                    column: x => x.ProductId,
                    principalSchema: "dbo",
                    principalTable: "Product",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.CheckConstraint(name: "CK_WorkToProductRelationship_Name", sql: "LEN(TRIM([Name])) > 0");
                table.CheckConstraint(name: "CK_WorkToProductRelationship_Description", sql: "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_WorkToProductRelationship_WorkId",
            schema: "dbo",
            table: "WorkToProductRelationship",
            column: "WorkId");

        migrationBuilder.CreateIndex(
            name: "IX_WorkToProductRelationship_ProductId",
            schema: "dbo",
            table: "WorkToProductRelationship",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "UIX_WorkToProductRelationship_WorkId_Order",
            schema: "dbo",
            table: "WorkToProductRelationship",
            columns: new[] { "WorkId", "Order" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "UIX_WorkToProductRelationship_ProductId_ReferenceOrder",
            schema: "dbo",
            table: "WorkToProductRelationship",
            columns: new[] { "ProductId", "ReferenceOrder" },
            unique: true);

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_WorkToProductRelationship_AfterInsertUpdateDelete_SetWorkUpdatedOn]
            ON [dbo].[WorkToProductRelationship]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedWork] AS
                (
                    SELECT [insertedWorkToProductRelationship].[WorkId] AS [Id]
                    FROM [inserted] AS [insertedWorkToProductRelationship]
                    UNION
                    SELECT [deletedWorkToProductRelationship].[WorkId] AS [Id]
                    FROM [deleted] AS [deletedWorkToProductRelationship]
                )
                UPDATE [dbo].[Work]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Work] AS [work]
                INNER JOIN [UpdatedWork] AS [updatedWork] ON [updatedWork].[Id] = [work].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[WorkToProductRelationship] AS TABLE
            (
                [WorkId] UNIQUEIDENTIFIER NOT NULL,
                [ProductId] UNIQUEIDENTIFIER NOT NULL,
                [Name] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
                [Order] INT NOT NULL,
                [ReferenceOrder] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_Internal_MergeWorkToProductRelationships]
            (
                @Id UNIQUEIDENTIFIER,
                @WorkToProductRelationships [dbo].[WorkToProductRelationship] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [SourceWorkToProductRelationship] AS
                (
                    SELECT
                        @Id AS [WorkId],
                        [sourceWorkToProductRelationship].[ProductId],
                        [sourceWorkToProductRelationship].[Name],
                        [sourceWorkToProductRelationship].[Description],
                        [sourceWorkToProductRelationship].[Order],
                        COALESCE([targetWorkToProductRelationship].[ReferenceOrder],
                            MAX([productWorkToProductRelationship].[ReferenceOrder]) + 1,
                            0) AS [ReferenceOrder]
                    FROM @WorkToProductRelationships AS [sourceWorkToProductRelationship]
                    LEFT JOIN [dbo].[WorkToProductRelationship] AS [targetWorkToProductRelationship]
                        ON [targetWorkToProductRelationship].[WorkId] = [sourceWorkToProductRelationship].[WorkId]
                        AND [targetWorkToProductRelationship].[ProductId] = [sourceWorkToProductRelationship].[ProductId]
                    LEFT JOIN [dbo].[WorkToProductRelationship] AS [productWorkToProductRelationship]
                        ON [targetWorkToProductRelationship].[ReferenceOrder] IS NULL
                        AND [productWorkToProductRelationship].[ProductId] = [sourceWorkToProductRelationship].[ProductId]
                    WHERE [sourceWorkToProductRelationship].[WorkId] = CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER)
                        OR [sourceWorkToProductRelationship].[WorkId] = @Id
                    GROUP BY
                        [sourceWorkToProductRelationship].[WorkId],
                        [sourceWorkToProductRelationship].[ProductId],
                        [sourceWorkToProductRelationship].[Name],
                        [sourceWorkToProductRelationship].[Description],
                        [sourceWorkToProductRelationship].[Order],
                        [targetWorkToProductRelationship].[ReferenceOrder]
                )
                MERGE INTO [dbo].[WorkToProductRelationship] AS [target]
                USING [SourceWorkToProductRelationship] AS [source]
                ON [target].[WorkId] = [source].[WorkId] AND [target].[ProductId] = [source].[ProductId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Name] = [source].[Name],
                    [target].[Description] = [source].[Description],
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [WorkId],
                    [ProductId],
                    [Name],
                    [Description],
                    [Order],
                    [ReferenceOrder]
                )
                VALUES
                (
                    [source].[WorkId],
                    [source].[ProductId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order],
                    [source].[ReferenceOrder]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[WorkId] = @Id THEN DELETE;

                WITH [UpdatedWorkToProductRelationship] AS
                (
                    SELECT
                        [workToProductRelationship].[WorkId],
                        [workToProductRelationship].[ProductId],
                        ROW_NUMBER() OVER (PARTITION BY [workToProductRelationship].[ProductId]
                            ORDER BY [workToProductRelationship].[ReferenceOrder]) - 1 AS [UpdatedReferenceOrder]
                    FROM [dbo].[WorkToProductRelationship] [workToProductRelationship]
                )
                UPDATE [workToProductRelationship]
                SET [ReferenceOrder] = [updatedWorkToProductRelationship].[UpdatedReferenceOrder]
                FROM [dbo].[WorkToProductRelationship] AS [workToProductRelationship]
                INNER JOIN [UpdatedWorkToProductRelationship] AS [updatedWorkToProductRelationship]
                    ON [updatedWorkToProductRelationship].[WorkId] = [workToProductRelationship].[WorkId]
                    AND [updatedWorkToProductRelationship].[ProductId] = [workToProductRelationship].[ProductId];
            END;");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateWork];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateWork];");

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
                @WorkToProductRelationships [dbo].[WorkToProductRelationship] READONLY,
                @WorkArtists [dbo].[WorkArtist] READONLY,
                @WorkFeaturedArtists [dbo].[WorkFeaturedArtist] READONLY,
                @WorkPerformers [dbo].[WorkPerformer] READONLY,
                @WorkComposers [dbo].[WorkComposer] READONLY,
                @WorkGenres [dbo].[WorkGenre] READONLY,
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

                EXEC [dbo].[sp_Internal_MergeWorkRelationships]
                    @Id,
                    @WorkRelationships;

                EXEC [dbo].[sp_Internal_MergeWorkToProductRelationships]
                    @Id,
                    @WorkToProductRelationships;

                EXEC [dbo].[sp_Internal_MergeWorkArtists]
                    @Id,
                    @WorkArtists;

                EXEC [dbo].[sp_Internal_MergeWorkFeaturedArtists]
                    @Id,
                    @WorkFeaturedArtists;

                EXEC [dbo].[sp_Internal_MergeWorkPerformers]
                    @Id,
                    @WorkPerformers;

                EXEC [dbo].[sp_Internal_MergeWorkComposers]
                    @Id,
                    @WorkComposers;

                EXEC [dbo].[sp_Internal_MergeWorkGenres]
                    @Id,
                    @WorkGenres;

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
                @WorkToProductRelationships [dbo].[WorkToProductRelationship] READONLY,
                @WorkArtists [dbo].[WorkArtist] READONLY,
                @WorkFeaturedArtists [dbo].[WorkFeaturedArtist] READONLY,
                @WorkPerformers [dbo].[WorkPerformer] READONLY,
                @WorkComposers [dbo].[WorkComposer] READONLY,
                @WorkGenres [dbo].[WorkGenre] READONLY,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                BEGIN TRANSACTION;

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

                EXEC [dbo].[sp_Internal_MergeWorkRelationships]
                    @Id,
                    @WorkRelationships;

                EXEC [dbo].[sp_Internal_MergeWorkToProductRelationships]
                    @Id,
                    @WorkToProductRelationships;

                EXEC [dbo].[sp_Internal_MergeWorkArtists]
                    @Id,
                    @WorkArtists;

                EXEC [dbo].[sp_Internal_MergeWorkFeaturedArtists]
                    @Id,
                    @WorkFeaturedArtists;

                EXEC [dbo].[sp_Internal_MergeWorkPerformers]
                    @Id,
                    @WorkPerformers;

                EXEC [dbo].[sp_Internal_MergeWorkComposers]
                    @Id,
                    @WorkComposers;

                EXEC [dbo].[sp_Internal_MergeWorkGenres]
                    @Id,
                    @WorkGenres;

                COMMIT TRANSACTION;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_UpdateWorkToProductRelationshipsOrder]
            (
                @UseReferenceOrder BIT,
                @WorkToProductRelationships [dbo].[WorkToProductRelationship] READONLY,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                WITH [SourceWorkToProductRelationship] AS
                (
                    SELECT * FROM @WorkToProductRelationships
                )
                MERGE INTO [dbo].[WorkToProductRelationship] AS [target]
                USING [SourceWorkToProductRelationship] AS [source]
                ON [target].[WorkId] = [source].[WorkId] AND [target].[ProductId] = [source].[ProductId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Order] = CASE WHEN @UseReferenceOrder = 0 THEN [source].[Order] ELSE [target].[Order] END,
                    [target].[ReferenceOrder] = CASE WHEN @UseReferenceOrder = 1 THEN [source].[ReferenceOrder] ELSE [target].[ReferenceOrder] END;

                SET @ResultRowsUpdated = @@ROWCOUNT;
            END;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateWork];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateWork];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateWorkToProductRelationshipsOrder];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_Internal_MergeWorkToProductRelationships];");

        migrationBuilder.DropTable(name: "WorkToProductRelationship", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[WorkToProductRelationship];");

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
                @WorkArtists [dbo].[WorkArtist] READONLY,
                @WorkFeaturedArtists [dbo].[WorkFeaturedArtist] READONLY,
                @WorkPerformers [dbo].[WorkPerformer] READONLY,
                @WorkComposers [dbo].[WorkComposer] READONLY,
                @WorkGenres [dbo].[WorkGenre] READONLY,
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

                EXEC [dbo].[sp_Internal_MergeWorkRelationships]
                    @Id,
                    @WorkRelationships;

                EXEC [dbo].[sp_Internal_MergeWorkArtists]
                    @Id,
                    @WorkArtists;

                EXEC [dbo].[sp_Internal_MergeWorkFeaturedArtists]
                    @Id,
                    @WorkFeaturedArtists;

                EXEC [dbo].[sp_Internal_MergeWorkPerformers]
                    @Id,
                    @WorkPerformers;

                EXEC [dbo].[sp_Internal_MergeWorkComposers]
                    @Id,
                    @WorkComposers;

                EXEC [dbo].[sp_Internal_MergeWorkGenres]
                    @Id,
                    @WorkGenres;

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
                @WorkArtists [dbo].[WorkArtist] READONLY,
                @WorkFeaturedArtists [dbo].[WorkFeaturedArtist] READONLY,
                @WorkPerformers [dbo].[WorkPerformer] READONLY,
                @WorkComposers [dbo].[WorkComposer] READONLY,
                @WorkGenres [dbo].[WorkGenre] READONLY,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                BEGIN TRANSACTION;

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

                EXEC [dbo].[sp_Internal_MergeWorkRelationships]
                    @Id,
                    @WorkRelationships;

                EXEC [dbo].[sp_Internal_MergeWorkArtists]
                    @Id,
                    @WorkArtists;

                EXEC [dbo].[sp_Internal_MergeWorkFeaturedArtists]
                    @Id,
                    @WorkFeaturedArtists;

                EXEC [dbo].[sp_Internal_MergeWorkPerformers]
                    @Id,
                    @WorkPerformers;

                EXEC [dbo].[sp_Internal_MergeWorkComposers]
                    @Id,
                    @WorkComposers;

                EXEC [dbo].[sp_Internal_MergeWorkGenres]
                    @Id,
                    @WorkGenres;

                COMMIT TRANSACTION;
            END;");
    }
}
