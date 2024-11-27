﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.EventBusModule.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddErrorPayload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorPayload",
                table: "EventBus2ProviderConnectionLog",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorPayload",
                table: "EventBus2ProviderConnectionLog");
        }
    }
}