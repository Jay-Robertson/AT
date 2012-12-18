using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mavo.Assets.Controllers
{
    public partial class HomeController : BaseController
    {
        public virtual ActionResult Index()
        {
            return RedirectToAction("Jobs", "Reports");
        }




    }
}
