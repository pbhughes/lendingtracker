using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendingTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class setdefaultlimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxBorrowers",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 5);

            migrationBuilder.AddColumn<int>(
                name: "MaxItems",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxBorrowers",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MaxItems",
                table: "Users");
        }
    }
}
