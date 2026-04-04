using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkyBot.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageToBotUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "BotUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "BotUsers");
        }
    }
}
