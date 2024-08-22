using IndexProfitAPI.Cache;
using IndexProfitAPI.IndexProfitBLL;
using IndexProfitAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IndexProfitAPI.Conrollers
{
    [Route("Index/[controller]")]
    [ApiController]
    public class ProfitController : ControllerBase
    {
        IndexProfitCalBLL _indexProfitCalBLL;
        private readonly ILogger<ProfitController> _logger;
        public ProfitController(IndexProfitCalBLL indexProfitCalBLL, ILogger<ProfitController> logger)
        {
            _indexProfitCalBLL = indexProfitCalBLL;
            _logger = logger;
        }

        [HttpGet()]
        [CacheFilter]
        [Route("{beginDate}/{endDate}")]
        public async Task<ActionResult<List<IndexProfitRes>>> GetProfitInfo(string beginDate, string endDate)
        {
            _logger.LogInformation($"-------请求时间：{DateTime.Now.ToString()}------");
            List<IndexProfitRes> res = await _indexProfitCalBLL.GetRes(beginDate, endDate);
            return new ActionResult<List<IndexProfitRes>>(res);
        }
    }
}
