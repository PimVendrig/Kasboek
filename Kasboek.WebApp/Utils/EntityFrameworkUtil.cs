using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace Kasboek.WebApp.Utils
{
    public static class EntityFrameworkUtil
    {

        public static bool HasUniqueIndexViolation(DbUpdateException ex, string indexName)
        {
            return ex?.InnerException is SqlException sqlEx
                && (sqlEx.Number == 2601 || sqlEx.Number == 2627)
                && sqlEx.Message.Contains($"'{indexName}'");
        }

    }
}
