﻿using System;
using System.Collections.Generic;

namespace Kasboek.WebApp.Models.VerslagViewModels
{
    public class BalansViewModel
    {
        public List<DateTime> Datums { get; set; }
        public List<VerslagRegelViewModel> VerslagRegels { get; set; }
        public List<decimal> TotaalRegel { get; set; }
    }
}
