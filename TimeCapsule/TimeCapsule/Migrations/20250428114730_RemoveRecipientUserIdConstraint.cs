using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeCapsule.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRecipientUserIdConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {           
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_name = 'capsulerecipients'
                          AND column_name = 'recipientuserid'
                    ) THEN
                        ALTER TABLE ""CapsuleRecipients"" DROP COLUMN ""RecipientUserId"";
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_name = 'capsulerecipients'
                          AND column_name = 'recipientuserid'
                    ) THEN
                        ALTER TABLE ""CapsuleRecipients"" ADD COLUMN ""RecipientUserId"" integer DEFAULT 0;
                    END IF;
                END $$;
            ");
        }
    }
}