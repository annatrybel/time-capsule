using TimeCapsule.Models;
using TimeCapsule.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace TimeCapsule.Seeders
{
    public class QuestionSeeder
    {
        private readonly TimeCapsuleContext context;

        public QuestionSeeder(TimeCapsuleContext context)
        {
            this.context = context;
        }

        public async Task Seed()
        {
            var sections = await context.CapsuleSections.ToDictionaryAsync(
                s => s.Name,
                s => s.Id
            );

            if (!sections.Any())
            {
                throw new InvalidOperationException("Brak sekcji w bazie danych. Najpierw uruchom SectionSeeder.");
            }

            var questions = new List<CapsuleQuestion>();

            if (sections.TryGetValue("O Tobie", out var oTobieId))
            {
                questions.AddRange(new[]
                {
                    new CapsuleQuestion { CapsuleSectionId = oTobieId, QuestionText = "Jak się dziś czujesz?", DisplayOrder = 1 },
                    new CapsuleQuestion { CapsuleSectionId = oTobieId, QuestionText = "Jak opisał(a)byś się w kilku zdaniach?", DisplayOrder = 2 },
                    new CapsuleQuestion { CapsuleSectionId = oTobieId, QuestionText = "Jakie są 3 rzeczy, które najbardziej lubisz?", DisplayOrder = 3 },
                    new CapsuleQuestion { CapsuleSectionId = oTobieId, QuestionText = "Jakie jest Twoje największe marzenie?", DisplayOrder = 4 }
                });
            }

            if (sections.TryGetValue("Życie codzienne", out var zycieCodzienneId))
            {
                questions.AddRange(new[]
                {
                    new CapsuleQuestion { CapsuleSectionId = zycieCodzienneId, QuestionText = "Jak wygląda Twój typowy dzień?", DisplayOrder = 1 },
                    new CapsuleQuestion { CapsuleSectionId = zycieCodzienneId, QuestionText = "Co ostatnio sprawiło Ci największą radość?", DisplayOrder = 2 },
                    new CapsuleQuestion { CapsuleSectionId = zycieCodzienneId, QuestionText = "Czego nauczyłeś/aś się w ostatnim czasie?", DisplayOrder = 3 },
                    new CapsuleQuestion { CapsuleSectionId = zycieCodzienneId, QuestionText = "Jakie masz obecnie hobby i zainteresowania?", DisplayOrder = 4 }
                });
            }

            if (sections.TryGetValue("Przyszłość", out var przyszloscId))
            {
                questions.AddRange(new[]
                {
                    new CapsuleQuestion { CapsuleSectionId = przyszloscId, QuestionText = "Gdzie widzisz siebie za 5 lat?", DisplayOrder = 1 },
                    new CapsuleQuestion { CapsuleSectionId = przyszloscId, QuestionText = "Jakie są Twoje cele na najbliższy rok?", DisplayOrder = 2 },
                    new CapsuleQuestion { CapsuleSectionId = przyszloscId, QuestionText = "Czego chciałbyś/chciałabyś się nauczyć w przyszłości?", DisplayOrder = 3 },
                    new CapsuleQuestion { CapsuleSectionId = przyszloscId, QuestionText = "Co chcesz powiedzieć swojemu przyszłemu ja?", DisplayOrder = 4 }
                });
            }

            if (sections.TryGetValue("Relacje", out var relacjeId))
            {
                questions.AddRange(new[]
                {
                    new CapsuleQuestion { CapsuleSectionId = relacjeId, QuestionText = "Kto jest obecnie najważniejszą osobą w Twoim życiu?", DisplayOrder = 1 },
                    new CapsuleQuestion { CapsuleSectionId = relacjeId, QuestionText = "Jakie relacje chciałbyś/chciałabyś rozwijać?", DisplayOrder = 2 },
                    new CapsuleQuestion { CapsuleSectionId = relacjeId, QuestionText = "Co cenisz najbardziej w swoich przyjaciołach?", DisplayOrder = 3 },
                    new CapsuleQuestion { CapsuleSectionId = relacjeId, QuestionText = "Jaką jedną rzecz chciałbyś/chciałabyś powiedzieć bliskim osobom?", DisplayOrder = 4 }
                });
            }

            if (sections.TryGetValue("Wspomnienia i relacje", out var wspomnieniaId))
            {
                questions.Add(new CapsuleQuestion { CapsuleSectionId = wspomnieniaId, QuestionText = "Jakie wspólne wspomnienie najbardziej utkwiło Ci w pamięci?", DisplayOrder = 1 });
            }

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                context.CapsuleQuestions.AddRange(questions);
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