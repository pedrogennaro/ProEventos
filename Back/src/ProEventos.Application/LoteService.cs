using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;

namespace ProEventos.Application
{
    public class LoteService : ILoteService
    {
        private readonly IGeralPersistence _geralPersistence;
        private readonly ILotePersistence _lotePersistence;
        private readonly IMapper _mapper;
        public LoteService(IGeralPersistence geralPersistence, ILotePersistence lotePersistence, IMapper mapper)
        {
            _geralPersistence = geralPersistence;
            _lotePersistence = lotePersistence;
            _mapper = mapper;
        }
        public async Task AddLote(int eventoId, LoteDto model)
        {
            try
            {
                var lote = _mapper.Map<Lote>(model);
                lote.EventoId = eventoId;
                
                _geralPersistence.Add<Lote>(lote);

                await _geralPersistence.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<LoteDto[]> SaveLotes(int eventoId, LoteDto[] models)
        {
            try
            {
                var lotes = await _lotePersistence.GetLotesByEventoIdAsync(eventoId);
                if(lotes == null) return null;
                
                foreach (var model in models)
                {
                    if(model.Id == 0){
                        await AddLote(eventoId, model);
                    }
                    else
                    {
                        var lote = lotes.FirstOrDefault(lote => lote.Id == model.Id);
                        model.EventoId = eventoId;

                        _mapper.Map(model, lote);

                        _geralPersistence.Update<Lote>(lote);
                        await _geralPersistence.SaveChangesAsync();
                    }
                    
                }

                var loteRetorno = await _lotePersistence.GetLotesByEventoIdAsync(eventoId);

                return _mapper.Map<LoteDto[]>(loteRetorno);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<bool> DeleteLote(int eventoId, int id)
        {
            try
            {
                var lote = await _lotePersistence.GetLoteByIdsAsync(eventoId, id);
                if(lote == null) throw new Exception("Lote para delete não foi encontrado.");

                _geralPersistence.Delete<Lote>(lote);
                return await _geralPersistence.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<LoteDto[]> GetLotesByEventoIdAsync(int eventoId)
        {
            try
            {
                var lotes = await _lotePersistence.GetLotesByEventoIdAsync(eventoId);
                if(lotes == null) return null;

                var result = _mapper.Map<LoteDto[]>(lotes);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<LoteDto> GetLoteByIdsAsync(int eventoId, int id)
        {
            try
            {
                var evento = await _lotePersistence.GetLoteByIdsAsync(eventoId, id);
                if(evento == null) return null;

                var result = _mapper.Map<LoteDto>(evento);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}