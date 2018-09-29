using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kasboek.WebApp.Models.ImporterenViewModels
{
    public class UploadViewModel
    {

        [Required]
        public IFormFile Bestand { get; set; }

        [Display(Name = "Resultaat")]
        public string ResultMessage { get; set; }
        public bool ShowResultMessage => !string.IsNullOrEmpty(ResultMessage);

        public string Action { get; set; }

        public Dictionary<string, string> NewTransactiesLinkParameters { get; set; }
        public Dictionary<string, string> NewRekeningenLinkParameters { get; set; }
        public bool ShowNewDataLinks => NewTransactiesLinkParameters != null && NewRekeningenLinkParameters != null;
    }
}
