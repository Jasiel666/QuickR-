using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AplicacionPoe.Sessions
{
    public class ValidateSessionAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["LoginUsers"]==null)
            {
                filterContext.Result = new RedirectResult("~/Home/Login");
            }
           
            base.OnActionExecuting(filterContext);
        }
    }
}