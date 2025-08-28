using CountryGwp.Api.DTO;
using CountryGwp.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CountryGwp.Api.Controllers
{
    [ApiController]
    [Route("server/api/gwp")]
    public class CountryGwpController : ControllerBase
    {
        private readonly IGwpServcie _service;
        public CountryGwpController(IGwpServcie service)
        {
            _service = service;
        }

        [HttpPost("avg")]
        public async Task<IActionResult> GetAvg([FromBody] GwpAvgRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req?.Country) || req.Lob is null || req.Lob.Count == 0)
            {
                return BadRequest(new { error = "country and lob are required" });
            }

            var data = await _service.GetAverageAsync(req.Country,req.Lob, ct);
            if(data.Count == 0)
            {
                return NotFound(new { error = "no data for given country/lob" });
            }
            return Ok(data);    
        }
    }
}
