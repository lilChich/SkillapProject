using Microsoft.EntityFrameworkCore.Migrations;

namespace Skillap.DAL.Migrations
{
    public partial class AddingLikes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Like",
                schema: "dbo",
                table: "LikedPosts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Like",
                schema: "dbo",
                table: "LikedComments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Like",
                schema: "dbo",
                table: "LikedPosts");

            migrationBuilder.DropColumn(
                name: "Like",
                schema: "dbo",
                table: "LikedComments");
        }
    }
}
