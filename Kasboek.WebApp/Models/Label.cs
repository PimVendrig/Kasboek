using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kasboek.WebApp.Models
{
    public class Label
    {

        public int LabelId { get; set; }

        [Required]
        [StringLength(100)]
        public string Omschrijving { get; set; }

        public List<TransactieLabel> TransactieLabels { get; set; }

        public List<RekeningLabel> RekeningLabels { get; set; }

    }
}
