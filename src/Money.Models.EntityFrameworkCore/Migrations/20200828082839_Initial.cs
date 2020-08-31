using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Money.EntityFrameworkCore.Migrations;

namespace Money.Models.Migrations
{
    public partial class Initial : MigrationWithSchema<ReadModelContext>
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (Schema.Name != null)
                migrationBuilder.EnsureSchema(Schema.Name);

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: Schema.Name,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    ColorA = table.Column<byte>(nullable: false),
                    ColorR = table.Column<byte>(nullable: false),
                    ColorG = table.Column<byte>(nullable: false),
                    ColorB = table.Column<byte>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                schema: Schema.Name,
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 36, nullable: false),
                    UniqueCode = table.Column<string>(maxLength: 128, nullable: false),
                    Symbol = table.Column<string>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => new { x.UserId, x.UniqueCode });
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                schema: Schema.Name,
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    TargetCurrency = table.Column<string>(nullable: true),
                    SourceCurrency = table.Column<string>(nullable: true),
                    Rate = table.Column<double>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Outcomes",
                schema: Schema.Name,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    When = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outcomes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutcomeCategories",
                schema: Schema.Name,
                columns: table => new
                {
                    OutcomeId = table.Column<Guid>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutcomeCategories", x => new { x.OutcomeId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_OutcomeCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: Schema.Name,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutcomeCategories_Outcomes_OutcomeId",
                        column: x => x.OutcomeId,
                        principalSchema: Schema.Name,
                        principalTable: "Outcomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeCategories_CategoryId",
                schema: Schema.Name,
                table: "OutcomeCategories",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currencies",
                schema: Schema.Name);

            migrationBuilder.DropTable(
                name: "ExchangeRates",
                schema: Schema.Name);

            migrationBuilder.DropTable(
                name: "OutcomeCategories",
                schema: Schema.Name);

            migrationBuilder.DropTable(
                name: "Categories",
                schema: Schema.Name);

            migrationBuilder.DropTable(
                name: "Outcomes",
                schema: Schema.Name);
        }
    }
}
