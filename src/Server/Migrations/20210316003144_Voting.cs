using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BlazorFilmCatalogCourse.Server.Migrations
{
    public partial class Voting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FilmVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Vote = table.Column<int>(type: "int", nullable: false),
                    VoteDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FilmId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilmVotes_Films_FilmId",
                        column: x => x.FilmId,
                        principalTable: "Films",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilmVotes_FilmId",
                table: "FilmVotes",
                column: "FilmId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilmVotes");
        }
    }
}
