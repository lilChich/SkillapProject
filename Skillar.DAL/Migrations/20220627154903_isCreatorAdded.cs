using Microsoft.EntityFrameworkCore.Migrations;

namespace Skillap.DAL.Migrations
{
    public partial class isCreatorAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isCreator",
                schema: "dbo",
                table: "LikedPosts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isCreator",
                schema: "dbo",
                table: "LikedComments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isCreator",
                schema: "dbo",
                table: "LikedPosts");

            migrationBuilder.DropColumn(
                name: "isCreator",
                schema: "dbo",
                table: "LikedComments");
        }
    }
}
