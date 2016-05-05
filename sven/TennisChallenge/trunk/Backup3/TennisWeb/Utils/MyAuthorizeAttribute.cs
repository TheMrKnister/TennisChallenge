using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TennisChallenge.Dal;

namespace TennisWeb.Utils
{
  public class MyAuthorizeAttribute : AuthorizeAttribute
  {
    private string _clubs;
    private string[] _clubsSplit = new string[0];

    public string Clubs
    {
      get { return _clubs ?? String.Empty; }
      set
      {
        if (String.IsNullOrWhiteSpace(value))
        {
          _clubs = String.Empty;
          _clubsSplit = new string[0];
        }
        else
        {
          _clubs = value;
          _clubsSplit = value
            .Split(',')
            .AsEnumerable().Select(s => s.Trim())
            .Where(s => !String.IsNullOrWhiteSpace(s)).ToArray();
        }
      }
    }

    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
      if (httpContext == null)
        throw new ArgumentNullException("httpContext");

      var user = httpContext.User;

      if (!user.Identity.IsAuthenticated)
        return false;

      if (!(Clubs.Any() && Users.Any() && Roles.Any()))
        return true;

      return new AccessorBase<UsersInClub>().GetAllWhere(u =>
        // Either no club is required or the required clubs contain the users club
        (String.IsNullOrWhiteSpace(Clubs) || Clubs.Contains(u.Club.Name)) &&
          // Either no Username is required or the Username is in the UserInClub
        (String.IsNullOrWhiteSpace(Users) || Users.Contains(u.User.UserName)) &&
          // Either no role is required or the role is in the UserInClub
        (String.IsNullOrWhiteSpace(Roles) || u.Roles.Any(r => Roles.Contains(r.RoleName)))
      ).Any(u => u.User.UserName == user.Identity.Name);
    }
  }
}