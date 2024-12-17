using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendingTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class addStandardMessagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "StandardMessages",
               columns: table => new
               {
                   Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                   Text = table.Column<string>(type: "nvarchar(166)", maxLength: 166, nullable: false)
               });

          

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}
