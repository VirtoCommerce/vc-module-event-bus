using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.EventBusModule.Data.Migrations
{
    public partial class InitialEventBus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventBusSubscription",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Provider = table.Column<string>(maxLength: 128, nullable: true),
                    ConnectionString = table.Column<string>(maxLength: 512, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ErrorMessage = table.Column<string>(maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventBusSubscription", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventBusSubscriptionEvent",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    EventId = table.Column<string>(maxLength: 128, nullable: true),
                    SubscriptionId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventBusSubscriptionEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventBusSubscriptionEvent_EventBusSubscription_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "EventBusSubscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventBusSubscriptionEvent_SubscriptionId",
                table: "EventBusSubscriptionEvent",
                column: "SubscriptionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventBusSubscriptionEvent");

            migrationBuilder.DropTable(
                name: "EventBusSubscription");
        }
    }
}
