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
    public class RedeSocialService : IRedeSocialService
    {
        private readonly IRedeSocialPersistence _redeSocialPersistence;
        private readonly IMapper _mapper;
        public RedeSocialService(IRedeSocialPersistence redeSocialPersistence, IMapper mapper)
        {
            _redeSocialPersistence = redeSocialPersistence;
            _mapper = mapper;
        }
        public async Task AddRedeSocial(int id, RedeSocialDto model, bool isEvento)
        {
            try
            {
                var redeSocial = _mapper.Map<RedeSocial>(model);
                if(isEvento){
                    redeSocial.EventoId = id;
                    redeSocial.PalestranteId = null;
                } else{
                    redeSocial.EventoId = null;
                    redeSocial.PalestranteId = id;
                }
                _redeSocialPersistence.Add<RedeSocial>(redeSocial);

                await _redeSocialPersistence.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<RedeSocialDto[]> SaveByEvento(int eventoId, RedeSocialDto[] models)
        {
            try
            {
                var redesSociais = await _redeSocialPersistence.GetAllByEventoIdAsync(eventoId);
                if(redesSociais == null) return null;
                
                foreach (var model in models)
                {
                    if(model.Id == 0){
                        await AddRedeSocial(eventoId, model, true);
                    }
                    else
                    {
                        var redeSocial = redesSociais.FirstOrDefault(lote => lote.Id == model.Id);
                        model.EventoId = eventoId;

                        _mapper.Map(model, redeSocial);

                        _redeSocialPersistence.Update<RedeSocial>(redeSocial);
                        await _redeSocialPersistence.SaveChangesAsync();
                    }
                    
                }

                var redeSocialRetorno = await _redeSocialPersistence.GetAllByEventoIdAsync(eventoId);

                return _mapper.Map<RedeSocialDto[]>(redeSocialRetorno);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<RedeSocialDto[]> SaveByPalestrante(int palestranteId, RedeSocialDto[] models)
        {
            try
            {
                var redesSociais = await _redeSocialPersistence.GetAllByPalestranteIdAsync(palestranteId);
                if(redesSociais == null) return null;
                
                foreach (var model in models)
                {
                    if(model.Id == 0){
                        await AddRedeSocial(palestranteId, model, false);
                    }
                    else
                    {
                        var redeSocial = redesSociais.FirstOrDefault(lote => lote.Id == model.Id);
                        model.PalestranteId = palestranteId;

                        _mapper.Map(model, redeSocial);

                        _redeSocialPersistence.Update<RedeSocial>(redeSocial);
                        await _redeSocialPersistence.SaveChangesAsync();
                    }
                    
                }

                var redeSocialRetorno = await _redeSocialPersistence.GetAllByPalestranteIdAsync(palestranteId);

                return _mapper.Map<RedeSocialDto[]>(redeSocialRetorno);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<bool> DeleteByEvento(int eventoId, int redeSocialId)
        {
            try
            {
                var redeSocial = await _redeSocialPersistence.GetRedeSocialEventoByIdsAsync(eventoId, redeSocialId);
                if(redeSocial == null) throw new Exception("Rede Social por Evento para delete não foi encontrado.");

                _redeSocialPersistence.Delete<RedeSocial>(redeSocial);
                return await _redeSocialPersistence.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> DeleteByPalestrante(int palestranteId, int redeSocialId)
        {
            try
            {
                var redeSocial = await _redeSocialPersistence.GetRedeSocialPalestranteByIdsAsync(palestranteId, redeSocialId);
                if(redeSocial == null) throw new Exception("Rede Social por Palestrante para delete não foi encontrado.");

                _redeSocialPersistence.Delete<RedeSocial>(redeSocial);
                return await _redeSocialPersistence.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<RedeSocialDto[]> GetAllByEventoIdAsync(int eventoId)
        {
            try
            {
                var redeSocial = await _redeSocialPersistence.GetAllByEventoIdAsync(eventoId);
                if(redeSocial == null) return null;

                var result = _mapper.Map<RedeSocialDto[]>(redeSocial);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<RedeSocialDto[]> GetAllByPalestranteIdAsync(int palestranteId)
        {
            try
            {
                var redeSocial = await _redeSocialPersistence.GetAllByPalestranteIdAsync(palestranteId);
                if(redeSocial == null) return null;

                var result = _mapper.Map<RedeSocialDto[]>(redeSocial);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<RedeSocialDto> GetRedeSocialEventoByIdsAsync(int eventoId, int redeSocialId)
        {
            try
            {
                var evento = await _redeSocialPersistence.GetRedeSocialEventoByIdsAsync(eventoId, redeSocialId);
                if(evento == null) return null;

                var result = _mapper.Map<RedeSocialDto>(evento);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<RedeSocialDto> GetRedeSocialPalestranteByIdsAsync(int palestranteId, int redeSocialId)
        {
            try
            {
                var evento = await _redeSocialPersistence.GetRedeSocialPalestranteByIdsAsync(palestranteId, redeSocialId);
                if(evento == null) return null;

                var result = _mapper.Map<RedeSocialDto>(evento);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}