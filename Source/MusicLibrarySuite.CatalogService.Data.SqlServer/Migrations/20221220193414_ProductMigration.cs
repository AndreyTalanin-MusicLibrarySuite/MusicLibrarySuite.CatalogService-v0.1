using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="ProductDto" /> entity.
/// </summary>
public partial class ProductMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Product",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                DisambiguationText = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                ReleasedOn = table.Column<DateTime>(type: "date", nullable: false),
                ReleasedOnYearOnly = table.Column<bool>(type: "bit", nullable: false),
                SystemEntity = table.Column<bool>(type: "bit", nullable: false),
                Enabled = table.Column<bool>(type: "bit", nullable: false),
                CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_Product", columns: x => x.Id);
                table.CheckConstraint(name: "CK_Product_Title", sql: "LEN(TRIM([Title])) > 0");
                table.CheckConstraint(name: "CK_Product_Description", sql: "[Description] IS NULL OR LEN(TRIM([Description])) > 0");
                table.CheckConstraint(name: "CK_Product_DisambiguationText", sql: "[DisambiguationText] IS NULL OR LEN(TRIM([DisambiguationText])) > 0");
            });

        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Product]
            ADD CONSTRAINT [DF_Product_Id] DEFAULT NEWID() FOR [Id];");

        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Product]
            ADD CONSTRAINT [DF_Product_CreatedOn] DEFAULT SYSDATETIMEOFFSET() FOR [CreatedOn];");

        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Product]
            ADD CONSTRAINT [DF_Product_UpdatedOn] DEFAULT SYSDATETIMEOFFSET() FOR [UpdatedOn];");

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_Product_AfterUpdate_SetUpdatedOn]
            ON [dbo].[Product]
            AFTER UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;

                UPDATE [dbo].[Product]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Product] AS [product]
                INNER JOIN [inserted] AS [updatedProduct] ON [updatedProduct].[Id] = [product].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[Product] AS TABLE
            (
                [Id] UNIQUEIDENTIFIER NOT NULL,
                [Title] NVARCHAR(256) NOT NULL,
                [Description] NVARCHAR(2048) NULL,
                [DisambiguationText] NVARCHAR(2048) NULL,
                [ReleasedOn] DATE NOT NULL,
                [ReleasedOnYearOnly] BIT NOT NULL,
                [SystemEntity] BIT NOT NULL,
                [Enabled] BIT NOT NULL,
                [CreatedOn] DATETIMEOFFSET NOT NULL,
                [UpdatedOn] DATETIMEOFFSET NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetProduct] (@ProductId UNIQUEIDENTIFIER)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT TOP (1) [product].* FROM [dbo].[Product] AS [product] WHERE [product].[Id] = @ProductId
            );");

        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetProducts] (@ProductIds [dbo].[GuidArray] READONLY)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT [product].* FROM [dbo].[Product] AS [product] INNER JOIN @ProductIds AS [productId] ON [productId].[Value] = [product].[Id]
            );");

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_Internal_MergeProduct]
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
                WITH [SourceProduct] AS
                (
                    SELECT
                        @Id AS [Id],
                        @Title AS [Title],
                        @Description AS [Description],
                        @DisambiguationText AS [DisambiguationText],
                        @ReleasedOn AS [ReleasedOn],
                        @ReleasedOnYearOnly AS [ReleasedOnYearOnly],
                        @SystemEntity AS [SystemEntity],
                        @Enabled AS [Enabled]
                )
                MERGE INTO [dbo].[Product] AS [target]
                USING [SourceProduct] AS [source]
                ON [target].[Id] = [source].[Id]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Title] = [source].[Title],
                    [target].[Description] = [source].[Description],
                    [target].[DisambiguationText] = [source].[DisambiguationText],
                    [target].[ReleasedOn] = [source].[ReleasedOn],
                    [target].[ReleasedOnYearOnly] = [source].[ReleasedOnYearOnly],
                    [target].[SystemEntity] = [source].[SystemEntity],
                    [target].[Enabled] = [source].[Enabled]
                WHEN NOT MATCHED THEN INSERT
                (
                    [Id],
                    [Title],
                    [Description],
                    [DisambiguationText],
                    [ReleasedOn],
                    [ReleasedOnYearOnly],
                    [SystemEntity],
                    [Enabled]
                )
                VALUES
                (
                    [source].[Id],
                    [source].[Title],
                    [source].[Description],
                    [source].[DisambiguationText],
                    [source].[ReleasedOn],
                    [source].[ReleasedOnYearOnly],
                    [source].[SystemEntity],
                    [source].[Enabled]
                );

                SET @ResultRowsUpdated = COALESCE(@ResultRowsUpdated, 0) + @@ROWCOUNT;
            END;");

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

        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_DeleteProduct]
            (
                @Id UNIQUEIDENTIFIER,
                @ResultRowsDeleted INT OUTPUT
            )
            AS
            BEGIN
                DELETE FROM [dbo].[Product] WHERE [Id] = @Id;

                SET @ResultRowsDeleted = @@ROWCOUNT;
            END;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetProduct];");

        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetProducts];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_CreateProduct];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateProduct];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_DeleteProduct];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_Internal_MergeProduct];");

        migrationBuilder.DropTable(name: "Product", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[Product];");
    }
}
