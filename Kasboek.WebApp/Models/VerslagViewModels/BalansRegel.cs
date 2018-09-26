using System.Collections.Generic;

namespace Kasboek.WebApp.Models.VerslagViewModels
{
    public class BalansRegel
    {
        public int RekeningId { get; set; }
        public string RekeningNaam { get; set; }
        public List<decimal> Saldos { get; set; }
    }
}
