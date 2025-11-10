using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATI.Revenue.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "REV");

            //migrationBuilder.CreateTable(
            //    name: "Case",
            //    schema: "REV",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CaseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        ClientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
            //        CaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
            //        Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
            //        CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
            //        LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Case", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ProductCategory",
            //    schema: "REV",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
            //        CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
            //        LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProductCategory", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Product",
            //    schema: "REV",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            //        Manufacturer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            //        ModelNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
            //        ProductCategoryId = table.Column<int>(type: "int", nullable: true),
            //        Cost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
            //        Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
            //        LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Product", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Product_ProductCategory_ProductCategoryId",
            //            column: x => x.ProductCategoryId,
            //            principalSchema: "REV",
            //            principalTable: "ProductCategory",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.SetNull);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CaseProduct",
            //    schema: "REV",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CaseId = table.Column<int>(type: "int", nullable: false),
            //        ProductId = table.Column<int>(type: "int", nullable: false),
            //        Quantity = table.Column<int>(type: "int", nullable: false),
            //        UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
            //        Discount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
            //        TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CaseProduct", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_CaseProduct_Case_CaseId",
            //            column: x => x.CaseId,
            //            principalSchema: "REV",
            //            principalTable: "Case",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_CaseProduct_Product_ProductId",
            //            column: x => x.ProductId,
            //            principalSchema: "REV",
            //            principalTable: "Product",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            migrationBuilder.CreateIndex(
                name: "IX_CaseProduct_CaseId",
                schema: "REV",
                table: "CaseProduct",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseProduct_ProductId",
                schema: "REV",
                table: "CaseProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductCategoryId",
                schema: "REV",
                table: "Product",
                column: "ProductCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "CaseProduct",
            //    schema: "REV");

            //migrationBuilder.DropTable(
            //    name: "Case",
            //    schema: "REV");

            //migrationBuilder.DropTable(
            //    name: "Product",
            //    schema: "REV");

            //migrationBuilder.DropTable(
            //    name: "ProductCategory",
            //    schema: "REV");
        }
    }
}
