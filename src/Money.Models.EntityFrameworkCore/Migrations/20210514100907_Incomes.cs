using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Money.EntityFrameworkCore.Migrations;

namespace Money.Models.Migrations
{
    public partial class Incomes : MigrationWithSchema<ReadModelContext>
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Incomes",
                schema: Schema.Name,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(maxLength: 36, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    When = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incomes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Incomes",
                schema: Schema.Name);
        }
    }
}
