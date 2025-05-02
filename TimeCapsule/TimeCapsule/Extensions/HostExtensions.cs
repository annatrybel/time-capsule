using TimeCapsule.Seeders;

namespace TimeCapsule.Extensions
{
    public static class HostExtensions
    {
        public async static Task UseSeeders(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var seedManager = scope.ServiceProvider.GetRequiredService<SeedManager>();
                await seedManager.Seed();
            }
        }
    }

}
