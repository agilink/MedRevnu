using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATI.Migrations
{
    /// <inheritdoc />
    public partial class AddHospitalProductPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HospitalProductPrice",
                schema: "REV",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HospitalId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HospitalProductPrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HospitalProductPrice_Facility_HospitalId",
                        column: x => x.HospitalId,
                        principalSchema: "ADM",
                        principalTable: "Facility",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HospitalProductPrice_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "REV",
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HospitalProductPrice_HospitalId",
                schema: "REV",
                table: "HospitalProductPrice",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "IX_HospitalProductPrice_HospitalId_ProductId_EffectiveDate",
                schema: "REV",
                table: "HospitalProductPrice",
                columns: new[] { "HospitalId", "ProductId", "EffectiveDate" });

            migrationBuilder.CreateIndex(
                name: "IX_HospitalProductPrice_ProductId",
                schema: "REV",
                table: "HospitalProductPrice",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HospitalProductPrice",
                schema: "REV");
        }
    }
}
