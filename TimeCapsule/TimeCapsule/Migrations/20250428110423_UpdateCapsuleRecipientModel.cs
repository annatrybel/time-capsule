using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeCapsule.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCapsuleRecipientModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_name='capsulerecipients' AND column_name='email'
                    ) THEN
                        ALTER TABLE ""CapsuleRecipients"" ADD COLUMN ""Email"" text NOT NULL DEFAULT '';
                    END IF;
                END $$;");

            migrationBuilder.Sql(
                @"DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_name='capsulerecipients' AND column_name='recipientuserid'
                    ) THEN
                        ALTER TABLE ""CapsuleRecipients"" DROP COLUMN ""RecipientUserId"";
                    END IF;
                    
                    IF EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_name='capsulerecipients' AND column_name='recipientuserid1'
                    ) THEN
                        ALTER TABLE ""CapsuleRecipients"" DROP COLUMN ""RecipientUserId1"";
                    END IF;
                END $$;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_name='capsulerecipients' AND column_name='email'
                    ) THEN
                        ALTER TABLE ""CapsuleRecipients"" DROP COLUMN ""Email"";
                    END IF;
                END $$;");            
        }
    }
}