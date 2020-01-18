using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication2.Migrations
{
    public partial class sdfgsdfgdfg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ViewsTurns",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WeaponListId = table.Column<int>(nullable: false),
                    ChatId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewsTurns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViewsTurns_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ViewsTurns_WeaponList_WeaponListId",
                        column: x => x.WeaponListId,
                        principalTable: "WeaponList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ViewsTurns_ChatId",
                table: "ViewsTurns",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewsTurns_WeaponListId",
                table: "ViewsTurns",
                column: "WeaponListId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ViewsTurns");
        }
    }
}
