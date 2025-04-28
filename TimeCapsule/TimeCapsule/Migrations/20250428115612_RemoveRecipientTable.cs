using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeCapsule.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRecipientTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CapsuleRecipients_Capsules_CapsuleId",
                table: "CapsuleRecipients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CapsuleRecipients",
                table: "CapsuleRecipients");

            migrationBuilder.RenameTable(
                name: "CapsuleRecipients",
                newName: "CapsuleRecipient");

            migrationBuilder.RenameIndex(
                name: "IX_CapsuleRecipients_CapsuleId",
                table: "CapsuleRecipient",
                newName: "IX_CapsuleRecipient_CapsuleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CapsuleRecipient",
                table: "CapsuleRecipient",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CapsuleRecipient_Capsules_CapsuleId",
                table: "CapsuleRecipient",
                column: "CapsuleId",
                principalTable: "Capsules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CapsuleRecipient_Capsules_CapsuleId",
                table: "CapsuleRecipient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CapsuleRecipient",
                table: "CapsuleRecipient");

            migrationBuilder.RenameTable(
                name: "CapsuleRecipient",
                newName: "CapsuleRecipients");

            migrationBuilder.RenameIndex(
                name: "IX_CapsuleRecipient_CapsuleId",
                table: "CapsuleRecipients",
                newName: "IX_CapsuleRecipients_CapsuleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CapsuleRecipients",
                table: "CapsuleRecipients",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CapsuleRecipients_Capsules_CapsuleId",
                table: "CapsuleRecipients",
                column: "CapsuleId",
                principalTable: "Capsules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
