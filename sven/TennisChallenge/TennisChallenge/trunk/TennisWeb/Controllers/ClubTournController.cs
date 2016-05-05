using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TennisWeb.Controllers
{
    public class ClubTournController : Controller
    {
        //
        // GET: /ClubTourn/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ClubTournStart()
        {
          return View();
        }

        public ActionResult ClubTournNew()
        {
          return View();
        }

        public ActionResult ClubTournOld()
        {
          return View();
        }

        public ActionResult ClubTournManage()
        {
          return View();
        }

    }
}
