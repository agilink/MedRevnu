using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATI.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonnelTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Personnel",
                schema: "ADM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonnelTypeID = table.Column<int>(type: "int", nullable: true),
                    EmployeeStatusID = table.Column<int>(type: "int", nullable: true),
                    FacilityId = table.Column<int>(type: "int", nullable: true),
                    FIRST_NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FIRST_NAME_PROPER = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MIDDLE_NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LAST_NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SSN = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    EMPLOYEE_ID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DATE_BIRTH = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DATE_HIRE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    COMPANY_NAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ADDRESS1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ADDRESS2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CITY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StateID = table.Column<int>(type: "int", nullable: true),
                    ZIP_CODE = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NUMBER_HOME = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NUMBER_MOBILE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EMAIL_WORK = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EMAIL_OTHER = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NOTES = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    START_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    END_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MODIFIED_BY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MODIFIED_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_Personnel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personnel_Facility_FacilityId",
                        column: x => x.FacilityId,
                        principalSchema: "ADM",
                        principalTable: "Facility",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Personnel_State_StateID",
                        column: x => x.StateID,
                        principalSchema: "ADM",
                        principalTable: "State",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_EMPLOYEE_ID",
                schema: "ADM",
                table: "Personnel",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_FacilityId",
                schema: "ADM",
                table: "Personnel",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_LAST_NAME",
                schema: "ADM",
                table: "Personnel",
                column: "LAST_NAME");

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_StateID",
                schema: "ADM",
                table: "Personnel",
                column: "StateID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Personnel",
                schema: "ADM");
        }
    }
}
