using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kasboek.WebApp.Models
{
    public class Transactie
    {

        public int TransactieId { get; set; }

        [Column(TypeName = "date")]
        public DateTime Datum { get; set; }

        public decimal Bedrag { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(1000)]
        public string Omschrijving { get; set; }

        //TODO: Als verplicht markeren. Werkt nu niet goed samen met Add-Migration en circular reference
        public int? VanRekeningId { get; set; }
        [ForeignKey("VanRekeningId")]
        public Rekening VanRekening { get; set; }

        //TODO: Als verplicht markeren. Werkt nu niet goed samen met Add-Migration en circular reference
        public int? NaarRekeningId { get; set; }
        [ForeignKey("NaarRekeningId")]
        public Rekening NaarRekening { get; set; }

    }
}
