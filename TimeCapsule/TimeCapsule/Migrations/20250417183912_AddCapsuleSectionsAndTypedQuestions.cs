using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TimeCapsule.Migrations
{
    /// <inheritdoc />
    public partial class AddCapsuleSectionsAndTypedQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Question1Answer",
                table: "Capsules");

            migrationBuilder.AddColumn<int>(
                name: "CapsuleSectionId",
                table: "CapsuleQuestions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "CapsuleQuestions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CapsuleSection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CapsuleType = table.Column<int>(type: "integer", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapsuleSection", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CapsuleQuestions_CapsuleSectionId",
                table: "CapsuleQuestions",
                column: "CapsuleSectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CapsuleQuestions_CapsuleSection_CapsuleSectionId",
                table: "CapsuleQuestions",
                column: "CapsuleSectionId",
                principalTable: "CapsuleSection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CapsuleQuestions_CapsuleSection_CapsuleSectionId",
                table: "CapsuleQuestions");

            migrationBuilder.DropTable(
                name: "CapsuleSection");

            migrationBuilder.DropIndex(
                name: "IX_CapsuleQuestions_CapsuleSectionId",
                table: "CapsuleQuestions");

            migrationBuilder.DropColumn(
                name: "CapsuleSectionId",
                table: "CapsuleQuestions");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "CapsuleQuestions");

            migrationBuilder.AddColumn<string>(
                name: "Question1Answer",
                table: "Capsules",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
