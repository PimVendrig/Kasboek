using Kasboek.WebApp.Models;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Services
{
    public interface IInstellingenService : ICrudService<Instellingen>
    {
        Task<Instellingen> GetRawSingleAsync();
        Task<Instellingen> GetSingleAsync();
        Task<int> GetIdAsync();
    }
}
