using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kasboek.WebApp.Models
{

    public class Categorie
    {

        public int CategorieId { get; set; }

        [Required]
        [StringLength(100)]
        public string Omschrijving { get; set; }

        public List<Rekening> Rekeningen { get; set; }

        public List<Transactie> Transacties { get; set; }

    }

}
