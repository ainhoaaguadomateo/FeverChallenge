using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fever.Migrations
{
    /// <inheritdoc />
    public partial class Fever : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "base_events",
                columns: table => new
                {
                    base_event_id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    sell_mode = table.Column<string>(
                        type: "varchar(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    title = table.Column<string>(type: "varchar(50)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_base_events", x => x.base_event_id);
                }
            );

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_id = table.Column<int>(type: "int", nullable: false),
                    base_event_id = table.Column<int>(type: "int", nullable: false),
                    event_start_date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    event_end_date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    sell_from = table.Column<DateTime>(type: "timestamp", nullable: false),
                    sell_to = table.Column<DateTime>(type: "timestamp", nullable: false),
                    sold_out = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_events_base_events_base_event_id",
                        column: x => x.base_event_id,
                        principalTable: "base_events",
                        principalColumn: "base_event_id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "resumed_events",
                columns: table => new
                {
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "varchar(100)", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    min_price = table.Column<decimal>(type: "decimal", nullable: false),
                    max_price = table.Column<decimal>(type: "decimal", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resumed_events", x => x.event_id);
                    table.ForeignKey(
                        name: "FK_resumed_events_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "zones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    zone_id = table.Column<int>(type: "int", nullable: false),
                    capacity = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    name = table.Column<string>(type: "varchar(100)", nullable: false),
                    numbered = table.Column<bool>(type: "boolean", nullable: false),
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_zones_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "idx_base_events_base_event_id",
                table: "base_events",
                column: "base_event_id"
            );

            migrationBuilder.CreateIndex(
                name: "idx_event_base_event_id",
                table: "events",
                column: "base_event_id"
            );

            migrationBuilder.CreateIndex(
                name: "idx_event_baseevent_event_id_base_event_id",
                table: "events",
                columns: new[] { "event_id", "base_event_id" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "idx_event_event_end_date",
                table: "events",
                column: "event_end_date"
            );

            migrationBuilder.CreateIndex(
                name: "idx_event_event_start_date",
                table: "events",
                column: "event_start_date"
            );

            migrationBuilder.CreateIndex(
                name: "idx_resume_event_end_date",
                table: "resumed_events",
                column: "end_date"
            );

            migrationBuilder.CreateIndex(
                name: "idx_resume_event_start_date",
                table: "resumed_events",
                column: "start_date"
            );

            migrationBuilder.CreateIndex(
                name: "IX_zones_event_id",
                table: "zones",
                column: "event_id"
            );

            migrationBuilder.CreateIndex(
                name: "ux_zone_zone_id_event_id",
                table: "zones",
                columns: new[] { "zone_id", "event_id" },
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "resumed_events");

            migrationBuilder.DropTable(name: "zones");

            migrationBuilder.DropTable(name: "events");

            migrationBuilder.DropTable(name: "base_events");
        }
    }
}
