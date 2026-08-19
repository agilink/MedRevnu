using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATI.Migrations
{
    /// <inheritdoc />
    public partial class MakeProductCreationTimeNotNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, update any NULL CreationTime values to current date
            migrationBuilder.Sql(@"
                UPDATE [REV].[Product]
                SET CreationTime = GETDATE()
                WHERE CreationTime IS NULL
            ");

            // Then alter the column to NOT NULL
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationTime",
                schema: "REV",
                table: "Product",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationTime",
                schema: "REV",
                table: "Product",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
