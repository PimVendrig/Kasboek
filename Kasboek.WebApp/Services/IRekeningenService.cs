using Kasboek.WebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Services
{
    public interface IRekeningenService : ICrudService<Rekening>
    {
        Task<IList<KeyValuePair<int, string>>> GetSelectListAsync();
    }
}
