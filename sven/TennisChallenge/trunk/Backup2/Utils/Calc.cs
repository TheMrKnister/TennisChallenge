using System;
using TennisChallenge.Dal;

namespace TennisWeb.Utils
{
  public static class Calc
  {

    //Funktion um auf Minuten zu runden
    public static DateTime RoundToNearestInterval(DateTime dt, TimeSpan d)
    {
      int f = 0;
      double m = (double)(dt.Ticks % d.Ticks) / d.Ticks;
      if (m >= 0.5)
        f = 1;
      return new DateTime(((dt.Ticks / d.Ticks) + f) * d.Ticks);
    }

    public static int RoundMinutes(DateTime endTime)
    {
      var toAdd = 6 - ((endTime.Minute + 1) % 5);
      if (toAdd < 3)
      {
        toAdd = toAdd + 5;
      }
      return toAdd;
    }

    public static DateTime NextAvailable(DateTime endTime, DateTime startTime)
    {
      if (startTime > DateTime.Now)
      {
        var toAdd = 6 - ((endTime.Minute + 1) % 5);

        return endTime.AddMinutes(toAdd);
      };

      return DateTime.Now.AddMinutes(5);
    }

    public static bool DateRangesIntersect(DateTime startTime1, DateTime endTime1, DateTime startTime2, DateTime endTime2)
    {
      var latestStart = startTime1 > startTime2 ? startTime1 : startTime2;
      var earliestEnd = endTime1 < endTime2 ? endTime1 : endTime2;
      return latestStart < earliestEnd;
    }

    public static bool BookingsIntersect(Booking booking1, Booking booking2)
    {
      return booking1.CourtFk == booking2.CourtFk && DateRangesIntersect(booking1.StartTime, booking1.EndTime, booking2.StartTime, booking2.EndTime);
    }

    public static bool BookingIntersectsDateRange(Booking booking, DateTime startTime, DateTime endTime)
    {
      return DateRangesIntersect(booking.StartTime, booking.EndTime, startTime, endTime);
    }
  }
}