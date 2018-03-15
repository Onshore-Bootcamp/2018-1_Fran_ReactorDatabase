using ReactorDB_DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace ReactorDB_DAL
{
    public class ModeratorDAO
    {
        public ModeratorDAO(string connectionString, string logPath)
        {
            _connectionString = connectionString;
            _logger = new LoggerDAL(logPath);
        }

        private readonly string _connectionString;
        private LoggerDAL _logger;

        public List<ModeratorDO> ObtainAllModerators()
        {
            List<ModeratorDO> moderatorList = new List<ModeratorDO>();
            SqlConnection connectionToSql = null;
            SqlCommand obtainModerators = null;
            SqlDataAdapter adapter = null;
            DataTable moderatorsTable = new DataTable();

            try
            {
                connectionToSql = new SqlConnection(_connectionString);
                obtainModerators = new SqlCommand("OBTAIN_MODERATORS", connectionToSql);
                obtainModerators.CommandType = CommandType.StoredProcedure;

                connectionToSql.Open();
                adapter = new SqlDataAdapter(obtainModerators);
                adapter.Fill(moderatorsTable);

                foreach (DataRow row in moderatorsTable.Rows)
                {
                    ModeratorDO moderatorDO = Mapping.Mapper.ModeratorTableRowToDO(row);
                    moderatorList.Add(moderatorDO);
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
            return moderatorList;
        }

        public ModeratorDO ObtainModeratorByID(int id)
        {
            ModeratorDO moderatorDO = null;
            SqlConnection connectionToSql = null;
            SqlCommand obtainModeratorInfo = null;
            SqlDataAdapter adapter = null;
            DataTable moderatorInfo = new DataTable();

            try
            {
                connectionToSql = new SqlConnection(_connectionString);
                obtainModeratorInfo = new SqlCommand("OBTAIN_MODERATOR_BY_ID", connectionToSql);
                obtainModeratorInfo.CommandType = CommandType.StoredProcedure;
                obtainModeratorInfo.Parameters.AddWithValue("@ModeratorID", id);

                connectionToSql.Open();

                adapter = new SqlDataAdapter(obtainModeratorInfo);
                adapter.Fill(moderatorInfo);

                if (moderatorInfo.Rows.Count > 0)
                {
                    DataRow row = moderatorInfo.Rows[0];
                    moderatorDO = Mapping.Mapper.ModeratorTableRowToDO(row);
                }
                else
                {
                    _logger.LogMessage("Warning",
                                      "Queried ID not found",
                                      MethodBase.GetCurrentMethod().ToString(),
                                      "Did not create moderatorDO. No rows found in moderatorInfo table.");
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
            return moderatorDO;
        }

        public int AddNewModerator(ModeratorDO moderatorDO)
        {
            int rowsAffected = 0;
            SqlConnection connectionToSql = null;
            SqlCommand addModerator = null;

            try
            {
                _logger.LogMessage("Info", "Add New Moderator start", MethodBase.GetCurrentMethod().ToString(),
                                    "Request to add new moderator to database received.");
                _logger.LogMessage("Instantiating SQL connection from connection string.");
                connectionToSql = new SqlConnection(_connectionString);
                _logger.LogMessage("Instantiating SQL command for stored procedure.");
                addModerator = new SqlCommand("ADD_NEW_MODERATOR", connectionToSql);
                addModerator.CommandType = CommandType.StoredProcedure;

                _logger.LogMessage("Adding parameters to stored procedure.");
                AddParameterValues(addModerator, moderatorDO);

                _logger.LogMessage("Opening SQL connection.");
                connectionToSql.Open();
                rowsAffected = addModerator.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    _logger.LogMessage("Info", "Nonquery completed", MethodBase.GetCurrentMethod().ToString(),
                                        "New moderator information was added to the database.");
                }
                else
                {
                    _logger.LogMessage("Warning", "No rows affected in nonquery", MethodBase.GetCurrentMethod().ToString(),
                                        "Moderator information was not added to the database.");
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
            }
            return rowsAffected;
        }

        public void UpdateModerator(ModeratorDO moderatorDO)
        {
            int rowsAffected = 0;
            SqlConnection connectionToSql = null;
            SqlCommand updateModerator = null;

            try
            {
                _logger.LogMessage("Info", "Update Moderator start", MethodBase.GetCurrentMethod().ToString(),
                                    "Request to update moderator with ID #" + moderatorDO.ModeratorID + " received.");
                _logger.LogMessage("Instantiating SQL connection from connection string.");
                connectionToSql = new SqlConnection(_connectionString);
                _logger.LogMessage("Instantiating SQL command for stored procedure.");
                updateModerator = new SqlCommand("UPDATE_MODERATOR", connectionToSql);
                updateModerator.CommandType = CommandType.StoredProcedure;

                _logger.LogMessage("Adding parameters to stored procedure.");
                updateModerator.Parameters.AddWithValue("@ModeratorID", moderatorDO.ModeratorID);
                AddParameterValues(updateModerator, moderatorDO);

                _logger.LogMessage("Opening SQL connection.");
                connectionToSql.Open();
                rowsAffected = updateModerator.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    _logger.LogMessage("Info", "Nonquery completed", MethodBase.GetCurrentMethod().ToString(),
                                        "Moderator information was updated.");
                }
                else
                {
                    _logger.LogMessage("Warning", "No rows affected in nonquery", MethodBase.GetCurrentMethod().ToString(),
                                        "Moderator information was not updated.");
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
            }
        }

        public int DeleteModeratorByID(int id)
        {
            int rowsAffected = 0;
            SqlConnection connection = null;
            SqlCommand deleteModerator = null;

            try
            {
                _logger.LogMessage("Info", "Moderator deletion attempt", MethodBase.GetCurrentMethod().ToString(),
                                "Request to delete moderator with ID #" + id + " received.");
                connection = new SqlConnection(_connectionString);
                deleteModerator = new SqlCommand("DELETE_MODERATOR_BY_ID", connection);
                deleteModerator.CommandType = CommandType.StoredProcedure;

                _logger.LogMessage("Adding parameter to stored procedure.");
                deleteModerator.Parameters.AddWithValue("ModeratorID", id);

                _logger.LogMessage("Opening connection to SQL.");
                connection.Open();
                _logger.LogMessage("Executing non-query stored procedure.");
                rowsAffected = deleteModerator.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    _logger.LogMessage("Info", "Moderator Deleted", MethodBase.GetCurrentMethod().ToString(),
                                        "Deletion of moderator with ID #" + id + " successful. " + rowsAffected + " database rows affected.");
                }
                else
                {
                    _logger.LogMessage("Warning", "Moderator deletion failed", MethodBase.GetCurrentMethod().ToString(),
                                        "Deletion of moderator with ID #" + id + " failed. " +
                                        "Possible deletion request on invalid ID.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return rowsAffected;
        }

        public void AddParameterValues(SqlCommand storedProcedure, ModeratorDO moderatorDO)
        {
            storedProcedure.Parameters.AddWithValue("@Name", moderatorDO.Name);
            storedProcedure.Parameters.AddWithValue("@ChemicalFormula", (object)moderatorDO.ChemicalFormula ?? DBNull.Value);
            storedProcedure.Parameters.AddWithValue("@Nucleus", moderatorDO.Nucleus);
            storedProcedure.Parameters.AddWithValue("@AtomicMass", moderatorDO.AtomicMass);
            storedProcedure.Parameters.AddWithValue("@EnergyDecrement", (moderatorDO.EnergyDecrement != 0) ? (object)moderatorDO.EnergyDecrement : DBNull.Value);
            storedProcedure.Parameters.AddWithValue("@NumberOfCollisions", (moderatorDO.Collisions != 0) ? (object)moderatorDO.Collisions : DBNull.Value);
            storedProcedure.Parameters.AddWithValue("@ScatteringXS", (moderatorDO.ScatteringXS != 0) ? (object)moderatorDO.ScatteringXS : DBNull.Value);
            storedProcedure.Parameters.AddWithValue("@AbsorptionXS", (moderatorDO.AbsorptionXS != 0) ? (object)moderatorDO.AbsorptionXS : DBNull.Value);
            storedProcedure.Parameters.AddWithValue("@Efficiency", (moderatorDO.ModerationEfficiency != 0) ? (object)moderatorDO.ModerationEfficiency : DBNull.Value);
        }
    }
}
