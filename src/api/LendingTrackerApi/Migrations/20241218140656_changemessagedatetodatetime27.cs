using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendingTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class changemessagedatetodatetime27 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
               name: "MessageDate",
               table: "Message",
               type: "datetime2(7)",
               nullable: false,
               oldClrType: typeof(DateTime),
               oldType: "datetime",
               defaultValueSql: "GETDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
               name: "MessageDate",
               table: "Message",
               type: "datetime",
               nullable: false,
               oldClrType: typeof(string),
               oldType: "nvarchar(max)",
               defaultValueSql: "GETDATE()");
        }
    }
}
