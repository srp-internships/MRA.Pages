using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MRA.Pages.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ContentApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Application",
                table: "Contents",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Application",
                table: "Contents");
        }
    }
}
