using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kasboek.WebApp.Models
{
    public class Rekening
    {

        public int RekeningId { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Rekeningnummer")]
        [StringLength(100)]
        public string RekeningNummer { get; set; }

        [Required]
        [StringLength(100)]
        public string Naam { get; set; }

        [Display(Name = "Is eigen rekening")]
        public bool IsEigenRekening { get; set; }

        [InverseProperty("VanRekening")]
        public List<Transactie> VanTransacties { get; set; }

        [InverseProperty("NaarRekening")]
        public List<Transactie> NaarTransacties { get; set; }

        public List<RekeningLabel> RekeningLabels { get; set; }

        [Display(Name = "Standaard categorie")]
        public int? StandaardCategorieId { get; set; }
        [Display(Name = "Standaard categorie")]
        [ForeignKey("StandaardCategorieId")]
        public Categorie StandaardCategorie { get; set; }

    }
}
