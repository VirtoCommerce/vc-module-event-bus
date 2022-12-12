using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.EventBusModule.Data.PostgreSql.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventBus2ProviderConnection",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ProviderName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ConnectionOptionsSerialized = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventBus2ProviderConnection", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventBus2ProviderConnectionLog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    ProviderName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventBus2ProviderConnectionLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventBus2Subscription",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ConnectionName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    JsonPathFilter = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    PayloadTransformationTemplate = table.Column<string>(type: "text", nullable: true),
                    EventSettingsSerialized = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventBus2Subscription", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventBus2SubscriptionEvent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    EventId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SubscriptionId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventBus2SubscriptionEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventBus2SubscriptionEvent_EventBus2Subscription_Subscripti~",
                        column: x => x.SubscriptionId,
                        principalTable: "EventBus2Subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventBus2SubscriptionEvent_SubscriptionId",
                table: "EventBus2SubscriptionEvent",
                column: "SubscriptionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventBus2ProviderConnection");

            migrationBuilder.DropTable(
                name: "EventBus2ProviderConnectionLog");

            migrationBuilder.DropTable(
                name: "EventBus2SubscriptionEvent");

            migrationBuilder.DropTable(
                name: "EventBus2Subscription");
        }
    }
}
