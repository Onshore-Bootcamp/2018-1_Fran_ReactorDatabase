using ReactorDatabaseWebApp.CustomCode;
using ReactorDatabaseWebApp.Models;
using ReactorDB_BLL;
using ReactorDB_DAL;
using ReactorDB_DAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web.Mvc;

namespace ReactorDatabaseWebApp.Controllers
{
    public class ModeratorController : Controller
    {
        public ModeratorController()
        {
            string connection = ConfigurationManager.ConnectionStrings["dataSource"].ConnectionString;
            string logPathDAL = ConfigurationManager.AppSettings["LogPathDAL"];
            string logPathMVC = ConfigurationManager.AppSettings["LogPathMVC"];
            string logPathBLL = ConfigurationManager.AppSettings["LogPathBLL"];
            _moderatorDAO = new ModeratorDAO(connection, logPathDAL);
            _moderatorBLO = new ModeratorBLO(logPathBLL);
            _logger = new LoggerMVC(logPathMVC, connection);
        }
        private ModeratorDAO _moderatorDAO;
        private ModeratorBLO _moderatorBLO;
        private LoggerMVC _logger;

        // GET: Moderator
        public ActionResult Index()
        {
            List<ModeratorPO> moderatorPOList = new List<ModeratorPO>();
            try
            {
                List<ModeratorDO> moderatorDOList = _moderatorDAO.ObtainAllModerators();
                foreach (ModeratorDO moderatorDO in moderatorDOList)
                {
                    ModeratorPO moderatorPO = Mapping.Mapper.ModeratorDOtoPO(moderatorDO);
                    moderatorPOList.Add(moderatorPO);
                }
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
            }
            finally { }
            return View(moderatorPOList);
        }

        [HttpGet]
        public ActionResult ModeratorDetails(int id)
        {
            ActionResult response = null;
            try
            {
                ModeratorDO moderatorDO = _moderatorDAO.ObtainModeratorByID(id);
                if (moderatorDO != null)
                {
                    _logger.LogMessage("Info", "Moderator Details", MethodBase.GetCurrentMethod().ToString(),
                                      "User request to view details of moderator with ID #" + id + " is valid.");
                    ModeratorPO moderatorPO = Mapping.Mapper.ModeratorDOtoPO(moderatorDO);
                    response = View(moderatorPO);
                }
                else
                {
                    _logger.LogMessage("Warning", "Page not found", MethodBase.GetCurrentMethod().ToString(),
                                      "User attempted to navigate to ModeratorDetails on null ID #" + id + ". " +
                                      "Redirecting to Index.");
                    TempData["idNotFound"] = "The page you were looking for could not be found." +
                                             "You have been returned to the moderator list.";
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
        public ActionResult AddModerator()
        {
            try
            {
                _logger.LogMessage("Info", "Add Moderator Get request", MethodBase.GetCurrentMethod().ToString(),
                                    "Request to view Add Moderator form received.");
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
            }
            finally { }

            ModeratorPO form = new ModeratorPO();
            return View(form);
        }

        [SessionCheck("Role", 2, 3)]
        [HttpPost]
        public ActionResult AddModerator(ModeratorPO form)
        {
            ActionResult response = null;
            int rowsAffected;

            try
            {
                _logger.LogMessage("Info", "Add Moderator Post request", MethodBase.GetCurrentMethod().ToString(),
                                "Request to add Moderator information received.");
                if (ModelState.IsValid)
                {
                    _logger.LogMessage("Info", "Model State check passed", MethodBase.GetCurrentMethod().ToString(),
                                        "ModeratorPO form model state is valid.");
                    form = CalculationHandler(form);

                    _logger.LogMessage("Attempting to map Moderator PO to DO.");
                    ModeratorDO moderatorDO = Mapping.Mapper.ModeratorPOtoDO(form);
                    rowsAffected = _moderatorDAO.AddNewModerator(moderatorDO);

                    if (rowsAffected > 0)
                    {
                        _logger.LogMessage("Info", "New moderator information added", MethodBase.GetCurrentMethod().ToString(),
                                      form.Name + " moderator was added to database.");
                        TempData["addNew"] = "Moderator added to database successfully.";
                    }
                    else
                    {
                        _logger.LogMessage("Warning", "Add Moderator attempt failed", MethodBase.GetCurrentMethod().ToString(),
                                            "Attempt to add new moderator (" 
                                            + form.Name + ") showed no database rows affected.");
                        TempData["addNew"] = "Failed to add new moderator to the database.";
                    }
                    response = RedirectToAction("Index");
                }
                else
                {
                    _logger.LogMessage("Warning", "Model State check failed", MethodBase.GetCurrentMethod().ToString(),
                                        "ModeratorPO form model state was not valid. Returning user to View.");
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
        public ActionResult UpdateModerator(int id)
        {
            ModeratorPO form = new ModeratorPO();
            try
            {
                _logger.LogMessage("Info", "Update Moderator Get request", MethodBase.GetCurrentMethod().ToString(),
                                    "Request for update form for moderator with ID#" + id + " received.");
                ModeratorDO moderatorDO = _moderatorDAO.ObtainModeratorByID(id);
                form = Mapping.Mapper.ModeratorDOtoPO(moderatorDO);
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
            }
            finally { }
            return View(form);
        }

        [SessionCheck("Role", 2, 3)]
        [HttpPost]
        public ActionResult UpdateModerator(ModeratorPO form)
        {
            ActionResult response = null;
            try
            {
                _logger.LogMessage("Info", "Update Moderator Post", MethodBase.GetCurrentMethod().ToString(),
                                "Request to update information for moderator with ID #" + form.ModeratorID + " received.");
                if (ModelState.IsValid)
                {
                    _logger.LogMessage("Info", "Model State check passed", MethodBase.GetCurrentMethod().ToString(),
                                        "ModeratorPO form model state is valid.");
                    form = CalculationHandler(form);
                    _logger.LogMessage("Attempting to map Moderator PO to DO.");
                    ModeratorDO moderatorDO = Mapping.Mapper.ModeratorPOtoDO(form);
                    _moderatorDAO.UpdateModerator(moderatorDO);

                    TempData["updateModerator"] = "Moderator information updated.";
                    response = RedirectToAction("ModeratorDetails", "Moderator", new { id = form.ModeratorID });
                }
                else
                {
                    _logger.LogMessage("Warning", "Model State check failed", MethodBase.GetCurrentMethod().ToString(),
                                        "ModeratorPO form model state was not valid. Returning user to View.");
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
        public ActionResult DeleteModerator(int id)
        {
            try
            {
                _logger.LogMessage("Info", "Moderator deletion attempt", MethodBase.GetCurrentMethod().ToString(),
                                "Request to delete moderator with ID #" + id + " received.");
                _logger.LogMessage("Calling DAO to execute deletion.");
                int rowsAffected = _moderatorDAO.DeleteModeratorByID(id);
                _logger.LogMessage("DAO method completed.");
                if (rowsAffected > 0)
                {
                    TempData["deletionResult"] = "Moderator deleted successfully.";
                    _logger.LogMessage("Info", "Moderator deleted", MethodBase.GetCurrentMethod().ToString(),
                                    "Moderator with ID #" + id + " deleted successfully. " + rowsAffected + " database rows affected.");
                }
                else
                {
                    TempData["deletionResult"] = "An error may have occurred in the deletion attempt.";
                    _logger.LogMessage("Warning", "Moderator deletion failed", MethodBase.GetCurrentMethod().ToString(),
                                        "Attempt to delete moderator with ID #" + id + " had no effect.");
                }
                _logger.LogMessage("Redirecting user to Index view.");
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
            }
            finally { }
            return RedirectToAction("Index", "Moderator");
        }

        [HttpGet]
        public ActionResult Calculations()
        {
            return View();
        }

        public ModeratorPO CalculationHandler(ModeratorPO form)
        {
            _logger.LogMessage("Calculating Energy Decrement per collision.");
            form.EnergyDecrement = (float)_moderatorBLO.CalculateEnergyDecrement(form.AtomicMass);
            if (form.EnergyDecrement != 0)
            {
                _logger.LogMessage("Calculating number of collisions to thermalize fission neutrons.");
                form.Collisions = _moderatorBLO.CalculateCollisions(form.EnergyDecrement);

                if (form.ScatteringXS != 0 && form.AbsorptionXS != 0)
                {
                    _logger.LogMessage("Calculating Moderation Efficiency.");
                    form.ModerationEfficiency =
                        _moderatorBLO.CalculateModerationEfficiency(form.EnergyDecrement, form.ScatteringXS, form.AbsorptionXS);
                }
                else
                {
                    _logger.LogMessage("Scattering or Absorption cross section not provided. " +
                                        "Did not calculate Moderation Efficiency.");
                }
            }
            else
            {
                _logger.LogMessage("Warning", "Energy Decrement 0", MethodBase.GetCurrentMethod().ToString(),
                                    "Possible error in energy decrement calculation. " +
                                    "Did not calculate Collisions or Moderation Efficiency.");
            }
            return form;
        }
    }
}