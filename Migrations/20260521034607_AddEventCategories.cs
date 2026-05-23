using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DayOneAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddEventCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "events");

            migrationBuilder.AddColumn<Guid>(
                name: "EventCategoryId",
                table: "events",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "event_categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_categories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_events_EventCategoryId",
                table: "events",
                column: "EventCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_events_event_categories_EventCategoryId",
                table: "events",
                column: "EventCategoryId",
                principalTable: "event_categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_events_event_categories_EventCategoryId",
                table: "events");

            migrationBuilder.DropTable(
                name: "event_categories");

            migrationBuilder.DropIndex(
                name: "IX_events_EventCategoryId",
                table: "events");

            migrationBuilder.DropColumn(
                name: "EventCategoryId",
                table: "events");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "events",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
