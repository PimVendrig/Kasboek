using Kasboek.WebApp.Models;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Services
{

    public interface ICategorieenService : ICrudService<Categorie>
    {

        Task<bool> ExistsAsync(int id);
        Task<bool> IsOmschrijvingInUseAsync(Categorie categorie);

    }
}
