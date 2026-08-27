using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATI.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonnelUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId",
                schema: "ADM",
                table: "Personnel",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_UserId",
                schema: "ADM",
                table: "Personnel",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL AND [IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Personnel_UserId",
                schema: "ADM",
                table: "Personnel");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "ADM",
                table: "Personnel");
        }
    }
}
