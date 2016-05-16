using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;

namespace dog2go.Controllers.Helpers
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class FullyAuthorizedAttribute : ActionMethodSelectorAttribute
    {
        [Authorize]
        public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
        {
            bool isValid = false;
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            string user = HttpContext.Current.User.Identity.Name;
            ConcurrentDictionary<string, User> users = UserRepository.Instance.Get();
            isValid = users?.FirstOrDefault(x => x.Value.Identifier == user).Value != null;
            if (!isValid)
            {
                HttpContext.Current.Response.Redirect("~/Account/Login");
            }
            return true;
        }
    }
}