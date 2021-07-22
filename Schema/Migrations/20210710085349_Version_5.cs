using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Schema.Migrations
{
    public partial class Version_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastRep",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RepCount",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMessage",
                table: "Experiences",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastClaim",
                table: "Balances",
                type: "datetime(6)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRep",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RepCount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastMessage",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "LastClaim",
                table: "Balances");
        }
    }
}
