using Microsoft.EntityFrameworkCore.Migrations;
using Money.EntityFrameworkCore.Migrations;

namespace Money.Migrations
{
    public partial class UserProperties : MigrationWithSchema<AccountContext>
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProperties",
                schema: Schema.Name,
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 36, nullable: false),
                    Key = table.Column<string>(maxLength: 36, nullable: false),
                    Value = table.Column<string>(maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProperties", x => new { x.UserId, x.Key });
                    table.ForeignKey(
                        name: "FK_UserProperties_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: Schema.Name,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProperties",
                schema: Schema.Name);
        }
    }
}
