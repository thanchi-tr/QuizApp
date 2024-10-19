using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuizApp.Migrations
{
    /// <inheritdoc />
    public partial class CodeFirstDBwithinitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MetaAnswers",
                columns: table => new
                {
                    type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    test_service = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answer_service = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    validation_service = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaAnswers", x => x.type);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    collection_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.collection_id);
                    table.ForeignKey(
                        name: "FK_Collections_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    question_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    collection_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    last_revision = table.Column<DateOnly>(type: "date", nullable: false, defaultValue: new DateOnly(2024, 10, 14))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.question_id);
                    table.ForeignKey(
                        name: "FK_Questions_Collections_collection_id",
                        column: x => x.collection_id,
                        principalTable: "Collections",
                        principalColumn: "collection_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    answer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    meta_answer_type = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.answer_id);
                    table.ForeignKey(
                        name: "FK_Answers_MetaAnswers_meta_answer_type",
                        column: x => x.meta_answer_type,
                        principalTable: "MetaAnswers",
                        principalColumn: "type",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Answers_Questions_answer_id",
                        column: x => x.answer_id,
                        principalTable: "Questions",
                        principalColumn: "question_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MetaAnswers",
                columns: new[] { "type", "answer_service", "test_service", "validation_service" },
                values: new object[] { "MultipleChoice", "QuizApp.MultiChoiceAnswerExtractor", "QuizApp.MultiChoiceTestExtractor", "QuizApp.MultiChoiceAnswerValidator" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "user_id", "user_name" },
                values: new object[] { new Guid("2a8c2fd1-4443-4e0e-ac39-062f7c1c75d3"), "June" });

            migrationBuilder.InsertData(
                table: "Collections",
                columns: new[] { "collection_id", "name", "user_id" },
                values: new object[,]
                {
                    { new Guid("22b7f73c-9be2-425c-a3cf-c8a34000ca80"), "Biolgy", new Guid("2a8c2fd1-4443-4e0e-ac39-062f7c1c75d3") },
                    { new Guid("a0afe69c-3417-4cbf-9c54-b962204350d6"), "Economic", new Guid("2a8c2fd1-4443-4e0e-ac39-062f7c1c75d3") }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "question_id", "collection_id", "value" },
                values: new object[,]
                {
                    { new Guid("11f4d460-ff85-40b0-ae94-55dbddaab8ff"), new Guid("a0afe69c-3417-4cbf-9c54-b962204350d6"), "What does a Cat say?" },
                    { new Guid("9b90392c-e13b-4c88-acbf-16a845c620c9"), new Guid("a0afe69c-3417-4cbf-9c54-b962204350d6"), "What does a dog say?" }
                });

            migrationBuilder.InsertData(
                table: "Answers",
                columns: new[] { "answer_id", "meta_answer_type", "value" },
                values: new object[] { new Guid("9b90392c-e13b-4c88-acbf-16a845c620c9"), "MultipleChoice", "{choices:[{content: 'woof woff', isCorrect: true},{content: 'woof 1woff', isCorrect: false},{content: 'woof 2woff', isCorrect: false},{content: 'woof coff', isCorrect: false}]}" });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_meta_answer_type",
                table: "Answers",
                column: "meta_answer_type");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_user_id",
                table: "Collections",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_collection_id",
                table: "Questions",
                column: "collection_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "MetaAnswers");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
