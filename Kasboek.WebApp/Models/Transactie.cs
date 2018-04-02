using System;
using System.Collections.Generic;
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

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(1000)]
        public string Omschrijving { get; set; }

        public int VanRekeningId { get; set; }
        [Required]
        [ForeignKey("VanRekeningId")]
        public Rekening VanRekening { get; set; }

        public int NaarRekeningId { get; set; }
        [Required]
        [ForeignKey("NaarRekeningId")]
        public Rekening NaarRekening { get; set; }

        public List<TransactieLabel> TransactieLabels { get; set; }

    }
}
