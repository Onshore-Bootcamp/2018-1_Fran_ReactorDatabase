using ReactorDatabaseWebApp.CustomCode;
using ReactorDatabaseWebApp.Models;
using ReactorDB_DAL;
using ReactorDB_DAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace ReactorDatabaseWebApp.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
            string connection = ConfigurationManager.ConnectionStrings["dataSource"].ConnectionString;
            string logPathMVC = ConfigurationManager.AppSettings["LogPathMVC"];
            _logDAO = new LogDAO(connection, logPathMVC);
            _logger = new LoggerMVC(logPathMVC, connection);
        }
        private LogDAO _logDAO;
        private LoggerMVC _logger;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [SessionCheck("Role", 3)]
        [HttpGet]
        public ActionResult RecentLogs()
        {
            List<LogPO> logPOList = new List<LogPO>();
            try
            {
                List<LogDO> logDOList = _logDAO.ObtainRecentLogs();
                foreach (LogDO logDO in logDOList)
                {
                    logPOList.Add(Mapping.Mapper.LogDOtoPO(logDO));
                }
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
            }
            finally { }
            return View(logPOList);
        }
    }
}