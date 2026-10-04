using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServePos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClientOrderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClientOrderId",
                table: "Orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                table: "MenuItems",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ClientOrderId",
                table: "Orders",
                column: "ClientOrderId",
                unique: true,
                filter: "\"ClientOrderId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_ClientOrderId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ClientOrderId",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                table: "MenuItems",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
