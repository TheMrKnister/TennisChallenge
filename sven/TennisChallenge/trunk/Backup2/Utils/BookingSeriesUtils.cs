using System;
using System.Collections.Generic;
using TennisChallenge.Dal;
using TennisWeb.Models;

namespace TennisWeb.Utils
{
  public static class BookingSeriesUtils
  {
    public static IEnumerable<Booking> BookingSeriesModelToBookings(BookingSeriesModel model, Guid memberKey)
    {
      var bookings = new List<Booking>();
      var begin = model.SeriesStart.Value;
      begin = begin.AddDays(-(double)begin.DayOfWeek).Date;
      if (model.Scheme == SerialBookingSchemes.Weekly)
      {
        for (var i = begin; i < model.SeriesEnd; i = i.AddDays(7d))
        {
          AddDaysOfWeek(model, memberKey, bookings, i);
        }
      }
      else if (model.Scheme == SerialBookingSchemes.Monthly)
      {
        for (var i = begin; i < model.SeriesEnd; i = i.AddMonths(1))
        {
          AddDaysOfWeek(model, memberKey, bookings, i.AddDays(7d * model.WeekOfMonth.Value));
        }
      }

      return bookings;
    }

    private static void AddDaysOfWeek(BookingSeriesModel model, Guid memberKey, List<Booking> bookings, DateTime currentWeek)
    {
      foreach (var dayOfWeek in model.DaysOfWeek)
      {
        var day = currentWeek.AddDays((double)dayOfWeek);
        var bookingBegin = day.Add(model.Start.Value.TimeOfDay);
        var bookingEnd = day.Add(model.End.Value.TimeOfDay);
        var booking = Booking.CreateBooking(Guid.NewGuid(), bookingBegin, bookingEnd, model.CourtFk.Value, memberKey, (int)model.BookingTypeEnum);
        booking.Comment = model.Title;
        bookings.Add(booking);
      }
    }
  }
}