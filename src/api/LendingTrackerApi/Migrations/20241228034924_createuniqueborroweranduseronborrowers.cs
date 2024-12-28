using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendingTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class createuniqueborroweranduseronborrowers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "UQ__Borrower__14F16AF6B87CB05D", table: "Borrowers");

            migrationBuilder.CreateIndex(name: "UQ__Borrower__User__14F16AF6B87CB05D", table: "Borrowers", columns: new[] { "BorrowerEmail", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "UQ__Borrower__User__14F16AF6B87CB05D");

            migrationBuilder.CreateIndex(name: "UQ__Borrower__14F16AF6B87CB05D", table: "Borrowers", columns: new [] { "BorrowerEmail" });
        }
    }
}
