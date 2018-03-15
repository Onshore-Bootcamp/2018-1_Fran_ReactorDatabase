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
    public class AccountController : Controller
    {
        public AccountController()
        {
            string connection = ConfigurationManager.ConnectionStrings["dataSource"].ConnectionString;
            string logPathDAL = ConfigurationManager.AppSettings["LogPathDAL"];
            string logPathMVC = ConfigurationManager.AppSettings["LogPathMVC"];
            _userDAO = new UserDAO(connection, logPathDAL);
            _logger = new LoggerMVC(logPathMVC, connection);
        }
        private UserDAO _userDAO;
        private LoggerMVC _logger;

        // GET: Account
        [SessionCheck("Role", 2, 3)]
        public ActionResult Index()
        {
            List<UserPO> userPOList = new List<UserPO>();
            try
            {
                List<UserDO> userDOList = _userDAO.ObtainAllUsers();
                foreach (UserDO userDO in userDOList)
                {
                    userPOList.Add(Mapping.Mapper.UserDOtoPO(userDO));
                }
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
            }
            finally { }
            return View(userPOList);
        }

        [HttpGet]
        public ActionResult Register()
        {
            ActionResult response = null;
            _logger.LogMessage("Info", "Register Get request", MethodBase.GetCurrentMethod().ToString(),
                                "Request to view Registration form received.");
            if (Session["Username"] == null)
            {
                UserPO form = new UserPO();
                form.RoleID = 1;
                response = View(form);
            }
            else
            {
                response = RedirectToAction("Index", "Home");
            }
            return response;
        }

        [HttpPost]
        public ActionResult Register(UserPO form)
        {
            ActionResult response = null;
            int userID;

            try
            {
                _logger.LogMessage("Info", "Register Post request", MethodBase.GetCurrentMethod().ToString(),
                                "Submission of new user information received.");
                if (ModelState.IsValid)
                {
                    _logger.LogMessage("Info", "Model State check passed", MethodBase.GetCurrentMethod().ToString(),
                                        "UserPO form model state is valid.");
                    UserDO userDO = _userDAO.ObtainUserByUsername(form.Username);
                    if (userDO == null)
                    {
                        _logger.LogMessage("Username availability check passed.");
                        userID = _userDAO.AddNewUser(Mapping.Mapper.UserPOtoDO(form));

                        if (userID != default(int))
                        {
                            _logger.LogMessage("Info", "New user information added", MethodBase.GetCurrentMethod().ToString(),
                                          "User '" + form.Username + "' was added successfully.");
                            TempData["registerSuccess"] = "Registration successful.";

                            SetSession(form.Username, form.RoleID);
                        }
                        else
                        {
                            _logger.LogMessage("Warning", "Add user attempt failed", MethodBase.GetCurrentMethod().ToString(),
                                                "Attempt to add new user '"
                                                + form.Username + "' showed no database rows affected.");
                            TempData["registerFail"] = "Something went wrong in the registration process.";
                        }
                        response = RedirectToAction("Index", "Reactor");
                    }
                    else
                    {
                        _logger.LogMessage("Registration request on unavailable username.");
                        TempData["registerFail"] = "Username unavailable.";
                        response = View(form);
                    }
                }
                else
                {
                    _logger.LogMessage("Warning", "Model State check failed", MethodBase.GetCurrentMethod().ToString(),
                                        "UserPO form model state was not valid. Returning user to View.");
                    TempData["registerFail"] = "Please see the indicated fields.";
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

        [HttpGet]
        public ActionResult Login()
        {
            ActionResult response;
            if (Session["Username"] != null)
            {
                response = RedirectToAction("Index", "Home");
            }
            else
            {
                if (Session["Attempts"] == null)
                {
                    Session["Attempts"] = 0;
                }
                else { }
                try
                {
                    _logger.LogMessage("Info", "Login Get request", MethodBase.GetCurrentMethod().ToString(),
                                        "Request to view login form received.");
                }
                catch (Exception ex)
                {
                    _logger.LogMessage(ex, "Fatal");
                }
                finally { }
                LoginPO form = new LoginPO();
                response = View(form);
            }
            return response;
        }

        [HttpPost]
        public ActionResult Login(LoginPO form)
        {
            ActionResult response = null;
            try
            {
                if (Session["Attempts"] == null)
                {
                    Session["Attempts"] = 0;
                }
                if (int.Parse(Session["Attempts"].ToString()) <= 5)
                {
                    _logger.LogMessage("Info", "Login Post request", MethodBase.GetCurrentMethod().ToString(),
                                    "Submission of login credentials received.");
                    if (ModelState.IsValid)
                    {
                        _logger.LogMessage("LoginPO model state check passed. Calling DAL to obtain user record.");
                        UserDO userDO = _userDAO.ObtainUserByUsername(form.Username);
                        if (userDO != null)
                        {
                            _logger.LogMessage("Information found for username '" + form.Username + "'. Mapping to UserPO.");
                            UserPO userPO = Mapping.Mapper.UserDOtoPO(userDO);
                            if (form.Password == userPO.Password)
                            {
                                //Successful login
                                _logger.LogMessage("Login credentials pass. Setting session for user.");
                                SetSession(userPO.Username, userPO.RoleID);

                                TempData["loginSuccess"] = "Login successful.";
                                response = RedirectToAction("Index", "Reactor");
                            }
                            else
                            {
                                if (Session["Attempts"] != null)
                                {
                                    Session["Attempts"] = Convert.ToInt32(Session["Attempts"]) + 1;
                                }
                                else
                                {
                                    Session["Attempts"] = 1;
                                }
                                _logger.LogMessage("Credential mismatch. Failed login attempt #" + Session["Attempts"].ToString());
                                TempData["loginFail"] = "Incorrect username or password.";
                                response = View();
                            }
                        }
                        else
                        {
                            if (Session["Attempts"] != null)
                            {
                                Session["Attempts"] = Convert.ToInt32(Session["Attempts"]) + 1;
                            }
                            else
                            {
                                Session["Attempts"] = 1;
                            }
                            _logger.LogMessage("Username not found. Failed login attempt #" + Session["Attempts"].ToString());
                            TempData["loginFail"] = "Incorrect username or password.";
                            response = View();
                        }
                    }
                    else
                    {
                        _logger.LogMessage("LoginPO model state check failed.");
                        response = View();
                    }
                }
                else
                {
                    _logger.LogMessage("Warning", "Login attempt limit exceeded", MethodBase.GetCurrentMethod().ToString(),
                                        "User exceeded login attempt limit.");
                    TempData["loginFail"] = "Login attempt limit exceeded.";
                    Session.Timeout = 1;
                    response = View();
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
        public ActionResult Logout()
        {
            if (Session["Username"] != null)
            {
                string logName = Session["Username"].ToString();
                Session.Abandon();
                try
                {
                    _logger.LogMessage("Info", "User logout", MethodBase.GetCurrentMethod().ToString(),
                                        "User '" + logName + "' logged out successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogMessage(ex, "Fatal");
                }
                finally { }
            }
            else
            {
                _logger.LogMessage("Guest attempted to access Logout. Redirecting to Login.");
            }
            return RedirectToAction("Login", "Account");
        }

        [SessionCheck("Role", 1, 2, 3)]
        [HttpGet]
        public ActionResult UserDetails(string username)
        {
            ActionResult response = null;
            UserPO userPO = new UserPO();
            try
            {
                _logger.LogMessage("Info", "User Details request", MethodBase.GetCurrentMethod().ToString(),
                                    "Request for details for username '" + username + "' received.");
                if (Session["Username"].ToString() != username && int.Parse(Session["Role"].ToString()) != 3)
                {
                    _logger.LogMessage("Warning", "User Details Request Forbidden", MethodBase.GetCurrentMethod().ToString(),
                                        "User attempted to access another user's details without permission.");
                    TempData["noPermission"] = "The page you were looking for could not be found. You have been returned to the home page.";
                    response = RedirectToAction("Index", "Home");
                }
                else
                {
                    UserDO userDO = _userDAO.ObtainUserByUsername(username);
                    if (userDO != null)
                    {
                        _logger.LogMessage("UserDO created. Mapping to PO.");
                        userPO = Mapping.Mapper.UserDOtoPO(userDO);
                        response = View(userPO);
                    }
                    else
                    {
                        _logger.LogMessage("User record not found. Redirecting to home page.");
                        TempData["notFound"] = "The page you were looking for could not be found. You have been returned to the home page.";
                        response = RedirectToAction("Index", "Home");
                    }
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
        public ActionResult UpdateUser(string username)
        {
            ActionResult response = null;
            bool isSelf = (Session["Username"].ToString() == username) ? true : false;
            if (isSelf)
            {
                TempData["updatingSelf"] = "true";
            }
            else { }

            UserPO form = new UserPO();
            try
            {
                _logger.LogMessage("Info", "Update User request", MethodBase.GetCurrentMethod().ToString(),
                                    "Request for update form for username '" + username + "' received.");
                if (!isSelf && int.Parse(Session["Role"].ToString()) < 3)
                {
                    _logger.LogMessage("Warning", "User Update request forbidden", MethodBase.GetCurrentMethod().ToString(),
                                        "User attempted to access another user's update form without permission.");
                    TempData["noPermission"] = "The page you were looking for could not be found. You have been returned to the home page.";
                    response = RedirectToAction("Index", "Home");
                }
                else
                {
                    UserDO userDO = _userDAO.ObtainUserByUsername(username);
                    if (userDO != null)
                    {
                        _logger.LogMessage("UserDO created. Mapping to PO.");
                        form = Mapping.Mapper.UserDOtoPO(userDO);
                        if (form.RoleID >= 3 && !isSelf)
                        {
                            _logger.LogMessage("Warning", "Attempted Admin edit", MethodBase.GetCurrentMethod().ToString(),
                                        "Administrator '" + Session["Username"].ToString() + "' attempted to access update for administrator with username '" + username + "'.");
                            response = RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["initialRole"] = form.RoleID;
                            TempData["initialID"] = form.UserID;
                            FillRoleDropDown(form);
                            response = View(form);
                        }
                    }
                    else
                    {
                        _logger.LogMessage("User record not found. Redirecting to home page.");
                        TempData["notFound"] = "The page you were looking for could not be found. You have been returned to the home page.";
                        response = RedirectToAction("Index", "Home");
                    }
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
        [HttpPost]
        public ActionResult UpdateUser(UserPO form)
        {
            ActionResult response = null;
            try
            {
                _logger.LogMessage("Info", "Update User Post", MethodBase.GetCurrentMethod().ToString(),
                                "Request to update information for user with ID #" + form.UserID + " received from user with ID #" + Session["ID"] + ".");
                if (ModelState.IsValid)
                {
                    _logger.LogMessage("Info", "Model State check passed", MethodBase.GetCurrentMethod().ToString(),
                                        "UserPO form model state is valid.");
                    //Allow role changes if updater is admin
                    if ((int.TryParse(Session["Role"].ToString(), out int role) && role >= 3) 
                        || (TempData["initialRole"] != null && (int)TempData["initialRole"] == form.RoleID))
                    {
                        //Under no circumstances allow altered UserID
                        if (TempData["initialID"] != null && (int)TempData["initialID"] == form.UserID)
                        {
                            _logger.LogMessage("Attempting to map User PO to DO.");
                            UserDO userDO = Mapping.Mapper.UserPOtoDO(form);
                            _userDAO.UpdateUser(userDO);

                            if (TempData["updatingSelf"] != null
                                && TempData["updatingSelf"].ToString() == "true")
                            {
                                TempData.Remove("updatingSelf");
                                //reset session in case own username was changed
                                SetSession(form.Username, form.RoleID);
                            }
                            else { }
                            TempData["updateSuccess"] = "User information updated.";
                            response = RedirectToAction("UserDetails", "Account", new { username = form.Username });
                        }
                        else
                        {
                            _logger.LogMessage("Warning", "User Update forbidden", MethodBase.GetCurrentMethod().ToString(),
                                                "User ID could not be verified or failed verification. Attempt to submit update form with altered User ID was denied.");
                            TempData["noPermission"] = "An error has been encountered. You have been returned to the home page.";
                            response = RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        _logger.LogMessage("Warning", "User Update forbidden", MethodBase.GetCurrentMethod().ToString(),
                                                "Non admin with User ID#" + Session["ID"].ToString() + " and username '" + Session["Username"].ToString() + "' attempted to change user role.");
                        TempData["noPermission"] = "An error has been encountered. You have been returned to the home page.";
                        response = RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    _logger.LogMessage("Warning", "Model State check failed", MethodBase.GetCurrentMethod().ToString(),
                                        "UserPO form model state was not valid. Returning user to View.");
                    FillRoleDropDown(form);
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

        [SessionCheck("Role", 1, 2, 3)]
        [HttpGet]
        public ActionResult ChangePassword(string username)
        {
            ActionResult response = null;
            _logger.LogMessage("Info", "Password Change request", MethodBase.GetCurrentMethod().ToString(),
                                "Request to change password for username '" + username + "' received.");
            try
            {
                UserPO form = Mapping.Mapper.UserDOtoPO(_userDAO.ObtainUserByUsername(username));
                _logger.LogMessage("User information found.");
                if (int.TryParse(Session["ID"].ToString(), out int accessorID) && form.UserID == accessorID)
                {
                    TempData["initialID"] = form.UserID;
                    response = View(form);
                }
                else
                {
                    TempData["notFound"] = "The page you were looking for could not be found. You have been redirected to the home page.";
                    _logger.LogMessage("Warning", "Password Change request forbidden", MethodBase.GetCurrentMethod().ToString(),
                                        "User with User ID #" + accessorID + "attempted to Password Change form for user with ID #" + form.UserID);
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

        [SessionCheck("Role", 1, 2, 3)]
        [HttpPost]
        public ActionResult ChangePassword(UserPO form)
        {
            ActionResult response = null;
            _logger.LogMessage("Password change form submitted.");
            if (ModelState.IsValid)
            {
                if (TempData["initialID"] != null && int.TryParse(TempData["initialID"].ToString(), out int initialID) 
                    && initialID == form.UserID)
                {
                    try
                    {
                        _userDAO.UpdatePassword(Mapping.Mapper.UserPOtoDO(form));
                        _logger.LogMessage("Password updated successfully");
                        TempData["updateSuccess"] = "Password saved.";
                        response = RedirectToAction("UserDetails", new { username = form.Username });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogMessage(ex, "Fatal");
                    }
                    finally { }
                }
                else
                {
                    _logger.LogMessage("Warning", "Password Change forbidden", MethodBase.GetCurrentMethod().ToString(),
                                        "UserID could not be verified or failed verification. Attempt to submit form with altered ID denied.");
                    TempData["noPermission"] = "An error has been encountered. You have been returned to the home page.";
                    response = RedirectToAction("Index", "Home");
                }
            }
            else
            {
                _logger.LogMessage("Model state check failed.");
                response = View(form);
            }
            return response;
        }

        [SessionCheck("Role", 1, 2, 3)]
        [HttpGet]
        public ActionResult DeleteUser(int id, string username, int roleID)
        {
            ActionResult response = null;
            bool isSelf = (Session["Username"].ToString() == username) ? true : false;
            
            try
            {
                _logger.LogMessage("Info", "User deletion attempt", MethodBase.GetCurrentMethod().ToString(),
                                "Request to delete user with ID #" + id + " received.");
                if (!isSelf && int.Parse(Session["Role"].ToString()) < 3)
                {
                    _logger.LogMessage("Warning", "User Deletion Request Forbidden", MethodBase.GetCurrentMethod().ToString(),
                                        "User attempted to access deletion for another user without permission.");
                    TempData["noPermission"] = "The page you were looking for could not be found. You have been returned to the home page.";
                    response = RedirectToAction("Index", "Home");
                }
                else
                {
                    if (roleID >= 3 && !isSelf)
                    {
                        _logger.LogMessage("Warning", "Attempted Admin deletion", MethodBase.GetCurrentMethod().ToString(),
                                        "Administrator '" + Session["Username"].ToString() + "' attempted to delete administrator with username '" + username + "'.");
                        TempData["noPermission"] = "Action forbidden.";
                        response = RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        _logger.LogMessage("Calling DAO to execute deletion.");
                        int rowsAffected = _userDAO.DeleteUserByID(id);
                        _logger.LogMessage("DAO method completed.");
                        if (rowsAffected > 0)
                        {
                            TempData["deletionResult"] = "User deleted successfully.";
                            _logger.LogMessage("Info", "User deleted", MethodBase.GetCurrentMethod().ToString(),
                                            "User with ID #" + id + " deleted successfully. " + rowsAffected + " database rows affected.");
                            if (isSelf)
                            {
                                Session.Abandon();
                            }
                            else { }
                        }
                        else
                        {
                            TempData["deletionResult"] = "An error may have occurred in the deletion attempt.";
                            _logger.LogMessage("Warning", "User deletion failed", MethodBase.GetCurrentMethod().ToString(),
                                                "Attempt to delete user with ID #" + id + " had no effect.");
                        }
                        response = RedirectToAction("Index", "Account");
                    }
                }
                _logger.LogMessage("Redirecting user.");
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
            }
            finally { }
            return response;
        }

        public void FillRoleDropDown(UserPO form)
        {
            form.RoleDropDown = new List<SelectListItem>();
            form.RoleDropDown.Add(new SelectListItem { Text = "Member", Value = "1" });
            form.RoleDropDown.Add(new SelectListItem { Text = "Contributor", Value = "2" });
            form.RoleDropDown.Add(new SelectListItem { Text = "Administrator", Value = "3" });
        }

        public void SetSession(string username, int roleID)
        {
            Session["Username"] = username;
            Session["Role"] = roleID;
            UserPO user = Mapping.Mapper.UserDOtoPO(_userDAO.ObtainUserByUsername(username));
            Session["ID"] = user.UserID;
            Session.Timeout = 8;
        }
    }
}