using System;
using System.Collections.Generic;
using System.Linq;

namespace TennisChallenge.Dal
{
  public partial class Member
  {
    public Guid Id { get { return MemberKey; } }

    public string FullName { get { return String.Format("{0} {1}", Firstname, Lastname); } }

    public IEnumerable<Booking> MatchBookings
    {
      get
      {
        return Bookings0
          .Concat(Bookings1)
          .Concat(Bookings2)
          .Concat(Bookings3)
          .Where(b => b.BookingTypeEnum == EBookingType.Match);
      }
    }

    // Ich vermute, dass diese Abfrage komplett ineffizient ist, habe aber im Moment keine Zeit dies zu klären.
    // (Werden alle Reservationen geladen und dann lokal gefiltert? Nicht in DB? Dies würde mit der Zeit immer 
    // schlimmer, wenn immer mehr alte Reservationen vorhanden sind.)
    public Booking NextOpenBooking
    {
      get { return MatchBookings.Where(b => b.BookingTypeEnum == EBookingType.Match && b.EndTime > DateTime.Now).OrderBy(m => m.StartTime).FirstOrDefault(); }
    }

    public IEnumerable<Booking> NextOpenBookings
    {
      get { return MatchBookings.Where(b => b.BookingTypeEnum == EBookingType.Match && b.EndTime > DateTime.Now).OrderBy(m => m.StartTime); }
    }

    public bool HasOpenBooking
    {
      get 
      {
        foreach (var booking in NextOpenBookings)
        {
          var game = new AccessorBase<RankedGame>().GetFirstOrDefaultWhere(rg => rg.BookingFk == booking.BookingKey);
          if (game == null)
          {
            return true;
          }
          return false;
        }
        return false;
      }
    }

    public IEnumerable<Club> Clubs
    {
      get
      {
        return User.Clubs;
      }
    }

    public IEnumerable<Guid> ClubFks
    {
      get
      {
        return User.ClubFks;
      }
    }

    public IEnumerable<UsersInClub> UserInClubs
    {
      get
      {
        return User.UsersInClubs;
      }

      set
      {
        User.UsersInClubs.Clear();
        if (value != null)
        {
          foreach (var v in value)
            User.UsersInClubs.Add(v);
        }
      }
    }
  }
}
