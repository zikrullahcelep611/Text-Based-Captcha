using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Text_Captcha.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class FixDatabaseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                schema: "identity",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_QuestionId",
                schema: "identity",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "IsCorrect",
                schema: "identity",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "OptionText",
                schema: "identity",
                table: "Answers",
                newName: "AnswerText");

            migrationBuilder.RenameColumn(
                name: "OptionId",
                schema: "identity",
                table: "Answers",
                newName: "AnswerId");

            migrationBuilder.CreateTable(
                name: "Options",
                schema: "identity",
                columns: table => new
                {
                    OptionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    OptionText = table.Column<string>(type: "text", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Options", x => x.OptionId);
                    table.ForeignKey(
                        name: "FK_Options_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "identity",
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Options_QuestionId",
                schema: "identity",
                table: "Options",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Options",
                schema: "identity");

            migrationBuilder.RenameColumn(
                name: "AnswerText",
                schema: "identity",
                table: "Answers",
                newName: "OptionText");

            migrationBuilder.RenameColumn(
                name: "AnswerId",
                schema: "identity",
                table: "Answers",
                newName: "OptionId");

            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                schema: "identity",
                table: "Answers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId",
                schema: "identity",
                table: "Answers",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                schema: "identity",
                table: "Answers",
                column: "QuestionId",
                principalSchema: "identity",
                principalTable: "Questions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
