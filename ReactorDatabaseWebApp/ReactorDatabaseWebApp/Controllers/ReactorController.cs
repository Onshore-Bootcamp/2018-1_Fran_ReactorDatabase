using ReactorDatabaseWebApp.CustomCode;
using ReactorDatabaseWebApp.Models;
using ReactorDB_BLL;
using ReactorDB_BLL.Models;
using ReactorDB_DAL;
using ReactorDB_DAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web.Mvc;

namespace ReactorDatabaseWebApp.Controllers
{
    public class ReactorController : Controller
    {
        public ReactorController()
        {
            string connection = ConfigurationManager.ConnectionStrings["dataSource"].ConnectionString;
            string logPathDAL = ConfigurationManager.AppSettings["LogPathDAL"];
            string logPathMVC = ConfigurationManager.AppSettings["LogPathMVC"];
            string logPathBLL = ConfigurationManager.AppSettings["LogPathBLL"];
            _reactorDAO = new ReactorDAO(connection, logPathDAL);
            _moderatorDAO = new ModeratorDAO(connection, logPathDAL);
            _bookmarkDAO = new BookmarkDAO(connection, logPathDAL);
            _reactorBLO = new ReactorBLO(logPathBLL);
            _logger = new LoggerMVC(logPathMVC, connection);
        }
        private ReactorDAO _reactorDAO;
        private ModeratorDAO _moderatorDAO;
        private BookmarkDAO _bookmarkDAO;
        private ReactorBLO _reactorBLO;
        private LoggerMVC _logger;

        // GET: Reactor
        public ActionResult Index()
        {
            List<ReactorPO> reactorPOList = new List<ReactorPO>();
            try
            {
                List<ReactorDO> reactorDOList = _reactorDAO.ObtainAllReactors();
                foreach (ReactorDO reactorDO in reactorDOList)
                {
                    ReactorPO reactorPO = Mapping.Mapper.ReactorDOtoPO(reactorDO);
                    if (reactorPO.ModeratorID != 0)
                    {
                        reactorPO.ModeratorName = _reactorDAO.ObtainModeratorNameByID(reactorPO.ModeratorID);
                    }
                    reactorPOList.Add(reactorPO);
                }
                if (Session["ID"] != null)
                {
                    List<BookmarkDO> bookmarkDOList = _bookmarkDAO.ObtainBookmarksByUserID(int.Parse(Session["ID"].ToString()));
                    if (bookmarkDOList.Count > 0)
                    {
                        List<int> bookmarkReactorIDs = new List<int>();
                        foreach (BookmarkDO bookmarkDO in bookmarkDOList)
                        {
                            bookmarkReactorIDs.Add(bookmarkDO.ReactorID);
                        }
                        ViewBag.Bookmarks = bookmarkReactorIDs;
                    }
                    else
                    {
                        List<int> emptyList = new List<int>();
                        ViewBag.Bookmarks = emptyList;
                    }
                }
                else { }
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
            }
            finally { }
            return View(reactorPOList);
        }

        [HttpGet]
        public ActionResult ReactorDetails(int id)
        {
            ActionResult response = null;
            try
            {
                ReactorDO reactorDO = _reactorDAO.ObtainReactorByID(id);
                if (reactorDO != null)
                {
                    _logger.LogMessage("Info", "Reactor Details", MethodBase.GetCurrentMethod().ToString(),
                                      "User request to view details of reactor with ID #" + id + " is valid.");
                    ReactorPO reactorPO = Mapping.Mapper.ReactorDOtoPO(reactorDO);
                    if (reactorPO.ModeratorID != 0)
                    {
                        reactorPO.ModeratorName = _reactorDAO.ObtainModeratorNameByID(reactorPO.ModeratorID);
                    }
                    else { }

                    if (Session["ID"] != null)
                    {
                        List<BookmarkDO> bookmarkDOList = _bookmarkDAO.ObtainBookmarksByUserID(int.Parse(Session["ID"].ToString()));
                        if (bookmarkDOList.Count > 0)
                        {
                            List<int> bookmarkReactorIDs = new List<int>();
                            foreach (BookmarkDO bookmarkDO in bookmarkDOList)
                            {
                                bookmarkReactorIDs.Add(bookmarkDO.ReactorID);
                            }
                            if (bookmarkReactorIDs.Contains(id))
                            {
                                ViewBag.isBookmarked = true;
                            }
                            else
                            {
                                ViewBag.isBookmarked = false;
                            }
                        }
                        else
                        {
                            ViewBag.isBookmarked = false;
                        }
                    }
                    else { }
                    response = View(reactorPO);
                }
                else
                {
                    _logger.LogMessage("Warning", "Page not found", MethodBase.GetCurrentMethod().ToString(),
                                      "User attempted to navigate to ReactorDetails on null ID " + id + ". " +
                                      "Redirecting to Index.");
                    TempData["idNotFound"] = "The page you were looking for could not be found." +
                                             "You have been returned to the reactor list.";
                    response = RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
            }
            finally { }
            return response;
        }

        [SessionCheck("Role", 2, 3)]
        [HttpGet]
        public ActionResult AddReactor()
        {
            try
            {
                _logger.LogMessage("Info", "Add Reactor Get request", MethodBase.GetCurrentMethod().ToString(),
                                    "Request to view Add Reactor form received.");
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
            }
            finally { }

            ReactorPO form = new ReactorPO();
            FillGenerationDropDown(form);
            FillModeratorDropDown(form);

            return View(form);
        }

        [SessionCheck("Role", 2, 3)]
        [HttpPost]
        public ActionResult AddReactor(ReactorPO form)
        {
            ActionResult response = null;
            int rowsAffected;
            try
            {
                _logger.LogMessage("Info", "Add Reactor Post request", MethodBase.GetCurrentMethod().ToString(),
                                "Request to add Reactor information received.");
                if (ModelState.IsValid)
                {
                    _logger.LogMessage("Info", "Model State check passed", MethodBase.GetCurrentMethod().ToString(),
                                        "ReactorPO form model state is valid.");
                    //If thermal and electric power entered, calculate efficiency with BLO
                    if (form.ThermalPower != 0 && form.ElectricPower != 0)
                    {
                        _logger.LogMessage("Attempting to calculate efficiency.");
                        ReactorBO reactorBO = Mapping.Mapper.ReactorPOtoBO(form);
                        form.Efficiency = _reactorBLO.CalculateEfficiency(reactorBO);
                    }
                    else
                    {
                        _logger.LogMessage("Thermal Power or Electric Power not provided. Did not calculate efficiency.");
                    }
                    _logger.LogMessage("Attempting to map Reactor PO to DO.");
                    ReactorDO reactorDO = Mapping.Mapper.ReactorPOtoDO(form);
                    rowsAffected = _reactorDAO.AddNewReactor(reactorDO);

                    if (rowsAffected > 0)
                    {
                        _logger.LogMessage("Info", "New reactor information added", MethodBase.GetCurrentMethod().ToString(),
                                      form.Name + " reactor was added to database.");
                        TempData["addNew"] = "Reactor added successfully.";
                    }
                    else
                    {
                        _logger.LogMessage("Warning", "Add Reactor attempt failed", MethodBase.GetCurrentMethod().ToString(),
                                            "Attempt to add new reactor showed no database rows affected.");
                        TempData["addNew"] = "Failed to add new reactor to database.";
                    }
                    response = RedirectToAction("Index");
                }
                else
                {
                    _logger.LogMessage("Warning", "Model State check failed", MethodBase.GetCurrentMethod().ToString(),
                                        "ReactorPO form model state was not valid. Returning user to View.");
                    FillGenerationDropDown(form);
                    FillModeratorDropDown(form);
                    response = View(form);
                }
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
            }
            finally { }
            return response;
        }

        [SessionCheck("Role", 2, 3)]
        [HttpGet]
        public ActionResult UpdateReactor(int id)
        {
            ReactorPO form = new ReactorPO();
            try
            {
                _logger.LogMessage("Info", "Update Reactor Get request", MethodBase.GetCurrentMethod().ToString(),
                                    "Request for update form for reactor with ID#" + id + " received.");
                ReactorDO reactorDO = _reactorDAO.ObtainReactorByID(id);
                form = Mapping.Mapper.ReactorDOtoPO(reactorDO);
                FillGenerationDropDown(form);
                FillModeratorDropDown(form);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { }
            return View(form);
        }

        [SessionCheck("Role", 2, 3)]
        [HttpPost]
        public ActionResult UpdateReactor(ReactorPO form)
        {
            ActionResult response = null;
            try
            {
                _logger.LogMessage("Info", "Update Reactor Post", MethodBase.GetCurrentMethod().ToString(),
                                "Request to update information for reactor with ID #" + form.ReactorID + " received.");
                if (ModelState.IsValid)
                {
                    _logger.LogMessage("Info", "Model State check passed", MethodBase.GetCurrentMethod().ToString(),
                                        "ReactorPO form model state is valid.");
                    if (form.ThermalPower != 0 && form.ElectricPower != 0)
                    {
                        _logger.LogMessage("Attempting to calculate efficiency.");
                        ReactorBO reactorBO = Mapping.Mapper.ReactorPOtoBO(form);
                        form.Efficiency = _reactorBLO.CalculateEfficiency(reactorBO);
                    }
                    else
                    {
                        _logger.LogMessage("Thermal Power or Electric Power not provided. Did not calculate efficiency.");
                    }
                    _reactorDAO.UpdateReactor(Mapping.Mapper.ReactorPOtoDO(form));
                    TempData["updateReactor"] = "Reactor information updated.";
                    response = RedirectToAction("ReactorDetails", "Reactor", new { id = form.ReactorID });
                }
                else
                {
                    _logger.LogMessage("Warning", "Model State check failed", MethodBase.GetCurrentMethod().ToString(),
                                        "ReactorPO form model state was not valid. Returning user to View.");
                    FillGenerationDropDown(form);
                    FillModeratorDropDown(form);
                    response = View(form);
                }
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
            }
            finally { }
            
            return response;
        }

        [SessionCheck("Role", 3)]
        [HttpGet]
        public ActionResult DeleteReactor(int id)
        {
            try
            {
                _logger.LogMessage("Info", "Reactor deletion attempt", MethodBase.GetCurrentMethod().ToString(),
                                "Request to delete reactor with ID #" + id + " received.");
                _logger.LogMessage("Calling DAO to execute deletion.");
                int rowsAffected = _reactorDAO.DeleteReactorByID(id);
                _logger.LogMessage("DAO method completed.");
                if (rowsAffected > 0)
                {
                    TempData["deletionResult"] = "Reactor deleted successfully.";
                    _logger.LogMessage("Info", "Reactor deleted", MethodBase.GetCurrentMethod().ToString(),
                                    "Reactor with ID #" + id + " deleted successfully. " + rowsAffected + " database rows affected.");
                }
                else
                {
                    TempData["deletionResult"] = "An error may have occurred in the deletion attempt.";
                    _logger.LogMessage("Warning", "Reactor deletion failed", MethodBase.GetCurrentMethod().ToString(),
                                        "Attempt to delete reactor with ID #" + id + " had no effect.");
                }
                _logger.LogMessage("Redirecting user to Index view.");
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
            }
            finally { }
            return RedirectToAction("Index", "Reactor");
        }

        [SessionCheck("Role", 1, 2, 3)]
        [HttpGet]
        public ActionResult ReactorStatistics()
        {
            StatisticsPO dbStats = new StatisticsPO();
            StatisticsBO statisticsBO = new StatisticsBO();
            List<ReactorBO> reactorBOList = new List<ReactorBO>();
            try
            {
                List<ReactorDO> reactorDOList = _reactorDAO.ObtainAllReactors();
                if (reactorDOList != null)
                {
                    foreach (ReactorDO reactorDO in reactorDOList)
                    {
                        ReactorBO reactorBO = Mapping.Mapper.ReactorDOtoBO(reactorDO);
                        reactorBOList.Add(reactorBO);
                    }
                    statisticsBO = _reactorBLO.SpectrumCount(reactorBOList, statisticsBO);
                    statisticsBO = _reactorBLO.SpectrumPercentage(statisticsBO);
                    statisticsBO = _reactorBLO.GenerationCounter(reactorBOList, statisticsBO);

                    int[] genCount = new int[6];
                    genCount[0] = statisticsBO.NoGenCount;
                    genCount[1] = statisticsBO.GenIcount;
                    genCount[2] = statisticsBO.GenIIcount;
                    genCount[3] = statisticsBO.GenIIIcount;
                    genCount[4] = statisticsBO.GenIVcount;
                    genCount[5] = statisticsBO.GenVcount;
                    statisticsBO = _reactorBLO.GenerationPercentage(genCount, statisticsBO);
                    dbStats = Mapping.Mapper.StatisticsBOtoPO(statisticsBO);
                }
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
            }
            finally { }
            return View(dbStats);
        }

        public void FillGenerationDropDown(ReactorPO form)
        {
            form.GenerationDropDown = new List<SelectListItem>();
            form.GenerationDropDown.Add(new SelectListItem { Text = "--Select--", Value = "" });
            form.GenerationDropDown.Add(new SelectListItem { Text = "I", Value = "I" });
            form.GenerationDropDown.Add(new SelectListItem { Text = "II", Value = "II" });
            form.GenerationDropDown.Add(new SelectListItem { Text = "III", Value = "III" });
            form.GenerationDropDown.Add(new SelectListItem { Text = "IV", Value = "IV" });
            form.GenerationDropDown.Add(new SelectListItem { Text = "V", Value = "V" });
        }

        public void FillModeratorDropDown(ReactorPO form)
        {
            form.ModeratorDropDown = new List<SelectListItem>();
            form.ModeratorDropDown.Add(new SelectListItem { Text = "None", Value = "0" });
            try
            {
                List<ModeratorDO> moderatorDOList = _moderatorDAO.ObtainAllModerators();
                foreach (ModeratorDO moderatorDO in moderatorDOList)
                {
                    ModeratorPO moderatorPO = Mapping.Mapper.ModeratorDOtoPO(moderatorDO);
                    form.ModeratorDropDown.Add(new SelectListItem { Text = moderatorPO.Name,
                        Value = moderatorPO.ModeratorID.ToString() });
                }
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
            }
            finally { }
        }
    }
}