using System;
using System.Configuration;
using System.IO;

namespace ReactorDB_DAL
{
    public class LoggerDAL
    {
        public LoggerDAL(string logPath)
        {
            _logPath = logPath;
        }

        private readonly string _logPath;

        public void LogMessage(Exception ex, string level)
        {
            LogMessage(level, ex.Source, ex.TargetSite.Name, ex.Message, ex.StackTrace);
        }

        public void LogMessage(string level, string source, string target, string message, string stackTrace = null)
        {
            string timeStamp = DateTime.Now.ToString();
            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(_logPath, true);
                writer.WriteLine("[{0}] - {1} - {2} - {3} - {4}",
                                 timeStamp, level, source, target, message);
                if (stackTrace != null)
                {
                    writer.WriteLine(stackTrace);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
            }
        }

        public void LogMessage(string message)
        {
            string timeStamp = DateTime.Now.ToString();
            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(_logPath, true);
                writer.WriteLine("    [{0}] {1}",
                                 timeStamp, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
            }
        }
    }
}
