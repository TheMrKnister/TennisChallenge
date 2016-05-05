using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TennisChallenge.Dal;

namespace TennisWeb.Models
{
  public class ClubManagementModel
  {
    public IEnumerable<Club> Clubs { get; set; }
    public IEnumerable<UsersInClub> ClubManagers {get; set;}
  }
}