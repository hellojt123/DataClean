using LinqToDB;
using LinqToDB.Data;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DataClean.Util
{
    public class ExcelOperator
    {
        public static IWorkbook ReadExcel(string fileName)
        {
            IWorkbook workbook = null;
            if (fileName.IndexOf(".xls") > 0) 
            {           
                FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                {
                    workbook = new XSSFWorkbook(fileStream);  //xlsx数据读入workbook
                }
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                {
                    workbook = new HSSFWorkbook(fileStream);  //xls数据读入workbook
                }

            }
            return workbook;
        }

       
        private static Thread currentThread=null;
        public static void ImportToSqLite(string fullPath, Action<string> act)
        {           
            if (currentThread == null || !currentThread.IsAlive) {
                currentThread = new Thread(new ThreadStart(() => _ImportToSqLite(fullPath, act)));//创建线程
                currentThread.Start();
            }            
        }

        private static void _ImportToSqLite(string fullPath, Action<string> act) {
            string fileName = Path.GetFileName(fullPath);
            string name = fileName.Replace(".xlsx", "").Replace(".xls", "");
            var num = Program.appControlers.dataControler.database1.Execute<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='" + name + "'");
            if (num == 0) {
                DataTable dataTable = null;           
                DataColumn column = null;
                DataRow dataRow = null;
                IWorkbook workbook = null;
                ISheet sheet = null;
                IRow row = null;
                ICell cell = null;
                int startRow = 0;

                workbook = ReadExcel(fullPath);
                if (workbook != null)
                {                    
                   sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet
                    dataTable = new DataTable();
                    if (sheet != null)
                    {
                        int rowCount = sheet.LastRowNum;//总行数
                        if (rowCount > 0)
                        {
                            IRow firstRow = sheet.GetRow(0);//第一行
                            int cellCount = firstRow.LastCellNum;//列数

                            //构建datatable的列

                            startRow = 1;//如果第一行是列名，则从第二行开始读取
                            string rowname = "";
                            string insertname = "";
                            for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                            {
                                cell = firstRow.GetCell(i);
                                if (cell != null)
                                {
                                    if (cell.CellType == CellType.Numeric) {
                                        if (cell.DateCellValue != null)
                                        {
                                            column = new DataColumn(cell.DateCellValue.ToString());
                                            dataTable.Columns.Add(column);
                                            rowname += "'" + cell.DateCellValue.ToString() + "' text,";
                                            insertname += "`" + cell.DateCellValue.ToString() + "`,";
                                        }
                                        else
                                        {
                                            column = new DataColumn(cell.NumericCellValue.ToString());
                                            dataTable.Columns.Add(column);
                                            rowname += "'" + cell.NumericCellValue.ToString() + "' text,";
                                            insertname += "`" + cell.NumericCellValue.ToString() + "`,";
                                        }                                      
                                    }
                                    else  if (cell.CellType == CellType.String)
                                    {
                                        column = new DataColumn(cell.StringCellValue);
                                        dataTable.Columns.Add(column);
                                        rowname += "'" + cell.StringCellValue + "' text,";
                                        insertname += "`"+cell.StringCellValue + "`,";
                                    }
                                }
                            }
                            insertname = insertname.Substring(0, insertname.Length - 1);
                            Program.appControlers.dataControler.database1.Execute("CREATE TABLE '"+name+"' ('d_tid' integer NOT NULL," + rowname + "PRIMARY KEY('d_tid'))");
                            //填充行
                            long tid = 0;
                            
                            for (int i = startRow; i <= rowCount; ++i)
                            {
                                tid++;
                                string rowstring = "";
                                row = sheet.GetRow(i);
                                if (row == null) continue;
                                dataRow = dataTable.NewRow();
                                for (int j = row.FirstCellNum; j < cellCount; ++j)
                                {
                                    cell = row.GetCell(j);
                                    if (cell == null) {
                                        dataRow[j] = "";
                                        rowstring += "'',";
                                    }
                                    else { 
                                        dataRow[j] = cell.ToString();
                                        rowstring += "'"+cell.ToString().Replace("'", "") + "',";                                      
                                    }
                                }
                            
                                rowstring = rowstring.Substring(0, rowstring.Length - 1);
                               
                                string sql = "INSERT INTO '" + name + "' (`d_tid`," + insertname + ") VALUES  (" + tid + "," + rowstring + ")";
                                Program.appControlers.dataControler.database1.Execute(sql);
                                dataTable.Rows.Add(dataRow);
                            }
                        }
                    }
                    if (act != null)
                    {
                        dataTable.TableName = fullPath;
                        act(name);
                    }
                }          
            }
            currentThread.Abort();
            currentThread = null;
        }

       


        private static string GetCellValue(ICell cell)
        {
            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case CellType.Blank: //空数据类型 这里类型注意一下，不同版本NPOI大小写可能不一样,有的版本是Blank（首字母大写)
                    return string.Empty;
                case CellType.Boolean: //bool类型
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Numeric: //数字类型
                    if (HSSFDateUtil.IsCellDateFormatted(cell))//日期类型
                    {
                        return cell.DateCellValue.ToString();
                    }
                    else //其它数字
                    {
                        return cell.NumericCellValue.ToString();
                    }
                case CellType.Unknown: //无法识别类型
                default: //默认类型
                    return cell.ToString();//
                case CellType.String: //string 类型
                    return cell.StringCellValue;
                case CellType.Formula: //带公式类型
                    try
                    {
                        HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }
    }
}
