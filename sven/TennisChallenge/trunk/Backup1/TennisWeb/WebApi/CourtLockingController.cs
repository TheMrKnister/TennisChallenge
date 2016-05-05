using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Web.Http;
using TennisChallenge.Dal;
using TennisWeb.Utils;

namespace TennisWeb.WebApi
{
  public class CourtLockingController : ApiController
  {
    public IEnumerable<Guid> Get()
    {
      var clubId = new AccessorBase<Club>()
        .GetFirstOrDefaultWhereFunc(FilterFunctions.ClubsWhereUserIsInRole(User.Identity.Name, RoleNames.Janitor),
        i => i.UsersInClubs, i => i.UsersInClubs.Select(u => u.Roles), i => i.UsersInClubs.Select(u => u.User))
        .ClubKey;

      return new AccessorBase<Booking>()
        .GetAllWhereFunc(b => b.BookingTypeEnum == EBookingType.Closed && b.Court.ClubFk == clubId, i => i.Court)
        .Select(b => b.CourtFk);
    }

    public void Put(Guid id, [FromBody] DoLockModel model)
    {
      // checks if a user is Janitor in the club of the court
      if (new AccessorBase<Club>()
        .GetAllWhereFunc(FilterFunctions.ClubsWhereUserIsInRole(User.Identity.Name, RoleNames.Janitor),
        i => i.UsersInClubs, i => i.UsersInClubs.Select(u => u.Roles), i => i.UsersInClubs.Select(u => u.User))
        .Where(c => c.Courts.Select(co => co.CourtKey).Contains(id))
        .Any())
      {
        if (model.Lock)
        {
          var memberFk = new AccessorBase<User>().GetFirstOrDefaultWhere(u => u.UserName == User.Identity.Name).UserId;
          if (memberFk != Guid.Empty)
          {
            var booking = Booking.CreateBooking(Guid.NewGuid(), SqlDateTime.MinValue.Value, SqlDateTime.MaxValue.Value.AddYears(-1), id, memberFk, (int)EBookingType.Closed);
            booking.CourtFk = id;
            new AccessorBase<Booking>().Add(booking);
          }
        }
        else
        {
          new AccessorBase<Booking>().RemoveAllWhereFunc(b => b.BookingTypeEnum == EBookingType.Closed && b.CourtFk == id);
        }
      }
    }

    public class DoLockModel
    {
      public bool Lock { get; set; }
    }
  }
}