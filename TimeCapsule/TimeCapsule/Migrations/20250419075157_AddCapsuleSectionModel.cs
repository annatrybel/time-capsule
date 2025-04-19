using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeCapsule.Migrations
{
    /// <inheritdoc />
    public partial class AddCapsuleSectionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CapsuleQuestions_CapsuleSection_CapsuleSectionId",
                table: "CapsuleQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CapsuleSection",
                table: "CapsuleSection");

            migrationBuilder.RenameTable(
                name: "CapsuleSection",
                newName: "CapsuleSections");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CapsuleSections",
                table: "CapsuleSections",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CapsuleQuestions_CapsuleSections_CapsuleSectionId",
                table: "CapsuleQuestions",
                column: "CapsuleSectionId",
                principalTable: "CapsuleSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CapsuleQuestions_CapsuleSections_CapsuleSectionId",
                table: "CapsuleQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CapsuleSections",
                table: "CapsuleSections");

            migrationBuilder.RenameTable(
                name: "CapsuleSections",
                newName: "CapsuleSection");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CapsuleSection",
                table: "CapsuleSection",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CapsuleQuestions_CapsuleSection_CapsuleSectionId",
                table: "CapsuleQuestions",
                column: "CapsuleSectionId",
                principalTable: "CapsuleSection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
