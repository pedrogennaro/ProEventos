using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProEventos.Domain;
using ProEventos.Persistence.Contextos;
using ProEventos.Persistence.Contratos;
using ProEventos.Persistence.Pagination;

namespace ProEventos.Persistence
{
    public class EventoPersistence : IEventoPersistence
    {
        private readonly ProEventosContext _context;
        public EventoPersistence(ProEventosContext context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public async Task<PageList<Evento>> GetAllEventosAsync(int userId, PageParams pageParams, bool includePalestrantes = false)
        {
            IQueryable<Evento> query = _context.Eventos.Include(e => e.Lotes).Include(e => e.RedesSociais);

            if(includePalestrantes){
                query = query.Include(e => e.PalestranteEventos).ThenInclude(pe => pe.Palestrante);
            }

            query = query.OrderBy(e => e.Id).Where(e => (e.Tema.ToLower().Contains(pageParams.Term.ToLower()) ||
                                                         e.Local.ToLower().Contains(pageParams.Term.ToLower())) &&
                                                         e.UserId == userId);

            return await PageList<Evento>.CreateAsync(query, pageParams.PageNumber, pageParams.pageSize);
        }

        // Método SEM PAGINAÇÃO

        // public async Task<Evento[]> GetAllEventosByTemaAsync(int userId, string tema, bool includePalestrantes = false)
        // {
        //     IQueryable<Evento> query = _context.Eventos.Include(e => e.Lotes).Include(e => e.RedesSociais);

        //     if(includePalestrantes){
        //         query = query.Include(e => e.PalestranteEventos).ThenInclude(pe => pe.Palestrante);
        //     }

        //     query = query.OrderBy(e => e.Id).Where(e => e.Tema.ToLower().Contains(tema.ToLower()) &&
        //                                                 e.UserId == userId);

        //     return await query.ToArrayAsync();
        // }

        public async Task<Evento> GetEventoByIdAsync(int userId, int eventoId, bool includePalestrantes = false)
        {
            IQueryable<Evento> query = _context.Eventos.Include(e => e.Lotes).Include(e => e.RedesSociais);

            if(includePalestrantes){
                query = query.Include(e => e.PalestranteEventos).ThenInclude(pe => pe.Palestrante);
            }

            query = query.OrderBy(e => e.Id).Where(e => e.Id == eventoId &&
                                                        e.UserId == userId);

            return await query.FirstOrDefaultAsync();
        }

    }
}