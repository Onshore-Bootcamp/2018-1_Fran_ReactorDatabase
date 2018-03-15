using ReactorDB_DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace ReactorDB_DAL
{
    public class UserDAO
    {
        public UserDAO(string connectionString, string logPath)
        {
            _connectionString = connectionString;
            _logger = new LoggerDAL(logPath);
        }
        private readonly string _connectionString;
        private LoggerDAL _logger;

        public UserDO ObtainUserByUsername(string username)
        {
            UserDO userDO = null;
            SqlConnection connectionToSql = null;
            SqlCommand obtainUserInfo = null;
            SqlDataAdapter adapter = null;
            DataTable userInfo = new DataTable();

            try
            {
                _logger.LogMessage("Info", "Obtain User By Username called", MethodBase.GetCurrentMethod().ToString(),
                                    "Attempting to obtain record for username '" + username + "'.");
                connectionToSql = new SqlConnection(_connectionString);
                obtainUserInfo = new SqlCommand("OBTAIN_USER_BY_USERNAME", connectionToSql);
                obtainUserInfo.CommandType = CommandType.StoredProcedure;
                obtainUserInfo.Parameters.AddWithValue("@Username", username);

                connectionToSql.Open();

                adapter = new SqlDataAdapter(obtainUserInfo);
                adapter.Fill(userInfo);

                if (userInfo.Rows.Count > 0)
                {
                    _logger.LogMessage("User record found. Mapping to UserDO.");
                    DataRow row = userInfo.Rows[0];
                    userDO = Mapping.Mapper.UserTableRowToDO(row);
                }
                else
                {
                    _logger.LogMessage("User not found.");
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
            return userDO;
        }

        public List<UserDO> ObtainAllUsers()
        {
            List<UserDO> userList = new List<UserDO>();
            SqlConnection connectionToSql = null;
            SqlCommand obtainUsers = null;
            SqlDataAdapter adapter = null;
            DataTable usersTable = new DataTable();

            try
            {
                connectionToSql = new SqlConnection(_connectionString);
                obtainUsers = new SqlCommand("OBTAIN_USERS", connectionToSql);
                obtainUsers.CommandType = CommandType.StoredProcedure;

                connectionToSql.Open();
                adapter = new SqlDataAdapter(obtainUsers);
                adapter.Fill(usersTable);

                foreach (DataRow row in usersTable.Rows)
                {
                    UserDO userDO = Mapping.Mapper.UserTableRowToDO(row);
                    userList.Add(userDO);
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
            return userList;
        }

        public int AddNewUser(UserDO userDO)
        {
            int rowsAffected = default(int);
            SqlConnection connectionToSql = null;
            SqlCommand addUser = null;

            try
            {
                _logger.LogMessage("Info", "Add New User start", MethodBase.GetCurrentMethod().ToString(),
                                    "Request to add new user to database received.");
                connectionToSql = new SqlConnection(_connectionString);
                addUser = new SqlCommand("ADD_NEW_USER", connectionToSql);
                addUser.CommandType = CommandType.StoredProcedure;

                _logger.LogMessage("Adding parameters to stored procedure.");
                AddParameterValues(addUser, userDO);

                _logger.LogMessage("Opening connection to SQL and executing nonquery.");
                connectionToSql.Open();
                rowsAffected = addUser.ExecuteNonQuery();
                if (rowsAffected != default(int))
                {
                    _logger.LogMessage("Info", "Nonquery completed", MethodBase.GetCurrentMethod().ToString(),
                                        "New user information was added to the database.");
                }
                else
                {
                    _logger.LogMessage("Warning", "No rows affected in nonquery", MethodBase.GetCurrentMethod().ToString(),
                                        "User information was not added to the database.");
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

        public void UpdateUser(UserDO userDO)
        {
            SqlConnection connectionToSql = null;
            SqlCommand updateUser = null;

            try
            {
                _logger.LogMessage("Info", "Update User start", MethodBase.GetCurrentMethod().ToString(),
                                    "Request to update user with ID #" + userDO.UserID + " received.");
                connectionToSql = new SqlConnection(_connectionString);
                updateUser = new SqlCommand("UPDATE_USER", connectionToSql);
                updateUser.CommandType = CommandType.StoredProcedure;

                _logger.LogMessage("Adding parameters to stored procedure.");
                updateUser.Parameters.AddWithValue("@UserID", userDO.UserID);
                AddParameterValues(updateUser, userDO);

                _logger.LogMessage("Opening connection to SQL and executing nonquery.");
                connectionToSql.Open();
                updateUser.ExecuteNonQuery();
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

        public void UpdatePassword(UserDO userDO)
        {
            SqlConnection connectionToSql = null;
            SqlCommand updatePassword = null;

            try
            {
                _logger.LogMessage("Info", "Update Password start", MethodBase.GetCurrentMethod().ToString(),
                                    "Request to update password for User with ID #" + userDO.UserID + " received.");
                connectionToSql = new SqlConnection(_connectionString);
                updatePassword = new SqlCommand("UPDATE_USER_PASSWORD", connectionToSql);
                updatePassword.CommandType = CommandType.StoredProcedure;

                _logger.LogMessage("Adding parameters to stored procedure.");
                updatePassword.Parameters.AddWithValue("@UserID", userDO.UserID);
                updatePassword.Parameters.AddWithValue("@Password", userDO.Password);

                _logger.LogMessage("Opening connection to SQL and executing nonquery.");
                connectionToSql.Open();
                updatePassword.ExecuteNonQuery();
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

        public int DeleteUserByID(int id)
        {
            int rowsAffected = 0;
            SqlConnection connection = null;
            SqlCommand deleteUser = null;

            try
            {
                _logger.LogMessage("Info", "User deletion attempt", MethodBase.GetCurrentMethod().ToString(),
                                "Request to delete user with ID #" + id + " received.");
                connection = new SqlConnection(_connectionString);
                deleteUser = new SqlCommand("DELETE_USER_BY_ID", connection);
                deleteUser.CommandType = CommandType.StoredProcedure;

                _logger.LogMessage("Adding parameter to stored procedure.");
                deleteUser.Parameters.AddWithValue("UserID", id);

                _logger.LogMessage("Opening connection to SQL.");
                connection.Open();
                _logger.LogMessage("Executing non-query stored procedure.");
                rowsAffected = deleteUser.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    _logger.LogMessage("Info", "User Deleted", MethodBase.GetCurrentMethod().ToString(),
                                        "Deletion of user with ID #" + id + " successful. " + rowsAffected + " database rows affected.");
                }
                else
                {
                    _logger.LogMessage("Warning", "User deletion failed", MethodBase.GetCurrentMethod().ToString(),
                                        "Deletion of user with ID #" + id + " failed. " +
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

        public void AddParameterValues(SqlCommand storedProcedure, UserDO userDO)
        {
            storedProcedure.Parameters.AddWithValue("@Username", userDO.Username);
            storedProcedure.Parameters.AddWithValue("@Password", userDO.Password);
            storedProcedure.Parameters.AddWithValue("@RoleID", userDO.RoleID);
            storedProcedure.Parameters.AddWithValue("@FirstName", userDO.FirstName);
            storedProcedure.Parameters.AddWithValue("@LastName", (object)userDO.LastName ?? DBNull.Value);
            storedProcedure.Parameters.AddWithValue("@Age", (userDO.Age != default(int)) ? (object)userDO.Age : DBNull.Value);
            storedProcedure.Parameters.AddWithValue("@Email", userDO.Email);
            storedProcedure.Parameters.AddWithValue("@Subscription", (object)userDO.Subscription ?? DBNull.Value);
        }
    }
}
