using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Money.EntityFrameworkCore.Migrations;

namespace Money.Migrations
{
    public partial class UserActivity : MigrationWithSchema<AccountContext>
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                schema: Schema.Name,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSignedAt",
                table: "AspNetUsers",
                schema: Schema.Name,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers",
                schema: Schema.Name);

            migrationBuilder.DropColumn(
                name: "LastSignedAt",
                table: "AspNetUsers",
                schema: Schema.Name);
        }
    }
}
