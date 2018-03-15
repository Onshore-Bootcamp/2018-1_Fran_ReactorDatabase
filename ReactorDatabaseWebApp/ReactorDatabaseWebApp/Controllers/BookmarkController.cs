using ReactorDatabaseWebApp.CustomCode;
using ReactorDatabaseWebApp.Models;
using ReactorDB_DAL;
using ReactorDB_DAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web.Mvc;

namespace ReactorDatabaseWebApp.Controllers
{
    public class BookmarkController : Controller
    {
        public BookmarkController()
        {
            string connection = ConfigurationManager.ConnectionStrings["dataSource"].ConnectionString;
            string logPathDAL = ConfigurationManager.AppSettings["LogPathDAL"];
            string logPathMVC = ConfigurationManager.AppSettings["LogPathMVC"];
            _bookmarkDAO = new BookmarkDAO(connection, logPathDAL);
            _reactorDAO = new ReactorDAO(connection, logPathDAL);
            _logger = new LoggerMVC(logPathMVC, connection);
        }
        private BookmarkDAO _bookmarkDAO;
        private ReactorDAO _reactorDAO;
        private LoggerMVC _logger;

        // GET: Bookmark
        [SessionCheck("Role", 1, 2, 3)]
        public ActionResult Index(int userID)
        {
            ActionResult response = null;
            List<BookmarkPO> bookmarks = new List<BookmarkPO>();
            try
            {
                if (int.TryParse(Session["ID"].ToString(), out int accessorID) && (accessorID == userID || accessorID >= 3))
                {
                    List<BookmarkDO> bookmarkDOList = _bookmarkDAO.ObtainBookmarksByUserID(userID);
                    foreach (BookmarkDO bookmarkDO in bookmarkDOList)
                    {
                        BookmarkPO bookmarkPO = Mapping.Mapper.BookmarkDOtoPO(bookmarkDO);
                        ReactorPO reactorPO = Mapping.Mapper.ReactorDOtoPO(_reactorDAO.ObtainReactorByID(bookmarkPO.ReactorID));
                        if (reactorPO.ModeratorID != 0)
                        {
                            reactorPO.ModeratorName = _reactorDAO.ObtainModeratorNameByID(reactorPO.ModeratorID);
                        }
                        else { }
                        bookmarkPO.ReactorInfo = reactorPO;
                        bookmarks.Add(bookmarkPO);
                    }
                    response = View(bookmarks);
                }
                else
                {
                    TempData["notFound"] = "The page you were looking for could not be found. You have been redirected to the home page.";
                    response = RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
            }
            finally { }
            return response;
        }

        [HttpGet]
        [SessionCheck("Role", 1, 2, 3)]
        public ActionResult AddBookmark(int userID, int reactorID)
        {
            ActionResult response = null;
            try
            {
                if (int.TryParse(Session["ID"].ToString(), out int accessorID) && accessorID == userID)
                {
                    _bookmarkDAO.AddBookmark(userID, reactorID);
                    response = RedirectToAction("Index", "Bookmark", new { userID = Session["ID"].ToString() });
                }
                else
                {
                    response = RedirectToAction("Index", "Home");
                    TempData["notFound"] = "The page you were looking for could not be found. You have been returned to the home page.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
            }
            finally { }
            return response;
        }

        [SessionCheck("Role", 1, 2, 3)]
        [HttpGet]
        public ActionResult RemoveBookmark(int userID, int reactorID)
        {
            ActionResult response = null;
            try
            {
                if(int.TryParse(Session["ID"].ToString(), out int accessorID) && accessorID == userID)
                {
                    int rowsAffected = _bookmarkDAO.DeleteBookmark(userID, reactorID);
                    response = RedirectToAction("Index", "Bookmark", new { userID = Session["ID"].ToString() });
                    if (rowsAffected == 0)
                    {
                        _logger.LogMessage("Warning", "Remove Bookmark failed", MethodBase.GetCurrentMethod().ToString(),
                                                "Attempt by user with ID #" + accessorID + " to remove bookmark showed no rows affected.");
                    }
                    else { }
                }
                else
                {
                    TempData["notFound"] = "The page you were looking for could not be found. You have been returned to the home page.";
                    response = RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
            }
            finally { }
            return response;
        }
    }
}