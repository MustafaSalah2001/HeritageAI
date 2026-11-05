using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeritageAI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderCancelledByBuyer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CancelledByBuyer",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelledByBuyer",
                table: "Orders");
        }
    }
}
