using System;
using System.Threading.Tasks;
using AutoMapper;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;

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
        public async Task<EventoDto> AddEventos(EventoDto model)
        {
            try
            {
                var evento = _mapper.Map<Evento>(model);
                
                _geralPersistence.Add<Evento>(evento);
                if(await _geralPersistence.SaveChangesAsync())
                {
                    var eventoRetorno = await _eventoPersistence.GetEventoByIdAsync(evento.Id, false);
                    
                    return _mapper.Map<EventoDto>(eventoRetorno);
                }
                return null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<EventoDto> UpdateEvento(int eventoId, EventoDto model)
        {
            try
            {
                var evento = await _eventoPersistence.GetEventoByIdAsync(eventoId, false);
                if(evento == null) return null;

                model.Id = evento.Id;

                _mapper.Map(model, evento);

                _geralPersistence.Update<Evento>(evento);
                if(await _geralPersistence.SaveChangesAsync())
                {
                    var eventoRetorno = await _eventoPersistence.GetEventoByIdAsync(evento.Id, false);

                    return _mapper.Map<EventoDto>(eventoRetorno);
                }
                return null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<bool> DeleteEvento(int eventoId)
        {
            try
            {
                var evento = await _eventoPersistence.GetEventoByIdAsync(eventoId, false);
                if(evento == null) throw new Exception("Evento para delete não foi encontrado.");

                _geralPersistence.Delete<Evento>(evento);
                return await _geralPersistence.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<EventoDto[]> GetAllEventosAsync(bool includePalestrantes = false)
        {
            try
            {
                var eventos = await _eventoPersistence.GetAllEventosAsync(includePalestrantes);
                if(eventos == null) return null;

                var result = _mapper.Map<EventoDto[]>(eventos);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<EventoDto[]> GetAllEventosByTemaAsync(string tema, bool includePalestrantes = false)
        {
            try
            {
                var eventos = await _eventoPersistence.GetAllEventosByTemaAsync(tema, includePalestrantes);
                if(eventos == null) return null;

                var result = _mapper.Map<EventoDto[]>(eventos);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<EventoDto> GetEventoByIdAsync(int eventoId, bool includePalestrantes = false)
        {
            try
            {
                var evento = await _eventoPersistence.GetEventoByIdAsync(eventoId, includePalestrantes);
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