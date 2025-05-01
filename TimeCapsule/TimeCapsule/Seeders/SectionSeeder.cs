using TimeCapsule.Models;
using TimeCapsule.Models.DatabaseModels;

namespace TimeCapsule.Seeders
{
    public class SectionSeeder
    {
        private readonly TimeCapsuleContext context;

        public SectionSeeder(TimeCapsuleContext context)
        {
            this.context = context;
        }

        public async Task Seed()
        {
            var sections = new List<CapsuleSection>
            {
                new CapsuleSection { Name = "O Tobie", CapsuleType = CapsuleType.Indywidualna, DisplayOrder = 1 },
                new CapsuleSection { Name = "Wspomnienia i relacje", CapsuleType = CapsuleType.DlaKogos, DisplayOrder = 5 },
                new CapsuleSection { Name = "Relacje", CapsuleType = CapsuleType.Indywidualna, DisplayOrder = 4 },
                new CapsuleSection { Name = "Przyszłość", CapsuleType = CapsuleType.Indywidualna, DisplayOrder = 3 },
                new CapsuleSection { Name = "Życie codzienne", CapsuleType = CapsuleType.Indywidualna, DisplayOrder = 2 }
            };

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                context.CapsuleSections.AddRange(sections);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}