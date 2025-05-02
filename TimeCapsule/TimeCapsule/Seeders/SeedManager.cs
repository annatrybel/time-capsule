using TimeCapsule.Models;
using Microsoft.EntityFrameworkCore;

namespace TimeCapsule.Seeders
{
    public class SeedManager
    {
        private readonly TimeCapsuleContext context;
        private readonly SectionSeeder sectionSeeder;
        private readonly QuestionSeeder questionSeeder;

        public SeedManager(TimeCapsuleContext context, SectionSeeder sectionSeeder, QuestionSeeder questionSeeder)
        {
            this.context = context;
            this.sectionSeeder = sectionSeeder;
            this.questionSeeder = questionSeeder;
        }

        public async Task Seed()
        {
            if (!await context.CapsuleSections.AnyAsync())
                await sectionSeeder.Seed();

            if (!await context.CapsuleQuestions.AnyAsync())
                await questionSeeder.Seed();
        }
    }
}

