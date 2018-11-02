using System;
using System.Collections.Generic;

namespace Kasboek.WebApp.Models.VerslagViewModels
{
    public class ResultatenrekeningViewModel
    {
        public List<Tuple<DateTime, DateTime>> Periodes { get; set; }
        public List<VerslagRegelViewModel> VerslagRegels { get; set; }
        public List<decimal> TotaalRegel { get; set; }
        public List<decimal?> TotaalPerMaandRegel { get; set; }
    }
}
