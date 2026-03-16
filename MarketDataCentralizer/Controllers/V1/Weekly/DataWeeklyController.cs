using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketDataCentralizer.Controllers.V1.Weekly
{

    [ApiController]
    [Route("api/v1/[controller]")]
    public class DataWeeklyController : ControllerBase
    {

        private readonly IWeeklyDataForConsultService _getWeeklyDataForConsultService;

        public DataWeeklyController(IWeeklyDataForConsultService getWeeklyDataForConsultService)
        {
            _getWeeklyDataForConsultService = getWeeklyDataForConsultService;
        }



        /// <summary>
        /// Retorna os dados semanais mais recentes de um ativo.
        /// Importante: o parâmetro "ativo" é obrigatório.
        /// </summary>
        /// <param name="ativo">Símbolo do ativo (ex.: MSFT, AAPL, IBM)</param>
        /// <returns>Lista FinanceDataModel com os dados das últimas 10 semanas</returns>
        [Authorize]
        [HttpGet("Last10Days/{ativo}")]
        [ProducesResponseType(typeof(IEnumerable<FinanceDataModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<FinanceDataModel>>> GetAllWeeklyDataController(string ativo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ativo))
                {
                    return BadRequest("Passe o ativo corretamente");
                }

                var data = await _getWeeklyDataForConsultService.GetLastTenWeeklys(ativo);

                if (data == null)
                {
                    return NotFound("Nenhum dado foi retornado do serviço");
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro inesperado", ex);
            }
        }

        /// <summary>
        /// Retorna os dados de uma semana específica para o ativo informado.
        /// Importante: o parâmetro "ativo" é obrigatório e a data deve ser um dia útil.
        /// </summary>
        /// <param name="ativo">Símbolo do ativo (ex.: MSFT, AAPL, IBM)</param>
        /// <param name="date">Data da semana desejada (formato yyyy-MM-dd)</param>
        /// <returns>Objeto FinanceDataModel com os dados da semana solicitada</returns>
        [HttpGet("DataSpecificFridayWeekly/{ativo}/{date}")]
        [ProducesResponseType(typeof(FinanceDataModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FinanceDataModel>> GetAllWeeklyDataController(string ativo, DateTime date)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ativo))
                {
                    return BadRequest("Passe o ativo corretamente");
                }

                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    return BadRequest("Data deve ser um dia útil.");
                }

                var data = await _getWeeklyDataForConsultService.GetDataByWeekly(ativo, date);

                if (data == null)
                {
                    return NotFound("Nenhum dado foi retornado do serviço");
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro inesperado", ex);
            }
        }
    }
}
