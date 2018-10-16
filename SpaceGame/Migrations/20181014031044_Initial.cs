using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SpaceGame.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GroupId = table.Column<long>(nullable: true),
                    GroupId1 = table.Column<long>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Role = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HostPlayerId = table.Column<long>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsInGame = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Group_Player_HostPlayerId",
                        column: x => x.HostPlayerId,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Group_HostPlayerId",
                table: "Group",
                column: "HostPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_GroupId",
                table: "Player",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_GroupId1",
                table: "Player",
                column: "GroupId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Player_Group_GroupId",
                table: "Player",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Player_Group_GroupId1",
                table: "Player",
                column: "GroupId1",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Group_Player_HostPlayerId",
                table: "Group");

            migrationBuilder.DropTable(
                name: "Player");

            migrationBuilder.DropTable(
                name: "Group");
        }
    }
}
