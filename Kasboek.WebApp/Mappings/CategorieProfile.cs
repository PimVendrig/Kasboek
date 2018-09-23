using AutoMapper;
using Kasboek.WebApp.Models;
using Kasboek.WebApp.Models.CategorieenViewModels;

namespace Kasboek.WebApp.Mappings
{
    public class CategorieProfile : Profile
    {
        public CategorieProfile()
        {
            CreateMap<Categorie, CategorieViewModel>();
            CreateMap<MergeViewModel, Categorie>();
        }
    }
}
