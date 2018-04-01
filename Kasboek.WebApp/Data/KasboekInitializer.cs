using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Kasboek.WebApp.Data
{
    public static class KasboekInitializer
    {

        public static void Initialize(KasboekDbContext context)
        {
            Migrate(context);
            Seed(context);
        }

        public static void Migrate(DbContext context)
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }

        public static void Seed(KasboekDbContext context)
        {
            //TODO: Enkele standaard waarden instellen
        }

    }
}
