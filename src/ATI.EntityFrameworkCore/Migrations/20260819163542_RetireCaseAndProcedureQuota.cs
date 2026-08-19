using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATI.Migrations
{
    /// <summary>
    /// Removes Case, CaseProduct and ProcedureQuota from the EF model.
    /// </summary>
    /// <remarks>
    /// ProcedureTransaction is now the single revenue model and ProductQuota the single
    /// quota model, so these three entities were deleted from the codebase.
    ///
    /// This migration deliberately drops NO tables. The scaffolded version dropped all
    /// three, which would have destroyed the rows still in REV.Case and REV.CaseProduct;
    /// retiring the code does not require discarding the data, and dropping a table is
    /// not reversible. REV.ProcedureQuota was never created in the first place - only
    /// ProcedureType came out of AddProcedureTypesIncrementally - so dropping it would
    /// also have failed outright.
    ///
    /// The tables therefore remain in the database, no longer mapped by the model, which
    /// EF ignores. DbScripts/ATI-78_REV.Case_ReportRetainedData.sql reports what they
    /// hold. Dropping them is a separate, deliberate decision once the client confirms
    /// the contents are not needed.
    ///
    /// Up and Down are both empty on purpose: the value of this migration is the model
    /// snapshot change, and a Down that recreated tables which were never dropped would
    /// fail.
    /// </remarks>
    public partial class RetireCaseAndProcedureQuota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty - see the class remarks.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty - Up() made no schema change to reverse.
        }
    }
}
