using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDataChecking.Common.CommonClass
{
    public static class SystemConst
    {
        public const string ExcelFilter = "Excel File|*.xlsx;*.xls";
        public const string CSV_Filter = "Text File|*.txt;*.csv";
        public const string Export_CSV_Filter = "CSV File|*.csv";

        public static string UserName { get; set; }
        public static int UserID { get; set; }
        public static string RoleID { get; set; }
        public static List<string> RoleAuthorizationList { get; set; }

        public static string ImportTempatePath
        {
            get
            {
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImportTemplate");
            }
        }
    }
}
