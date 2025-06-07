using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Text_Captcha.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class CaptchaTextToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaptchaTextTokens",
                schema: "identity",
                columns: table => new
                {
                    CaptchaTextTokenId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: false),
                    CaptchaTextId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaptchaTextTokens", x => x.CaptchaTextTokenId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaptchaTextTokens",
                schema: "identity");
        }
    }
}
