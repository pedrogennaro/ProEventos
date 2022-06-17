using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProEventos.Domain;
using ProEventos.Persistence.Pagination;

namespace ProEventos.Persistence.Contratos
{
    public interface IEventoPersistence
    {
        //Task<PageList<Evento>> GetAllEventosByTemaAsync(int userId, PageParams pageParams, string tema, bool includePalestrantes = false);
        Task<PageList<Evento>> GetAllEventosAsync(int userId, PageParams pageParams, bool includePalestrantes = false);
        Task<Evento> GetEventoByIdAsync(int userId, int eventoId, bool includePalestrantes = false);
    }
}