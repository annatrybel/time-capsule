using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeCapsule.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCapsuleAttachmentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "CapsuleAttachments");

            migrationBuilder.DropColumn(
                name: "Link",
                table: "CapsuleAttachments");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "CapsuleAttachments");

            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                table: "CapsuleAttachments",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "CapsuleAttachments");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "CapsuleAttachments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "CapsuleAttachments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "CapsuleAttachments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
