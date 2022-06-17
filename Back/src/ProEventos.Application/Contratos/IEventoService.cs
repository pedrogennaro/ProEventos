using System.Threading.Tasks;
using ProEventos.Application.Dtos;
using ProEventos.Persistence.Pagination;

namespace ProEventos.Application.Contratos
{
    public interface IEventoService
    {
        Task<EventoDto> AddEventos(int userId, EventoDto model);
        Task<EventoDto> UpdateEvento(int userId, int eventoId, EventoDto model);
        Task<bool> DeleteEvento(int userId, int eventoId);

        Task<PageList<EventoDto>> GetAllEventosAsync(int userId, PageParams pageParams, bool includePalestrantes = false);
        //Task<EventoDto[]> GetAllEventosByTemaAsync(int userId, string tema, bool includePalestrantes = false);
        Task<EventoDto> GetEventoByIdAsync(int userId, int eventoId, bool includePalestrantes = false);
    }
}