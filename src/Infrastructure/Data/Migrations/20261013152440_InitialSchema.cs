using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: true),
                    body = table.Column<string>(type: "text", nullable: true),
                    template_key = table.Column<string>(type: "text", nullable: true),
                    template_version = table.Column<string>(type: "text", nullable: true),
                    source_service = table.Column<string>(type: "text", nullable: true),
                    source_event_type = table.Column<string>(type: "text", nullable: true),
                    source_event_id = table.Column<string>(type: "text", nullable: true),
                    correlation_id = table.Column<string>(type: "text", nullable: true),
                    deep_link_url = table.Column<string>(type: "text", nullable: true),
                    metadata_json = table.Column<string>(type: "text", nullable: true),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    scheduled_for = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    processed_at = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    created_date = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_date = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    updated_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notifications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification_recipients",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    notification_id = table.Column<long>(type: "bigint", nullable: false),
                    recipient_id = table.Column<string>(type: "text", nullable: true),
                    recipient_type = table.Column<int>(type: "integer", nullable: false),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    whats_app_number = table.Column<string>(type: "text", nullable: true),
                    push_token = table.Column<string>(type: "text", nullable: true),
                    in_app_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    read_at = table.Column<Instant>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_recipients", x => x.id);
                    table.ForeignKey(
                        name: "fk_notification_recipients_notifications_notification_id",
                        column: x => x.notification_id,
                        principalTable: "notifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notification_deliveries",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    recipient_id = table.Column<long>(type: "bigint", nullable: false),
                    channel = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    attempt_count = table.Column<int>(type: "integer", nullable: false),
                    last_attempted_at = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    sent_at = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    failed_at = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    next_attempt_at = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    provider = table.Column<string>(type: "text", nullable: true),
                    provider_message_id = table.Column<string>(type: "text", nullable: true),
                    failure_reason = table.Column<string>(type: "text", nullable: true),
                    payload_json = table.Column<string>(type: "text", nullable: true),
                    external_reference = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_deliveries", x => x.id);
                    table.ForeignKey(
                        name: "fk_notification_deliveries_notification_recipients_recipient_id",
                        column: x => x.recipient_id,
                        principalTable: "notification_recipients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_notification_deliveries_recipient_id",
                table: "notification_deliveries",
                column: "recipient_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_recipients_notification_id",
                table: "notification_recipients",
                column: "notification_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification_deliveries");

            migrationBuilder.DropTable(
                name: "notification_recipients");

            migrationBuilder.DropTable(
                name: "notifications");
        }
    }
}
