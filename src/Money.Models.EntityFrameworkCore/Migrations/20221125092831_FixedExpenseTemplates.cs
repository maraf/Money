using Microsoft.EntityFrameworkCore.Migrations;
using Money.EntityFrameworkCore.Migrations;

namespace Money.Models.Migrations
{
    public partial class FixedExpenseTemplates : MigrationWithSchema<ReadModelContext>
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFixed",
                table: "ExpenseTemplates",
                schema: Schema.Name,
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFixed",
                table: "ExpenseTemplates",
                schema: Schema.Name);
        }
    }
}
