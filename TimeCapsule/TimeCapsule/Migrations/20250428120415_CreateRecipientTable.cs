using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TimeCapsule.Migrations
{
    /// <inheritdoc />
    public partial class CreateRecipientTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Najpierw usuń tabelę, jeśli istnieje (na wszelki wypadek)
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""CapsuleRecipients"" CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""CapsuleRecipient"" CASCADE;");

            // Utwórz tabelę CapsuleRecipients
            migrationBuilder.CreateTable(
                name: "CapsuleRecipients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CapsuleId = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    EmailSent = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapsuleRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CapsuleRecipients_Capsules_CapsuleId",
                        column: x => x.CapsuleId,
                        principalTable: "Capsules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CapsuleRecipients_CapsuleId",
                table: "CapsuleRecipients",
                column: "CapsuleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CapsuleRecipients");
        }

    }
}
