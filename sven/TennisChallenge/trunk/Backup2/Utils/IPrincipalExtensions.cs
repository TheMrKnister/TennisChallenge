using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using TennisChallenge.Dal;

namespace TennisWeb.Utils
{
  public static class IPrincipalExtensions
  {

    public static short? GetGender(this IPrincipal principal)
    {
      try
      {
        var user = principal.Identity.Name;
        var userFk = new AccessorBase<UsersInClub>().GetFirstOrDefaultWhere(u => u.User.UserName.ToLower() == user.ToLower()).UserFK;
        var gender = new AccessorBase<Member>().GetFirstOrDefaultWhere(m => m.MemberKey == userFk).TitleFk;

        return gender;
      }
      catch
      {
        return null;
      }
    }

    /// <summary>
    /// Extension method that filters checks if a user is in a club and if he
    /// a certain role in said club.
    /// </summary>
    /// <param name="roles">Name of the role to check for.</param>
    /// <returns>true if the user is in a certain club with a certain role, false otherwise.</returns>
    public static bool IsInRoles(this IPrincipal principal, IEnumerable<string> roles)
    {
      if (roles.Count() < 1)
        throw new ArgumentException("Provide at least one role");

      var user = principal.Identity.Name;

      return new AccessorBase<UsersInClub>().GetAllWhere(u =>
        u.Roles.Any(r => roles.Contains(r.RoleName)) &&
        u.User.UserName.ToLower() == user.ToLower(),
        u => u.Club, u => u.User, u => u.Roles)
      .Any();
    }

    /// <summary>
    /// Extension method that filters checks if a user is in a club and if he
    /// a certain role in said club.
    /// </summary>
    /// <param name="roles">Name of the role to check for.</param>
    /// <returns>true if the user is in a certain club with a certain role, false otherwise.</returns>
    public static bool IsInRoles(this IPrincipal principal, params string[] roles)
    {
      return IsInRoles(principal, roles.AsEnumerable());
    }
  }
}