using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;
using System.Text;
using NPOI.XSSF.Model;

namespace DynamicDataChecking.Common
{
    public class ExcelHelper
    {
        #region Import

        public static DataTable ExcelToDatable(string filePath, int sheetIndex, int startRow)
        {
            return ExcelToDaTaSet(filePath, sheetIndex, startRow).Tables[0];
        }

        public static DataSet ExcelToDaTaSet(string filePath, int sheetIndex, int startRow)
        {
            DataSet ds = new DataSet();
            try
            {
                IWorkbook workbook;
                using (FileStream stream = File.OpenRead(filePath))
                {
                    if (System.IO.Path.GetExtension(filePath).ToUpper() == ".XLSX")
                    {
                        //workbook = new XSSFWorkbook(filePath);
                        workbook = new XSSFWorkbook(stream);
                    }
                    else
                    {
                        //FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                        workbook = new HSSFWorkbook(stream);
                    }
                }

                int count = workbook.NumberOfSheets;
                ISheet sheet;
                IRow row;
                DataRow dr;
                object obj;

                sheet = workbook.GetSheetAt(sheetIndex);
                DataTable dt = new DataTable(sheet.SheetName);
                //将第一列作为列表头  
                row = sheet.GetRow(startRow);

                if (row == null)
                    return new DataSet();
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    obj = row.GetCell(j);
                    if (obj != null)
                    {
                        if (dt.Columns.Contains(obj.ToString().Trim()))
                            dt.Columns.Add(obj.ToString().Trim() + j.ToString());
                        else
                            dt.Columns.Add(obj.ToString().Trim());
                    }
                }
                for (int j = startRow + 1; j <= sheet.LastRowNum; j++)
                {
                    row = sheet.GetRow(j);
                    if (row == null)
                        continue;
                    dr = dt.NewRow();
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        //obj = row.GetCell(k);
                        //if (obj != null)
                        //    dr[k] = obj.ToString();
                        ICell excelcell = row.GetCell(k);
                        dr[k] = excelcell;
                        //DateTime Cell
                        if (excelcell != null && excelcell.CellType == CellType.Numeric)
                        {
                            short format = excelcell.CellStyle.DataFormat;
                            if (format == 14 || format == 31 || format == 57 || format == 58)
                            {
                                dr[k] = excelcell.DateCellValue;
                            }
                        }
                    }
                    dt.Rows.Add(dr);
                }
                ds.Tables.Add(dt);
                workbook = null;
            }
            catch (Exception ex)
            {
                throw;
            }
            return ds;
        }
        #endregion


        #region Import

        /// <summary>
        /// 由Excel导入DataTable
        /// </summary>
        /// <param name="excelFilePath">Excel文件路径，为物理路径。</param>
        /// <param name="sheetName">Excel工作表名称</param>
        /// <param name="headerRowIndex">Excel表头行索引</param>
        /// <returns>DataTable</returns>
        public static DataTable ImportDataTableFromExcel(string excelFilePath, string sheetName, int headerRowIndex)
        {
            using (FileStream stream = File.OpenRead(excelFilePath))
            {
                return ImportDataTableFromExcel(stream, sheetName, headerRowIndex);
            }
        }

        /// <summary>
        /// 由Excel导入DataTable
        /// </summary>
        /// <param name="excelFileStream">Excel文件流</param>
        /// <param name="sheetName">Excel工作表名称</param>
        /// <param name="headerRowIndex">Excel表头行索引</param>
        /// <returns>DataTable</returns>
        private static DataTable ImportDataTableFromExcel(Stream excelFileStream, string sheetName, int headerRowIndex)
        {
            IWorkbook workbook = new XSSFWorkbook(excelFileStream);
            var sheet = workbook.GetSheet(sheetName);
            if (sheet == null) return null;
            DataTable table = new DataTable();
            var headerRow = sheet.GetRow(headerRowIndex);
            int cellCount = headerRow.LastCellNum;

            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                if (headerRow.GetCell(i) == null || headerRow.GetCell(i).StringCellValue.Trim() == "")
                {
                    // 如果遇到第一个空列，则不再继续向后读取
                    cellCount = i + 1;
                    break;
                }
                // var se = headerRow.GetCell(i).StringCellValue;
                //DataColumn column = new DataColumn(se);
                DataColumn column = new DataColumn((1 + i).ToString());
                table.Columns.Add(column);
            }
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                //if (row == null || row.GetCell(0) == null || row.GetCell(0).ToString().Trim() == "")
                //{
                //    // 如果遇到第一个空行，则不再继续向后读取
                //    break;
                //}
                if (row == null)
                {
                    // 如果遇到第一个空行，则不再继续向后读取
                    break;
                }
                var isEmptyRow = true;
                DataRow dataRow = table.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    SetDataRow(row, ref dataRow, j);
                    if (isEmptyRow && row.GetCell(j) == null) isEmptyRow = true;
                    else isEmptyRow = false;
                }
                if (!isEmptyRow)
                    table.Rows.Add(dataRow);
            }
            excelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 由Excel导入DataTable
        /// </summary>
        /// <param name="excelFilePath">Excel文件路径，为物理路径。</param>
        /// <param name="sheetIndex">Excel工作表索引</param>
        /// <param name="headerRowIndex">Excel表头行索引</param>
        /// <returns>DataTable</returns>
        public static DataTable ImportDataTableFromExcel(string excelFilePath, int sheetIndex, int headerRowIndex)
        {
            using (FileStream stream = File.OpenRead(excelFilePath))
            {
                return ImportDataTableFromExcel(stream, sheetIndex, headerRowIndex);
            }
        }
        /// <summary>
        /// 由Excel导入DataTable
        /// </summary>
        /// <param name="excelFilePath">Excel文件路径，为物理路径。</param>
        /// <param name="sheetIndex">Excel工作表索引</param>
        /// <param name="headerRowIndex">Excel表头行索引</param>
        /// <returns>DataTable</returns>
        public static DataTable ImportDataTableFromExcel_ScrapList(string excelFilePath, int sheetIndex, int headerRowIndex)
        {
            using (FileStream stream = File.OpenRead(excelFilePath))
            {
                return ImportDataTableFromExcel_Scrap(stream, sheetIndex, headerRowIndex);
            }
        }
        public static DataTable ImportDataTableFromExcel(string excelFilePath, int sheetIndex, int headerRowIndex, Boolean allowfirstCellEmpty)
        {
            using (FileStream stream = File.OpenRead(excelFilePath))
            {
                return ImportDataTableFromExcel(stream, sheetIndex, headerRowIndex, allowfirstCellEmpty);
            }
        }

        public static DataTable ImportDataTableFromExcel(string excelFilePath, int sheetIndex, int row, int cell)
        {
            using (FileStream stream = File.OpenRead(excelFilePath))
            {
                return ImportDataTableFromExcel(stream, sheetIndex, row, cell);
            }
        }

        public static DataTable ImportDataTableFromExcel2PK(string excelFilePath, int sheetIndex, int headerRowIndex)
        {
            using (FileStream stream = File.OpenRead(excelFilePath))
            {
                return ImportDataTableFromExcel2PK(stream, sheetIndex, headerRowIndex);
            }
        }

        /// <summary>
        /// 由Excel导入DataTable
        /// </summary>
        /// <param name="excelFileStream">Excel文件流</param>
        /// <param name="sheetIndex">Excel工作表索引</param>
        /// <param name="headerRowIndex">Excel表头行索引</param>
        /// <returns>DataTable</returns>
        private static DataTable ImportDataTableFromExcel_Scrap(Stream excelFileStream, int sheetIndex, int headerRowIndex)
        {
            IWorkbook workbook = new XSSFWorkbook(excelFileStream);
            var sheet = workbook.GetSheetAt(sheetIndex);
            DataTable table = new DataTable();
            var headerRow = sheet.GetRow(headerRowIndex);
            var NewheaderRow = sheet.GetRow(headerRowIndex + 1);
            int cellCount = headerRow.LastCellNum;
            string Flage = string.Empty;
            string NewFlage = string.Empty;
            #region 表头处理
            for (int i = 0; i < headerRow.Cells.Count; i++)
            {
                try
                {
                    Flage = headerRow.Cells[i].StringCellValue.ToString();
                }
                catch (Exception)
                {
                    Flage = "";
                }
                try
                {
                    NewFlage = NewheaderRow.Cells[i].StringCellValue.ToString();
                }
                catch (Exception)
                {
                    NewFlage = "";
                }
                Flage = Flage + NewFlage;
                Flage = Flage.Replace("\\", "");
                Flage = Flage.Replace("n", "");
                Flage = Flage.Replace(Environment.NewLine, "");
                DataColumn column = new DataColumn(Flage);
                table.Columns.Add(column);
            }
            #endregion
            for (int i = headerRowIndex + (sheet.FirstRowNum + 2); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null)
                { continue; }
                var isEmptyRow = true;
                DataRow dataRow = table.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.Cells[0].StringCellValue.ToString().StartsWith("<End>----------------------------------------------------"))
                    {
                        excelFileStream.Close();
                        workbook = null;
                        sheet = null;
                        return table;
                    }
                    SetDataRow(row, ref dataRow, j);
                    if (isEmptyRow && row.GetCell(j) == null) isEmptyRow = true;
                    else isEmptyRow = false;
                }
                if (!isEmptyRow)
                    table.Rows.Add(dataRow);
            }
            excelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }
        /// <summary>
        /// 由Excel导入DataTable
        /// </summary>
        /// <param name="excelFileStream">Excel文件流</param>
        /// <param name="sheetIndex">Excel工作表索引</param>
        /// <param name="headerRowIndex">Excel表头行索引</param>
        /// <returns>DataTable</returns>
        private static DataTable ImportDataTableFromExcel(Stream excelFileStream, int sheetIndex, int headerRowIndex)
        {
            DataTable table = new DataTable();
            try
            {
                IWorkbook workbook = new XSSFWorkbook(excelFileStream);
                var sheet = workbook.GetSheetAt(sheetIndex);
                var headerRow = sheet.GetRow(headerRowIndex);
                int cellCount = headerRow.LastCellNum;
                // for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    if (headerRow.GetCell(i) == null || headerRow.GetCell(i).StringCellValue.Trim() == "")
                    {
                        // 如果遇到第一个空列，则不再继续向后读取
                        cellCount = i + 1;
                        break;
                    }
                    // var se = headerRow.GetCell(i).StringCellValue;
                    //DataColumn column = new DataColumn(se);
                    DataColumn column = new DataColumn((1 + i).ToString());
                    table.Columns.Add(column);
                }
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    var row = sheet.GetRow(i);
                    //if (row == null || row.GetCell(0) == null || row.GetCell(0).ToString().Trim() == "")
                    //{
                    //    // 如果遇到第一个空行，则不再继续向后读取
                    //    break;
                    //}
                    if (row == null)
                    {
                        // 如果遇到第一个空行，则不再继续向后读取
                        break;
                    }
                    var isEmptyRow = true;
                    DataRow dataRow = table.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        SetDataRow(row, ref dataRow, j);
                        if (isEmptyRow && row.GetCell(j) == null) isEmptyRow = true;
                        else isEmptyRow = false;
                    }
                    if (!isEmptyRow)
                        table.Rows.Add(dataRow);
                }
                excelFileStream.Close();
                workbook = null;
                sheet = null;
                return table;
            }
            catch (Exception es)
            {
                LogHelper.WriteLog("ExcelHelper", "ImportDataTableFromExcel    ", "发生异常：" + es.ToString());
            }
            return table;
        }
        /// <summary>
        /// 由Excel导入DataTable ,允许行的第一列为空或null
        /// </summary>
        /// <param name="excelFileStream">Excel文件流</param>
        /// <param name="sheetIndex">Excel工作表索引</param>
        /// <param name="headerRowIndex">Excel表头行索引</param>
        /// <param name="allowFirstCellEmpty">是否允许模版的第一列为空或null</param>
        /// <returns>DataTable</returns>
        private static DataTable ImportDataTableFromExcel(Stream excelFileStream, int sheetIndex, int headerRowIndex, Boolean allowFirstCellEmpty)
        {
            IWorkbook workbook = new XSSFWorkbook(excelFileStream);
            var sheet = workbook.GetSheetAt(sheetIndex);
            DataTable table = new DataTable();
            var headerRow = sheet.GetRow(headerRowIndex);
            int cellCount = headerRow.LastCellNum;
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                if (headerRow.GetCell(i) == null || headerRow.GetCell(i).StringCellValue.Trim() == "")
                {
                    // 如果遇到第一个空列，则不再继续向后读取
                    cellCount = i + 1;
                    break;
                }
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (!allowFirstCellEmpty)
                {
                    if (row == null || row.GetCell(0) == null || row.GetCell(0).ToString().Trim() == "")
                    {
                        // 如果遇到第一个空行，则不再继续向后读取
                        break;
                    }
                }
                else
                {
                    if (row == null)
                    {
                        // 如果遇到第一个空行，则不再继续向后读取
                        break;
                    }
                }


                DataRow dataRow = table.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    SetDataRow(row, ref dataRow, j);
                    //dataRow[j] = row.GetCell(j);
                }
                table.Rows.Add(dataRow);
            }
            excelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 由Excel导入DataTable
        /// </summary>
        /// <param name="excelFileStream">Excel文件流</param>
        /// <param name="sheetIndex">Excel工作表索引</param>
        /// <param name="headerRowIndex">Excel表头行索引</param>
        /// <returns>DataTable</returns>
        private static DataTable ImportDataTableFromExcel2PK(Stream excelFileStream, int sheetIndex, int headerRowIndex)
        {
            IWorkbook workbook = new XSSFWorkbook(excelFileStream);
            var sheet = workbook.GetSheetAt(sheetIndex);
            DataTable table = new DataTable();
            var headerRow = sheet.GetRow(headerRowIndex);
            int cellCount = headerRow.LastCellNum;
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                var sds = headerRow.GetCell(i);
                if (sds == null || headerRow.GetCell(i).StringCellValue.Trim() == "")
                {
                    // 如果遇到第一个空列，则不再继续向后读取
                    cellCount = i + 1;
                    break;
                }
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                var isNotEmptyRow = true;
                var row = sheet.GetRow(i);
                if (row == null || row.GetCell(0) == null || row.GetCell(0).ToString().Trim() == "" || row.GetCell(1) == null || row.GetCell(1).ToString().Trim() == "")
                {
                    // isNotEmptyRow = true;
                    // 如果遇到第一个空行，则不再继续向后读取
                    // break;
                }
                else isNotEmptyRow = false;
                if (!isNotEmptyRow)
                {
                    DataRow dataRow = table.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        SetDataRow(row, ref dataRow, j);
                    }
                    table.Rows.Add(dataRow);
                }
            }
            excelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        private static void SetDataRow(IRow excelrow, ref DataRow dataRow, int index)
        {
            ICell excelcell = excelrow.GetCell(index);
            dataRow[index] = excelcell;
            //DateTime Cell
            if (excelcell != null && excelcell.CellType == CellType.Numeric)
            {
                if (DateUtil.IsADateFormat(excelcell.CellStyle.DataFormat, excelcell.CellStyle.GetDataFormatString()))
                {
                    dataRow[index] = excelcell.DateCellValue;
                }
            }
        }
        /// <summary>
        /// 由Excel导入DataTable,不取表头行信息，只取内容，同时允许当在X行y列之内的表格内容可以为空或null。
        /// </summary>
        /// <param name="excelFileStream"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static DataTable ImportDataTableFromExcel(Stream excelFileStream, int sheetIndex, int x, int y)
        {
            IWorkbook workbook = new XSSFWorkbook(excelFileStream);
            var sheet = workbook.GetSheetAt(sheetIndex);
            var headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            DataTable table = new DataTable();
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn((1 + i).ToString());
                table.Columns.Add(column);
            }
            for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);

                if (row == null && i >= x)
                {
                    // 如果遇到第一个空行，则不再继续向后读取
                    break;
                }

                DataRow dataRow = table.NewRow();
                for (int j = 0; j < cellCount; j++)
                {

                    if (i < x && j < y) j = y;
                    SetDataRow(row, ref dataRow, j);
                }
                table.Rows.Add(dataRow);
            }
            excelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }
        #endregion

        #region Export

        public static bool ExportExcelforTemplate(string tempfile, string outfile, DataTable dt, int rowstart, string[] arrStyle, bool isExcel03)
        {
            IWorkbook workbook = null;
            bool flag = false;
            try
            {
                FileStream readfile = new FileStream(tempfile, FileMode.Open);
                if (isExcel03)
                {
                    workbook = new HSSFWorkbook(readfile);
                }
                else
                {
                    workbook = new XSSFWorkbook(readfile);
                }
                readfile.Close();

                //边框线
                ICellStyle cellStyle = workbook.CreateCellStyle();
                cellStyle.BorderBottom = BorderStyle.Thin;
                cellStyle.BorderLeft = BorderStyle.Thin;
                cellStyle.BorderRight = BorderStyle.Thin;
                cellStyle.BorderTop = BorderStyle.Thin;
                //字体
                IFont font = workbook.CreateFont();
                font.FontHeight = 10;
                font.FontName = "Arial";
                cellStyle.SetFont(font);
                //边框颜色
                cellStyle.BottomBorderColor = HSSFColor.Black.Index;
                cellStyle.TopBorderColor = HSSFColor.Black.Index;

                StylesTable st = ((XSSFWorkbook)workbook).GetStylesSource();
                IDataFormat df = new XSSFDataFormat(st);

                ISheet sheet = workbook.GetSheetAt(0);

                int rowNum = rowstart;
                string format = string.Empty;
                float num = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (sheet.GetRow(rowNum) == null)
                        {
                            sheet.CreateRow(rowNum);
                        }
                        if (sheet.GetRow(rowNum).GetCell(i) == null)
                        {
                            sheet.GetRow(rowNum).CreateCell(i);
                        }

                        ICell cell = sheet.GetRow(rowNum).GetCell(i);
                        format = arrStyle[i];
                        if (format == "float" && float.TryParse(dr[i].ToString(), out num))
                        {
                            cellStyle.DataFormat = df.GetFormat("#,##0");
                            cell.SetCellValue(num);
                        }
                        else if (format == "percent" && float.TryParse(dr[i].ToString(), out num))
                        {
                            cellStyle.DataFormat = df.GetFormat("0%");
                            cell.SetCellValue(num);
                        }
                        else
                        {
                            cell.SetCellValue(dr[i].ToString());
                        }
                        //画线
                        cell.CellStyle = cellStyle;
                    }
                    rowNum++;
                }

                FileStream writefile = File.Create(outfile);
                workbook.Write(writefile);
                writefile.Close();

                flag = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return flag;
        }
        //public static bool ExportExcelforTemplate(string tempfile, string outfile, DataTable dt, int rowstart, bool isExcel03)
        //{
        //    IWorkbook workbook = null;
        //    bool flag = false;
        //    try
        //    {
        //        FileStream readfile = new FileStream(tempfile, FileMode.Open);
        //        if (isExcel03)
        //        {
        //            workbook = new HSSFWorkbook(readfile);
        //        }
        //        else
        //        {
        //            workbook = new XSSFWorkbook(readfile);
        //        }
        //        readfile.Close();

        //        //边框线
        //        ICellStyle cellStyle = workbook.CreateCellStyle();
        //        cellStyle.BorderBottom = BorderStyle.Thin;
        //        cellStyle.BorderLeft = BorderStyle.Thin;
        //        cellStyle.BorderRight = BorderStyle.Thin;
        //        cellStyle.BorderTop = BorderStyle.Thin;
        //        //字体
        //        IFont font = workbook.CreateFont();
        //        font.FontHeight = 10;
        //        font.FontName = "Arial";
        //        cellStyle.SetFont(font);
        //        //边框颜色
        //        cellStyle.BottomBorderColor = HSSFColor.Black.Index;
        //        cellStyle.TopBorderColor = HSSFColor.Black.Index;

        //        StylesTable st = ((XSSFWorkbook)workbook).GetStylesSource();
        //        IDataFormat df = new XSSFDataFormat(st);

        //        ISheet sheet = workbook.GetSheetAt(0);

        //        int rowNum = rowstart;
        //        string format = string.Empty;
        //        float num = 0;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            for (int i = 0; i < dt.Columns.Count; i++)
        //            {
        //                if (sheet.GetRow(rowNum) == null)
        //                {
        //                    sheet.CreateRow(rowNum);
        //                }
        //                if (sheet.GetRow(rowNum).GetCell(i) == null)
        //                {
        //                    sheet.GetRow(rowNum).CreateCell(i);
        //                }

        //                ICell cell = sheet.GetRow(rowNum).GetCell(i);
        //                if (dr[i].GetType() == typeof(decimal) || dr[i].GetType() == typeof(int) || dr[i].GetType() == typeof(double))
        //                {
        //                    cell.CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
        //                    cell.SetCellValue(dr[i].ToString().ToDouble());
        //                }
        //                else
        //                    cell.SetCellValue(dr[i].ToString());
        //                //画线
        //                cell.CellStyle = cellStyle;
        //            }
        //            rowNum++;
        //        }

        //        FileStream writefile = File.Create(outfile);
        //        workbook.Write(writefile);
        //        writefile.Close();

        //        flag = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }

        //    return flag;
        //}

        //可以hide 多个列。
        public static MemoryStream WriteToStream(string[] colHeads, string[] colNames, DataTable dataSource, bool lockhead = true, bool lockreadonly = false, List<string> editcolunms = null, List<int> hiddenColumnIndexs = null, string excelpwd = "", bool setautofilter = false, string templatefilepath = "")
        {
            DataView dv = dataSource.DefaultView;
            dv.Sort = "ID Asc";
            DataTable dt2 = dv.ToTable();
            var workbook = WriteHSSFWorkbook(colHeads, colNames, dt2, lockhead, lockreadonly, editcolunms, hiddenColumnIndexs, excelpwd, setautofilter, templatefilepath);
            MemoryStream ms = new MemoryStream();

            workbook.Write(ms);
            ms.Flush();
            //ms.Position = 0;
            return ms;
        }
        public static MemoryStream WriteToStream_NewSAP(string[] colHeads, string[] colNames, DataTable dataSource, bool lockhead = true, bool lockreadonly = false, List<string> editcolunms = null, List<int> hiddenColumnIndexs = null, string excelpwd = "", bool setautofilter = false, string templatefilepath = "")
        {
            var workbook = WriteHSSFWorkbook(colHeads, colNames, dataSource, lockhead, lockreadonly, editcolunms, hiddenColumnIndexs, excelpwd, setautofilter, templatefilepath);
            MemoryStream ms = new MemoryStream();

            workbook.Write(ms);
            ms.Flush();
            //ms.Position = 0;
            return ms;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="colHeads"></param>
        /// <param name="colNames"></param>
        /// <param name="dataSource"></param>
        /// <param name="lockhead"></param>
        /// <param name="lockreadonly"></param>
        /// <param name="editcolunms"></param>
        /// <param name="hiddenColumnIndexs"></param>
        /// <param name="excelpwd"></param>
        /// <param name="templatefilepath"></param>
        /// <param name="setautofilter"></param>
        /// <returns></returns>
        public static IWorkbook WriteHSSFWorkbook(string[] colHeads, string[] colNames, DataTable dataSource, bool lockhead = true, bool lockreadonly = false, List<string> editcolunms = null,
            List<int> hiddenColumnIndexs = null, string excelpwd = "", bool setautofilter = false, string templatefilepath = "")
        {
            FileStream stream = null;

            try
            {
                bool istemplate = false;
                IWorkbook workbook;
                ISheet sheet;
                if (!string.IsNullOrEmpty(templatefilepath) && File.Exists(templatefilepath))
                {
                    stream = File.OpenRead(templatefilepath);
                    workbook = new XSSFWorkbook(stream);
                    sheet = workbook.GetSheetAt(0);
                    istemplate = true;
                }
                else
                {
                    workbook = CreateHSSFWorkbook();
                    sheet = workbook.CreateSheet();
                    WriteHead(sheet, colHeads);
                }

                WriteData(workbook, sheet, colNames, dataSource, lockhead, lockreadonly, editcolunms);

                if (hiddenColumnIndexs != null && hiddenColumnIndexs.Count > 0)
                {
                    foreach (int index in hiddenColumnIndexs)
                    {
                        sheet.SetColumnHidden(index, true);
                    }
                }

                if (setautofilter)
                {
                    sheet.SetAutoFilter(new CellRangeAddress(0, 0, 0, colHeads.Length - 1));
                }

                if (!istemplate && (lockhead || lockreadonly))
                {
                    sheet.ProtectSheet(excelpwd);
                }

                ////move to head, only care head, cells too big memory
                //var cells = sheet.GetRow(0).Cells;
                //foreach (ICell cell in cells)
                //{s
                //    sheet.AutoSizeColumn(cell.ColumnIndex);
                //}

                //for (int i = 0; i < colHeads.Length -1; i++)
                //{
                //    sheet.AutoSizeColumn(i);
                //}

                return workbook;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }

        public static bool DataTableToExcel(DataTable dt, string filePath)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(dt.Copy());
            return DataSetToExcel(ds, filePath);
        }
        public static bool DataSetToExcel(DataSet ds, string filePath)
        {
            try
            {
                IWorkbook workbook = new XSSFWorkbook();
                IRow row;
                ICell cell;
                int rowIndex = 0;
                //默认表头样式 
                XSSFCellStyle style_temp = (XSSFCellStyle)workbook.CreateCellStyle();
                style_temp.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                style_temp.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey40Percent.Index;
                style_temp.FillPattern = FillPattern.SolidForeground;
                style_temp.BorderBottom = BorderStyle.Thin;
                style_temp.BorderRight = BorderStyle.Thin;
                IFont font = workbook.CreateFont();
                font.Boldweight = (short)FontBoldWeight.Bold;
                style_temp.SetFont(font);
                //end
                foreach (DataTable dt in ds.Tables)
                {
                    ISheet sheet = workbook.CreateSheet(dt.TableName);
                    row = sheet.CreateRow(rowIndex);
                    foreach (DataColumn dc in dt.Columns)
                    {
                        cell = row.CreateCell(dc.Ordinal);
                        cell.CellStyle = style_temp;
                        cell.SetCellValue(dc.ColumnName);
                    }
                    rowIndex++;
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = sheet.CreateRow(rowIndex++);
                        foreach (DataColumn column in dt.Columns)
                        {
                            cell = row.CreateCell(column.Ordinal);
                            if (column.DataType == typeof(decimal) || column.DataType == typeof(int) || column.DataType == typeof(double))
                            {
                                cell.CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                cell.SetCellValue(dr[column].ToString().ToDouble());
                            }
                            else
                                cell.SetCellValue(dr[column].ToString());
                        }
                    }
                    rowIndex = 0;
                }
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                FileStream sw = File.Create(filePath);
                workbook.Write(sw);
                sw.Close();
                sw.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static Dictionary<string, ICellStyle> GetICellStyles(IWorkbook workbook)
        {
            Dictionary<string, ICellStyle> styles = new Dictionary<string, ICellStyle>();
            IFont readOnlyColor = GetLockFont(workbook);
            var types = new List<Type> { typeof(DateTime) };
            styles.Add(CellTypeStyleEnum.NormalCell.ToString(), GetICellStyle(workbook, CellTypeEnum.Cell));
            styles.Add(CellTypeStyleEnum.LockedCell.ToString(), GetICellStyle(workbook, CellTypeEnum.Cell, true, readOnlyColor));
            styles.Add(CellTypeStyleEnum.ReadOnlyColorCell.ToString(), GetICellStyle(workbook, CellTypeEnum.Cell, false, readOnlyColor));
            styles.Add(CellTypeStyleEnum.NormalDateTimeCell.ToString(), GetICellStyle(workbook, CellTypeEnum.Cell, false, null, types));
            styles.Add(CellTypeStyleEnum.LockedDateTimeCell.ToString(), GetICellStyle(workbook, CellTypeEnum.Cell, true, readOnlyColor, types));
            styles.Add(CellTypeStyleEnum.ReadOnlyColorDateTimeCell.ToString(), GetICellStyle(workbook, CellTypeEnum.Cell, false, readOnlyColor, types));
            styles.Add(CellTypeStyleEnum.NormalHead.ToString(), GetICellStyle(workbook, CellTypeEnum.Head));
            styles.Add(CellTypeStyleEnum.ReadOnlyColorHead.ToString(), GetICellStyle(workbook, CellTypeEnum.Head, false, readOnlyColor));
            styles.Add(CellTypeStyleEnum.LockedHead.ToString(), GetICellStyle(workbook, CellTypeEnum.Head, true));
            styles.Add(CellTypeStyleEnum.LockedReadOnlyColorHead.ToString(), GetICellStyle(workbook, CellTypeEnum.Head, true, readOnlyColor));
            return styles;
        }

        public static object GetCellValue(Stream excelFileStream, int sheetIndex, int row, int col)
        {
            IWorkbook workbook = new XSSFWorkbook(excelFileStream);
            var sheet = workbook.GetSheetAt(sheetIndex);
            object value = sheet.GetRow(row).GetCell(col);
            return value;
        }

        private static ICellStyle GetICellStyle(IWorkbook workbook, CellTypeEnum type, bool locked = false, IFont font = null, List<Type> datatypes = null)
        {
            if (type == CellTypeEnum.Cell)
            {
                ICellStyle cellStyle = workbook.CreateCellStyle();
                cellStyle.BorderTop = BorderStyle.Thin;
                cellStyle.BorderRight = BorderStyle.Thin;
                cellStyle.BorderBottom = BorderStyle.Thin;
                cellStyle.BorderLeft = BorderStyle.Thin;
                cellStyle.IsLocked = locked;
                if (datatypes != null)
                {
                    foreach (Type datatype in datatypes)
                    {
                        if (datatype.Name == typeof(DateTime).Name)
                        {
                            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("m/d/yy");
                        }
                    }
                }
                else
                {
                    //@
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("Text");
                }
                if (font != null)
                {
                    cellStyle.SetFont(font);
                }
                return cellStyle;
            }

            if (type == CellTypeEnum.Head)
            {
                ICellStyle headStyle = workbook.CreateCellStyle();
                headStyle.FillForegroundColor = HSSFColor.Grey40Percent.Index;
                headStyle.FillPattern = FillPattern.SolidForeground;
                headStyle.BorderTop = BorderStyle.Thin;
                headStyle.BorderRight = BorderStyle.Thin;
                headStyle.BorderBottom = BorderStyle.Thin;
                headStyle.BorderLeft = BorderStyle.Thin;
                headStyle.WrapText = true;
                headStyle.Alignment = HorizontalAlignment.Center;
                headStyle.VerticalAlignment = VerticalAlignment.Center;
                headStyle.IsLocked = locked;

                if (font != null)
                {
                    headStyle.SetFont(font);
                }
                return headStyle;
            }
            return workbook.CreateCellStyle();
        }

        private static IFont GetLockFont(IWorkbook workbook)
        {
            IFont font = workbook.CreateFont();
            font.Color = HSSFColor.RoyalBlue.Index;
            return font;
        }

        private static IWorkbook CreateHSSFWorkbook()
        {
            IWorkbook workbook = new XSSFWorkbook();
            //DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            //dsi.Company = "Andritz Co., Ltd.";
            //SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            //si.Author = "PumpBook System";
            //workbook.DocumentSummaryInformation = dsi;
            //workbook.SummaryInformation = si;
            return workbook;
        }

        //private static void WriteBoldHead(ISheet sheet, string[] colHeads)
        //{
        //    IRow row = sheet.CreateRow(0);
        //    IFont f = sheet.Workbook.CreateFont();
        //    f.Boldweight = (short)FontBoldWeight.BOLD;
        //    ICellStyle style = GetICellStyle(sheet.Workbook, CellTypeEnum.Cell, false, f); //sheet.Workbook.CreateCellStyle();          

        //    for (int i = 0; i < colHeads.Length; i++)
        //    {                
        //        ////only care head, cells too big memory
        //        sheet.AutoSizeColumn(i);
        //        var wid = sheet.GetColumnWidth(i);
        //        sheet.SetColumnWidth(i, wid + 2000);

        //        ICell cell = row.CreateCell(i, CellType.STRING);
        //        cell.CellStyle = style;
        //        cell.SetCellValue(colHeads[i]);
        //    }
        //    row.RowStyle = style;
        //}

        private static void WriteHead(ISheet sheet, string[] colHeads)
        {
            IRow row = sheet.CreateRow(0);
            //row.Height = 300;

            for (int i = 0; i < colHeads.Length; i++)
            {
                ICell cell = row.CreateCell(i, CellType.String);
                //cell.CellStyle = headStyle;
                cell.SetCellValue(colHeads[i]);

                ////only care head, cells too big memory
                sheet.AutoSizeColumn(i);
                var wid = sheet.GetColumnWidth(i);
                sheet.SetColumnWidth(i, wid + 2000);
            }
        }

        private static void WriteData(IWorkbook workbook, ISheet sheet, string[] colNames, DataTable dataSource, bool lockhead, bool lockreadonly, List<string> editcolunms = null)
        {
            int rowNum = 1;
            var iCellStyles = GetICellStyles(workbook);

            IFont f = sheet.Workbook.CreateFont();
            f.FontName = "宋体";
            f.Boldweight = (short)FontBoldWeight.Bold;
            ICellStyle locked_header_styles = iCellStyles[CellTypeStyleEnum.LockedHead.ToString()];
            ICellStyle normal_header_styles = iCellStyles[CellTypeStyleEnum.NormalHead.ToString()];
            locked_header_styles.SetFont(f);
            normal_header_styles.SetFont(f);

            foreach (DataRow dr in dataSource.Rows)
            {
                for (int i = 0; i < colNames.Length; i++)
                {
                    if (dataSource.Columns.Contains(colNames[i]))
                    {
                        ICell cell = WriteCell(sheet, rowNum, i, dr[colNames[i]]);

                        if (editcolunms != null)
                        {
                            if (editcolunms.Contains(colNames[i]))
                            {
                                if (rowNum == 1)
                                {
                                    if (dataSource.Columns[colNames[i]].DataType.Name == typeof(DateTime).Name)
                                    {
                                        sheet.SetDefaultColumnStyle(cell.ColumnIndex, iCellStyles[CellTypeStyleEnum.NormalDateTimeCell.ToString()]);
                                    }
                                    else
                                    {
                                        sheet.SetDefaultColumnStyle(cell.ColumnIndex, iCellStyles[CellTypeStyleEnum.NormalCell.ToString()]);
                                    }
                                    sheet.GetRow(0).Cells[i].CellStyle = lockhead ? locked_header_styles : normal_header_styles;
                                    //sheet.GetRow(0).Cells[i].CellStyle = lockhead ? iCellStyles[CellTypeStyleEnum.LockedHead.ToString()] : iCellStyles[CellTypeStyleEnum.NormalHead.ToString()];
                                }

                                if (dataSource.Columns[colNames[i]].DataType.Name == typeof(DateTime).Name)
                                {
                                    cell.CellStyle = iCellStyles[CellTypeStyleEnum.NormalDateTimeCell.ToString()];
                                }
                                else
                                {
                                    cell.CellStyle = iCellStyles[CellTypeStyleEnum.NormalCell.ToString()];
                                }
                            }
                            else
                            {
                                if (rowNum == 1)
                                {
                                    if (dataSource.Columns[colNames[i]].DataType.Name == typeof(DateTime).Name)
                                    {

                                        sheet.SetDefaultColumnStyle(cell.ColumnIndex, lockreadonly ? iCellStyles[CellTypeStyleEnum.LockedDateTimeCell.ToString()] : iCellStyles[CellTypeStyleEnum.ReadOnlyColorDateTimeCell.ToString()]);
                                    }
                                    else
                                    {
                                        sheet.SetDefaultColumnStyle(cell.ColumnIndex, lockreadonly ? iCellStyles[CellTypeStyleEnum.LockedCell.ToString()] : iCellStyles[CellTypeStyleEnum.ReadOnlyColorCell.ToString()]);
                                    }

                                    sheet.GetRow(0).Cells[i].CellStyle = lockhead ? iCellStyles[CellTypeStyleEnum.LockedReadOnlyColorHead.ToString()] : iCellStyles[CellTypeStyleEnum.ReadOnlyColorHead.ToString()];
                                }

                                if (dataSource.Columns[colNames[i]].DataType.Name == typeof(DateTime).Name)
                                {
                                    cell.CellStyle = lockreadonly ? iCellStyles[CellTypeStyleEnum.LockedDateTimeCell.ToString()] : iCellStyles[CellTypeStyleEnum.ReadOnlyColorDateTimeCell.ToString()];
                                }
                                else
                                {
                                    cell.CellStyle = lockreadonly ? iCellStyles[CellTypeStyleEnum.LockedCell.ToString()] : iCellStyles[CellTypeStyleEnum.ReadOnlyColorCell.ToString()];
                                }
                            }
                        }
                        else
                        {
                            if (rowNum == 1)
                            {
                                if (dataSource.Columns[colNames[i]].DataType.Name == typeof(DateTime).Name)
                                {
                                    sheet.SetDefaultColumnStyle(cell.ColumnIndex, iCellStyles[CellTypeStyleEnum.NormalDateTimeCell.ToString()]);
                                }
                                else
                                {
                                    sheet.SetDefaultColumnStyle(cell.ColumnIndex, iCellStyles[CellTypeStyleEnum.NormalCell.ToString()]);
                                }
                                sheet.GetRow(0).Cells[i].CellStyle = lockhead ? iCellStyles[CellTypeStyleEnum.LockedHead.ToString()] : iCellStyles[CellTypeStyleEnum.NormalHead.ToString()];
                            }

                            if (dataSource.Columns[colNames[i]].DataType.Name == typeof(DateTime).Name)
                            {
                                cell.CellStyle = iCellStyles[CellTypeStyleEnum.NormalDateTimeCell.ToString()];
                            }
                            else
                            {
                                cell.CellStyle = iCellStyles[CellTypeStyleEnum.NormalCell.ToString()];
                            }
                        }
                    }
                }
                rowNum++;
            }
        }

        private static ICell WriteCell(ISheet sheet, int rowNum, int cellNum, object cellValue)
        {
            IRow row = sheet.GetRow(rowNum);
            if (row == null)
            {
                row = sheet.CreateRow(rowNum);
            }
            return WriteCell(row, cellNum, cellValue);
        }

        private static ICell WriteCell(IRow row, int cellNum, object cellValue)
        {
            ICell cell = row.GetCell(cellNum);
            if (cell == null)
            {
                cell = row.CreateCell(cellNum);
            }
            if (cellValue is string)
            {
                cell.SetCellValue((string)cellValue);
            }
            else if (cellValue is bool)
            {
                cell.SetCellValue((bool)cellValue);
            }
            else if (cellValue is DateTime)
            {
                cell.SetCellValue((DateTime)cellValue);
                //cell.SetCellValue(((DateTime) cellValue).ToString("yyyy-MM-dd"));
            }
            else if (cellValue.GetType().IsValueType)
            {
                cell.SetCellValue(Convert.ToDouble(cellValue));
            }
            else
            {
                cell.SetCellValue(cellValue.ToString());
            }
            return cell;
        }

        private enum CellTypeEnum
        {
            Cell,
            Head
        }

        private enum CellTypeStyleEnum
        {
            NormalCell,
            LockedCell,
            ReadOnlyColorCell,
            NormalDateTimeCell,
            LockedDateTimeCell,
            ReadOnlyColorDateTimeCell,
            NormalHead,
            ReadOnlyColorHead,
            LockedHead,
            LockedReadOnlyColorHead
        }
        #endregion

        public static bool ExportExcelforTemplate(string tempfile, string outfile, DataTable dt, int rowstart, bool isExcel03)
        {
            IWorkbook workbook = null;
            bool flag = false;
            try
            {
                FileStream readfile = new FileStream(tempfile, FileMode.Open);
                if (isExcel03)
                {
                    workbook = new HSSFWorkbook(readfile);
                }
                else
                {
                    workbook = new XSSFWorkbook(readfile);
                }
                readfile.Close();

                //边框线
                ICellStyle cellStyle = workbook.CreateCellStyle();
                cellStyle.BorderBottom = BorderStyle.Thin;
                cellStyle.BorderLeft = BorderStyle.Thin;
                cellStyle.BorderRight = BorderStyle.Thin;
                cellStyle.BorderTop = BorderStyle.Thin;
                IFont font = workbook.CreateFont();
                font.FontHeight = 10;
                font.FontName = "Arial";
                cellStyle.SetFont(font);

                //边框颜色
                cellStyle.BottomBorderColor = HSSFColor.Black.Index;
                cellStyle.TopBorderColor = HSSFColor.Black.Index;

                ISheet sheet = workbook.GetSheetAt(0);

                int rowNum = rowstart;
                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (sheet.GetRow(rowNum) == null)
                            sheet.CreateRow(rowNum);

                        if (sheet.GetRow(rowNum).GetCell(i) == null)
                            sheet.GetRow(rowNum).CreateCell(i);

                        if (dr[i].GetType() == typeof(decimal) || dr[i].GetType() == typeof(int) || dr[i].GetType() == typeof(double))
                        {
                            sheet.GetRow(rowNum).GetCell(i).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                            sheet.GetRow(rowNum).GetCell(i).SetCellValue(dr[i].ToString().ToDouble());
                        }
                        else
                            sheet.GetRow(rowNum).GetCell(i).SetCellValue(dr[i].ToString());

                        sheet.GetRow(rowNum).GetCell(i).CellStyle = cellStyle;//画线
                    }
                    rowNum++;
                }

                FileStream writefile = File.Create(outfile);
                workbook.Write(writefile);
                writefile.Close();

                flag = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return flag;
        }

        public static bool DataTableToExcelForScanReport(DataTable dt, string filePath, string po, string macFrom, string macTo, string snFrom, string snTo)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(dt.Copy());
            return DataSetToExcelForScanReport(ds, filePath,po,macFrom,macTo,snFrom,snTo);
        }
        public static bool DataSetToExcelForScanReport(DataSet ds, string filePath,string po,string macFrom,string macTo,string snFrom,string snTo)
        {
            try
            {
                IWorkbook workbook = new XSSFWorkbook();
                IRow row;
                ICell cell;
                int rowIndex = 0;
                //默认表头样式 
                XSSFCellStyle style_temp = (XSSFCellStyle)workbook.CreateCellStyle();
                style_temp.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                style_temp.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey40Percent.Index;
                style_temp.FillPattern = FillPattern.SolidForeground;
                style_temp.BorderBottom = BorderStyle.Thin;
                style_temp.BorderRight = BorderStyle.Thin;
                IFont font = workbook.CreateFont();
                font.Boldweight = (short)FontBoldWeight.Bold;
                style_temp.SetFont(font);
                //end
                foreach (DataTable dt in ds.Tables)
                {
                    ISheet sheet = workbook.CreateSheet(po);
                    //POnumber行
                    row = sheet.CreateRow(0);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("PO number:" + po);
                    cell = row.CreateCell(1);
                    cell.SetCellValue("MAC Address:");
                    cell = row.CreateCell(2);
                    cell.SetCellValue("Start:" + macFrom);
                    cell = row.CreateCell(3);
                    cell.SetCellValue("End:" + macTo);

                    row = sheet.CreateRow(1);
                    cell = row.CreateCell(1);
                    cell.SetCellValue("S/N:");
                    cell = row.CreateCell(2);
                    cell.SetCellValue("Start:" + snFrom);
                    cell = row.CreateCell(3);
                    cell.SetCellValue("End:" + snTo);

                    row = sheet.CreateRow(2);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("实际扫描情况");

                    rowIndex=3;
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = sheet.CreateRow(rowIndex++);
                        foreach (DataColumn column in dt.Columns)
                        {
                            if (column.ColumnName == "FullMAC" || column.ColumnName == "FullMAC2" || column.ColumnName == "FullSN" || column.ColumnName == "ScanTime")
                            {
                                cell = row.CreateCell(column.Ordinal-2);
                                if (column.DataType == typeof(decimal) || column.DataType == typeof(int) || column.DataType == typeof(double))
                                {
                                    cell.CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cell.SetCellValue(dr[column].ToString().ToDouble());
                                }
                                else if (column.DataType == typeof(DateTime))
                                {
                                    //HSSFDataFormat format = (HSSFDataFormat)excelWorkBook.CreateDataFormat();
                                    //cellStyle.DataFormat = format.GetFormat("yyyy-mm-dd hh:mm:ss");
                                    //newCell.CellStyle = cellStyle;
                                    cell.CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("yyyy-mm-dd hh:mm:ss");
                                    cell.SetCellValue(dr[column].ToString());
                                }
                                else
                                    cell.SetCellValue(dr[column].ToString());
                            }
                        }
                    }
                    rowIndex = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        sheet.AutoSizeColumn(i);
                    }
                }
                string fileName = System.IO.Path.Combine(filePath, po + ".xlsx");
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                FileStream sw = File.Create(fileName);
                workbook.Write(sw);
                sw.Close();
                sw.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
