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
        public ProfitController(IndexProfitCalBLL indexProfitCalBLL) {
            _indexProfitCalBLL = indexProfitCalBLL;
        }

        [HttpGet()]
        [Route("{beginDate}/{endDate}")]
        public async Task<ActionResult<List<IndexProfitRes>>> GetProfitInfo(string beginDate, string endDate)
        {
            List<IndexProfitRes> res = await _indexProfitCalBLL.GetRes(beginDate, endDate);
            return new ActionResult<List<IndexProfitRes>>(res);
        }
    }
}
