using ReactorDB_DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace ReactorDB_DAL
{
    public class ReactorDAO
    {
        //constructor
        public ReactorDAO(string connectionString, string logPath)
        {
            _connectionString = connectionString;
            _logger = new LoggerDAL(logPath);
        }
        private readonly string _connectionString;
        private LoggerDAL _logger;

        public List<ReactorDO> ObtainAllReactors()
        {
            List<ReactorDO> reactorList = new List<ReactorDO>();
            SqlConnection connectionToSql = null;
            SqlCommand obtainReactors = null;
            SqlDataAdapter adapter = null;
            DataTable reactorsTable = new DataTable();

            try
            {
                //Instantiate SQL connection with connection string
                connectionToSql = new SqlConnection(_connectionString);
                //Instantiate SQL command for stored procedure to run
                obtainReactors = new SqlCommand("OBTAIN_REACTORS", connectionToSql);
                obtainReactors.CommandType = CommandType.StoredProcedure;

                //Open connection to SQL
                connectionToSql.Open();
                //Instantiate Data Adapter to fill table with records obtained from query
                adapter = new SqlDataAdapter(obtainReactors);
                adapter.Fill(reactorsTable);

                //Map each row to a Data Object, and create list of DOs for the method to return
                foreach (DataRow row in reactorsTable.Rows)
                {
                    ReactorDO reactorDO = Mapping.Mapper.ReactorTableRowToDO(row);
                    reactorList.Add(reactorDO);
                }
            }
            catch (Exception ex)
            {
                //Log exceptions with DAL logger
                _logger.LogMessage(ex, "Fatal");
                throw ex;
            }
            finally
            {
                //Close and dispose of connection, and dispose adapter
                if (connectionToSql != null)
                {
                    connectionToSql.Close();
                    connectionToSql.Dispose();
                }
                else { }
                if (adapter != null)
                {
                    adapter.Dispose();
                }
                else { }
            }
            return reactorList;
        }

        public ReactorDO ObtainReactorByID(int id)
        {
            ReactorDO reactorDO = null;
            SqlConnection connectionToSql = null;
            SqlCommand obtainReactorInfo = null;
            SqlDataAdapter adapter = null;
            DataTable reactorInfo = new DataTable();

            try
            {
                connectionToSql = new SqlConnection(_connectionString);
                obtainReactorInfo = new SqlCommand("OBTAIN_REACTOR_BY_ID", connectionToSql);
                obtainReactorInfo.CommandType = CommandType.StoredProcedure;
                obtainReactorInfo.Parameters.AddWithValue("@ReactorID", id);

                connectionToSql.Open();

                adapter = new SqlDataAdapter(obtainReactorInfo);
                adapter.Fill(reactorInfo);

                if (reactorInfo.Rows.Count > 0)
                {
                    DataRow row = reactorInfo.Rows[0];
                    reactorDO = Mapping.Mapper.ReactorTableRowToDO(row);
                }
                else
                {
                    _logger.LogMessage("Warning",
                                      "Queried ID not found",
                                      MethodBase.GetCurrentMethod().ToString(),
                                      "Did not create reactorDO. No rows found in reactorInfo table.");
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
            return reactorDO;
        }

        public int AddNewReactor(ReactorDO reactorDO)
        {
            int rowsAffected = 0;
            SqlConnection connectionToSql = null;
            SqlCommand addReactor = null;

            try
            {
                //Logging informational messages
                _logger.LogMessage("Info", "Add New Reactor start", MethodBase.GetCurrentMethod().ToString(),
                                    "Request to add new reactor to database received.");
                _logger.LogMessage("Instantiating SQL connection from connection string.");
                connectionToSql = new SqlConnection(_connectionString);
                _logger.LogMessage("Instantiating SQL command for stored procedure.");
                addReactor = new SqlCommand("ADD_NEW_REACTOR", connectionToSql);
                addReactor.CommandType = CommandType.StoredProcedure;

                //Method that adds parameter values to stored procedure
                _logger.LogMessage("Adding parameters to stored procedure.");
                AddParameterValues(addReactor, reactorDO);

                _logger.LogMessage("Opening SQL connection.");
                connectionToSql.Open();
                //Execute non query stored procedure and obtain number of rows affected
                rowsAffected = addReactor.ExecuteNonQuery();

                //Check whether data was added successfully, and log accordingly
                if (rowsAffected > 0)
                {
                    _logger.LogMessage("Info", "Nonquery completed", MethodBase.GetCurrentMethod().ToString(),
                                        "New reactor information was added to the database.");
                }
                else
                {
                    _logger.LogMessage("Warning", "No rows affected in nonquery", MethodBase.GetCurrentMethod().ToString(),
                                        "Reactor information was not added to the database.");
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
            //Allow method to check for success from MVC as well
            return rowsAffected;
        }

        public void UpdateReactor(ReactorDO reactorDO)
        {
            int rowsAffected = 0;
            SqlConnection connectionToSql = null;
            SqlCommand updateReactor = null;

            try
            {
                _logger.LogMessage("Info", "Update Reactor start", MethodBase.GetCurrentMethod().ToString(),
                                    "Request to update reactor with ID #" + reactorDO.ReactorID + " received.");
                _logger.LogMessage("Instantiating SQL connection from connection string.");
                connectionToSql = new SqlConnection(_connectionString);
                _logger.LogMessage("Instantiating SQL command for stored procedure.");
                updateReactor = new SqlCommand("UPDATE_REACTOR", connectionToSql);
                updateReactor.CommandType = CommandType.StoredProcedure;

                //Add parameters, including ID
                _logger.LogMessage("Adding parameters to stored procedure.");
                updateReactor.Parameters.AddWithValue("@ReactorID", reactorDO.ReactorID);
                AddParameterValues(updateReactor, reactorDO);

                _logger.LogMessage("Opening SQL connection.");
                connectionToSql.Open();
                rowsAffected = updateReactor.ExecuteNonQuery();
                //Check whether the row was affected, and log accordingly
                if (rowsAffected > 0)
                {
                    _logger.LogMessage("Info", "Nonquery completed", MethodBase.GetCurrentMethod().ToString(),
                                        "Reactor information was updated.");
                }
                else
                {
                    _logger.LogMessage("Warning", "No rows affected in nonquery", MethodBase.GetCurrentMethod().ToString(),
                                        "Reactor information was not updated.");
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

        public int DeleteReactorByID(int id)
        {
            int rowsAffected;
            SqlConnection connection = null;
            SqlCommand deleteReactor = null;

            try
            {
                _logger.LogMessage("Info", "Reactor deletion attempt", MethodBase.GetCurrentMethod().ToString(),
                                "Request to delete reactor with ID #" + id + " received.");
                connection = new SqlConnection(_connectionString);
                deleteReactor = new SqlCommand("DELETE_REACTOR_BY_ID", connection);
                deleteReactor.CommandType = CommandType.StoredProcedure;

                //Requires ID as only parameter
                _logger.LogMessage("Adding parameters to stored procedure.");
                deleteReactor.Parameters.AddWithValue("ReactorID", id);

                _logger.LogMessage("Opening connection to SQL.");
                connection.Open();
                _logger.LogMessage("Executing non-query stored procedure.");
                rowsAffected = deleteReactor.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    _logger.LogMessage("Info", "Reactor Deleted", MethodBase.GetCurrentMethod().ToString(),
                                        "Deletion of reactor with ID #" + id + " successful. " + rowsAffected + " database rows affected.");
                }
                else
                {
                    _logger.LogMessage("Warning", "Reactor deletion failed", MethodBase.GetCurrentMethod().ToString(),
                                        "Deletion of reactor with ID #" + id + " failed. " +
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

        public void AddParameterValues(SqlCommand storedProcedure, ReactorDO reactorDO)
        {
            //Adding parameter values to stored procedure from DO
            //Check if nullable fields are null to pass DBNull
            storedProcedure.Parameters.AddWithValue("@Name", reactorDO.Name);
            storedProcedure.Parameters.AddWithValue("@FullName", (object)reactorDO.FullName ?? DBNull.Value);
            storedProcedure.Parameters.AddWithValue("@IsThermal", reactorDO.IsThermal);
            storedProcedure.Parameters.AddWithValue("@ModeratorID", (reactorDO.ModeratorID != 0) ? (object)reactorDO.ModeratorID : DBNull.Value);
            storedProcedure.Parameters.AddWithValue("@PrimaryCoolant", reactorDO.PrimaryCoolant);
            storedProcedure.Parameters.AddWithValue("@Fuel", reactorDO.Fuel);
            storedProcedure.Parameters.AddWithValue("@ThermalPower", (reactorDO.ThermalPower != 0) ? (object)reactorDO.ThermalPower : DBNull.Value);
            storedProcedure.Parameters.AddWithValue("@ElectricPower", (reactorDO.ElectricPower != 0) ? (object)reactorDO.ElectricPower : DBNull.Value);
            storedProcedure.Parameters.AddWithValue("@Efficiency", (reactorDO.Efficiency != 0) ? (object)reactorDO.Efficiency : DBNull.Value);
            storedProcedure.Parameters.AddWithValue("@Year", reactorDO.Year);
            storedProcedure.Parameters.AddWithValue("@Generation", (object)reactorDO.Generation ?? DBNull.Value);
            storedProcedure.Parameters.AddWithValue("@CountryOfOrigin", (object)reactorDO.CountryOfOrigin ?? DBNull.Value);
            storedProcedure.Parameters.AddWithValue("@NumberActive", (reactorDO.NumberActive != 0) ? (object)reactorDO.NumberActive : DBNull.Value);
            storedProcedure.Parameters.AddWithValue("@Notes", (object)reactorDO.Notes ?? DBNull.Value);
        }

        public string ObtainModeratorNameByID(int id)
        {
            string moderator = null;

            SqlConnection connectionToSql = null;
            SqlCommand obtainModeratorName = null;
            SqlDataAdapter adapter = null;
            DataTable moderatorName = new DataTable();

            try
            {
                connectionToSql = new SqlConnection(_connectionString);
                obtainModeratorName = new SqlCommand("OBTAIN_MODERATOR_NAME", connectionToSql);
                obtainModeratorName.CommandType = CommandType.StoredProcedure;
                obtainModeratorName.Parameters.AddWithValue("@ModeratorID", id);

                connectionToSql.Open();

                adapter = new SqlDataAdapter(obtainModeratorName);
                adapter.Fill(moderatorName);

                if (moderatorName.Rows.Count > 0)
                {
                    DataRow row = moderatorName.Rows[0];
                    moderator = row["Name"].ToString().Trim();
                }
                else
                {
                    _logger.LogMessage("Warning",
                                      "Queried ID not found",
                                      MethodBase.GetCurrentMethod().ToString(),
                                      "Could not obtain moderator name.");
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
            return moderator;
        }
    }
}
