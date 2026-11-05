using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeritageAI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCraftsmanSkillsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CraftsmanSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CraftsmanId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftsmanSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CraftsmanSkills_Craftsmen_CraftsmanId",
                        column: x => x.CraftsmanId,
                        principalTable: "Craftsmen",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CraftsmanSkills_CraftsmanId",
                table: "CraftsmanSkills",
                column: "CraftsmanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CraftsmanSkills");
        }
    }
}
