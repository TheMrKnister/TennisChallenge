using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TennisChallenge.Dal
{
  public class BookingAccessor : AccessorBase<Booking>
  {
    public override void Add(Booking booking)
    {
      if (booking == null)
        throw new ArgumentNullException();

      var memberAccessor = new MemberAccessor();
      var bookingMember = memberAccessor.GetFirstOrDefaultWhere(
        m => m.MemberKey == booking.Member0Fk, m => m.User.UsersInClubs, m => m.User);

      if ((EBookingType)booking.BookingType == EBookingType.Match)
      {
        if (!booking.Member1Fk.HasValue)    // Mindestens 1 zweiter Spieler ist notwendig.
          throw new InvalidOperationException("Invalid player selection.");

        // Falls Spieler 2 oder 3 gesetzt ist, so wird ein Doppel gebucht und es müssen alle 4 Spieler gesetzt sein.
        if ((booking.Member2Fk.HasValue && !booking.Member3Fk.HasValue) ||
           (booking.Member3Fk.HasValue && !booking.Member2Fk.HasValue))
          throw new InvalidOperationException("Invalid player selection.");

        // Spare die Überprüfung, ob Spieler 1 bis 3 im Klub sind. Sollte diese evtl. doch noch gemacht werden?
      }

      base.Add(booking);
    }
  }
}
