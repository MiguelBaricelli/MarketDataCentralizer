using MarketDataCentralizer.Application.Services.Redis;
using Microsoft.AspNetCore.Mvc;

namespace MarketDataCentralizer.Controllers.V1.Redis
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RedisController : ControllerBase
    {

        private readonly RedisTestService _redisTestService;
        public RedisController(RedisTestService redisTestService)
        {
            _redisTestService = redisTestService;
        }

        [HttpPut("TestRedis/{collection}/{value}/{time}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string?>> TestRedis(string collection, string value, double time)
        {
            try
            {
                var result = await _redisTestService.TestRedis(collection, value, time);
                if (result == null)
                {
                    return NotFound("Dados não encontrados");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar os dados.", ex);
            }
        }
        [HttpGet("GetRedis/{collection}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string?>> GetRedis(string collection)
        {
            try
            {
                var result = await _redisTestService.GetAsync(collection);
                if (result == null)
                {
                    return NotFound("Dados não encontrados no REDIS");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar os dados.", ex);
            }
        }
    }
}
