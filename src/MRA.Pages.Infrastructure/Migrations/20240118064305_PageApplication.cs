using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MRA.Pages.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PageApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Application",
                table: "Pages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Application",
                table: "Pages");
        }
    }
}
