using Kasboek.WebApp.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Linq;

namespace Kasboek.WebApp.Services
{

    public static class SelectListService
    {

        public static SelectList GetCategorieen(KasboekDbContext context, int? selectedCategorieId = null)
        {
            var categorieen = context.Categorieen
                .OrderBy(c => c.Omschrijving)
                .Select(c => new { Value = c.CategorieId, Text = c.Omschrijving });

            return GetSelectList(categorieen, selectedCategorieId);
        }

        public static SelectList GetRekeningen(KasboekDbContext context, int? selectedRekeningId = null)
        {
            var rekeningen = context.Rekeningen
                .Include(r => r.StandaardCategorie)
                .OrderByDescending(r => r.IsEigenRekening)
                .ThenBy(r => r.Naam)
                .Select(r => new { Value = r.RekeningId, Text = r.Naam });

            return GetSelectList(rekeningen, selectedRekeningId);
        }

        private static SelectList GetSelectList(IEnumerable items, int? selectedValue)
        {
            return new SelectList(items, "Value", "Text", selectedValue);
        }

    }
}
