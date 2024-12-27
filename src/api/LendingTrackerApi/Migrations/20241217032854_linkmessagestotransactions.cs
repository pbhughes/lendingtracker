using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendingTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class linkmessagestotransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint("PK__Transact__55433A6B92A192EE", "Transactions");

            migrationBuilder.DropColumn("TransactionId", "Transactions");

            migrationBuilder.AddColumn<Guid>(
                name: "TransactionId",
                table: "Message",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NewId()");

          


            migrationBuilder.CreateIndex(
                name: "IX_Message_ItemId1",
                table: "Message",
                column: "TransactionId");

           

        
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Items_ItemId",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_Transactions_TransactionId",
                table: "Message");

            migrationBuilder.DropTable(
                name: "StandardMessages");

            migrationBuilder.DropIndex(
                name: "IX_Message_ItemId1",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Message");

            migrationBuilder.AlterColumn<int>(
                name: "TransactionId",
                table: "Transactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "ItemId",
                table: "Message",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Items_ItemId",
                table: "Message",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
