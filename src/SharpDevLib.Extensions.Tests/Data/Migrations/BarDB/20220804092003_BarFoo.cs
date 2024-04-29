using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace SharpDevLib.Extensions.Tests.Data.Migrations.BarDB
{
    public partial class BarFoo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BarSamples",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    CreateTime = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarSamples", x => x.Id);
                });

            migrationBuilder.CreateTable(
               name: "Users",
               columns: table => new
               {
                   Id = table.Column<Guid>(type: "TEXT", nullable: false),
                   Name = table.Column<string>(type: "TEXT", nullable: true),
                   CreateTime = table.Column<long>(type: "INTEGER", nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_Users", x => x.Id);
               });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BarSamples");

            migrationBuilder.DropTable(
               name: "Users");
        }
    }
}
