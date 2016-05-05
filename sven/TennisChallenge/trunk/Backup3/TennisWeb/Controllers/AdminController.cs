using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Omu.ValueInjecter;
using TennisChallenge.Dal;
using TennisWeb.Models;
using TennisWeb.Utils;

namespace TennisWeb.Controllers
{
  public class AdminController : Controller
  {
    protected override void Initialize(RequestContext requestContext)
    {
      ViewBag.NavTopCat = NavigationTopCat.Admin;
      base.Initialize(requestContext);
    }

    public PartialViewResult AdminMenu(AdminNavigationCat? adminNavigationCat)
    {
      return PartialView("_AdminMenu", adminNavigationCat);
    }

    private BookingSeriesModel CreateBookingSeriesModel(EBookingType bookingType, string roleName)
    {
      var clubId = new AccessorBase<Club>().GetFirstOrDefaultWhereFunc(FilterFunctions.ClubsWhereUserIsInRole(User.Identity.Name, roleName)).ClubKey;

      var model = new BookingSeriesModel();
      model.AllCourts = new AccessorBase<Court>()
        .GetAllWhere(c => c.ClubFk == clubId)
        .OrderBy(c => c.Id)
        .Select(c => { var cs = new CourtSimple(); cs.InjectFrom(c); return cs; });

      model.BookingTypeEnum = bookingType;
      return model;
    }

    public PartialViewResult CreateBookingSeries(EBookingType bookingType)
    {
      BookingSeriesModel model = null;
      switch (bookingType)
      {
        case EBookingType.School:
          model = CreateBookingSeriesModel(bookingType, RoleNames.TennisTeacher);
          break;
        case EBookingType.Interclub:
          model = CreateBookingSeriesModel(bookingType, RoleNames.InterClubOrganizer);
          break;
        case EBookingType.Tournament:
          model = CreateBookingSeriesModel(bookingType, RoleNames.CasualTournamentOrganizer);
          break;
      }
      return PartialView("_CreateBookingSeries", model);
    }

    //
    // GET: /Admin

    public ActionResult Index()
    {
      return User.IsInRoles(RoleNames.ClubAdmin) ? RedirectToAction("EditUser")
           : User.IsInRoles(RoleNames.Janitor) ? RedirectToAction("EditCourtLockings")
           : User.IsInRoles(RoleNames.TennisTeacher) ? RedirectToAction("CreateTeacherBooking")
           : User.IsInRoles(RoleNames.CasualTournamentOrganizer) ? RedirectToAction("CreateTournamentBooking")
           : User.IsInRoles(RoleNames.InterClubOrganizer) ? RedirectToAction("CreateInterclubBooking")
           : User.IsInRoles(RoleNames.Host) ? RedirectToAction("Host")
           : User.IsInRoles(RoleNames.EventManager) ? RedirectToAction("Events")
                                                    : RedirectToAction("Index", "Home");
    }

    //
    // GET: /Admin/UsersInClubsList

    [MyAuthorize(Roles = RoleNames.ClubAdmin)]
    public ActionResult UsersInClubsList()
    {
      // make sure to only get the data where the Logged in user is admin.
      var clubs = new AccessorBase<Club>().GetAllWhere(c => c.UsersInClubs.Any(uic => uic.User.UserName == User.Identity.Name && uic.Roles.Any(r => r.RoleName == RoleNames.ClubAdmin)), c => c.UsersInClubs);

      return View(new UserListModel(clubs));
    }

    //
    // GET: /Admin/CreateUser

    [MyAuthorize(Roles = RoleNames.ClubAdmin)]
    public ActionResult CreateUser()
    {
      var clubsThisUserIsAdminOf = new AccessorBase<Club>().GetAllWhere(
        c => c.UsersInClubs.Any(
          uic =>
            uic.Roles.Any(r => r.RoleName == RoleNames.ClubAdmin) &&
            uic.User.UserName == User.Identity.Name
          )
        ).Select(c => c.Name).ToArray();

      var model = new CreateUserModel();
      model.FillTitles();
      model.FillAllClubs(clubsThisUserIsAdminOf);
      return View(model);
    }

    //
    // POST: /Admin/CreateUser

    [HttpPost]
    [MyAuthorize(Roles = RoleNames.ClubAdmin)]
    public ActionResult CreateUser(CreateUserModel model,
      float crop_x = 0f, float crop_y = 0f, float crop_w = 0f, float crop_h = 0f)
    {
      if (ModelState.IsValid)
      {
        try
        {
          var memberAccessor = new MemberAccessor();
          var member = Member.CreateMember(Guid.NewGuid(), true);
          member.InjectFrom(model);
          var createStatus = memberAccessor.CreateMember(model.UserName, model.Password, member);
          if (createStatus == MembershipCreateStatus.Success)
          {
            var user = Membership.GetUser(model.UserName);
            user.IsApproved = true;
            Membership.UpdateUser(user);

            member.UserInClubs = model.ClubFks.Select(c => UsersInClub.CreateUsersInClub(Guid.NewGuid(), member.MemberKey, c));
            memberAccessor.SaveChanges();

            return RedirectToAction("Index");
          }
        }
        catch
        {
        }
      }
      ModelState.AddModelError("", "Etwas ist falsch gelaufen. Nichts wurde verändert");

      var clubsThisUserIsAdminOf = new AccessorBase<Club>().GetAllWhere(
        c => c.UsersInClubs.Any(
          uic =>
            uic.Roles.Any(r => r.RoleName == RoleNames.ClubAdmin) &&
            uic.User.UserName == User.Identity.Name
          )
        ).Select(c => c.Name).ToArray();

      model.FillTitles();
      model.FillAllClubs(clubsThisUserIsAdminOf);

      return View(model);
    }

    //
    // GET: /Admin/EditUser/5

    [MyAuthorize(Roles = RoleNames.ClubAdmin)]
    public ActionResult EditUser()
    {
      ViewBag.AdminCat = AdminNavigationCat.EditUser;

      var adminEditViewModel = new EditUserInClubModel
      {
        AllRoles = new AccessorBase<Role>().GetAllWhere(r => RoleNames.AllRoles.Contains(r.RoleName)),
      };
      return View(adminEditViewModel);
    }

    //
    // POST: /Admin/EditUser/5

    [HttpPost]
    [MyAuthorize(Roles = RoleNames.ClubAdmin)]
    public ActionResult EditUser(EditUserInClubModel editUserInClubModel)
    {
      try
      {
        var dbContext = new DbContext(new TennisChallengeEntities(), true);

        var usersInClubAccessor = new AccessorBase<UsersInClub>(dbContext);

        var userInClub = usersInClubAccessor
          .GetFirstOrDefaultWhere(u => u.UsersInClubsKey == editUserInClubModel.UsersInClubsKey, u => u.Roles);

        if (userInClub == null)
          throw new HttpException((int)HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());

        // remove all the roles which are in the database but where not sent by the view
        var rolesToRemove = userInClub.Roles.Where(r => !editUserInClubModel.Roles.Contains(r.RoleId)).ToList();
        foreach (var roleToRemove in rolesToRemove)
          userInClub.Roles.Remove(roleToRemove);

        // add all the roles which came from the view but are not in the database.
        var rolesToAdd = new AccessorBase<Role>(dbContext).GetAllWhereFunc(r =>
          editUserInClubModel.Roles.Contains(r.RoleId) &&
          !userInClub.Roles.Select(r_ => r_.RoleId).Contains(r.RoleId));
        foreach (var roleToAdd in rolesToAdd)
          userInClub.Roles.Add(roleToAdd);

        usersInClubAccessor.SaveChanges();

        return RedirectToAction("Index");
      }
      catch (InvalidOperationException)
      {
        ModelState.AddModelError("", "Etwas ist passiert. Nichts wurde verändert");
        return RedirectToAction("EditUser", new { id = editUserInClubModel.UsersInClubsKey });
      }
    }

    //
    // GET: /Admin/EditUserPassword/5
    [MyAuthorize(Roles = RoleNames.ClubAdmin)]
    public ActionResult EditUserPassword(Guid id)
    {
      var model = new ResetPasswordModel();

      return View(model);
    }


    //
    // POST: /Admin/EditUserPassword/5
    [HttpPost]
    [MyAuthorize(Roles = RoleNames.ClubAdmin)]
    public ActionResult EditUserPassword(Guid id, ResetPasswordModel model)
    {
      var user = new AccessorBase<User>().GetFirstOrDefaultWhere(u =>
        u.UserId == id &&
        u.UsersInClubs.Any(uic => uic.Club.UsersInClubs.Any(uic_ => uic_.User.UserName == User.Identity.Name && uic_.Roles.Select(r => r.RoleName).Contains(RoleNames.ClubAdmin))),
        i => i.UsersInClubs,
        i => i.UsersInClubs.Select(uic => uic.Club),
        i => i.UsersInClubs.Select(uic => uic.Club).Select(c => c.UsersInClubs));

      if (ModelState.IsValid && user != null)
      {
        var member = Membership.GetUser(id);
        member.ChangePassword(member.ResetPassword(), model.Password);
        Membership.UpdateUser(member);

        return RedirectToAction("Index");
      }

      ModelState.AddModelError("", "Es ist ein Fehler aufgetreten");

      return View(model);
    }

    [MyAuthorize(Roles = RoleNames.ClubAdmin)]
    public ActionResult DeleteConnectedCards(Guid id)
    {
      try
      {
        var cards = new AccessorBase<Rfid>();
        cards.RemoveAllWhere(c => c.MemberFk == id);

        return RedirectToAction("Index");
      }
      catch (InvalidOperationException)
      {
        return RedirectToAction("Index");
      }
    }

    //
    // GET: /Admin/DeleteUser/5

    [MyAuthorize(Roles = RoleNames.ClubAdmin)]
    public ActionResult DeleteUser(Guid id)
    {
      try
      {
        var member = new AccessorBase<Member>();
        var bookings = new AccessorBase<Booking>();
        var usersInClub = new AccessorBase<UsersInClub>();
        var userName = new AccessorBase<User>().GetFirstOrDefaultWhere(u => u.UserId == id).UserName;
        bookings.RemoveAllWhere(b => b.Member0Fk == id || b.Member1Fk == id || b.Member2Fk == id || b.Member3Fk == id);
        member.RemoveAllWhereFunc(m => m.Id == id);
        usersInClub.RemoveAllWhere(uic => uic.UserFK == id);
        Membership.DeleteUser(userName, true);

        return RedirectToAction("Index");
      }
      catch (InvalidOperationException)
      {
        return RedirectToAction("Index");
      }
    }

    //
    // POST: /Admin/DeleteUser/5

    [HttpPost]
    [MyAuthorize(Roles = RoleNames.ClubAdmin)]
    public ActionResult DeleteUser(Guid id, FormCollection formCollection)
    {
      try
      {
        new AccessorBase<UsersInClub>().RemoveAllWhere(uic => uic.UsersInClubsKey == id);
        return RedirectToAction("Index");
      }
      catch
      {
        return View();
      }
    }

    [MyAuthorize(Roles = RoleNames.CasualTournamentOrganizer)]
    public ActionResult AddToRanked()
    {
      return View();
    }

    public bool CascadeRanked(int rank, bool directionUp, short? gender)
    {
      var accessor = new AccessorBase<RankedMember>();
      var ranks = accessor.GetAllWhere(rm => rm.ClubRank >= rank && rm.UsersInClub.User.Member.TitleFk == gender);

      foreach (var ranked in ranks)
      {
        if (directionUp)
        {
          ranked.ClubRank += 1;
        }
        else
        {
          ranked.ClubRank -= 1;
        }
      }
      accessor.SaveChanges();
      return true;
    }

    public List<int?> GetAllRanks(short? gender)
    {
      List<int?> allRanks = new List<int?>();
      var ranks = new AccessorBase<RankedMember>().GetAllWhere(rm => rm.UsersInClub.User.Member.TitleFk == gender);

      foreach (var ranked in ranks)
      {
        var rank = ranked.ClubRank;
        allRanks.Add(rank);
      }

      return allRanks;
    }

    public ActionResult AddPlayerToRanked(Guid id, int clubRank)
    {
      try
      {
        var clubId = new AccessorBase<Club>().GetFirstOrDefaultWhereFunc(FilterFunctions.ClubsWhereUserIsInRole(User.Identity.Name, RoleNames.CasualTournamentOrganizer)).ClubKey;
        var UserInClubId = new AccessorBase<UsersInClub>().GetFirstOrDefaultWhere(uic => uic.UserFK == id && uic.ClubFK == clubId).UsersInClubsKey;
        var gender = new AccessorBase<Member>().GetFirstOrDefaultWhere(m => m.MemberKey == id).TitleFk;
        var RankedMemberAcc = new AccessorBase<RankedMember>();
        var model = new RankedMember();
        var allRanks = GetAllRanks(gender);

        if (allRanks.Contains(clubRank))
        {
          CascadeRanked(clubRank, true, gender);
        }

        model.ClubRank = clubRank;
        model.RankedMemberFk = UserInClubId;
        model.RankedMemberKey = Guid.NewGuid();
        model.SwissTennisRank = 0;

        RankedMemberAcc.Add(model);

        return RedirectToAction("AddToRanked");
      }
      catch
      {
        return View("index");
      }
    }

    public ActionResult DeletePlayerFromRanked(Guid id)
    {
      try
      {
        var UserInClubId = new AccessorBase<UsersInClub>().GetFirstOrDefaultWhere(uic => uic.UserFK == id).UsersInClubsKey;
        var gender = new AccessorBase<Member>().GetFirstOrDefaultWhere(m => m.MemberKey == id).TitleFk;
        var rank = new AccessorBase<RankedMember>().GetFirstOrDefaultWhere(m => m.RankedMemberFk == UserInClubId).ClubRank;
        var intRank = rank.Value;
        new AccessorBase<RankedMember>().RemoveAllWhere(m => m.RankedMemberFk == UserInClubId);
        CascadeRanked(intRank, false, gender);
        return RedirectToAction("AddToRanked");
      }
      catch
      {
        return View("Index");
      }
    }

    public ActionResult GetRankedPlayers()
    {
      var rankedMembers = new AccessorBase<RankedMember>().GetAll().OrderBy(m => m.ClubRank);
      List<RankedMemberSimple> memberModel = new List<RankedMemberSimple>();

      foreach (var member in rankedMembers)
      {
        var nMemberSimple = new RankedMemberSimple();
        nMemberSimple.FullName = member.UsersInClub.User.Member.FullName;
        nMemberSimple.Id = member.UsersInClub.User.Member.MemberKey;
        nMemberSimple.Rank = member.ClubRank;
        memberModel.Add(nMemberSimple);
      }

      memberModel.OrderBy(m => m.Rank);

      return Json(memberModel, JsonRequestBehavior.AllowGet);
    }

    private FullcalendarEventEditModel CreateBookingModel(string roleName, EBookingType bookingType)
    {
      var clubId = new AccessorBase<Club>()
      .GetFirstOrDefaultWhereFunc(FilterFunctions.ClubsWhereUserIsInRole(User.Identity.Name, roleName))
      .ClubKey;

      var courts = new AccessorBase<Court>().GetAllWhere(c => c.ClubFk == clubId);
      var tournaments = new AccessorBase<Tournament>().GetAllWhere(t => (t.TournamentType == 2 || t.TournamentType == 3) && t.TournamentOpen == null);
      var model = new FullcalendarEventEditModel();
      model.AllCourts = courts
        .OrderBy(c => c.Id)
        .Select(c => { var m = new CourtSimple(); m.InjectFrom(c); return m; });

      model.AllTournaments = tournaments.Select(t => { var m = new TournSimple(); m.InjectFrom(t); m.TournName = t.BookingBases.FirstOrDefault().Comment; return m; });
      model.BookingTypeEnum = bookingType;

      return model;
    }

    //
    // GET: /Admin/CreateTeacherBooking

    [MyAuthorize(Roles = RoleNames.TennisTeacher)]
    public ActionResult CreateTeacherBooking()
    {
      var model = CreateBookingModel(RoleNames.TennisTeacher, EBookingType.School);

      return View(model);
    }

    //
    // POST: /Admin/CreateBookingSeries
    [HttpPost]
    [MyAuthorize(Roles = RoleNames.TennisTeacher + "," + RoleNames.CasualTournamentOrganizer + "," + RoleNames.InterClubOrganizer)]
    public ActionResult CreateBookingSeries(BookingSeriesModel model)
    {
      if (ModelState.IsValid)
      {
        var memberFk = new AccessorBase<User>().GetFirstOrDefaultWhere(u => u.UserName == User.Identity.Name).UserId;
        var bookingsToAdd = BookingSeriesUtils.BookingSeriesModelToBookings(model, memberFk);
        var currentBookings = new BookingAccessor().GetAllWhereFunc(b => b.CourtFk == model.CourtFk && Calc.DateRangesIntersect(b.StartTime, b.EndTime, model.SeriesStart.Value, model.SeriesEnd.Value));

        if (bookingsToAdd.Any(ba => currentBookings.Any(cb => Calc.BookingsIntersect(cb, ba))))
        {
          ModelState.AddModelError("", "Eine der Buchungen kollidiert mit einer der bestehenden Buchungen");
          return Redirect(Request.UrlReferrer.LocalPath);
        }
        else
        {
          var bookingSeries = BookingSeries.CreateBookingSeries(Guid.NewGuid(), model.Name);

          foreach (var bookingToAdd in bookingsToAdd)
          {
            bookingSeries.Bookings.Add(bookingToAdd);
          }

          new AccessorBase<BookingSeries>().Add(bookingSeries);
        }
      }

      return RedirectToAction("Index");
    }

    [MyAuthorize]
    public ActionResult ListBookingSeries()
    {
      var userId = new AccessorBase<User>().GetFirstOrDefaultWhere(u => u.UserName == User.Identity.Name).UserId;
      var bookingSeries = new AccessorBase<BookingSeries>().GetAllWhere(bs => bs.Bookings.Any(b => b.Member0Fk == userId), i => i.Bookings)
        .OrderBy(b => b.Name.ToLower());

      var models = bookingSeries.Select(b =>
      {
        var m = new BookingSeriesSimple();
        m.InjectFrom(b);
        return m;
      });

      return View(models);
    }

    [MyAuthorize]
    public static DateTime GetFirstDateOfBooking(Guid id)
    {
      var bookings = new AccessorBase<Booking>().GetAllWhere(b => b.BookingSeriesFk == id).OrderByDescending(b => b.StartTime);
      var day = bookings.Last().StartTime.Date;
      return day;
    }

    [MyAuthorize]
    public static TimeSpan GetStartTime(Guid id)
    {
      var bookings = new AccessorBase<Booking>().GetAllWhere(b => b.BookingSeriesFk == id).OrderByDescending(b => b.StartTime);
      var time = bookings.Last().StartTime.TimeOfDay;
      return time;
    }

    [MyAuthorize]
    public static String GetBookingSeriesCourt(Guid id)
    {
      var bookings = new AccessorBase<Booking>().GetAllWhere(b => b.BookingSeriesFk == id).OrderByDescending(b => b.StartTime);
      var court = bookings.Last().Court.Name;
      return court;
    }

    [MyAuthorize]
    public ActionResult DeleteBookingSeries(Guid id)
    {
      var bookingSeriesAccessor = new AccessorBase<BookingSeries>();
      var userId = new AccessorBase<User>().GetFirstOrDefaultWhere(u => u.UserName == User.Identity.Name).UserId;
      var bookingSeries = bookingSeriesAccessor.GetEntity(id);

      if (bookingSeries.Bookings.All(bs => bs.Member0Fk == userId))
      {
        bookingSeriesAccessor.Remove(bookingSeries);
      }

      return RedirectToAction("Index");
    }

    [MyAuthorize(Roles = RoleNames.InterClubOrganizer)]
    public ActionResult DeleteCurrentInterclubOnCourt(Guid id)
    {
      var bookingBase = new AccessorBase<Booking>();
      bookingBase.RemoveAllWhere(b => b.BookingType == 3 && b.StartTime.Day == DateTime.Now.Day && b.StartTime.Month == DateTime.Now.Month && b.CourtFk == id);

      return RedirectToAction("CreateInterclubBooking");
    }

    [MyAuthorize(Roles = RoleNames.CasualTournamentOrganizer)]
    public ActionResult DeleteCurrentTournamentOnCourt(Guid id)
    {
      var bookingBase = new AccessorBase<Booking>();
      bookingBase.RemoveAllWhere(b => b.BookingType == 4 && b.StartTime.Day == DateTime.Now.Day && b.StartTime.Month == DateTime.Now.Month && b.CourtFk == id);

      return RedirectToAction("CreateTournamentBooking");
    }

    [MyAuthorize(Roles = RoleNames.CasualTournamentOrganizer)]
    public ActionResult CreateTournamentBooking()
    {
      var model = CreateBookingModel(RoleNames.CasualTournamentOrganizer, EBookingType.Tournament);
      return View(model);
    }

    [MyAuthorize(Roles = RoleNames.InterClubOrganizer)]
    public ActionResult CreateInterclubBooking()
    {
      var model = CreateBookingModel(RoleNames.InterClubOrganizer, EBookingType.Interclub);

      return View(model);
    }

    [MyAuthorize(Roles = RoleNames.Janitor)]
    public ActionResult EditCourtLockings()
    {
      var clubId = new AccessorBase<Club>()
        .GetFirstOrDefaultWhereFunc(FilterFunctions.ClubsWhereUserIsInRole(User.Identity.Name, RoleNames.Janitor))
        .ClubKey;

      var model = new AccessorBase<Court>()
      .GetAllWhere(c => c.ClubFk == clubId)
      .OrderBy(c => c.Id)
      .Select(c => new CourtSimple().InjectFrom(c))
      .Cast<CourtSimple>();

      return View(model);
    }

    [MyAuthorize(Roles = RoleNames.AdvertisementManager)]
    public PartialViewResult CreateAdvertisement()
    {
      var clubKey = new AccessorBase<Club>()
        .GetFirstOrDefaultWhereFunc(FilterFunctions.ClubsWhereUserIsInRole(User.Identity.Name, RoleNames.AdvertisementManager))
        .ClubKey;

      var model = new AdvertisementModel { Id = Guid.NewGuid(), Active = true, ClubKey = clubKey };

      ViewData.TemplateInfo.HtmlFieldPrefix = "[0]";

      return PartialView("~/Views/Shared/EditorTemplates/AdvertisementModel.cshtml", model);
    }

    [MyAuthorize(Roles = RoleNames.AdvertisementManager)]
    public ActionResult EditAdvertisements()
    {
      var clubKey = new AccessorBase<Club>()
        .GetFirstOrDefaultWhereFunc(FilterFunctions.ClubsWhereUserIsInRole(User.Identity.Name, RoleNames.AdvertisementManager))
        .ClubKey;

      var advertisemets = new AccessorBase<Advertisement>().GetAllWhere(a => a.ClubKey == clubKey);

      var model = advertisemets.Select(a =>
      {
        var am = new AdvertisementModel();
        am.InjectFrom(a)
          .InjectFrom<NormalToNullable>(a);
        return am;
      });

      return View(model);
    }

    [MyAuthorize(Roles = RoleNames.AdvertisementManager)]
    [HttpPost]
    public ActionResult EditAdvertisements(IEnumerable<AdvertisementModel> model)
    {
      try
      {
        var advertisementAccessor = new AccessorBase<Advertisement>();
        var clubKey = new AccessorBase<Club>().GetFirstOrDefaultWhereFunc(FilterFunctions.ClubsWhereUserIsInRole(User.Identity.Name, RoleNames.AdvertisementManager),
          i => i.UsersInClubs, i => i.UsersInClubs.Select(uic => uic.Roles), i => i.UsersInClubs.Select(uic => uic.User)).ClubKey;

        if (ModelState.IsValid && model.All(a => clubKey == a.ClubKey))
        {
          foreach (var advertisementModel in model)
          {
            if (advertisementModel.Delete)
            {
              advertisementAccessor.RemoveByKey(advertisementModel.Id);
            }
            else
            {
              if (advertisementModel.Image != null)
              {
                var imageName = Picture.AdvertisementImagePath + Picture.GeneratePictureName(advertisementModel.Image);
                var serverPath = Server.MapPath(imageName);
                advertisementModel.Image.SaveAs(serverPath);
                advertisementModel.ImageUrl = imageName;
              }

              var existingAdvertisement = advertisementAccessor.GetFirstOrDefaultWhere(a => a.Id == advertisementModel.Id);

              if (existingAdvertisement != null)
              {
                existingAdvertisement
                .InjectFrom(advertisementModel)
                .InjectFrom<NullableToNormal>(advertisementModel);

                advertisementAccessor.SaveChanges();
              }
              else
              {
                var advertisement = new Advertisement();
                advertisement
                  .InjectFrom(advertisementModel)
                  .InjectFrom<NullableToNormal>(advertisementModel);
                advertisementAccessor.Add(advertisement);
              }
            }
          }

          return RedirectToAction("Index");
        }
      }
      catch
      {
      }

      ModelState.AddModelError("", "Es ist ein Fehler aufgetretten");
      return View(model);
    }

    [MyAuthorize(Roles = RoleNames.TopicalityManger)]
    public PartialViewResult CreateNewsFeed()
    {
      var clubKey = new AccessorBase<Club>()
        .GetFirstOrDefaultWhereFunc(FilterFunctions.ClubsWhereUserIsInRole(User.Identity.Name, RoleNames.TopicalityManger))
        .ClubKey;

      var model = new NewsFeedModel { Id = Guid.NewGuid(), Active = true, ClubKey = clubKey };

      ViewData.TemplateInfo.HtmlFieldPrefix = "NewsFeeds[" + model.Id.ToString() + "]";

      return PartialView("~/Views/Shared/EditorTemplates/NewsFeedModel.cshtml", model);
    }

    [MyAuthorize(Roles = RoleNames.TopicalityManger)]
    public ActionResult EditNewsFeeds()
    {
      var club = new AccessorBase<Club>().GetFirstOrDefaultWhereFunc(FilterFunctions.ClubsWhereUserIsInRole(User.Identity.Name, RoleNames.TopicalityManger),
        i => i.UsersInClubs, i => i.UsersInClubs.Select(uic => uic.Roles), i => i.UsersInClubs.Select(uic => uic.User), i => i.NewsFeeds);
      var model = new NewsFeedsModel();
      model
        .InjectFrom(club)
        .InjectFrom<NormalToNullable>(club);
      model.NewsFeeds = club.NewsFeeds.Select(n =>
      {
        var nm = new NewsFeedModel();
        nm
          .InjectFrom(n)
          .InjectFrom<NormalToNullable>(n);
        return nm;
      });

      return View(model);
    }

    [HttpPost]
    [MyAuthorize(Roles = RoleNames.TopicalityManger)]
    public ActionResult EditNewsFeeds(NewsFeedsModel model)
    {
      try
      {
        var clubAccessor = new AccessorBase<Club>();
        var newsFeedAccessor = new AccessorBase<NewsFeed>();
        var club = clubAccessor.GetFirstOrDefaultWhereFunc(FilterFunctions.ClubsWhereUserIsInRole(User.Identity.Name, RoleNames.TopicalityManger),
          i => i.UsersInClubs, i => i.UsersInClubs.Select(uic => uic.Roles), i => i.UsersInClubs.Select(uic => uic.User));

        if (ModelState.IsValid && club.ClubKey == model.ClubKey)
        {
          club
            .InjectFrom(model)
            .InjectFrom<NullableToNormal>(model);
          clubAccessor.SaveChanges();

          if (model.NewsFeeds != null)
          {
            foreach (var newsFeedModel in model.NewsFeeds)
            {
              if (newsFeedModel.Delete)
              {
                newsFeedAccessor.RemoveByKey(newsFeedModel.Id);
              }
              else
              {
                var existingNewsFeed = newsFeedAccessor.GetEntity(newsFeedModel.Id);
                if (existingNewsFeed != null)
                {
                  existingNewsFeed
                    .InjectFrom(newsFeedModel)
                    .InjectFrom<NullableToNormal>(newsFeedModel);
                  newsFeedAccessor.SaveChanges();
                }
                else
                {
                  var newsFeed = new NewsFeed();
                  newsFeed
                    .InjectFrom(newsFeedModel)
                    .InjectFrom<NullableToNormal>(newsFeedModel);
                  newsFeedAccessor.Add(newsFeed);
                }
              }
            }
          }

          return RedirectToAction("Index");
        }
      }
      catch
      {
      }

      ModelState.AddModelError("", "Es ist ein Fehler aufgetretten");
      return View(model);
    }
  }
}
