using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Money.EntityFrameworkCore.Migrations;

#nullable disable

namespace Money.Models.Migrations
{
    /// <inheritdoc />
    public partial class ExpenseTemplateCreateDeleteDate : MigrationWithSchema<ReadModelContext>
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ExpenseTemplates",
                schema: Schema.Name,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ExpenseTemplates",
                schema: Schema.Name,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ExpenseTemplates",
                schema: Schema.Name,
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ExpenseTemplates",
                schema: Schema.Name);

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ExpenseTemplates",
                schema: Schema.Name);

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ExpenseTemplates",
                schema: Schema.Name);
        }
    }
}
