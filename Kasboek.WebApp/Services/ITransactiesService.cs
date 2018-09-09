using Kasboek.WebApp.Models;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Services
{
    public interface ITransactiesService : ICrudService<Transactie>
    {
        Task<bool> ExistsAsync(int id);
    }
}
