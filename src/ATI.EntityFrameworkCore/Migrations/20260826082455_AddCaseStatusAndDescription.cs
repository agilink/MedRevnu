using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATI.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseStatusAndDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProcedureTransaction_CaseNumber",
                schema: "REV",
                table: "ProcedureTransaction");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "REV",
                table: "ProcedureTransaction",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            // CaseStatus starts at Open = 1; a scaffolded default of 0 would leave every
            // existing case holding a value outside the enum.
            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "REV",
                table: "ProcedureTransaction",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureTransaction_CaseNumber",
                schema: "REV",
                table: "ProcedureTransaction",
                column: "CaseNumber",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProcedureTransaction_CaseNumber",
                schema: "REV",
                table: "ProcedureTransaction");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "REV",
                table: "ProcedureTransaction");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "REV",
                table: "ProcedureTransaction");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureTransaction_CaseNumber",
                schema: "REV",
                table: "ProcedureTransaction",
                column: "CaseNumber",
                unique: true);
        }
    }
}
