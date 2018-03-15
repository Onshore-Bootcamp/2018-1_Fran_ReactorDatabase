using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReactorDatabaseWebApp.CustomCode
{
    public class SessionCheck : ActionFilterAttribute
    {
        private readonly string _Key = "";
        private readonly int[] _AllowedRoles;
        public SessionCheck(string keyName, params int[] roles)
        {
            _Key = keyName;
            _AllowedRoles = roles ?? new int[0];
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (session[_Key] != null && int.TryParse(session[_Key].ToString(), out int roleID))
            {
                if (!_AllowedRoles.Contains(roleID))
                {
                    filterContext.Result = new RedirectResult("/Account/Login", false);
                }
            }
            else //Session key null
            {
                filterContext.Result = new RedirectResult("/Account/Login", false);
            }
            base.OnActionExecuting(filterContext);
        }
    }
}