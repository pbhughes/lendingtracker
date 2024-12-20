using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendingTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class changeTransactionDatesToDateTime27 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "MessageDate",
                table: "Message",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
              name: "ReturnedAt",
              table: "Message",
              type: "datetime2",
              nullable: false,
              oldClrType: typeof(DateTime),
              oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
              name: "DueDate",
              table: "Message",
              type: "datetime2",
              nullable: false,
              oldClrType: typeof(DateTime),
              oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "MessageDate",
                table: "Message",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<DateTime>(
               name: "ReturnedAt",
               table: "Message",
               type: "datetime2",
               nullable: false,
               oldClrType: typeof(DateTime),
               oldType: "datetime2");


            migrationBuilder.AlterColumn<DateTime>(
               name: "DueDate",
               table: "Message",
               type: "datetime2",
               nullable: false,
               oldClrType: typeof(DateTime),
               oldType: "datetime2");
        }
    }
}
