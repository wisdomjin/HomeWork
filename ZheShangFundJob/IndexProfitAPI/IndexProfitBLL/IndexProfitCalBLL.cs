using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using System.Data;

namespace IndexProfitAPI.IndexProfitBLL
{
    public class IndexProfitCalBLL
    {
        public async Task<string> GetRes(string beginDate, string endDate)
        {

            DataTable dataTable = this.GetExcelData("../IndexProfitAPI/File/作业.xlsx", "数据",7);
            DataRow[] dataRows = dataTable.AsEnumerable().Where(row => DateTime.Parse(beginDate) <= row.Field<DateTime>("日期") &&
            DateTime.Parse(endDate) >= row.Field<DateTime>("日期")).OrderBy(row=> row.Field<DateTime>("日期")).ToArray();

            //上证指数涨跌幅 贵州茅台涨跌幅 股票单日涨跌幅 - 上证指数涨跌幅 相对收益
            double indexPlus,
            for (int rows = 0; rows < dataRows.Length; rows++)
            {
                if (rows == 0)
                {

                }
                else
                { 
                
                }
            }
            return $"{beginDate}__{endDate}------{dataRows.Count()}";
        }

        /// <summary>
        /// 按路径，sheet页名称要求，读取sheet页
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="sheetName">sheet页名称</param>
        /// <returns></returns>
        private ISheet ReadExcel(string filePath,string sheetName)
        {
            using (var stream = File.OpenRead(filePath))
            {
                IWorkbook workbook = WorkbookFactory.Create(stream);
                ISheet sheet = workbook.GetSheet(sheetName);
                return sheet;
            }
        }
        /// <summary>
        /// sheet页解析为Datatable
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="sheetName">sheet页名称</param>
        /// <param name="readColumn">读取的前多少列</param>
        /// <returns></returns>
        private DataTable GetExcelData(string filePath,string sheetName,int readColumn)
        {
            DataTable dataTable = new DataTable("SourceData");

            ISheet sheet = this.ReadExcel(filePath, sheetName);
            Dictionary<int, string> columnNameDic = new Dictionary<int, string>();
            for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                if (rowIndex == 0)
                {
                    if (sheet.GetRow(rowIndex) != null) // 检查行是否存在  
                    {
                        for (int columnIndex = 0; columnIndex < readColumn; columnIndex++) // 遍历列  
                        {
                            ICell cell = sheet.GetRow(rowIndex).GetCell(columnIndex); // 获取单元格  
                            if (cell != null && !string.IsNullOrEmpty(cell.ToString())) // 检查单元格是否存在  
                            {
                                DataColumn column = columnIndex == 0 ? new DataColumn(cell.ToString().Trim(), typeof(DateTime)) 
                                    : new DataColumn(cell.ToString().Trim(), dataType: typeof(double));
                                dataTable.Columns.Add(column);
                                columnNameDic.Add(columnIndex, cell.ToString().Trim());
                            }
                        }
                    }
                }
                else
                {
                    if (sheet.GetRow(rowIndex) != null) // 检查行是否存在  
                    {
                        DataRow newRow = dataTable.NewRow();
                        for (int columnIndex = 0; columnIndex < readColumn; columnIndex++) // 遍历列  
                        {
                            ICell cell = sheet.GetRow(rowIndex).GetCell(columnIndex); // 获取单元格  
                            if (cell != null) // 检查单元格是否存在  
                            {
                                if (columnIndex == 0)
                                    newRow[columnNameDic[columnIndex]] = ConvertExcelDate(cell.NumericCellValue);
                                else
                                    newRow[columnNameDic[columnIndex]] = cell.CellType == CellType.Numeric ? cell.NumericCellValue : cell.ToString();

                            }
                        } 
                        dataTable.Rows.Add(newRow);
                    }
                }
            }
            return dataTable;
        }

        private DateTime ConvertExcelDate(double dateAsDouble)
        {
            // Excel日期起始点
            DateTime startDate = new DateTime(1900, 1, 1);
            // 将数字部分转换为天数
            TimeSpan days = TimeSpan.FromDays(dateAsDouble - 2);
            // 将天数加到起始日期上
            DateTime excelDate = startDate.Add(days);
            return excelDate;
        }
    }

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
