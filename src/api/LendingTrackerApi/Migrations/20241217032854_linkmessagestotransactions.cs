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
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NewId()"),
                    LenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BorrowerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    BorrowedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    ReturnedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Transact__55433A6B92A192EE", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK__Transacti__Borro__47DBAE45",
                        column: x => x.BorrowerId,
                        principalTable: "Borrowers",
                        principalColumn: "BorrowerId");
                    table.ForeignKey(
                        name: "FK__Transacti__ItemI__48CFD27E",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId");
                    table.ForeignKey(
                        name: "FK__Transacti__Lende__46E78A0C",
                        column: x => x.LenderId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Items_ItemId",
                table: "Message");

        

       

            migrationBuilder.AlterColumn<int>(
                name: "ItemId",
                table: "Message",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Items_ItemId",
                table: "Message",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Transactions_TransactionId",
                table: "Message",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Cascade);
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
