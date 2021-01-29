using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.EventBusModule.Data.Migrations
{
    public partial class AddAccessKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessKey",
                table: "EventBusSubscription",
                maxLength: 512,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessKey",
                table: "EventBusSubscription");
        }
    }
}
