using System;
using System.Threading.Tasks;
using AutoMapper;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;
using ProEventos.Persistence.Pagination;

namespace ProEventos.Application
{
    public class EventoService : IEventoService
    {
        private readonly IGeralPersistence _geralPersistence;
        private readonly IEventoPersistence _eventoPersistence;
        private readonly IMapper _mapper;
        public EventoService(IGeralPersistence geralPersistence, IEventoPersistence eventoPersistence, IMapper mapper)
        {
            _geralPersistence = geralPersistence;
            _eventoPersistence = eventoPersistence;
            _mapper = mapper;
        }
        public async Task<EventoDto> AddEventos(int userId, EventoDto model)
        {
            try
            {
                var evento = _mapper.Map<Evento>(model);
                evento.UserId = userId;
                
                _geralPersistence.Add<Evento>(evento);
                if(await _geralPersistence.SaveChangesAsync())
                {
                    var eventoRetorno = await _eventoPersistence.GetEventoByIdAsync(userId, evento.Id, false);
                    
                    return _mapper.Map<EventoDto>(eventoRetorno);
                }
                return null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<EventoDto> UpdateEvento(int userId, int eventoId, EventoDto model)
        {
            try
            {
                var evento = await _eventoPersistence.GetEventoByIdAsync(userId, eventoId, false);
                if(evento == null) return null;

                model.Id = evento.Id;
                model.UserId = userId;

                _mapper.Map(model, evento);

                _geralPersistence.Update<Evento>(evento);
                if(await _geralPersistence.SaveChangesAsync())
                {
                    var eventoRetorno = await _eventoPersistence.GetEventoByIdAsync(userId, evento.Id, false);

                    return _mapper.Map<EventoDto>(eventoRetorno);
                }
                return null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<bool> DeleteEvento(int userId, int eventoId)
        {
            try
            {
                var evento = await _eventoPersistence.GetEventoByIdAsync(userId, eventoId, false);
                if(evento == null) throw new Exception("Evento para delete não foi encontrado.");

                _geralPersistence.Delete<Evento>(evento);
                return await _geralPersistence.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<PageList<EventoDto>> GetAllEventosAsync(int userId, PageParams pageParams, bool includePalestrantes = false)
        {
            try
            {
                var eventos = await _eventoPersistence.GetAllEventosAsync(userId, pageParams, includePalestrantes);
                if(eventos == null) return null;

                var result = _mapper.Map<PageList<EventoDto>>(eventos);

                result.CurrentPage = eventos.CurrentPage;
                result.TotalPages = eventos.TotalPages;
                result.PageSize = eventos.PageSize;
                result.TotalCount = eventos.TotalCount;

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // Método SEM PAGINAÇÃO

        // public async Task<EventoDto[]> GetAllEventosByTemaAsync(int userId, string tema, bool includePalestrantes = false)
        // {
        //     try
        //     {
        //         var eventos = await _eventoPersistence.GetAllEventosByTemaAsync(userId, tema, includePalestrantes);
        //         if(eventos == null) return null;

        //         var result = _mapper.Map<EventoDto[]>(eventos);

        //         return result;
        //     }
        //     catch (Exception e)
        //     {
        //         throw new Exception(e.Message);
        //     }
        // }

        public async Task<EventoDto> GetEventoByIdAsync(int userId, int eventoId, bool includePalestrantes = false)
        {
            try
            {
                var evento = await _eventoPersistence.GetEventoByIdAsync(userId, eventoId, includePalestrantes);
                if(evento == null) return null;

                var result = _mapper.Map<EventoDto>(evento);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}