using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MRA.Pages.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TitleToContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Contents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Contents");
        }
    }
}
