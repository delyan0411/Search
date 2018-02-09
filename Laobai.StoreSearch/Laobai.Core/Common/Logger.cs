using System;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;

namespace Laobai.Core.Common
{
    public class Logger
    {
        #region WriteLog
        private static void WriteLog(string val, string folder)
        {
            string logPath = Laobai.Core.Common.Settings.LogsPath;
            if (!logPath.EndsWith("\\"))
                logPath += "\\";
            string path = logPath + folder + "\\"
                + DateTime.Now.ToString("yyyy-MM")
                + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            //path = HttpUtils.GetMapPath(path);
            Utils.CreateDir(path);
            path += DateTime.Now.ToString("yyyyMMddHH") + ".log";

            System.Text.StringBuilder sb = new StringBuilder();
            sb.AppendLine(">>>Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine(val);
            sb.AppendLine("<<<-------------------------------------------------------------------");
            Utils.AppenText(path, sb.ToString());
        }
        #endregion

        #region Log
        public static void Log(string val)
        {
            WriteLog(val, "Log");
        }
        #endregion

        #region Debug
        public static void Debug(string val)
        {
            WriteLog(val, "Debug");
        }
        #endregion

        #region Error
        public static void Error(string val)
        {
            WriteLog(val, "Error");
        }
        #endregion
    }
}
