using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATI.Migrations
{
    /// <inheritdoc />
    public partial class MakeProductSubcategoryImplantTypeNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ImplantType",
                schema: "REV",
                table: "ProductSubcategory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ImplantType",
                schema: "REV",
                table: "ProductSubcategory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
