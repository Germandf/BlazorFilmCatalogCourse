using Microsoft.EntityFrameworkCore.Migrations;

namespace BlazorFilmCatalogCourse.Server.Migrations
{
    public partial class AdminRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3e9f7823-6f61-429b-b3b1-21baed9bc159", "11b52661-9e25-467d-9a7d-9c564a2b9df7", "admin", "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3e9f7823-6f61-429b-b3b1-21baed9bc159");
        }
    }
}
