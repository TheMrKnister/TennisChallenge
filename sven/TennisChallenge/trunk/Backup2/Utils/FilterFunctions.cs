using System;
using System.Linq;
using TennisChallenge.Dal;

namespace TennisWeb.Utils
{
  public static class FilterFunctions
  {
    public static Func<Club, bool> ClubsWhereUserIsInRole(string userName, string roleName)
    {
      return club =>
        club.UsersInClubs.Any(usersInClub =>
          usersInClub.User.UserName == userName && usersInClub.Roles.Any(role =>
            role.RoleName == roleName));
    }
  }
}