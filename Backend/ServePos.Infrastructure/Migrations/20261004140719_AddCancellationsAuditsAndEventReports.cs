using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ServePos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCancellationsAuditsAndEventReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Orders",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "OrderItem",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AuditEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Action = table.Column<string>(type: "text", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    StaffId = table.Column<int>(type: "integer", nullable: false),
                    DeviceId = table.Column<string>(type: "text", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditEvents_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderCancellations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderItemId = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    PerformedById = table.Column<int>(type: "integer", nullable: false),
                    DeviceId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCancellations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderCancellations_OrderItem_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderCancellations_Staff_PerformedById",
                        column: x => x.PerformedById,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_EventId",
                table: "Orders",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_StaffId",
                table: "AuditEvents",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCancellations_OrderItemId",
                table: "OrderCancellations",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCancellations_PerformedById",
                table: "OrderCancellations",
                column: "PerformedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Events_EventId",
                table: "Orders",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Events_EventId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "AuditEvents");

            migrationBuilder.DropTable(
                name: "OrderCancellations");

            migrationBuilder.DropIndex(
                name: "IX_Orders_EventId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "OrderItem");
        }
    }
}
