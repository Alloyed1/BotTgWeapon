using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication2.Migrations
{
    public partial class sdfgsdfgsdfg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    ChatId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LastQuery",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Query = table.Column<string>(nullable: true),
                    ChatId = table.Column<string>(nullable: true),
                    IsWatching = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastQuery", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeaponList",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Src = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    GroupId = table.Column<long>(nullable: false),
                    AlbumId = table.Column<long>(nullable: false),
                    FirstComment = table.Column<string>(nullable: true),
                    PhotoId = table.Column<long>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserViews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<string>(nullable: true),
                    PhotoId = table.Column<string>(nullable: true),
                    ChatId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserViews_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserViews_ChatId",
                table: "UserViews",
                column: "ChatId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LastQuery");

            migrationBuilder.DropTable(
                name: "UserViews");

            migrationBuilder.DropTable(
                name: "WeaponList");

            migrationBuilder.DropTable(
                name: "Chats");
        }
    }
}
