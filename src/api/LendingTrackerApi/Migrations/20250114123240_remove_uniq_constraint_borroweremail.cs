using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendingTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class remove_uniq_constraint_borroweremail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ__Borrower__User__14F16AF6B87CB05D",
                table: "Borrowers");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowers_BorrowerId_UserId",
                table: "Borrowers",
                columns: new[] { "BorrowerId", "UserId" },
                unique: false);

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Borrowers_BorrowerId_UserId",
                table: "Borrowers");

            migrationBuilder.DropIndex(
                name: "UQ__Borrower__User__14F16AF6B87CB05D",
                table: "Borrowers");

          
        }
    }
}
