using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Omu.ValueInjecter;
using TennisChallenge.Dal;
using TennisWeb.Utils;
using TennisWeb.Models;

namespace TennisWeb.Controllers
{
  public class ClubManagementController : Controller
  {
    //
    // GET: /ClubManagement/

    public ActionResult Overview()
    {
      var model = new ClubManagementModel();

      model.Clubs = new AccessorBase<Club>().GetAll();
      model.ClubManagers = new AccessorBase<UsersInClub>().GetAllWhere(u => u.Roles.Any(r => r.RoleName == RoleNames.ClubAdmin)).OrderBy(uic => uic.Club.Name);

      return View(model);
    }
    public ActionResult GetClubMembers(Guid clubFk)
    {
      return View(new AccessorBase<UsersInClub>().GetAllWhere(uic => uic.ClubFK == clubFk));
    }

    public ActionResult AddManagerToClub(Guid clubKey, Guid userId)
    {
      var clubMember = new AccessorBase<UsersInClub>().GetFirstOrDefaultWhere(uic => uic.Club.ClubKey == clubKey && uic.User.UserId == userId);
      //clubMember.Roles.Add(RoleNames.ClubAdmin);
      return null;
    }
  }
}
