using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Mvc.Controllers;

namespace BissTalk.SitecoreSynergy.Controllers
{
    public class SampleController: SitecoreController
    {
        public override ActionResult Index()
        {
            return PartialView("~/Views/BissTalk/Synergy/Sample/Index.cshtml");
        }
    }
}