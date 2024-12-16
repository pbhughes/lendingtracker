using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendingTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddNameToBorrower : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Borrowers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Borrowers");
        }
    }
}
