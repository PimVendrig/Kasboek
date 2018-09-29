using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kasboek.WebApp.Models
{
    public class Instellingen
    {

        public int InstellingenId { get; set; }

        [Display(Name = "Standaard van rekening")]
        public int? StandaardVanRekeningId { get; set; }
        [Display(Name = "Standaard van rekening")]
        [ForeignKey("StandaardVanRekeningId")]
        public Rekening StandaardVanRekening { get; set; }

        [Display(Name = "Transactie meteen bewerken")]
        public bool TransactieMeteenBewerken { get; set; }

        [Display(Name = "Portemonnee rekening")]
        public int? PortemonneeRekeningId { get; set; }
        [Display(Name = "Portemonnee rekening")]
        [ForeignKey("PortemonneeRekeningId")]
        public Rekening PortemonneeRekening { get; set; }
    }
}
