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
    public class PalestranteService : IPalestranteService
    {
        private readonly IPalestrantePersistence _palestrantePersistence;
        private readonly IMapper _mapper;
        public PalestranteService(IPalestrantePersistence palestrantePersistence, IMapper mapper)
        {
            _palestrantePersistence = palestrantePersistence;
            _mapper = mapper;
        }
        public async Task<PalestranteDto> AddPalestrantes(int userId, PalestranteAddDto model)
        {
            try
            {
                var palestrante = _mapper.Map<Palestrante>(model);
                palestrante.UserId = userId;
                
                _palestrantePersistence.Add<Palestrante>(palestrante);
                if(await _palestrantePersistence.SaveChangesAsync())
                {
                    var palestranteRetorno = await _palestrantePersistence.GetPalestranteByUserIdAsync(userId, false);
                    
                    return _mapper.Map<PalestranteDto>(palestranteRetorno);
                }
                return null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<PalestranteDto> UpdatePalestrante(int userId, PalestranteUpdateDto model)
        {
            try
            {
                var palestrante = await _palestrantePersistence.GetPalestranteByUserIdAsync(userId, false);
                if(palestrante == null) return null;

                model.Id = palestrante.Id;
                model.UserId = userId;

                _mapper.Map(model, palestrante);

                _palestrantePersistence.Update<Palestrante>(palestrante);
                if(await _palestrantePersistence.SaveChangesAsync())
                {
                    var palestranteRetorno = await _palestrantePersistence.GetPalestranteByUserIdAsync(userId, false);

                    return _mapper.Map<PalestranteDto>(palestranteRetorno);
                }
                return null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<PageList<PalestranteDto>> GetAllPalestrantesAsync(PageParams pageParams, bool includeEventos = false)
        {
            try
            {
                var palestrantes = await _palestrantePersistence.GetAllPalestrantesAsync(pageParams, includeEventos);
                if(palestrantes == null) return null;

                var result = _mapper.Map<PageList<PalestranteDto>>(palestrantes);

                result.CurrentPage = palestrantes.CurrentPage;
                result.TotalPages = palestrantes.TotalPages;
                result.PageSize = palestrantes.PageSize;
                result.TotalCount = palestrantes.TotalCount;

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<PalestranteDto> GetPalestranteByIdAsync(int userId, bool includeEventos = false)
        {
            try
            {
                var evento = await _palestrantePersistence.GetPalestranteByUserIdAsync(userId, includeEventos);
                if(evento == null) return null;

                var result = _mapper.Map<PalestranteDto>(evento);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}