using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Money.EntityFrameworkCore.Migrations;

#nullable disable

namespace Money.Models.Migrations
{
    /// <inheritdoc />
    public partial class ExpenseExpectedWhen : MigrationWithSchema<ReadModelContext>
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedWhen",
                table: "Outcomes",
                schema: Schema.Name,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpectedWhen",
                table: "Outcomes",
                schema: Schema.Name);
        }
    }
}
