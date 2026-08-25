using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATI.Migrations
{
    /// <inheritdoc />
    public partial class SplitProcedureTransactionIntoCaseAndDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // A case can involve two or three devices, so the single ProductId, Quantity
            // and UnitPrice move off ProcedureTransaction onto a line table. The
            // scaffolded version dropped those columns before the line table existed,
            // which would have discarded every recorded device; this adds the new shape,
            // copies the data across, and drops the old columns last.

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                schema: "REV",
                table: "ProcedureTransaction",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AddColumn<string>(
                name: "CaseNumber",
                schema: "REV",
                table: "ProcedureTransaction",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ProcedureTransactionProduct",
                schema: "REV",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcedureTransactionId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedureTransactionProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcedureTransactionProduct_ProcedureTransaction_ProcedureTransactionId",
                        column: x => x.ProcedureTransactionId,
                        principalSchema: "REV",
                        principalTable: "ProcedureTransaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcedureTransactionProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "REV",
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });


            // Each existing row was one case with one device: carry that device over.
            migrationBuilder.Sql(@"
                INSERT INTO [REV].[ProcedureTransactionProduct]
                    ([ProcedureTransactionId], [ProductId], [Quantity], [UnitPrice], [LineTotal],
                     [CreationTime], [CreatorUserId], [IsDeleted])
                SELECT
                    pt.[Id],
                    pt.[ProductId],
                    CASE WHEN pt.[Quantity] > 0 THEN pt.[Quantity] ELSE 1 END,
                    pt.[UnitPrice],
                    pt.[UnitPrice] * CASE WHEN pt.[Quantity] > 0 THEN pt.[Quantity] ELSE 1 END,
                    pt.[CreationTime],
                    pt.[CreatorUserId],
                    0
                FROM [REV].[ProcedureTransaction] pt
                WHERE pt.[ProductId] IS NOT NULL;");

            // Case numbers are new and must be unique before the index is created. Rows
            // that predate the column get a generated one the client can rename.
            migrationBuilder.Sql(@"
                UPDATE [REV].[ProcedureTransaction]
                SET [CaseNumber] = 'CASE-' + CAST([Id] AS NVARCHAR(20))
                WHERE [CaseNumber] IS NULL OR LTRIM(RTRIM([CaseNumber])) = '';");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcedureTransaction_Product_ProductId",
                schema: "REV",
                table: "ProcedureTransaction");

            migrationBuilder.DropIndex(
                name: "IX_ProcedureTransaction_ProductId",
                schema: "REV",
                table: "ProcedureTransaction");

            migrationBuilder.DropColumn(
                name: "ProductId",
                schema: "REV",
                table: "ProcedureTransaction");

            migrationBuilder.DropColumn(
                name: "Quantity",
                schema: "REV",
                table: "ProcedureTransaction");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                schema: "REV",
                table: "ProcedureTransaction");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureTransaction_CaseNumber",
                schema: "REV",
                table: "ProcedureTransaction",
                column: "CaseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureTransactionProduct_ProcedureTransactionId",
                schema: "REV",
                table: "ProcedureTransactionProduct",
                column: "ProcedureTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureTransactionProduct_ProductId",
                schema: "REV",
                table: "ProcedureTransactionProduct",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcedureTransactionProduct",
                schema: "REV");

            migrationBuilder.DropIndex(
                name: "IX_ProcedureTransaction_CaseNumber",
                schema: "REV",
                table: "ProcedureTransaction");

            migrationBuilder.DropColumn(
                name: "CaseNumber",
                schema: "REV",
                table: "ProcedureTransaction");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                schema: "REV",
                table: "ProcedureTransaction",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                schema: "REV",
                table: "ProcedureTransaction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                schema: "REV",
                table: "ProcedureTransaction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                schema: "REV",
                table: "ProcedureTransaction",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureTransaction_ProductId",
                schema: "REV",
                table: "ProcedureTransaction",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcedureTransaction_Product_ProductId",
                schema: "REV",
                table: "ProcedureTransaction",
                column: "ProductId",
                principalSchema: "REV",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
