using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kasboek.WebApp.Models.CategorieenViewModels
{
    public class MergeViewModel
    {
        public List<int> CategorieIds { get; set; }

        [Required]
        [StringLength(100)]
        public string Omschrijving { get; set; }

        [DataType(DataType.Currency)]
        public decimal Saldo { get; set; }
    }
}
