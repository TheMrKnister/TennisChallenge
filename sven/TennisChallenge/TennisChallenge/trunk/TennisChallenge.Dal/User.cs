using System;
using System.Collections.Generic;
using System.Linq;

namespace TennisChallenge.Dal
{
  public partial class User
  {
    public IEnumerable<Club> Clubs
    {
      get
      {
        return UsersInClubs.Select(uic => uic.Club);
      }
    }

    public IEnumerable<Guid> ClubFks
    {
      get
      {
        return Clubs.Select(c => c.ClubKey);
      }
    }
  }
}