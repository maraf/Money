using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Money.EntityFrameworkCore.Migrations
{
    public partial class Initial : MigrationWithSchema<EventSourcingContext>
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (Schema.Name != null)
                migrationBuilder.EnsureSchema(Schema.Name);

            migrationBuilder.CreateTable(
                name: "Command",
                schema: Schema.Name,
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommandID = table.Column<Guid>(nullable: false),
                    CommandType = table.Column<string>(nullable: true),
                    Payload = table.Column<string>(nullable: true),
                    RaisedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Command", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                schema: Schema.Name,
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventID = table.Column<Guid>(nullable: false),
                    EventType = table.Column<string>(nullable: true),
                    AggregateID = table.Column<Guid>(nullable: false),
                    AggregateType = table.Column<string>(nullable: true),
                    Payload = table.Column<string>(nullable: true),
                    Version = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UnPublishedCommand",
                schema: Schema.Name,
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommandID = table.Column<int>(nullable: false),
                    IsHandled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnPublishedCommand", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UnPublishedCommand_Command_CommandID",
                        column: x => x.CommandID,
                        principalSchema: Schema.Name,
                        principalTable: "Command",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnPublishedEvent",
                schema: Schema.Name,
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnPublishedEvent", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UnPublishedEvent_Event_EventID",
                        column: x => x.EventID,
                        principalSchema: Schema.Name,
                        principalTable: "Event",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventPublishedToHandler",
                schema: Schema.Name,
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventID = table.Column<int>(nullable: false),
                    HandlerIdentifier = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventPublishedToHandler", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EventPublishedToHandler_UnPublishedEvent_EventID",
                        column: x => x.EventID,
                        principalSchema: Schema.Name,
                        principalTable: "UnPublishedEvent",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventPublishedToHandler_EventID",
                schema: Schema.Name,
                table: "EventPublishedToHandler",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_UnPublishedCommand_CommandID",
                schema: Schema.Name,
                table: "UnPublishedCommand",
                column: "CommandID");

            migrationBuilder.CreateIndex(
                name: "IX_UnPublishedEvent_EventID",
                schema: Schema.Name,
                table: "UnPublishedEvent",
                column: "EventID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventPublishedToHandler",
                schema: Schema.Name);

            migrationBuilder.DropTable(
                name: "UnPublishedCommand",
                schema: Schema.Name);

            migrationBuilder.DropTable(
                name: "UnPublishedEvent",
                schema: Schema.Name);

            migrationBuilder.DropTable(
                name: "Command",
                schema: Schema.Name);

            migrationBuilder.DropTable(
                name: "Event",
                schema: Schema.Name);
        }
    }
}
