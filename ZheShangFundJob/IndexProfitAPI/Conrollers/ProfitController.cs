using IndexProfitAPI.IndexProfitBLL;
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
            _indexProfitCalBLL=indexProfitCalBLL;
        }

        [HttpGet("/{beginDate}/{endDate}")]
        public async Task<ActionResult<string>> GetProfitInfo(string beginDate, string endDate)
        {
            string res = await _indexProfitCalBLL.GetRes(beginDate, endDate);
            return new ActionResult<string>(res);
        }
    }
}
