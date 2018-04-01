using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kasboek.WebApp.Models
{
    public class Rekening
    {

        public int RekeningId { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(100)]
        public string RekeningNummer { get; set; }

        [Required]
        [StringLength(100)]
        public string Naam { get; set; }

        public bool IsEigenRekening { get; set; }

        [InverseProperty("VanRekening")]
        public List<Transactie> VanTransacties { get; set; }

        [InverseProperty("NaarRekening")]
        public List<Transactie> NaarTransacties { get; set; }

        public List<RekeningLabel> RekeningLabels { get; set; }

    }
}
