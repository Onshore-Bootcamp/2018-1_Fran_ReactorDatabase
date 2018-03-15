using ReactorDB_DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace ReactorDB_DAL
{
    public class BookmarkDAO
    {
        public BookmarkDAO(string connectionString, string logPath)
        {
            _connectionString = connectionString;
            _logger = new LoggerDAL(logPath);
        }

        private readonly string _connectionString;            
        private LoggerDAL _logger;

        public List<BookmarkDO> ObtainBookmarksByUserID(int userID)
        {
            _logger.LogMessage("Info", "Obtain Bookmarks called", MethodBase.GetCurrentMethod().ToString(),
                                "Request to obtain bookmarks for user with ID #" + userID + " received.");
            List<BookmarkDO> bookmarkList = new List<BookmarkDO>();
            SqlConnection connectionToSql = null;
            SqlCommand obtainUserBookmarks = null;
            SqlDataAdapter adapter = null;
            DataTable bookmarkTable = new DataTable();

            try
            {
                connectionToSql = new SqlConnection(_connectionString);
                obtainUserBookmarks = new SqlCommand("OBTAIN_USER_BOOKMARKS", connectionToSql);
                obtainUserBookmarks.CommandType = CommandType.StoredProcedure;
                obtainUserBookmarks.Parameters.AddWithValue("@UserID", userID);

                connectionToSql.Open();
                adapter = new SqlDataAdapter(obtainUserBookmarks);
                adapter.Fill(bookmarkTable);

                foreach (DataRow row in bookmarkTable.Rows)
                {
                    BookmarkDO bookmarkDO = Mapping.Mapper.BookmarkTableRowToDO(row);
                    bookmarkList.Add(bookmarkDO);
                }

                if (bookmarkList.Count > 0)
                {
                    _logger.LogMessage("BookmarkDO list obtained.");
                }
                else
                {
                    _logger.LogMessage("No bookmarks found.");
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
            return bookmarkList;
        }

        public void AddBookmark(int userID, int reactorID)
        {
            _logger.LogMessage("Info", "Add Bookmark request received", MethodBase.GetCurrentMethod().ToString(),
                                "User with ID #" + userID + " requesting to bookmark reactor with ID #" + reactorID + ".");
            SqlConnection connectionToSql = null;
            SqlCommand addBookmark = null;

            try
            {
                connectionToSql = new SqlConnection(_connectionString);
                addBookmark = new SqlCommand("ADD_BOOKMARK", connectionToSql);
                addBookmark.CommandType = CommandType.StoredProcedure;

                addBookmark.Parameters.AddWithValue("@UserID", userID);
                addBookmark.Parameters.AddWithValue("@ReactorID", reactorID);

                connectionToSql.Open();
                addBookmark.ExecuteNonQuery();
                _logger.LogMessage("Bookmark added successfully");
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

        public int DeleteBookmark(int userID, int reactorID)
        {
            _logger.LogMessage("Info", "Delete Bookmark Request", MethodBase.GetCurrentMethod().ToString(),
                                "Request to delete bookmark for User ID #" + userID + " received.");
            int rowsAffected;
            SqlConnection connectionToSql = null;
            SqlCommand deleteBookmark = null;

            try
            {
                connectionToSql = new SqlConnection(_connectionString);
                deleteBookmark = new SqlCommand("DELETE_BOOKMARK", connectionToSql);
                deleteBookmark.CommandType = CommandType.StoredProcedure;
                deleteBookmark.Parameters.AddWithValue("@UserID", userID);
                deleteBookmark.Parameters.AddWithValue("@ReactorID", reactorID);

                connectionToSql.Open();
                rowsAffected = deleteBookmark.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    _logger.LogMessage("Info", "Bookmark deleted", MethodBase.GetCurrentMethod().ToString(),
                                        "Deletion of bookmark with User ID #" + userID + " and Reactor ID #" + reactorID + " successful.");
                }
                else
                {
                    _logger.LogMessage("Warning", "Bookmark deletion failed", MethodBase.GetCurrentMethod().ToString(),
                                        "Deletion of bookmark with User ID #" + userID + " and Reactor ID #" + reactorID + " failed. Possible deletion attempt on invalid ID.");
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
    }
}
