using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATI.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTransactionEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                schema: "REV",
                table: "ProductCategory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                schema: "REV",
                table: "Product",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                schema: "REV",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                schema: "REV",
                table: "Product",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SubproductCategoryId",
                schema: "REV",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProcedureTransaction",
                schema: "REV",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcedureDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HospitalId = table.Column<int>(type: "int", nullable: true),
                    PhysicianId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProcedureType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
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
                    table.PrimaryKey("PK_ProcedureTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcedureTransaction_Facility_HospitalId",
                        column: x => x.HospitalId,
                        principalSchema: "ADM",
                        principalTable: "Facility",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProcedureTransaction_Personnel_PhysicianId",
                        column: x => x.PhysicianId,
                        principalSchema: "ADM",
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcedureTransaction_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "REV",
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductQuota",
                schema: "REV",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HospitalId = table.Column<int>(type: "int", nullable: false),
                    ProductCategoryId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    PeriodMonth = table.Column<int>(type: "int", nullable: false),
                    PeriodYear = table.Column<int>(type: "int", nullable: false),
                    TargetAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    TargetUnits = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductQuota", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductQuota_Facility_HospitalId",
                        column: x => x.HospitalId,
                        principalSchema: "ADM",
                        principalTable: "Facility",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductQuota_ProductCategory_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalSchema: "REV",
                        principalTable: "ProductCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductQuota_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "REV",
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProductSubcategory",
                schema: "REV",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductCategoryId = table.Column<int>(type: "int", nullable: false),
                    SubcategoryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProcedureType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSubcategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSubcategory_ProductCategory_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalSchema: "REV",
                        principalTable: "ProductCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductCode",
                schema: "REV",
                table: "Product",
                column: "ProductCode");

            migrationBuilder.CreateIndex(
                name: "IX_Product_SubproductCategoryId",
                schema: "REV",
                table: "Product",
                column: "SubproductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureTransaction_HospitalId",
                schema: "REV",
                table: "ProcedureTransaction",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureTransaction_PhysicianId",
                schema: "REV",
                table: "ProcedureTransaction",
                column: "PhysicianId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureTransaction_ProcedureDate",
                schema: "REV",
                table: "ProcedureTransaction",
                column: "ProcedureDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureTransaction_ProductId",
                schema: "REV",
                table: "ProcedureTransaction",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductQuota_HospitalId_ProductCategoryId_PeriodMonth_PeriodYear",
                schema: "REV",
                table: "ProductQuota",
                columns: new[] { "HospitalId", "ProductCategoryId", "PeriodMonth", "PeriodYear" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductQuota_ProductCategoryId",
                schema: "REV",
                table: "ProductQuota",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductQuota_ProductId",
                schema: "REV",
                table: "ProductQuota",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubcategory_ProductCategoryId",
                schema: "REV",
                table: "ProductSubcategory",
                column: "ProductCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_ProductSubcategory_SubproductCategoryId",
                schema: "REV",
                table: "Product",
                column: "SubproductCategoryId",
                principalSchema: "REV",
                principalTable: "ProductSubcategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_ProductSubcategory_SubproductCategoryId",
                schema: "REV",
                table: "Product");

            migrationBuilder.DropTable(
                name: "ProcedureTransaction",
                schema: "REV");

            migrationBuilder.DropTable(
                name: "ProductQuota",
                schema: "REV");

            migrationBuilder.DropTable(
                name: "ProductSubcategory",
                schema: "REV");

            migrationBuilder.DropIndex(
                name: "IX_Product_ProductCode",
                schema: "REV",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_SubproductCategoryId",
                schema: "REV",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                schema: "REV",
                table: "ProductCategory");

            migrationBuilder.DropColumn(
                name: "BasePrice",
                schema: "REV",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IsSystem",
                schema: "REV",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                schema: "REV",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "SubproductCategoryId",
                schema: "REV",
                table: "Product");
        }
    }
}
