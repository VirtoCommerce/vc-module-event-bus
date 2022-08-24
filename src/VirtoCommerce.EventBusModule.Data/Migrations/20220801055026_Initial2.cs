using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.EventBusModule.Data.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventBus2ProviderConnection",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ProviderName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ConnectionOptionsSerialized = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventBus2ProviderConnection", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventBus2ProviderConnectionLog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProviderName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventBus2ProviderConnectionLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventBus2Subscription",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ConnectionName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    JsonPathFilter = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    PayloadTransformationTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventSettingsSerialized = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventBus2Subscription", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventBus2SubscriptionEvent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    EventId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SubscriptionId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventBus2SubscriptionEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventBus2SubscriptionEvent_EventBus2Subscription_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "EventBus2Subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventBus2SubscriptionEvent_SubscriptionId",
                table: "EventBus2SubscriptionEvent",
                column: "SubscriptionId");

            // Preserve compatibility: convert existing subscriptions to a newly added tables
            migrationBuilder.Sql(@"
                -- Make connections from existing subscriptions
                with tmpConnections as
                (
                    select 
                    newid() Id, 
                    'AzureEventGrid' + cast(ROW_NUMBER() OVER(order by ConnectionString, AccessKey) as char(3)) Name, 
                    'AzureEventGrid' ProviderName,
                    '{\""ConnectionString\"": \""'+isnull(ConnectionString,'')+'\"", \""AccessKey\"": \""'+isnull(AccessKey,'')+'\""}' ConnectionOptionsSerialized,
                    getDate() CreatedDate,
                    getDate() ModifiedDate,
                    'unknown' CreatedBy,
                    'unknown' ModifiedBy
                    from EventBusSubscription
                    group by ConnectionString, AccessKey
                )
                insert into EventBus2ProviderConnection
                select * from tmpConnections;

                -- Convert subscriptions and link to newly created connections
                with tmpConnections as
                (
                    select 
                    newid() Id, 
                    'AzureEventGrid' + cast(ROW_NUMBER() OVER(order by ConnectionString, AccessKey) as char(3)) Name, 
                    AccessKey,
                    ConnectionString
                    from EventBusSubscription
                    group by ConnectionString, AccessKey
                )
                insert into EventBus2Subscription
                select 
                    EventBusSubscription.Id Id,
                    EventBusSubscription.Id [Name],
                    tmpConnections.[Name] ConnectionName,
                    '$' JsonPathFilter,
                    '' PayloadTransformationTemplate,
                    null EventSettingsSerialized,
                    EventBusSubscription.CreatedDate,
                    EventBusSubscription.ModifiedDate,
                    EventBusSubscription.CreatedBy,
                    EventBusSubscription.ModifiedBy
                     from EventBusSubscription inner join tmpConnections on 
	                    isnull(EventBusSubscription.AccessKey,'') = isnull(tmpConnections.AccessKey,'')
	                    and isnull(EventBusSubscription.ConnectionString, '') = isnull(tmpConnections.ConnectionString, '');

                -- Convert subscription events
                insert into EventBus2SubscriptionEvent
                select Id, EventId, SubscriptionId, CreatedDate, ModifiedDate, CreatedBy, ModifiedBy from EventBusSubscriptionEvent;

            ");
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
