using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mavo.Assets.Models;
using Mavo.Assets.Services;
using Microsoft.Practices.ServiceLocation;

namespace Mavo.Assets.Attributes
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        public UserRole UserRole { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }

            UserRole privilegeLevels = ServiceLocator.Current.GetInstance<ICurrentUserService>().GetCurrent().Role;

            if (privilegeLevels.HasFlag(UserRole))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}