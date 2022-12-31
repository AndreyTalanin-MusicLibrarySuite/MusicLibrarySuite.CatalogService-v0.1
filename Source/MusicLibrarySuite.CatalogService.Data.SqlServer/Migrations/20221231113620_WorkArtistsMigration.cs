using System;

using Microsoft.EntityFrameworkCore.Migrations;

using MusicLibrarySuite.CatalogService.Data.Entities;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Migrations;

/// <summary>
/// Represents a database migration adding the <see cref="WorkArtistDto" />, <see cref="WorkFeaturedArtistDto" />,
/// <see cref="WorkPerformerDto" /> and <see cref="WorkComposerDto" /> entities.
/// </summary>
public partial class WorkArtistsMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "WorkArtist",
            schema: "dbo",
            columns: table => new
            {
                WorkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_WorkArtist", columns: x => new { x.WorkId, x.ArtistId });
                table.ForeignKey(
                    name: "FK_WorkArtist_Work_WorkId",
                    column: x => x.WorkId,
                    principalSchema: "dbo",
                    principalTable: "Work",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_WorkArtist_Artist_ArtistId",
                    column: x => x.ArtistId,
                    principalSchema: "dbo",
                    principalTable: "Artist",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "WorkFeaturedArtist",
            schema: "dbo",
            columns: table => new
            {
                WorkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_WorkFeaturedArtist", columns: x => new { x.WorkId, x.ArtistId });
                table.ForeignKey(
                    name: "FK_WorkFeaturedArtist_Work_WorkId",
                    column: x => x.WorkId,
                    principalSchema: "dbo",
                    principalTable: "Work",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_WorkFeaturedArtist_Artist_ArtistId",
                    column: x => x.ArtistId,
                    principalSchema: "dbo",
                    principalTable: "Artist",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "WorkPerformer",
            schema: "dbo",
            columns: table => new
            {
                WorkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_WorkPerformer", columns: x => new { x.WorkId, x.ArtistId });
                table.ForeignKey(
                    name: "FK_WorkPerformer_Work_WorkId",
                    column: x => x.WorkId,
                    principalSchema: "dbo",
                    principalTable: "Work",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_WorkPerformer_Artist_ArtistId",
                    column: x => x.ArtistId,
                    principalSchema: "dbo",
                    principalTable: "Artist",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "WorkComposer",
            schema: "dbo",
            columns: table => new
            {
                WorkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_WorkComposer", columns: x => new { x.WorkId, x.ArtistId });
                table.ForeignKey(
                    name: "FK_WorkComposer_Work_WorkId",
                    column: x => x.WorkId,
                    principalSchema: "dbo",
                    principalTable: "Work",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_WorkComposer_Artist_ArtistId",
                    column: x => x.ArtistId,
                    principalSchema: "dbo",
                    principalTable: "Artist",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_WorkArtist_WorkId",
            schema: "dbo",
            table: "WorkArtist",
            column: "WorkId");

        migrationBuilder.CreateIndex(
            name: "IX_WorkArtist_ArtistId",
            schema: "dbo",
            table: "WorkArtist",
            column: "ArtistId");

        migrationBuilder.CreateIndex(
            name: "UIX_WorkArtist_WorkId_Order",
            schema: "dbo",
            table: "WorkArtist",
            columns: new[] { "WorkId", "Order" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_WorkFeaturedArtist_WorkId",
            schema: "dbo",
            table: "WorkFeaturedArtist",
            column: "WorkId");

        migrationBuilder.CreateIndex(
            name: "IX_WorkFeaturedArtist_ArtistId",
            schema: "dbo",
            table: "WorkFeaturedArtist",
            column: "ArtistId");

        migrationBuilder.CreateIndex(
            name: "UIX_WorkFeaturedArtist_WorkId_Order",
            schema: "dbo",
            table: "WorkFeaturedArtist",
            columns: new[] { "WorkId", "Order" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_WorkPerformer_WorkId",
            schema: "dbo",
            table: "WorkPerformer",
            column: "WorkId");

        migrationBuilder.CreateIndex(
            name: "IX_WorkPerformer_ArtistId",
            schema: "dbo",
            table: "WorkPerformer",
            column: "ArtistId");

        migrationBuilder.CreateIndex(
            name: "UIX_WorkPerformer_WorkId_Order",
            schema: "dbo",
            table: "WorkPerformer",
            columns: new[] { "WorkId", "Order" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_WorkComposer_WorkId",
            schema: "dbo",
            table: "WorkComposer",
            column: "WorkId");

        migrationBuilder.CreateIndex(
            name: "IX_WorkComposer_ArtistId",
            schema: "dbo",
            table: "WorkComposer",
            column: "ArtistId");

        migrationBuilder.CreateIndex(
            name: "UIX_WorkComposer_WorkId_Order",
            schema: "dbo",
            table: "WorkComposer",
            columns: new[] { "WorkId", "Order" },
            unique: true);

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_WorkArtist_AfterInsertUpdateDelete_SetWorkUpdatedOn]
            ON [dbo].[WorkArtist]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedWork] AS
                (
                    SELECT [insertedWorkArtist].[WorkId] AS [Id]
                    FROM [inserted] AS [insertedWorkArtist]
                    UNION
                    SELECT [deletedWorkArtist].[WorkId] AS [Id]
                    FROM [deleted] AS [deletedWorkArtist]
                )
                UPDATE [dbo].[Work]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Work] AS [work]
                INNER JOIN [UpdatedWork] AS [updatedWork] ON [updatedWork].[Id] = [work].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_WorkFeaturedArtist_AfterInsertUpdateDelete_SetWorkUpdatedOn]
            ON [dbo].[WorkFeaturedArtist]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedWork] AS
                (
                    SELECT [insertedWorkFeaturedArtist].[WorkId] AS [Id]
                    FROM [inserted] AS [insertedWorkFeaturedArtist]
                    UNION
                    SELECT [deletedWorkFeaturedArtist].[WorkId] AS [Id]
                    FROM [deleted] AS [deletedWorkFeaturedArtist]
                )
                UPDATE [dbo].[Work]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Work] AS [work]
                INNER JOIN [UpdatedWork] AS [updatedWork] ON [updatedWork].[Id] = [work].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_WorkPerformer_AfterInsertUpdateDelete_SetWorkUpdatedOn]
            ON [dbo].[WorkPerformer]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedWork] AS
                (
                    SELECT [insertedWorkPerformer].[WorkId] AS [Id]
                    FROM [inserted] AS [insertedWorkPerformer]
                    UNION
                    SELECT [deletedWorkPerformer].[WorkId] AS [Id]
                    FROM [deleted] AS [deletedWorkPerformer]
                )
                UPDATE [dbo].[Work]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Work] AS [work]
                INNER JOIN [UpdatedWork] AS [updatedWork] ON [updatedWork].[Id] = [work].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TRIGGER [dbo].[TR_WorkComposer_AfterInsertUpdateDelete_SetWorkUpdatedOn]
            ON [dbo].[WorkComposer]
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH [UpdatedWork] AS
                (
                    SELECT [insertedWorkComposer].[WorkId] AS [Id]
                    FROM [inserted] AS [insertedWorkComposer]
                    UNION
                    SELECT [deletedWorkComposer].[WorkId] AS [Id]
                    FROM [deleted] AS [deletedWorkComposer]
                )
                UPDATE [dbo].[Work]
                SET [UpdatedOn] = SYSDATETIMEOFFSET()
                FROM [dbo].[Work] AS [work]
                INNER JOIN [UpdatedWork] AS [updatedWork] ON [updatedWork].[Id] = [work].[Id];
            END;");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[WorkArtist] AS TABLE
            (
                [WorkId] UNIQUEIDENTIFIER NOT NULL,
                [ArtistId] UNIQUEIDENTIFIER NOT NULL,
                [Order] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[WorkFeaturedArtist] AS TABLE
            (
                [WorkId] UNIQUEIDENTIFIER NOT NULL,
                [ArtistId] UNIQUEIDENTIFIER NOT NULL,
                [Order] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[WorkPerformer] AS TABLE
            (
                [WorkId] UNIQUEIDENTIFIER NOT NULL,
                [ArtistId] UNIQUEIDENTIFIER NOT NULL,
                [Order] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[WorkComposer] AS TABLE
            (
                [WorkId] UNIQUEIDENTIFIER NOT NULL,
                [ArtistId] UNIQUEIDENTIFIER NOT NULL,
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
                @WorkArtists [dbo].[WorkArtist] READONLY,
                @WorkFeaturedArtists [dbo].[WorkFeaturedArtist] READONLY,
                @WorkPerformers [dbo].[WorkPerformer] READONLY,
                @WorkComposers [dbo].[WorkComposer] READONLY,
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

                WITH [SourceWorkArtist] AS
                (
                    SELECT * FROM @WorkArtists WHERE [WorkId] = @Id
                )
                MERGE INTO [dbo].[WorkArtist] AS [target]
                USING [SourceWorkArtist] AS [source]
                ON [target].[WorkId] = [source].[WorkId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [WorkId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[WorkId],
                    [source].[ArtistId],
                    [source].[Order]
                );

                WITH [SourceWorkFeaturedArtist] AS
                (
                    SELECT * FROM @WorkFeaturedArtists WHERE [WorkId] = @Id
                )
                MERGE INTO [dbo].[WorkFeaturedArtist] AS [target]
                USING [SourceWorkFeaturedArtist] AS [source]
                ON [target].[WorkId] = [source].[WorkId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [WorkId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[WorkId],
                    [source].[ArtistId],
                    [source].[Order]
                );

                WITH [SourceWorkPerformer] AS
                (
                    SELECT * FROM @WorkPerformers WHERE [WorkId] = @Id
                )
                MERGE INTO [dbo].[WorkPerformer] AS [target]
                USING [SourceWorkPerformer] AS [source]
                ON [target].[WorkId] = [source].[WorkId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [WorkId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[WorkId],
                    [source].[ArtistId],
                    [source].[Order]
                );

                WITH [SourceWorkComposer] AS
                (
                    SELECT * FROM @WorkComposers WHERE [WorkId] = @Id
                )
                MERGE INTO [dbo].[WorkComposer] AS [target]
                USING [SourceWorkComposer] AS [source]
                ON [target].[WorkId] = [source].[WorkId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN NOT MATCHED THEN INSERT
                (
                    [WorkId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[WorkId],
                    [source].[ArtistId],
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
                @WorkArtists [dbo].[WorkArtist] READONLY,
                @WorkFeaturedArtists [dbo].[WorkFeaturedArtist] READONLY,
                @WorkPerformers [dbo].[WorkPerformer] READONLY,
                @WorkComposers [dbo].[WorkComposer] READONLY,
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

                WITH [SourceWorkArtist] AS
                (
                    SELECT * FROM @WorkArtists WHERE [WorkId] = @Id
                )
                MERGE INTO [dbo].[WorkArtist] AS [target]
                USING [SourceWorkArtist] AS [source]
                ON [target].[WorkId] = [source].[WorkId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [WorkId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[WorkId],
                    [source].[ArtistId],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[WorkId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceWorkFeaturedArtist] AS
                (
                    SELECT * FROM @WorkFeaturedArtists WHERE [WorkId] = @Id
                )
                MERGE INTO [dbo].[WorkFeaturedArtist] AS [target]
                USING [SourceWorkFeaturedArtist] AS [source]
                ON [target].[WorkId] = [source].[WorkId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [WorkId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[WorkId],
                    [source].[ArtistId],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[WorkId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceWorkPerformer] AS
                (
                    SELECT * FROM @WorkPerformers WHERE [WorkId] = @Id
                )
                MERGE INTO [dbo].[WorkPerformer] AS [target]
                USING [SourceWorkPerformer] AS [source]
                ON [target].[WorkId] = [source].[WorkId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [WorkId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[WorkId],
                    [source].[ArtistId],
                    [source].[Order]
                )
                WHEN NOT MATCHED BY SOURCE AND [target].[WorkId] = @Id THEN DELETE;

                SET @ResultRowsUpdated = @ResultRowsUpdated + @@ROWCOUNT;

                WITH [SourceWorkComposer] AS
                (
                    SELECT * FROM @WorkComposers WHERE [WorkId] = @Id
                )
                MERGE INTO [dbo].[WorkComposer] AS [target]
                USING [SourceWorkComposer] AS [source]
                ON [target].[WorkId] = [source].[WorkId] AND [target].[ArtistId] = [source].[ArtistId]
                WHEN MATCHED THEN UPDATE
                SET
                    [target].[Order] = [source].[Order]
                WHEN NOT MATCHED THEN INSERT
                (
                    [WorkId],
                    [ArtistId],
                    [Order]
                )
                VALUES
                (
                    [source].[WorkId],
                    [source].[ArtistId],
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

        migrationBuilder.DropTable(name: "WorkArtist", schema: "dbo");

        migrationBuilder.DropTable(name: "WorkFeaturedArtist", schema: "dbo");

        migrationBuilder.DropTable(name: "WorkPerformer", schema: "dbo");

        migrationBuilder.DropTable(name: "WorkComposer", schema: "dbo");

        migrationBuilder.Sql("DROP TYPE [dbo].[WorkArtist];");

        migrationBuilder.Sql("DROP TYPE [dbo].[WorkFeaturedArtist];");

        migrationBuilder.Sql("DROP TYPE [dbo].[WorkPerformer];");

        migrationBuilder.Sql("DROP TYPE [dbo].[WorkComposer];");

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
}
