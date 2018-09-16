using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Kasboek.WebApp.Models.ImporterenViewModels
{
    public class RekeningenViewModel
    {

        [Required]
        [Display(Name = "Rekeningen")]
        public IFormFile UploadRekeningen { get; set; }

        [Display(Name = "Resultaat")]
        public string ResultMessage { get; set; }
        public bool ShowResultMessage => !string.IsNullOrEmpty(ResultMessage);

    }
}
