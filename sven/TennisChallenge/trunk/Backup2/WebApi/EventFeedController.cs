using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Omu.ValueInjecter;
using TennisChallenge.Dal;
using TennisWeb.Models;
using TennisWeb.Utils;
using TennisWeb.Controllers;

namespace TennisWeb.WebApi
{
  public class EventFeedController : ApiController
  {
    private Member CurrentMember { get; set; }

    protected override void Initialize(HttpControllerContext controllerContext)
    {
      CurrentMember = new MemberAccessor().GetFirstOrDefaultWhere(m => m.User.UserName == User.Identity.Name, i => i.User);

      base.Initialize(controllerContext);
    }

    public IEnumerable<FullcalendarEvent> Get(Guid id, double start, double end)
    {
      var startTime = new DateTime(1970, 1, 1).AddSeconds(start);
      var endTime = new DateTime(1970, 1, 1).AddSeconds(end);

      var bookings = new AccessorBase<Booking>()
        .GetAllWhere(b =>
          (b.StartTime > startTime && b.EndTime < endTime ||
          b.StartTime < endTime && b.EndTime > startTime)
          && b.CourtFk == id);

      var events = bookings.Select(b =>
      {
        var e = new FullcalendarEvent();
        e.InjectFrom(b)
         .InjectFrom<BookingToFullcalendarEventInjection>(b);
        e.Editable = b.Member0Fk == CurrentMember.MemberKey;
        return e;
      });

      return events;
    }

    public void Post([FromBody]FullcalendarEvent model)
    {
      var dbContext = new DbContext(new TennisChallengeEntities(), true);
      var role =
        model.BookingTypeEnum == EBookingType.Closed ? RoleNames.Janitor :
        model.BookingTypeEnum == EBookingType.Interclub ? RoleNames.InterClubOrganizer :
        model.BookingTypeEnum == EBookingType.School ? RoleNames.TennisTeacher :
        model.BookingTypeEnum == EBookingType.Tournament ? RoleNames.CasualTournamentOrganizer :
        null;

      var clubId = new AccessorBase<Club>(dbContext).GetFirstOrDefaultWhereFunc(FilterFunctions.ClubsWhereUserIsInRole(User.Identity.Name, role)).ClubKey;
      var memberKey = new AccessorBase<Member>(dbContext).GetFirstOrDefaultWhereFunc(m => m.User.UserName == User.Identity.Name && m.Clubs.Any(c => c.ClubKey == clubId)).MemberKey;


      //IPA Relevant Beginn
      if (model.BookingTypeEnum == EBookingType.Tournament)
      {
        if (model.TournamentId.ToString() == "5087e240-1c83-4c2a-8f74-0e7e4bdfd03c")
        {
          var tournamentId = Guid.NewGuid();
          TournamentController.CreateTounament(tournamentId, (int)model.Mode, (int)model.Type, model.TournComment);
          model.TournamentId = tournamentId;
        }

        foreach (var court in model.TournamentCourts)
        {
          var booking = new Booking();
          booking.InjectFrom(model)
               .InjectFrom<FullcalendarEventToBookingInjection>(model);

          booking.BookingKey = Guid.NewGuid();
          booking.Member0Fk = memberKey;
          booking.CourtFk = court;

          new AccessorBase<Booking>().Add(booking);
        }
      }
      else
      {
        //IPA Relevant End
        var booking = new Booking();
        booking.InjectFrom(model)
               .InjectFrom<FullcalendarEventToBookingInjection>(model);
        booking.BookingKey = Guid.NewGuid();
        booking.Member0Fk = memberKey;

        new AccessorBase<Booking>().Add(booking);
      }
    }

    public void Put([FromBody]FullcalendarEvent model)
    {
      var bookingAccessor = new AccessorBase<Booking>();

      var booking = bookingAccessor.GetFirstOrDefaultWhere(b => b.BookingKey == model.Id);
      booking.InjectFrom(model)
             .InjectFrom<FullcalendarEventToBookingInjection>(model);
      bookingAccessor.SaveChanges();
    }

    public HttpResponseMessage Delete([FromBody]Guid bookingKey)
    {
      var bookingAccessor = new BookingAccessor();
      var booking = bookingAccessor.GetFirstOrDefaultWhere(b => b.BookingKey == bookingKey);

      switch (booking.BookingTypeEnum)
      {
        case EBookingType.Match:
          var memberFk = new AccessorBase<User>().GetFirstOrDefaultWhere(u => u.UserName == User.Identity.Name).UserId;
          if (booking.Member0Fk != memberFk)
            return new HttpResponseMessage(HttpStatusCode.Forbidden);
          break;
        case EBookingType.Closed:
          if (!User.IsInRoles(RoleNames.Janitor))
            return new HttpResponseMessage(HttpStatusCode.Forbidden);
          break;
        case EBookingType.School:
          if (!User.IsInRoles(RoleNames.TennisTeacher))
            return new HttpResponseMessage(HttpStatusCode.Forbidden);
          break;
        case EBookingType.Interclub:
          if (!User.IsInRoles(RoleNames.InterClubOrganizer))
            return new HttpResponseMessage(HttpStatusCode.Forbidden);
          break;
        case EBookingType.Tournament:
          if (!User.IsInRoles(RoleNames.CasualTournamentOrganizer))
            return new HttpResponseMessage(HttpStatusCode.Forbidden);
          break;
      }
      bookingAccessor.Remove(booking);
      return new HttpResponseMessage(HttpStatusCode.NoContent);
    }
  }
}
