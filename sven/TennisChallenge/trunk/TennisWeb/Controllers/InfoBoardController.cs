using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Omu.ValueInjecter;
using TennisChallenge.Dal;
using TennisWeb.Models;
using TennisWeb.Utils;

namespace TennisWeb.Controllers
{
  public class InfoBoardController : Controller
  {
    public ActionResult Index(Guid? clubId)
    {
      if (clubId.HasValue)
      {
        Response.Cookies["clubId"].Value = clubId.Value.ToString();
      }

      return View();
    }

    public ActionResult Standby()
    {
      try
      {
        var clubId = Guid.Parse(Request.Cookies["clubId"].Value);

        var model = new StandbyModel();
        model.ClubKey = clubId;

        return View(model);
      }
      catch
      {
      }

      return RedirectToAction("Index");
    }

    public ActionResult RankedStart(int gender)
    {
      try
      {
        var model = new RankedStartModel();
        model.gender = gender;

        return View(model);
      }
      catch
      {
      }

      return View();
    }

    [HttpPost]
    [MyAuthorize]
    public ActionResult BookTennisCourt(BookingRequest bookingRequest)
    {
      try
      {
        var courtKey = new AccessorBase<Court>().GetFirstOrDefaultWhere(c => c.Id == bookingRequest.CourtId).CourtKey;

        var booking = new Booking { BookingKey = Guid.NewGuid(), CourtFk = courtKey, BookingTypeEnum = EBookingType.Match };

        booking.InjectFrom(bookingRequest);

        new BookingAccessor().Add(booking);

        return Json(new { success = true });
      }
      catch (Exception e)
      {
        return Json(new { success = false, message = e.Message });
      }
    }

    public ActionResult InfoBoardLogOff()
    {
      FormsAuthentication.SignOut();

      return RedirectToAction("Index", "Infoboard");
    }


    [HttpPost]
    [MyAuthorize]
    public ActionResult CancelBooking(Guid id)
    {
      try
      {
        var bookingAccessor = new AccessorBase<Booking>();
        var booking = bookingAccessor.GetFirstOrDefaultWhere(b => b.BookingKey == id, b => b.Court.Club);

        if (booking != null && booking.Court.Club.UsersInClubs.Any(uic => uic.User.UserName == User.Identity.Name))
        {
          bookingAccessor.Remove(booking);
          return Json(new { success = true });
        }
      }
      catch (Exception)
      { }

      return Json(new { success = false });
    }

    private Guid GetClubId()
    {
      if (Request.Cookies["clubId"] != null)
      {
        return Guid.Parse(Request.Cookies["clubId"].Value);
      }
      else if (User.Identity.IsAuthenticated)
      {
        return new AccessorBase<UsersInClub>().GetFirstOrDefaultWhere(uic => uic.User.UserName == User.Identity.Name, i => i.User).ClubFK;
      }
      else
      {
        return Guid.NewGuid();
      }
    }

    /*TODO: Clean up this whole function. I am sure things can be done in a far better way*/
    public ActionResult GetInfoboardCalendarBookings()
    {
      var todayDate = DateTime.Today.Date;
      var bookings = new AccessorBase<Booking>().GetAllWhereFunc(b => b.StartTime.Date >= todayDate && b.StartTime.Date < todayDate.AddDays(5));
      var firstDayOfMonth = new DateTime(todayDate.Year, todayDate.Month, 1);
      var firstDayOfWeekInMonth = new DateTime();
      var isFirstDay = false;

      while (!isFirstDay)
      {
        if (firstDayOfMonth.Date.DayOfWeek != todayDate.DayOfWeek)
        {
          firstDayOfMonth = firstDayOfMonth.AddDays(1);
        }
        else
        {
          firstDayOfWeekInMonth = firstDayOfMonth;
          if (firstDayOfWeekInMonth.Day <= 7 && firstDayOfWeekInMonth.Day != 1)
          {
            firstDayOfWeekInMonth = firstDayOfWeekInMonth.AddDays(-7);
          }
          isFirstDay = true;
        }
      }

      var jBookings = bookings.Select(b => new
      {
        start = b.StartTime,
        end = b.EndTime,
        court = b.Court.Name,
        comment = b.Comment,
        type = b.BookingType
      });

      List<InfoboardCalendarModel> calModel = new List<InfoboardCalendarModel>();

      foreach (var booking in jBookings)
      {
        var model = new InfoboardCalendarModel();
        var firstDay = firstDayOfWeekInMonth.Date.Day;
        var start = booking.start;
        var end = booking.end;
        var toAdd = ((int)start.DayOfWeek - (int)todayDate.DayOfWeek);

        if (toAdd < 0)
        {
          toAdd += 7;
        }

        switch (booking.court)
        {
          case "Platz 1":
            var day = firstDayOfWeekInMonth.AddDays(toAdd);
            model.Start = new DateTime(start.Year, day.Month, day.Day, start.Hour, start.Minute, start.Second, start.Millisecond);
            model.End = new DateTime(end.Year, day.Month, day.Day, end.Hour, end.Minute, end.Second, end.Millisecond);
            model.Row = "week0";
            break;
          case "Platz 2":
            var court2DayDate = firstDayOfWeekInMonth.AddDays(7);
            court2DayDate = court2DayDate.AddDays(toAdd);
            var court2Day = Convert.ToInt32(court2DayDate.Day);
            model.Start = new DateTime(start.Year, court2DayDate.Month, court2Day, start.Hour, start.Minute, start.Second, start.Millisecond);
            model.End = new DateTime(end.Year, court2DayDate.Month, court2Day, end.Hour, end.Minute, end.Second, end.Millisecond);
            model.Row = "week1";
            break;
          case "Platz 3":
            var court3DayDate = firstDayOfWeekInMonth.AddDays(14);
            court3DayDate = court3DayDate.AddDays(toAdd);
            var court3Day = Convert.ToInt32(court3DayDate.Day);
            model.Start = new DateTime(start.Year, court3DayDate.Month, court3Day, start.Hour, start.Minute, start.Second, start.Millisecond);
            model.End = new DateTime(end.Year, court3DayDate.Month, court3Day, end.Hour, end.Minute, end.Second, end.Millisecond);
            model.Row = "week2";
            break;
          case "Platz 4":
            var court4DayDate = firstDayOfWeekInMonth.AddDays(21);
            court4DayDate = court4DayDate.AddDays(toAdd);
            var court4Day = Convert.ToInt32(court4DayDate.Day);
            model.Start = new DateTime(start.Year, court4DayDate.Month, court4Day, start.Hour, start.Minute, start.Second, start.Millisecond);
            model.End = new DateTime(end.Year, court4DayDate.Month, court4Day, end.Hour, end.Minute, end.Second, end.Millisecond);
            model.Row = "week3";
            break;
          case "Platz 5":
            var court5DayDate = firstDayOfWeekInMonth.AddDays(28);
            court5DayDate = court5DayDate.AddDays(toAdd);
            var court5Day = Convert.ToInt32(court5DayDate.Day);
            model.Start = new DateTime(start.Year, court5DayDate.Month, court5Day, start.Hour, start.Minute, start.Second, start.Millisecond);
            model.End = new DateTime(end.Year, court5DayDate.Month, court5Day, end.Hour, end.Minute, end.Second, end.Millisecond);
            model.Row = "week4";
            break;
        }

        model.Court = booking.court;
        model.Type = booking.type;
        model.StartTime = booking.start.Hour + ":" + booking.start.Minute;
        model.Comment = booking.comment;

        calModel.Add(model);
      }
      return Json(calModel, JsonRequestBehavior.AllowGet);
    }

    public ActionResult GetNextSevenBookings(int courtIdBook)
    {
      var courtId = new AccessorBase<Court>().GetFirstOrDefaultWhere(c => c.Id == courtIdBook).CourtKey;
      var bookings = new AccessorBase<Booking>().GetAllWhere(b => b.CourtFk == courtId && b.EndTime > DateTime.Now).OrderBy(b => b.StartTime);
      List<BookingInfoModel> nextBookings = new List<BookingInfoModel>();

      foreach (Booking booking in bookings)
      {
        var nextbooking = new BookingInfoModel();
        nextbooking.CourtComment = booking.Comment;
        nextbooking.TypeOfGame = booking.BookingType;
        nextbooking.StartTime = booking.StartTime;
        nextbooking.EndTime = booking.EndTime;
        nextbooking.Player0Name = booking.Member0.FullName;
        nextbooking.Player1Name = booking.Member1 != null ? booking.Member1.FullName : "";
        nextbooking.Player2Name = booking.Member2 != null ? booking.Member2.FullName : "";
        nextbooking.Player3Name = booking.Member3 != null ? booking.Member3.FullName : "";
        nextBookings.Add(nextbooking);
      }

      return Json(nextBookings, JsonRequestBehavior.AllowGet);
    }

    public void AddToBank(string memberId)
    {
      var memberGuid = new Guid(memberId);
      var bankAcc = new AccessorBase<Bank>();
      var bankModel = new Bank();

      bankModel.TransactionId = Guid.NewGuid();
      bankModel.MemberFk = memberGuid;
      bankModel.TransactionDate = DateTime.Now;
      //this is a standart amount for the guest player
      bankModel.ValueAmount = 12;

      bankAcc.Add(bankModel);
      bankAcc.SaveChanges();
    }

    //returns the owed amount the member has
    public JsonResult GetOwedAmount(string memberId)
    {
      var memberGuid = new Guid(memberId);
      var transaction = new AccessorBase<Bank>().GetAllWhere(b => b.MemberFk == memberGuid);
      double? amount = 0.0;

      foreach (var trans in transaction)
      {
        amount += trans.ValueAmount;
      }

      return Json(amount, JsonRequestBehavior.AllowGet);
    }

    //returns a ranked member for a username
    public ActionResult GetRankedMember(int gender, string userName)
    {
      var clubId = Guid.NewGuid();
      if (userName != "null")
      {
        clubId = new AccessorBase<UsersInClub>().GetFirstOrDefaultWhere(uic => uic.User.UserName == userName).ClubFK;
      }
      else
      {
        clubId = GetClubId();
      }
      var members = new AccessorBase<RankedMember>().GetAllWhere(rm => rm.UsersInClub.User.Member.TitleFk == gender && rm.UsersInClub.ClubFK == clubId).OrderBy(rm => rm.ClubRank);
      List<RankedMemberSimple> memberModel = new List<RankedMemberSimple>();

      foreach (var member in members)
      {
        var nMemberSimple = new RankedMemberSimple();
        nMemberSimple.FullName = member.UsersInClub.User.Member.FullName;
        nMemberSimple.Id = member.UsersInClub.User.Member.MemberKey;
        nMemberSimple.Rank = member.ClubRank;
        memberModel.Add(nMemberSimple);
      }

      return Json(memberModel, JsonRequestBehavior.AllowGet);
    }

    //returns all past bookings for a court
    public ActionResult GetPastBookings(string courtId)
    {
      var clubId = GetClubId();
      var courtGuid = new Guid(courtId);
      var games = new AccessorBase<Booking>().GetAllWhere(b => b.EndTime.Day == DateTime.Now.Day
                                                          && b.EndTime.Month == DateTime.Now.Month
                                                          && b.EndTime.Year == DateTime.Now.Year
                                                          && b.EndTime < DateTime.Now && b.CourtFk == courtGuid
                                                          && b.Court.ClubFk == clubId).OrderBy(b => b.EndTime);

      List<BookingInfoModel> gamesList = new List<BookingInfoModel>();
      foreach (var game in games)
      {
        var model = new BookingInfoModel();
        model.InjectFrom(game);
        model.TypeOfGame = game.BookingType;
        model.Player0Name = game.Member0.FullName;
        if (game.Member1 != null)
        {
          model.Player1Name = game.Member1.FullName;
        }
        if (game.Member2 != null)
        {
          model.Player2Name = game.Member2.FullName;
        }
        if (game.Member3 != null)
        {
          model.Player3Name = game.Member3.FullName;
        }
        gamesList.Add(model);
      }

      return Json(gamesList, JsonRequestBehavior.AllowGet);
    }

    [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
    public ActionResult GetCurrentBookings()
    {
      // TODO: Add an additional parameter to enable users to login in the inforboard of multiple clubs
      var clubId = GetClubId();

      var bookingsAccessor = new BookingAccessor();

      var now = DateTime.Now;
      var bookings =
        bookingsAccessor.GetAllWhereFunc(b => Calc.BookingIntersectsDateRange(b, now.AddHours(-1), now.AddHours(6)) && b.Court.ClubFk == clubId, i => i.Court).
          OrderBy(b => b.StartTime).
          GroupBy(b => b.Court);

      var courtList = new List<CourtBookingsModel>();

      foreach (IGrouping<Court, Booking> courtBookings in bookings)
      {
        var courtBookingModel = new CourtBookingsModel { CourtId = courtBookings.Key.Id, Bookings = new List<BookingModel>() };

        BookingOffer singleOffer, doubleOffer;
        var bookingsAvailable = CalculateBookingOffers(bookingsAccessor, courtBookings.Key.Id, out singleOffer, out doubleOffer);
        //if (bookingsAvailable)
        //  courtBookingModel.NextStartTime = singleOffer.StartTime;

        var firstGame = courtBookings.FirstOrDefault(g => g.EndTime > now);
        var gameBefore = courtBookings.FirstOrDefault(g => g.EndTime < now);

        if (firstGame != null)
        {
          courtBookingModel.Game1Player0Name = firstGame.Member0 != null ? firstGame.Member0.FullName : "";
          courtBookingModel.Game1Player1Name = firstGame.Member1 != null ? firstGame.Member1.FullName : "";
          if (firstGame.Member2 != null)
            courtBookingModel.Game1Player2Name = firstGame.Member2.FullName;
          if (firstGame.Member3 != null)
            courtBookingModel.Game1Player3Name = firstGame.Member3.FullName;
        }

        if (gameBefore != null && (firstGame == null || firstGame.StartTime > now.AddHours(1)))
        {
          courtBookingModel.EndLastGame = now - gameBefore.EndTime;
        }
        else
        {
          courtBookingModel.NextStartTime = singleOffer.StartTime;
        }

        foreach (var booking in courtBookings)
        {
          //IPA Relevant Begin (i just added the neccessary parameters player2, player3, and tournamentMode)
          var bookingModel = new BookingModel { StartTime = booking.StartTime, EndTime = booking.EndTime, BookingType = booking.BookingTypeEnum, CourtComment = booking.Comment, Player2 = booking.Member1Fk.ToString(), Player3 = booking.Member2Fk.ToString() };
          //IPA Relevant End

          if (booking.Tournament != null)
          {
            bookingModel.TournamentMode = booking.Tournament.Mode;
          }

          courtBookingModel.Bookings.Add(bookingModel);
        }

        courtList.Add(courtBookingModel);
      }

      return Json(courtList, JsonRequestBehavior.AllowGet);
    }

    [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
    //[MyAuthorize]

    //returns the next time a court can be booked.
    public ActionResult GetNextAvailableTime(int courtId)
    {
      var bookingsAccessor = new BookingAccessor();

      BookingOffer singleOffer, doubleOffer;
      var bookingsAvailable = CalculateBookingOffers(bookingsAccessor, courtId, out singleOffer, out doubleOffer);

      if (bookingsAvailable)
        return Json(new { Success = true, Single = singleOffer, @Double = doubleOffer }, JsonRequestBehavior.AllowGet);
      else
        return Json(new { success = false }, JsonRequestBehavior.AllowGet);
    }

    private bool CalculateBookingOffers(BookingAccessor accessor, int courtId, out BookingOffer singleOffer, out BookingOffer doubleOffer)
    {
      var now = DateTime.Now;

      var bookings =
        accessor.GetAllWhere(b => b.Court.Id == courtId && b.EndTime > now)
        .OrderBy(b => b.StartTime);

      var setStartTime = now;
      var toAddSingle = Calc.RoundMinutes(now.AddMinutes(TennisChallenge.Dal.Parameters.SingleDuration));
      var toAddDouble = Calc.RoundMinutes(now.AddMinutes(TennisChallenge.Dal.Parameters.DoubleDuration));


      singleOffer = new BookingOffer { StartTime = setStartTime, Duration = TimeSpan.FromMinutes(TennisChallenge.Dal.Parameters.SingleDuration + toAddSingle) };
      doubleOffer = new BookingOffer { StartTime = setStartTime, Duration = TimeSpan.FromMinutes(TennisChallenge.Dal.Parameters.DoubleDuration + toAddDouble) };
      bool singleOk = false, doubleOk = false;

      foreach (var booking in bookings)
      {
        if (booking.BookingTypeEnum == EBookingType.Closed)
          return false;

        if (Calc.DateRangesIntersect(booking.StartTime, booking.EndTime, singleOffer.StartTime, singleOffer.EndTime))
        {
          singleOffer.StartTime = Calc.RoundToNearestInterval(booking.EndTime, TimeSpan.FromMinutes(5));
          singleOffer.Duration = TimeSpan.FromMinutes(TennisChallenge.Dal.Parameters.SingleDuration + 5);
        }
        else
        {
          singleOk = true;
        }

        if (Calc.DateRangesIntersect(booking.StartTime, booking.EndTime, doubleOffer.StartTime, doubleOffer.EndTime))
        {
          doubleOffer.StartTime = Calc.RoundToNearestInterval(booking.EndTime, TimeSpan.FromMinutes(5));
          doubleOffer.Duration = TimeSpan.FromMinutes(TennisChallenge.Dal.Parameters.DoubleDuration + 5);
        }
        else
        {
          doubleOk = true;
        }

        if (singleOk && doubleOk)
          break;
      }

      return true;
    }

    public class BookingOffer
    {
      public DateTime StartTime { get; set; }
      public DateTime EndTime
      {
        get { return StartTime + Duration; }
      }
      public TimeSpan Duration { get; set; }
    }
  }
}