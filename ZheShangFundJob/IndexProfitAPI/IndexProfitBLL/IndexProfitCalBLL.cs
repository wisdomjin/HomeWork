using IndexProfitAPI.Model;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using System.Data;

namespace IndexProfitAPI.IndexProfitBLL
{
    public class IndexProfitCalBLL
    {
        public async Task<List<IndexProfitRes>> GetRes(string beginDate, string endDate)
        {
            List<IndexProfitRes> res = new List<IndexProfitRes>();
            DataTable dataTable = this.GetExcelData("File/作业.xlsx", "数据",7);
            DataRow[] dataRows = dataTable.AsEnumerable().Where(row => DateTime.Parse(beginDate) <= row.Field<DateTime>("日期") &&
            DateTime.Parse(endDate) >= row.Field<DateTime>("日期")).OrderBy(row=> row.Field<DateTime>("日期")).ToArray();
            if (dataRows.Count() > 0)
            { 
                Dictionary<string,double> dicInfo= new Dictionary<string,double>();
                foreach (var column in dataRows[0].Table.Columns) {
                    if (column.ToString() != "日期" && column.ToString() != "上证指数")
                    {
                        dicInfo.Add(column.ToString(), double.MinValue);
                    }
                }
                //上证指数涨跌幅 贵州茅台涨跌幅 股票单日涨跌幅 - 上证指数涨跌幅 相对收益
                for (int rows = 0; rows < dataRows.Length; rows++)
                {
                    string tDate= ((DateTime)dataRows[rows]["日期"]).ToString("yyyy-MM-dd");
                    foreach (var info in dicInfo)
                    {
                        IndexProfitRes indexProfitRes = new IndexProfitRes();
                        indexProfitRes.TDate = tDate;
                        indexProfitRes.StockName = info.Key;
                        if (dataRows[rows][info.Key].GetType() != typeof(System.DBNull))
                        {
                            if (info.Value == double.MinValue)
                                dicInfo[info.Key] = 1;
                            else
                            {
                                if (rows != 0)
                                {
                                    DataRow drLast = dataRows[rows - 1];
                                    if (drLast[info.Key].GetType() != typeof(System.DBNull))
                                    {
                                        double growStock = (double)dataRows[rows][info.Key] /
                                            (double)drLast[info.Key] - 1;
                                        double growIndex = (double)dataRows[rows]["上证指数"] /
                                           (double)drLast["上证指数"] - 1;
                                        double growth = (growStock - growIndex + 1) * (double)dicInfo[info.Key];
                                        dicInfo[info.Key] = growth;
                                    }
                                }
                            }
                            indexProfitRes.IndexProfit = dicInfo[info.Key].ToString("0.00");
                            res.Add(indexProfitRes);
                        }
                        else
                        {
                            if (info.Value != double.MinValue)
                            {
                                indexProfitRes.IndexProfit = dicInfo[info.Key].ToString("0.00");
                                res.Add(indexProfitRes);
                            }
                            else
                            {
                                indexProfitRes.IndexProfit = "";
                                res.Add(indexProfitRes);
                            }
                        }
                    }
                }
            }
            return res;
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

}
