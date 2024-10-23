using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizApp.Migrations
{
    /// <inheritdoc />
    public partial class Introducerefreshtoken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "last_revision",
                table: "Questions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2024, 10, 23),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValue: new DateOnly(2024, 10, 22));

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    token_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.token_id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "question_id",
                keyValue: new Guid("11f4d460-ff85-40b0-ae94-55dbddaab8ff"),
                column: "last_revision",
                value: new DateOnly(2024, 10, 23));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "question_id",
                keyValue: new Guid("9b90392c-e13b-4c88-acbf-16a845c620c9"),
                column: "last_revision",
                value: new DateOnly(2024, 10, 23));

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "last_revision",
                table: "Questions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2024, 10, 22),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValue: new DateOnly(2024, 10, 23));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "question_id",
                keyValue: new Guid("11f4d460-ff85-40b0-ae94-55dbddaab8ff"),
                column: "last_revision",
                value: new DateOnly(2024, 10, 22));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "question_id",
                keyValue: new Guid("9b90392c-e13b-4c88-acbf-16a845c620c9"),
                column: "last_revision",
                value: new DateOnly(2024, 10, 22));
        }
    }
}
