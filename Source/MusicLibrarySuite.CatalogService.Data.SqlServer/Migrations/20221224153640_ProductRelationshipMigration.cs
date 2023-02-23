using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ProductRelationshipDto" /> entity.
/// </summary>
public partial class ProductRelationshipMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ProductRelationship",
            schema: "dbo",
            columns: table => new
            {
                ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DependentProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_ProductRelationship", columns: x => new { x.ProductId, x.DependentProductId });
                table.ForeignKey(
                    name: "FK_ProductRelationship_Product_ProductId",
                    column: x => x.ProductId,
                    principalSchema: "dbo",
                    principalTable: "Product",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ProductRelationship_Product_DependentProductId",
                    column: x => x.DependentProductId,
                    principalSchema: "dbo",
                    principalTable: "Product",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.CheckConstraint(name: "CK_ProductRelationship_Name", sql: "LEN(TRIM([Name])) > 0");
                table.CheckConstraint(name: "CK_ProductRelationship_Description", sql: "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ProductRelationship_ProductId",
            schema: "dbo",
            table: "ProductRelationship",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_ProductRelationship_DependentProductId",
            schema: "dbo",
            table: "ProductRelationship",
            column: "DependentProductId");

        migrationBuilder.CreateIndex(
            name: "UIX_ProductRelationship_ProductId_Order",
            schema: "dbo",
            table: "ProductRelationship",
            columns: new[] { "ProductId", "Order" },
            unique: true);

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_ProductRelationship_AfterInsertUpdateDelete_SetProductUpdatedOn]
            ON [dbo].[ProductRelationship]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedProduct] AS
                (
                    SELECT [insertedProductRelationship].[ProductId] AS [Id]
                    FROM [inserted] AS [insertedProductRelationship]
                    UNION
                    SELECT [deletedProductRelationship].[ProductId] AS [Id]
                    FROM [deleted] AS [deletedProductRelationship]
                )
                UPDATE [dbo].[Product]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Product] AS [product]
                INNER JOIN [UpdatedProduct] AS [updatedProduct] ON [updatedProduct].[Id] = [product].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[ProductRelationship] AS TABLE
            (
                [ProductId] UNIQUEIDENTIFIER NOT NULL,
                [DependentProductId] UNIQUEIDENTIFIER NOT NULL,
                [Name] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
                [Order] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_Internal_MergeProductRelationships]
            (
                @Id UNIQUEIDENTIFIER,
                @ProductRelationships [dbo].[ProductRelationship] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [SourceProductRelationship] AS
                (
                    SELECT
                        @Id AS [ProductId],
                        [DependentProductId],
                        [Name],
                        [Description],
                        [Order]
                    FROM @ProductRelationships
                    WHERE [ProductId] = CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER)
                        OR [ProductId] = @Id
                )
                MERGE INTO [dbo].[ProductRelationship] AS [target]
                USING [SourceProductRelationship] AS [source]
                ON [target].[ProductId] = [source].[ProductId] AND [target].[DependentProductId] = [source].[DependentProductId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Name] = [source].[Name],
                    [target].[Description] = [source].[Description],
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [ProductId],
                    [DependentProductId],
                    [Name],
                    [Description],
                    [Order]
                )
                VALUES
                (
                    [source].[ProductId],
                    [source].[DependentProductId],
                    [source].[Name],
                    [source].[Description],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[ProductId] = @Id THEN DELETE;
            END;");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateProduct];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateProduct];");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_CreateProduct]
            (
                @Id UNIQUEIDENTIFIER,
                @Title NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
                @ReleasedOn DATE,
                @ReleasedOnYearOnly BIT,
                @SystemEntity BIT,
                @Enabled BIT,
                @ProductRelationships [dbo].[ProductRelationship] READONLY,
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

                EXEC [dbo].[sp_Internal_MergeProduct]
                    @Id,
                    @Title,
                    @Description,
                    @DisambiguationText,
                    @ReleasedOn,
                    @ReleasedOnYearOnly,
                    @SystemEntity,
                    @Enabled,
                    NULL;

                EXEC [dbo].[sp_Internal_MergeProductRelationships]
                    @Id,
                    @ProductRelationships;

                COMMIT TRANSACTION;

                SELECT TOP (1)
                    @ResultId = @Id,
                    @ResultCreatedOn = [product].[CreatedOn],
                    @ResultUpdatedOn = [product].[UpdatedOn]
                FROM [dbo].[Product] AS [product]
                WHERE [product].[Id] = @Id;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_UpdateProduct]
            (
                @Id UNIQUEIDENTIFIER,
                @Title NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
                @ReleasedOn DATE,
                @ReleasedOnYearOnly BIT,
                @SystemEntity BIT,
                @Enabled BIT,
                @ProductRelationships [dbo].[ProductRelationship] READONLY,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                BEGIN TRANSACTION;

                EXEC [dbo].[sp_Internal_MergeProduct]
                    @Id,
                    @Title,
                    @Description,
                    @DisambiguationText,
                    @ReleasedOn,
                    @ReleasedOnYearOnly,
                    @SystemEntity,
                    @Enabled,
                    @ResultRowsUpdated OUTPUT;

                EXEC [dbo].[sp_Internal_MergeProductRelationships]
                    @Id,
                    @ProductRelationships;

                COMMIT TRANSACTION;
            END;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateProduct];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateProduct];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_Internal_MergeProductRelationships];");

        migrationBuilder.DropTable(name: "ProductRelationship", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[ProductRelationship];");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_CreateProduct]
            (
                @Id UNIQUEIDENTIFIER,
                @Title NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
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

                EXEC [dbo].[sp_Internal_MergeProduct]
                    @Id,
                    @Title,
                    @Description,
                    @DisambiguationText,
                    @ReleasedOn,
                    @ReleasedOnYearOnly,
                    @SystemEntity,
                    @Enabled,
                    NULL;

                SELECT TOP (1)
                    @ResultId = @Id,
                    @ResultCreatedOn = [product].[CreatedOn],
                    @ResultUpdatedOn = [product].[UpdatedOn]
                FROM [dbo].[Product] AS [product]
                WHERE [product].[Id] = @Id;
            END;");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_UpdateProduct]
            (
                @Id UNIQUEIDENTIFIER,
                @Title NVARCHAR(256),
                @Description NVARCHAR(2048),
                @DisambiguationText NVARCHAR(2048),
                @ReleasedOn DATE,
                @ReleasedOnYearOnly BIT,
                @SystemEntity BIT,
                @Enabled BIT,
                @ResultRowsUpdated INT OUTPUT
            )
            AS
            BEGIN
                EXEC [dbo].[sp_Internal_MergeProduct]
                    @Id,
                    @Title,
                    @Description,
                    @DisambiguationText,
                    @ReleasedOn,
                    @ReleasedOnYearOnly,
                    @SystemEntity,
                    @Enabled,
                    @ResultRowsUpdated OUTPUT;
            END;");
    }
}
