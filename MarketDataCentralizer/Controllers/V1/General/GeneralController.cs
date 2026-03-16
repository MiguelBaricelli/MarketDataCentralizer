using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Models;
using MarketDataCentralizer.Domain.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketDataCentralizer.Controllers.V1.General
{
    [ApiController]
    [Route("api/v1")]
    public class GeneralController : ControllerBase
    {

        private readonly IGeneralResponseService _generalResponseService;

        public GeneralController(IGeneralResponseService generateResponseService)
        {
            _generalResponseService = generateResponseService;

        }

        [Authorize]
        [HttpGet("GetGeneral/{asset}/{date}/{function}")]
        [ProducesResponseType(typeof(FinanceDataModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FinanceDataModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(FinanceDataModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FinanceDataModel>> GetGeneral(string asset, DateTime date, FunctionAlphaVantageEnum function)
        {
            if (string.IsNullOrWhiteSpace(asset))
            {
                return BadRequest("O ativo não pode ser nulo ou vazio");
            }
            if (date == default)
            {
                return BadRequest("A data fornecida é inválida");
            }

            if (function == 0)
            {
                return BadRequest("A função fornecida é inválida");
            }

            var response = await _generalResponseService.GeneralResponseServiceAsync(asset, date, function);

            if (response is null)
            {
                return NotFound("Não encontrado nenhum dado");
            }


            return Ok(response);
        }

        [Authorize]
        [HttpGet("GetGeneralDataByQuantity/{asset}/{date}/{function}/{n}")]
        [ProducesResponseType(typeof(FinanceDataModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FinanceDataModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(FinanceDataModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FinanceDataModel>> GetGeneralDataByQuantity(string asset, DateTime date, FunctionAlphaVantageEnum function, int n)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(asset))
                {
                    Console.WriteLine("O ativo não pode ser nulo ou vazio");
                    return BadRequest("O ativo não pode ser nulo ou vazio");
                }
                if (date == default)
                {
                    Console.WriteLine("A data fornecida é inválida");
                    return BadRequest("A data fornecida é inválida");
                }
                if (n <= 0)
                {
                    Console.WriteLine("A quantidade fornecida é inválida");
                    return BadRequest("A quantidade fornecida é inválida");
                }

                var response = await _generalResponseService.GetGeneralData(asset, date, function, n);

                if (response is null)
                {
                    return NotFound("Não encontrado nenhum dado");
                }


                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar a solicitação: {ex.Message}");
                return StatusCode(500, "Erro interno do servidor");
            }

        }

    }
}
