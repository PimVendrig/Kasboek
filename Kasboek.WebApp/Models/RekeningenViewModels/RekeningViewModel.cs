using System.ComponentModel.DataAnnotations;

namespace Kasboek.WebApp.Models.RekeningenViewModels
{
    public class RekeningViewModel
    {
        public int RekeningId { get; set; }

        public bool Selected { get; set; }

        public string Naam { get; set; }

        public string Rekeningnummer { get; set; }

        [Display(Name = "Is eigen rekening")]
        public bool IsEigenRekening { get; set; }

        [Display(Name = "Standaard categorie")]
        public string StandaardCategorieOmschrijving { get; set; }

    }
}
