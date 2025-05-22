using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeCapsule.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCascadeDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CapsuleRecipients_CapsuleId",
                table: "CapsuleRecipients");

            migrationBuilder.CreateIndex(
                name: "IX_CapsuleSections_DisplayOrder",
                table: "CapsuleSections",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_CapsuleSections_Name_CapsuleType",
                table: "CapsuleSections",
                columns: new[] { "Name", "CapsuleType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Capsules_OpeningDate",
                table: "Capsules",
                column: "OpeningDate");

            migrationBuilder.CreateIndex(
                name: "IX_Capsules_Status",
                table: "Capsules",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CapsuleRecipients_CapsuleId_Email",
                table: "CapsuleRecipients",
                columns: new[] { "CapsuleId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CapsuleQuestions_DisplayOrder",
                table: "CapsuleQuestions",
                column: "DisplayOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CapsuleSections_DisplayOrder",
                table: "CapsuleSections");

            migrationBuilder.DropIndex(
                name: "IX_CapsuleSections_Name_CapsuleType",
                table: "CapsuleSections");

            migrationBuilder.DropIndex(
                name: "IX_Capsules_OpeningDate",
                table: "Capsules");

            migrationBuilder.DropIndex(
                name: "IX_Capsules_Status",
                table: "Capsules");

            migrationBuilder.DropIndex(
                name: "IX_CapsuleRecipients_CapsuleId_Email",
                table: "CapsuleRecipients");

            migrationBuilder.DropIndex(
                name: "IX_CapsuleQuestions_DisplayOrder",
                table: "CapsuleQuestions");

            migrationBuilder.CreateIndex(
                name: "IX_CapsuleRecipients_CapsuleId",
                table: "CapsuleRecipients",
                column: "CapsuleId");
        }
    }
}
