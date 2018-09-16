using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kasboek.WebApp.Models.RekeningenViewModels
{
    public class MergeViewModel
    {
        public List<int> RekeningIds { get; set; }

        [Required]
        [StringLength(100)]
        public string Naam { get; set; }

        [StringLength(100)]
        public string Rekeningnummer { get; set; }

        [Display(Name = "Is eigen rekening")]
        public bool IsEigenRekening { get; set; }

        [Display(Name = "Standaard categorie")]
        public int? StandaardCategorieId { get; set; }

        [DataType(DataType.Currency)]
        public decimal Saldo { get; set; }

        public List<int> CategorieIds { get; set; }
    }
}
