using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeCapsule.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCapsuleAnswerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CapsuleAnswers_CapsuleQuestions_CapsuleQuestionId",
                table: "CapsuleAnswers");

            migrationBuilder.RenameColumn(
                name: "CapsuleQuestionId",
                table: "CapsuleAnswers",
                newName: "QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_CapsuleAnswers_CapsuleQuestionId",
                table: "CapsuleAnswers",
                newName: "IX_CapsuleAnswers_QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CapsuleAnswers_CapsuleQuestions_QuestionId",
                table: "CapsuleAnswers",
                column: "QuestionId",
                principalTable: "CapsuleQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CapsuleAnswers_CapsuleQuestions_QuestionId",
                table: "CapsuleAnswers");

            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "CapsuleAnswers",
                newName: "CapsuleQuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_CapsuleAnswers_QuestionId",
                table: "CapsuleAnswers",
                newName: "IX_CapsuleAnswers_CapsuleQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CapsuleAnswers_CapsuleQuestions_CapsuleQuestionId",
                table: "CapsuleAnswers",
                column: "CapsuleQuestionId",
                principalTable: "CapsuleQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
