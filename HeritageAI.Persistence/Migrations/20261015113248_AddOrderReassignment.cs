using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeritageAI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderReassignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedCraftsmanId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderTransfers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    FromCraftsmanId = table.Column<int>(type: "int", nullable: false),
                    ToCraftsmanId = table.Column<int>(type: "int", nullable: false),
                    AdminId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTransfers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AssignedCraftsmanId",
                table: "Orders",
                column: "AssignedCraftsmanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Craftsmen_AssignedCraftsmanId",
                table: "Orders",
                column: "AssignedCraftsmanId",
                principalTable: "Craftsmen",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Craftsmen_AssignedCraftsmanId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "OrderTransfers");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AssignedCraftsmanId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AssignedCraftsmanId",
                table: "Orders");
        }
    }
}
