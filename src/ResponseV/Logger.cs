using System.Collections.Generic;
using System.Linq;

namespace ResponseV
{
    internal class Logger
    {
        public enum ELogLevel
        {
            LL_INFO,
            LL_WARNING,
            LL_ERROR,
            LL_TRACE
        }

        private readonly string m_LogFile = $"{System.Windows.Forms.Application.StartupPath}\\Plugins\\LSPDFR\\ResponseV.log";

        public Logger()
        {
            if (System.IO.File.Exists(m_LogFile))
            {
                System.IO.File.Delete(m_LogFile);
            }

            // TODO: if m_AppVersion > what's in the log then delete the log

            WriteLog("--------------------");
            WriteLog($"ResponseV v{Main.m_AppVersion?.ToString()}");
            WriteLog("Initialized Log");
            WriteLog("--------------------");
        }

        public void Log(string Message, ELogLevel LogLevel = ELogLevel.LL_INFO)
        {
            string prefix;
            switch (LogLevel)
            {
            default:
            case ELogLevel.LL_INFO: prefix = "[INFO] "; break;
            case ELogLevel.LL_WARNING: prefix = "[WARNING] "; break;
            case ELogLevel.LL_ERROR: prefix = "[ERROR] "; break;
            case ELogLevel.LL_TRACE: prefix = "[TRACE] "; break;
            }

            WriteLog($"[{System.DateTime.Now}] ResponseV {prefix}: {Message}");
        }

        private void WriteLog(string Message)
        {
            if (string.IsNullOrEmpty(Message) || string.IsNullOrWhiteSpace(Message)) return;

            try
            {
                using (System.IO.StreamWriter StreamWriter = new System.IO.StreamWriter(m_LogFile, true, System.Text.Encoding.UTF8))
                {
                    StreamWriter.WriteLine(Message);
                }
            }
            catch { }
        }
    }
}
