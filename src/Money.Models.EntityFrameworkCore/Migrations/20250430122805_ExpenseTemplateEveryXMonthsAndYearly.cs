using Microsoft.EntityFrameworkCore.Migrations;
using Money.EntityFrameworkCore.Migrations;

#nullable disable

namespace Money.Models.Migrations
{
    /// <inheritdoc />
    public partial class ExpenseTemplateEveryXMonthsAndYearly : MigrationWithSchema<ReadModelContext>
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EveryXPeriods",
                table: "ExpenseTemplates",
                schema: Schema.Name,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MonthInPeriod",
                table: "ExpenseTemplates",
                schema: Schema.Name,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EveryXPeriods",
                table: "ExpenseTemplates",
                schema: Schema.Name);

            migrationBuilder.DropColumn(
                name: "MonthInPeriod",
                table: "ExpenseTemplates",
                schema: Schema.Name);
        }
    }
}
