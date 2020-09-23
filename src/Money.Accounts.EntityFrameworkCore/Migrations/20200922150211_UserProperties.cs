using Microsoft.EntityFrameworkCore.Migrations;
using Money.EntityFrameworkCore.Migrations;

namespace Money.Migrations
{
    public partial class UserProperties : MigrationWithSchema<AccountContext>
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPropertyKeys",
                schema: Schema.Name,
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPropertyKeys", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "UserPropertyValues",
                schema: Schema.Name,
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 36, nullable: false),
                    KeyName = table.Column<string>(maxLength: 256, nullable: false),
                    Value = table.Column<string>(maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPropertyValues", x => new { x.UserId, x.KeyName });
                    table.ForeignKey(
                        name: "FK_UserPropertyValues_UserPropertyKeys_KeyName",
                        column: x => x.KeyName,
                        principalTable: "UserPropertyKeys",
                        principalSchema: Schema.Name,
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPropertyValues_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalSchema: Schema.Name,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPropertyValues_KeyName",
                table: "UserPropertyValues",
                schema: Schema.Name,
                column: "KeyName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPropertyKeys",
                schema: Schema.Name);

            migrationBuilder.DropTable(
                name: "UserPropertyValues",
                schema: Schema.Name);
        }
    }
}
