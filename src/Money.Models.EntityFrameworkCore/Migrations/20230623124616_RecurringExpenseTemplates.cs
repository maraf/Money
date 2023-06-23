using Microsoft.EntityFrameworkCore.Migrations;
using Money.EntityFrameworkCore.Migrations;

#nullable disable

namespace Money.Models.Migrations
{
    /// <inheritdoc />
    public partial class RecurringExpenseTemplates : MigrationWithSchema<ReadModelContext>
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DayInPeriod",
                table: "ExpenseTemplates",
                schema: Schema.Name,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Period",
                table: "ExpenseTemplates",
                schema: Schema.Name,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayInPeriod",
                table: "ExpenseTemplates",
                schema: Schema.Name);

            migrationBuilder.DropColumn(
                name: "Period",
                table: "ExpenseTemplates",
                schema: Schema.Name);
        }
    }
}
