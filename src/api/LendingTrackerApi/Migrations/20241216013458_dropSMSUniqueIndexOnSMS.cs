using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendingTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class dropSMSUniqueIndexOnSMS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex("UQ__Borrower__0E4EE86AE3B83218", table : "Borrowers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
