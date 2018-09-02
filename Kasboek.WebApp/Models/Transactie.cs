using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kasboek.WebApp.Models
{
    public class Transactie
    {

        public int TransactieId { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime Datum { get; set; }

        [DataType(DataType.Currency)]
        public decimal Bedrag { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DataType(DataType.MultilineText)]
        [StringLength(1000)]
        public string Omschrijving { get; set; }

        [Display(Name = "Van rekening")]
        public int VanRekeningId { get; set; }
        [Display(Name = "Van rekening")]
        [ForeignKey("VanRekeningId")]
        public Rekening VanRekening { get; set; }

        [Display(Name = "Naar rekening")]
        public int NaarRekeningId { get; set; }
        [Display(Name = "Naar rekening")]
        [ForeignKey("NaarRekeningId")]
        public Rekening NaarRekening { get; set; }

        [Display(Name = "Categorie")]
        public int? CategorieId { get; set; }
        public Categorie Categorie { get; set; }

    }
}
