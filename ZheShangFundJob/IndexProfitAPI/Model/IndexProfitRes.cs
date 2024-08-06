namespace IndexProfitAPI.Model
{
    public class IndexProfitRes
    {
        /// <summary>
        /// 日期
        /// </summary>
        public string? TDate { get; set; }
        /// <summary>
        /// 股票名称
        /// </summary>
        public string? StockName { get; set; }
        /// <summary>
        /// 当日上证指数相对收益
        /// </summary>
        public string? IndexProfit { get; set; }

    }
}
