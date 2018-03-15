using ReactorDB_DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ReactorDB_DAL
{
    public class LogDAO
    {
        public LogDAO(string connectionString, string logPath)
        {
            _connectionString = connectionString;
            _logger = new LoggerDAL(logPath);
        }
        private readonly string _connectionString;
        private LoggerDAL _logger;

        public void LogToDatabase(string timestamp, string message)
        {
            SqlConnection connectionToSql = null;
            SqlCommand logMessage = null;
            try
            {
                connectionToSql = new SqlConnection(_connectionString);
                logMessage = new SqlCommand("LOG_MESSAGE", connectionToSql);
                logMessage.CommandType = CommandType.StoredProcedure;
                logMessage.Parameters.AddWithValue("@Timestamp", timestamp);
                logMessage.Parameters.AddWithValue("@Message", message);
                connectionToSql.Open();
                logMessage.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
                throw ex;
            }
            finally
            {
                if (connectionToSql != null)
                {
                    connectionToSql.Close();
                    connectionToSql.Dispose();
                }
            }
        }

        public List<LogDO> ObtainRecentLogs()
        {
            List<LogDO> logs = new List<LogDO>();
            SqlConnection connectionToSql = null;
            SqlCommand obtainLogs = null;
            SqlDataAdapter adapter = null;
            DataTable logTable = new DataTable();

            try
            {
                connectionToSql = new SqlConnection(_connectionString);
                obtainLogs = new SqlCommand("OBTAIN_RECENT_LOGS", connectionToSql);
                obtainLogs.CommandType = CommandType.StoredProcedure;
                connectionToSql.Open();
                adapter = new SqlDataAdapter(obtainLogs);
                adapter.Fill(logTable);
                foreach(DataRow row in logTable.Rows)
                {
                    LogDO logDO = Mapping.Mapper.LogTableRowToDO(row);
                    logs.Add(logDO);
                }
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
                throw ex;
            }
            finally
            {
                if (connectionToSql != null)
                {
                    connectionToSql.Close();
                    connectionToSql.Dispose();
                }
                if (adapter != null)
                {
                    adapter.Dispose();
                }
            }
            return logs;
        }
    }
}
