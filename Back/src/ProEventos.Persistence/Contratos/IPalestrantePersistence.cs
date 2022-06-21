using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProEventos.Domain;
using ProEventos.Persistence.Pagination;

namespace ProEventos.Persistence.Contratos
{
    public interface IPalestrantePersistence : IGeralPersistence
    {
        //Task<Palestrante[]> GetAllPalestrantesByNomeAsync(string nome, bool includeEventos = false);
        Task<PageList<Palestrante>> GetAllPalestrantesAsync(PageParams pageParams, bool includeEventos = false);
        Task<Palestrante> GetPalestranteByUserIdAsync(int userId, bool includeEventos = false);
    }
}