﻿using System;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Application.Contratos;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ProEventos.Application.Dtos;

namespace ProEventos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LotesController : ControllerBase
    {
        private readonly ILoteService _loteService;
        public LotesController(ILoteService loteService)
        {
            _loteService = loteService;
        }

        [HttpGet("{eventoId}")]
        public async Task<IActionResult> Get(int eventoId)
        {
            try
            {
                var lotes = await _loteService.GetLotesByEventoIdAsync(eventoId);
                if (lotes == null) return NoContent();

                return Ok(lotes);
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar recuperar lotes. Erro {e.Message}");
            }
        }

        [HttpPut("{eventoId}")]
        public async Task<IActionResult> SaveLotes(int eventoId, LoteDto[] models)
        {
            try
            {
                var lotes = await _loteService.SaveLotes(eventoId, models);
                if (lotes == null) return NoContent();

                return Ok(lotes);
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar salvar os lotes. Erro {e.Message}");
            }
        }

        [HttpDelete("{eventoId}/{loteId}")]
        public async Task<IActionResult> Delete(int eventoId, int loteId)
        {
            try
            {
                var lote = await _loteService.GetLoteByIdsAsync(eventoId, loteId);
                if (lote == null) return NoContent();

                return await _loteService.DeleteLote(lote.EventoId, lote.Id) ? Ok(new {message = "Deletado"}) : throw new Exception("Ocorreu um erro ao tentar deletar o lote");
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar excluir o lote. Erro {e.Message}");
            }
        }
    }
}