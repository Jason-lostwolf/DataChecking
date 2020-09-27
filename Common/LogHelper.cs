using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataChecking.Common
{
    public class LogHelper
    {
        /// <summary>
        /// 写入系统日志
        /// </summary>
        /// <param name="strLog">记录到日志文件的信息</param>
        public static void WriteLog(string strLog)
        {
            string DateTimeToTxt = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            string strPath = string.Empty;

            strPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"log";

            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
            strPath = strPath + "\\" + DateTimeToTxt;
            FileStream fs = new FileStream(strPath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter thisStreamWriter = new StreamWriter(fs, Encoding.UTF8);
            thisStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
            thisStreamWriter.WriteLine("--------------------------------------------------");
            thisStreamWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"));
            thisStreamWriter.WriteLine("--------------------------------------------------");
            thisStreamWriter.WriteLine(strLog);
            thisStreamWriter.WriteLine();//用来区分下一条日志
            thisStreamWriter.Flush();
            thisStreamWriter.Close();
            fs.Close();
        }

        /// <summary>
        /// 写入系统日志
        /// </summary>
        /// <param name="strLog">记录到日志文件的信息</param>
        public static void WriteLog(string module, string function, string strLog)
        {
            string DateTimeToTxt = DateTime.Now.ToString("yyyy-MM") + ".txt";
            string strPath = string.Empty;

            strPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"log";

            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
            strPath = strPath + "\\" + DateTimeToTxt;
            FileStream fs = new FileStream(strPath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter thisStreamWriter = new StreamWriter(fs, Encoding.UTF8);
            thisStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
            // thisStreamWriter.WriteLine("");
            thisStreamWriter.WriteLine("--------------------------------------------------");
            thisStreamWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"));
            thisStreamWriter.WriteLine("--------------------------------------------------");
            thisStreamWriter.WriteLine(string.Format("Module Name: {0}, Function Name: {1}, Issue Time: {2},    Message:    {3}", module.Trim(), function.Trim(), DateTime.Now.ToString("yy-MM-dd HH:mm:ss:ffff"), strLog));
            // thisStreamWriter.WriteLine(string.Format("Message: {0}", strLog));
            //thisStreamWriter.WriteLine(string.Format("Module Name: {0}, Function Name: {1}", module, function));
            //thisStreamWriter.WriteLine(string.Format("Issue Time: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff")));
            //thisStreamWriter.WriteLine(string.Format("Message: {0}", strLog));
            thisStreamWriter.WriteLine("");
            thisStreamWriter.Flush();
            thisStreamWriter.Close();
            fs.Close();
        }

        public static void WritePrintLog(string content,string functionName)
        {
            string DateTimeToTxt = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            string strPath = string.Empty;

            strPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Printlog";

            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
            strPath = strPath + "\\" + DateTimeToTxt;
            FileStream fs = new FileStream(strPath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter thisStreamWriter = new StreamWriter(fs, Encoding.UTF8);
            thisStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
            // thisStreamWriter.WriteLine("");
            thisStreamWriter.WriteLine(string.Format("Print Time:{0},FunctionName:{1},Content:{2}",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"), functionName,content));
            thisStreamWriter.Flush();
            thisStreamWriter.Close();
            fs.Close();
        }
    }
}
