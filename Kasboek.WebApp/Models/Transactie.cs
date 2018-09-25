using Kasboek.WebApp.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kasboek.WebApp.Models
{
    public class Transactie
    {

        public int TransactieId { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:ddd d/M/yyyy}")]
        public DateTime Datum { get; set; }

        [Range(0.0, 999999999999999.99)]//Max van decimal in db is 9999999999999999.99, maar vanwege afronding naar 1E16 iets lager gezet.
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Bedrag { get; set; }

        [DisplayFormat(NullDisplayText = "- Geen omschrijving -")]
        [DataType(DataType.MultilineText)]
        [StringLength(500)]
        public string Omschrijving { get; set; }

        [Display(Name = "Van rekening")]
        public int VanRekeningId { get; set; }
        [Display(Name = "Van rekening")]
        [ForeignKey("VanRekeningId")]
        public Rekening VanRekening { get; set; }

        [Display(Name = "Naar rekening")]
        [Unlike(nameof(VanRekeningId), "Van rekening")]
        public int NaarRekeningId { get; set; }
        [Display(Name = "Naar rekening")]
        [ForeignKey("NaarRekeningId")]
        public Rekening NaarRekening { get; set; }

        [Display(Name = "Categorie")]
        public int? CategorieId { get; set; }
        [ForeignKey("CategorieId")]
        public Categorie Categorie { get; set; }

    }
}
