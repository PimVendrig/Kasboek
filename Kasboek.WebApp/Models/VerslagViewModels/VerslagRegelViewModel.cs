using System.Collections.Generic;

namespace Kasboek.WebApp.Models.VerslagViewModels
{
    public class VerslagRegelViewModel
    {
        public int Id { get; set; }
        public string Tekst { get; set; }
        public List<decimal> Bedragen { get; set; }
        public List<decimal?> BedragenPerMaand { get; set; }
    }
}
