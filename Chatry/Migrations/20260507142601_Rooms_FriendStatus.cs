using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chatry.Migrations
{
    /// <inheritdoc />
    public partial class Rooms_FriendStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FriendStatus",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FriendStatus",
                table: "Rooms");
        }
    }
}
