using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TennisChallenge.Dal;
using TennisWeb.Models;
using TennisWeb.Utils;

namespace TennisWeb.WebApi
{
  public class UserController : ApiController
  {
    [MyAuthorize(Roles = RoleNames.ClubAdmin + "," + RoleNames.CasualTournamentOrganizer)]
    public IEnumerable<MemberSimple> Get(string filter = "")
    {
      var clubId = Guid.NewGuid();
      try
      {
        clubId = new AccessorBase<Club>()
          .GetFirstOrDefaultWhereFunc(FilterFunctions.ClubsWhereUserIsInRole(User.Identity.Name, RoleNames.ClubAdmin))
          .ClubKey;
      }

      catch
      {
        clubId = new AccessorBase<Club>()
        .GetFirstOrDefaultWhereFunc(FilterFunctions.ClubsWhereUserIsInRole(User.Identity.Name, RoleNames.CasualTournamentOrganizer))
        .ClubKey;
      }

      var model = new MemberAccessor()
        .GetAllWhereFunc(m => m.FullName.ToLower().Contains(filter.ToLower()) && m.ClubFks.Contains(clubId));

      List<MemberSimple> list = new List<MemberSimple>();

      foreach (var member in model)
      {
        var ms = new MemberSimple();
        ms.FullName = member.FullName;
        ms.Id = member.Id;
        ms.Rfid = member.Rfids.Count;
        ms.Gender = member.TitleFk;
        list.Add(ms);

      }

      return list;
    }

    public EditUserInClubModel Get(Guid id)
    {
      var model = new EditUserInClubModel();

      var userInClub = new AccessorBase<UsersInClub>()
        .GetFirstOrDefaultWhere(u => u.User.UserId == id, i => i.Roles);

      model.UsersInClubsKey = userInClub.UsersInClubsKey;
      model.Roles = userInClub.Roles.Select(r => r.RoleId);

      return model;
    }
  }
}