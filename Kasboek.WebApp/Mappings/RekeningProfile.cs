using AutoMapper;
using Kasboek.WebApp.Models;
using Kasboek.WebApp.Models.RekeningenViewModels;

namespace Kasboek.WebApp.Mappings
{
    public class RekeningProfile : Profile
    {
        public RekeningProfile()
        {
            CreateMap<Rekening, RekeningViewModel>();
            CreateMap<MergeViewModel, Rekening>();
        }
    }
}
