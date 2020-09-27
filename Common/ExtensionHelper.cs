using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DynamicDataChecking.Common
{
    public static class ExtensionHelper
    {
        #region String

        # region  基础操作扩展
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }
        public static bool IsNotNullOrEmpty(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }
        public static string FormatStr(this string format, params object[] args) { return string.Format(format, args); }
        public static string FormatStr(this string format, object arg0) { return string.Format(format, arg0); }
        public static string FormatStr(this string format, object arg0, object arg1) { return string.Format(format, arg0, arg1); }
        public static string FormatStr(this string format, object arg0, object arg1, object arg2) { return string.Format(format, arg0, arg1, arg2); }
        public static string FormatStr(this string format, IFormatProvider provider, params object[] args) { return string.Format(provider, format, args); }
        public static int ToInt(this string s)
        {
            int result = 0;
            int.TryParse(s, out result);
            return result;
        }
        public static decimal ToDecimal(this string s)
        {
            decimal result = 0;
            decimal.TryParse(s, out result);
            return result;
        }
        public static double ToDouble(this string s)
        {
            double result = 0;
            double.TryParse(s, out result);
            return result;
        }
        public static DateTime ToDateTime(this string s)
        {
            DateTime time;
            DateTime.TryParse(s, out time);
            return time;
        }
        public static bool IsMatch(this string s, string pattern)
        {
            if (s.IsNullOrEmpty()) return false;
            else return Regex.IsMatch(s, pattern);
        }

        public static string Match(this string s, string pattern)
        {
            if (s.IsNullOrEmpty()) return string.Empty;
            return Regex.Match(s, pattern).Value;
        }
        /// <summary>
        /// 剔除字符串中所有空格字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string TrimAll(this string s)
        {
            return s.Replace(" ", "");
        }
        /// <summary>
        /// 转全角(SBC case)
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>全角字符串</returns>
        public static string ToSBC(this string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }
        /// <summary>
        /// 转半角(DBC case)
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>半角字符串</returns>
        public static string ToDBC(this string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        public static string RemovePreZero(this string input)
        {
            string s = input.Trim();
            if (s.StartsWith("0"))
            {
                int index = 0;
                char[] c = input.ToCharArray();
                for (int i = 0; i < c.Length; i++)
                {
                    if (c[i] != '0')
                    {
                        index = i;
                        break;
                    }
                }
                return s.Substring(index);
            }
            else
            {
                return s;
            }
        }
        #endregion

        #region 文件及目录操作
        public static void CreateDirectory(this string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }
        public static void WriteText(this string path, string contents)
        {
            File.WriteAllText(path, contents);
        }
        public static void DeleteFile(this string path)
        {
            if (File.Exists(path)) File.Delete(path);
        }
        /// <summary>
        /// 获取绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFullPath(this string path)
        {
            return Path.GetFullPath(path);
        }
        /// <summary>
        /// 字符串末尾追加 _yyyyMMddHHmmss 日期时间
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AddDateTime(this string str)
        {
            return str += DateTime.Now.ToString("_yyyyMMddHHmmss");
        }

        #endregion

        #region  特殊扩展

        /// <summary>
        /// 启动一个进程 可以打开网址或文件等
        /// </summary>
        /// <param name="s"></param>
        public static void Open(this string s)
        {
            System.Diagnostics.Process.Start(s);
        }

        /// <summary>
        /// 执行DOS命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static string ExecuteDOS(this string cmd, out string error)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.StandardInput.WriteLine(cmd);
            process.StandardInput.WriteLine("exit");
            error = process.StandardError.ReadToEnd();
            return process.StandardOutput.ReadToEnd();
        }
        #endregion
        #endregion

        #region DataTable

        /// <summary>
        /// 获取以,分隔的表头字符串集
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetColumnNames(this DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataColumn item in dt.Columns)
            {
                sb.Append(item.ColumnName);
                sb.Append(',');
            }
            return sb.ToString().Trim(',');
        }


        /// <summary>
        ///  更改DataTable表头名称
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnNames">需要更改表头名称集合</param>
        /// <param name="columnNames1">对应的更改后表头名称集合</param>
        public static void SetColumnNames(this DataTable dt, string[] columnNames, string[] columnNames1)
        {
            for (int i = 0; i < columnNames.Length; i++)
            {
                dt.Columns[i].ColumnName = columnNames1[i];
            }
        }

        /// <summary>
        ///  更改DataTable表头名称
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnNames">对应的更改后表头名称集合</param>
        public static void SetColumnNames(this DataTable dt, string[] columnNames)
        {
            for (int i = 0; i < columnNames.Length; i++)
            {
                dt.Columns[i].ColumnName = columnNames[i];
            }
        }
        /// <summary>
        /// 获取指定ColumnNames数据,可选剔除重复
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        public static DataTable GetDT(this DataTable dt, string[] columnNames, bool distinct = false)
        {
            return dt.DefaultView.ToTable(distinct, columnNames);
        }
        /// <summary>
        /// 获取N行数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="topCount">行数</param>
        /// <returns></returns>
        public static DataTable GetTopRows(this DataTable dt, int topCount)
        {
            if (dt.Rows.Count < topCount) return dt;
            DataTable NewTable = dt.Clone();
            for (int i = 0; i < topCount; i++)
                NewTable.ImportRow(dt.Rows[i]);
            return NewTable;
        }

        /// <summary>
        /// 获取指定列数据 将其转换成以，分隔的字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="ColumnName"></param>
        /// <param name="isStr">是否带有''，默认true</param>
        /// <returns></returns>
        public static string GetCellStr(this DataTable dt, string ColumnName, bool isStr = true)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRow dr in dt.Rows)
            {
                if (isStr)
                    sb.Append('\'' + dr[ColumnName].ToString() + '\'');
                else
                    sb.Append(dr[ColumnName].ToString());
                sb.Append(',');
            }
            return sb.ToString().Trim(',');
        }
        /// <summary>
        /// DataTable 转换成 List<T>
        /// </summary>
        public static List<T> ToList<T>(this System.Data.DataTable dt) where T : class,new()
        {
            List<T> list = new List<T>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                T t = new T();
                var row = dt.Rows[i];
                var propertis = t.GetType().GetProperties().ToList();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    var column = dt.Columns[j];
                    var properinfo = propertis.FirstOrDefault(b => b.Name == column.ColumnName);
                    if (properinfo != null && row[properinfo.Name].ToString().IsNotNullOrEmpty())
                    {
                        properinfo.SetValue(t, row[properinfo.Name], null);
                    }
                }
                list.Add(t);
            }
            return list;
        }
        /// <summary>
        /// 移除空数据行
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DataTable TrimEmptyRows(this System.Data.DataTable dt)
        {
            List<DataRow> list = new List<DataRow>();
            foreach (DataRow dr in dt.Rows)
            {
                int columnsCount = dt.Columns.Count;
                for (int i = 0; i < columnsCount; i++)
                {
                    if (dr[i].ToString().Trim().IsNotNullOrEmpty())
                        break;
                    if (i == columnsCount - 1)
                        list.Add(dr);
                }
            }
            if (list.Count > 0)
            {
                foreach (DataRow item in list)
                {
                    dt.Rows.Remove(item);
                }
            }
            return dt;
        }
        #endregion
    }
}