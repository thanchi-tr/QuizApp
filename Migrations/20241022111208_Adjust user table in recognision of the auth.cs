using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizApp.Migrations
{
    /// <inheritdoc />
    public partial class Adjustusertableinrecognisionoftheauth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "last_revision",
                table: "Questions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2024, 10, 22),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValue: new DateOnly(2024, 10, 20));

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

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "user_id",
                keyValue: new Guid("2a8c2fd1-4443-4e0e-ac39-062f7c1c75d3"),
                column: "password",
                value: "test");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "password",
                table: "Users");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "last_revision",
                table: "Questions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2024, 10, 20),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValue: new DateOnly(2024, 10, 22));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "question_id",
                keyValue: new Guid("11f4d460-ff85-40b0-ae94-55dbddaab8ff"),
                column: "last_revision",
                value: new DateOnly(2024, 10, 20));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "question_id",
                keyValue: new Guid("9b90392c-e13b-4c88-acbf-16a845c620c9"),
                column: "last_revision",
                value: new DateOnly(2024, 10, 20));
        }
    }
}
