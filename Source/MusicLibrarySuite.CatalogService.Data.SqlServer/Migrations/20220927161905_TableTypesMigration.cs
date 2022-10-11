using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding commonly used table types.
/// </summary>
public partial class TableTypesMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[Int16Array] AS TABLE
            (
                [Value] SMALLINT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[Int32Array] AS TABLE
            (
                [Value] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[Int64Array] AS TABLE
            (
                [Value] BIGINT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[GuidArray] AS TABLE
            (
                [Value] UNIQUEIDENTIFIER NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[StringArray] AS TABLE
            (
                [Value] NVARCHAR(MAX) NOT NULL
            );");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP TYPE [dbo].[Int16Array];");

        migrationBuilder.Sql("DROP TYPE [dbo].[Int32Array];");

        migrationBuilder.Sql("DROP TYPE [dbo].[Int64Array];");

        migrationBuilder.Sql("DROP TYPE [dbo].[GuidArray];");

        migrationBuilder.Sql("DROP TYPE [dbo].[StringArray];");
    }
}
