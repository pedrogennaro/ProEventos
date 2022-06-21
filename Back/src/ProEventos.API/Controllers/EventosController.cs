using System;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Application.Contratos;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ProEventos.Application.Dtos;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using ProEventos.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using ProEventos.Persistence.Pagination;
using ProEventos.API.Helpers;

namespace ProEventos.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {
        private readonly IEventoService _eventoService;
        private readonly IUtil _util;
        private readonly IAccountService _accountService;
        private readonly string _destino = "images";

        public EventosController(IEventoService eventoService,
                                    IUtil util,
                                    IAccountService accountService)
        {
            _eventoService = eventoService;
            _util = util;
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]PageParams pageParams)
        {
            try
            {

                var eventos = await _eventoService.GetAllEventosAsync(User.GetUserId(), pageParams, true);
                if (eventos == null) return NoContent();

                Response.AddPagination(eventos.CurrentPage, eventos.PageSize, eventos.TotalCount, eventos.TotalPages);

                return Ok(eventos);
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar recuperar eventos. Erro {e.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var evento = await _eventoService.GetEventoByIdAsync(User.GetUserId(), id, true);
                if (evento == null) return NoContent();

                return Ok(evento);
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar recuperar eventos. Erro {e.Message}");
            }
        }

        // [HttpGet("tema/{tema}")]
        // public async Task<IActionResult> GetByTema(string tema)
        // {
        //     try
        //     {
        //         var evento = await _eventoService.GetAllEventosByTemaAsync(User.GetUserId(), tema, true);
        //         if (evento == null) return NoContent();

        //         return Ok(evento);
        //     }
        //     catch (Exception e)
        //     {
        //         return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar recuperar eventos. Erro {e.Message}");
        //     }
        // }

        [HttpPost]
        public async Task<IActionResult> Post(EventoDto model)
        {
            try
            {
                var evento = await _eventoService.AddEventos(User.GetUserId(), model);
                if (evento == null) return NoContent();

                return Ok(evento);
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar adicionar evento. Erro {e.Message}");
            }
        }

        [HttpPost("upload/{eventoId}")]
        public async Task<IActionResult> Upload(int eventoId)
        {
            try
            {
                var evento = await _eventoService.GetEventoByIdAsync(User.GetUserId(), eventoId, true);
                if (evento == null) return NoContent();

                var file = Request.Form.Files[0];
                if(file.Length > 0){
                    _util.DeleteImage(evento.ImagemURL, _destino);

                    evento.ImagemURL = await _util.SaveImage(file, _destino);
                }

                var eventoRetorno = await _eventoService.UpdateEvento(User.GetUserId(), eventoId, evento);

                return Ok(eventoRetorno);
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar realizar upload de foto do evento. Erro {e.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, EventoDto model)
        {
            try
            {
                var evento = await _eventoService.UpdateEvento(User.GetUserId(), id, model);
                if (evento == null) return NoContent();

                return Ok(evento);
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar atualizar evento. Erro {e.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var evento = await _eventoService.GetEventoByIdAsync(User.GetUserId(), id, true);
                if (evento == null) return NoContent();

                if(await _eventoService.DeleteEvento(User.GetUserId(), id)){
                    _util.DeleteImage(evento.ImagemURL, _destino);
                    return Ok(new {message = "Deletado"});
                } else{
                    throw new Exception("Ocorreu um erro ao tentar deletar o evento");
                }
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar excluir evento. Erro {e.Message}");
            }
        }
    }
}
