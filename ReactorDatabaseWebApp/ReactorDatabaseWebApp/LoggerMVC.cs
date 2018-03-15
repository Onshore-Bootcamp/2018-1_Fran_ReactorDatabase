using ReactorDB_DAL;
using System;
using System.Configuration;
using System.IO;

namespace ReactorDatabaseWebApp
{
    public class LoggerMVC
    {
        public LoggerMVC(string logPath, string connectionString)
        {
            _logPath = logPath;
            _connectionString = connectionString;
            _logDAO = new LogDAO(connectionString, logPath);
        }
        private readonly string _logPath;
        private readonly string _connectionString;
        private LogDAO _logDAO;

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
                else { }

                if (level == "Warning")
                {
                    _logDAO.LogToDatabase(timeStamp, message);
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