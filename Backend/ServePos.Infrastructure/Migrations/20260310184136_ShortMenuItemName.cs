using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServePos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ShortMenuItemName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "MenuItems",
                type: "text",
                nullable: true,
                defaultValue: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "MenuItems");
        }
    }
}
