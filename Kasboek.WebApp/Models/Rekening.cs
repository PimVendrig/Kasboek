using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Kasboek.WebApp.Models
{
    [DebuggerDisplay("{Naam}")]
    public class Rekening
    {

        public int RekeningId { get; set; }

        [Required]
        [StringLength(100)]
        public string Naam { get; set; }

        [StringLength(100)]
        public string Rekeningnummer { get; set; }

        [Display(Name = "Is eigen rekening")]
        public bool IsEigenRekening { get; set; }

        [InverseProperty("VanRekening")]
        public List<Transactie> VanTransacties { get; set; }

        [InverseProperty("NaarRekening")]
        public List<Transactie> NaarTransacties { get; set; }

        [Display(Name = "Standaard categorie")]
        public int? StandaardCategorieId { get; set; }
        [Display(Name = "Standaard categorie")]
        [ForeignKey("StandaardCategorieId")]
        public Categorie StandaardCategorie { get; set; }

    }
}
