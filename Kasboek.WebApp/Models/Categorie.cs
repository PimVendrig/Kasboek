using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Kasboek.WebApp.Models
{

    [DebuggerDisplay("{Omschrijving}")]
    public class Categorie
    {

        public int CategorieId { get; set; }

        [Required]
        [StringLength(100)]
        public string Omschrijving { get; set; }

        public List<Transactie> Transacties { get; set; }

    }

}
