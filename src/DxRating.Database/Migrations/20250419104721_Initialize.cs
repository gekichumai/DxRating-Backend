using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DxRating.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    email_confirmed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    mfa_enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "crypto_wallet",
                columns: table => new
                {
                    address = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crypto_wallet", x => x.address);
                    table.ForeignKey(
                        name: "FK_crypto_wallet_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "session",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    access_token = table.Column<string>(type: "text", nullable: false),
                    refresh_token = table.Column<string>(type: "text", nullable: false),
                    access_token_expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    refresh_token_expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    ip_address = table.Column<IPAddress>(type: "inet", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_session", x => x.id);
                    table.ForeignKey(
                        name: "FK_session_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "social_login",
                columns: table => new
                {
                    connection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    platform = table.Column<string>(type: "text", nullable: false),
                    identifier = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_social_login", x => x.connection_id);
                    table.ForeignKey(
                        name: "FK_social_login_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "token",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_type = table.Column<string>(type: "text", nullable: false),
                    verification_token = table.Column<string>(type: "text", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_token", x => x.id);
                    table.ForeignKey(
                        name: "FK_token_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "totp",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    secret = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_totp", x => x.id);
                    table.ForeignKey(
                        name: "FK_totp_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "webauthn_device",
                columns: table => new
                {
                    descriptor_id = table.Column<byte[]>(type: "bytea", nullable: false),
                    public_key = table.Column<byte[]>(type: "bytea", nullable: false),
                    user_handle = table.Column<byte[]>(type: "bytea", nullable: false),
                    aa_guid = table.Column<Guid>(type: "uuid", nullable: false),
                    signature_counter = table.Column<long>(type: "bigint", nullable: false),
                    cred_type = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_webauthn_device", x => x.descriptor_id);
                    table.ForeignKey(
                        name: "FK_webauthn_device_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_crypto_wallet_user_id",
                table: "crypto_wallet",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_session_access_token",
                table: "session",
                column: "access_token");

            migrationBuilder.CreateIndex(
                name: "IX_session_access_token_expires_at",
                table: "session",
                column: "access_token_expires_at");

            migrationBuilder.CreateIndex(
                name: "IX_session_refresh_token",
                table: "session",
                column: "refresh_token");

            migrationBuilder.CreateIndex(
                name: "IX_session_refresh_token_expires_at",
                table: "session",
                column: "refresh_token_expires_at");

            migrationBuilder.CreateIndex(
                name: "IX_session_user_id",
                table: "session",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_social_login_platform_identifier",
                table: "social_login",
                columns: new[] { "platform", "identifier" });

            migrationBuilder.CreateIndex(
                name: "IX_social_login_user_id",
                table: "social_login",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_token_expires_at",
                table: "token",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "IX_token_user_id",
                table: "token",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_token_verification_token",
                table: "token",
                column: "verification_token");

            migrationBuilder.CreateIndex(
                name: "IX_totp_user_id",
                table: "totp",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_email",
                table: "user",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_webauthn_device_user_id",
                table: "webauthn_device",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "crypto_wallet");

            migrationBuilder.DropTable(
                name: "session");

            migrationBuilder.DropTable(
                name: "social_login");

            migrationBuilder.DropTable(
                name: "token");

            migrationBuilder.DropTable(
                name: "totp");

            migrationBuilder.DropTable(
                name: "webauthn_device");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
