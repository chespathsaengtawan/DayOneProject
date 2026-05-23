using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DayOneAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddEventCategoryColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "event_categories",
                type: "character varying(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "event_categories");
        }
    }
}
