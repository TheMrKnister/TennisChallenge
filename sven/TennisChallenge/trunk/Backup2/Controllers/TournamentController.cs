using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using TennisChallenge.Dal;
using TennisWeb.Models;
using TennisWeb.Utils;
using System.Text.RegularExpressions;
using TennisWeb.Controllers;

namespace TennisWeb.Controllers
{
  public class TournamentController : Controller
  {
    //
    // GET: /Default1/

    public ActionResult Index()
    {
      return View();
    }

    public ActionResult ChallengePlayer()
    {
      return View();
    }

    public ActionResult EnterResult()
    {
      return View();
    }

    public ActionResult TournamentStart()
    {
      return View();
    }

    public ActionResult Rules()
    {
      return View();
    }

    public ActionResult ChallengeAdmin()
    {
      return View();
    }

    public ActionResult RankedGames()
    {
      return View();
    }

    public ActionResult RankedAdmin()
    {
      return View();
    }

    public ActionResult ViewRankedGames()
    {
      return View();
    }

    //IPA RELEVANT BEGIN

    public ActionResult TournamentBeginn()
    {
      return View();
    }

    public ActionResult TournamentManagment()
    {
      return View();
    }

    public ActionResult TournamentOld()
    {
      return View();
    }

    public ActionResult TournamentNew()
    {
      return View();
    }

    //Creates Tournaments with an Id, a mode, and a type mode: 0 = einzel, mode: 1 = doppel, type: 0 = plausch, type: 1 = klassisch
    public static void CreateTounament(Guid tournamentId, int? mode, int? type, string tournComment)
    {
      var tournAcc = new AccessorBase<Tournament>();
      var ladderAcc = new AccessorBase<TournamentLadder>();

      var tournament = new Tournament();
      var ladder = new TournamentLadder();

      tournament.TournamentId = tournamentId;
      tournament.Mode = mode;
      tournament.TournamentType = type;
      tournament.TournComment = tournComment;

      ladder.LadderId = tournamentId;

      tournAcc.Add(tournament);
      ladderAcc.Add(ladder);

      return;
    }

    //deletes Tournament where TournamentId = tourId
    public ActionResult DeleteTournament(string tourId)
    {
      var tourGuid = new Guid(tourId);
      var tournAcc = new AccessorBase<Tournament>();
      var tournament = tournAcc.GetFirstOrDefaultWhere(t => t.TournamentId == tourGuid);

      tournAcc.Remove(tournament);

      return RedirectToAction("TournamentBeginn");
    }

    //Gets all the Courts, where the TornamentId is tournament
    public string GetTournamentCourts(Tournament tournament)
    {
      List<string> courts = new List<string>();
      var courtString = "";

      var nBookings = new AccessorBase<Booking>().GetAllWhere(b => b.TournamentId == tournament.TournamentId);

      foreach (var booking in nBookings)
      {
        if (!courts.Contains(booking.Court.Name))
        {
          courts.Add((booking.Court.Id + 1).ToString()); ;
        }
      }

      foreach (var court in courts)
      {
        courtString = courtString + court + ", ";
      }

      return courtString;
    }

    //return the next Tournaments of each type.
    public JsonResult GetNextTournaments()
    {
      var tournAcc = new AccessorBase<Tournament>();
      var funTournaments = tournAcc.GetAllWhere(t => t.TournamentType == 0 && t.TournamentOpen == null
                                                && ((t.BookingBases.FirstOrDefault().EndTime.Year == DateTime.Now.Year
                                                && t.BookingBases.FirstOrDefault().EndTime.Month == DateTime.Now.Month
                                                && t.BookingBases.FirstOrDefault().EndTime.Day == DateTime.Now.Day)
                                                || t.BookingBases.FirstOrDefault().EndTime > DateTime.Now));
      var siriusTournaments = tournAcc.GetAllWhere(t => t.TournamentType == 1 && t.TournamentOpen == null
                                                && ((t.BookingBases.FirstOrDefault().EndTime.Year == DateTime.Now.Year
                                                && t.BookingBases.FirstOrDefault().EndTime.Month == DateTime.Now.Month
                                                && t.BookingBases.FirstOrDefault().EndTime.Day == DateTime.Now.Day)
                                                || t.BookingBases.FirstOrDefault().EndTime > DateTime.Now));
      var tournamenFunModel = new TournamentViewModel();
      var tournamenSiriustModel = new TournamentViewModel();
      List<TournamentViewModel> tournamentList = new List<TournamentViewModel>();
      List<Tournament> funTournamentList = new List<Tournament>();
      List<Tournament> siriusTournamentList = new List<Tournament>();

      funTournamentList = funTournaments.OrderBy(t => t.BookingBases.FirstOrDefault().StartTime.Date).ToList();
      siriusTournamentList = siriusTournaments.OrderBy(t => t.BookingBases.FirstOrDefault().StartTime.Date).ToList();

      if (funTournamentList.Count != 0)
      {
        var firstFun = funTournamentList.First();

        var mode = firstFun.Mode == 0 ? "Einzel" : "Doppel";
        var type = firstFun.TournamentType == 0 ? "Plausch" : "Klassisch";

        tournamenFunModel.InjectFrom(firstFun);
        tournamenFunModel.TournamentType = type;
        tournamenFunModel.Mode = mode;
        tournamenFunModel.Start = firstFun.BookingBases.FirstOrDefault().StartTime.Date.ToString();
        tournamenFunModel.Members = firstFun.Members.Count;
        tournamenFunModel.Courts = GetTournamentCourts(firstFun);
        tournamentList.Add(tournamenFunModel);
      }

      if (siriusTournamentList.Count != 0)
      {
        var firstSirius = siriusTournamentList.First();

        var mode = firstSirius.Mode == 0 ? "Einzel" : "Doppel";
        var type = firstSirius.TournamentType == 0 ? "Plausch" : "Klassisch";

        tournamenSiriustModel.InjectFrom(firstSirius);
        tournamenSiriustModel.TournamentType = type;
        tournamenSiriustModel.Mode = mode;
        tournamenSiriustModel.Start = firstSirius.BookingBases.FirstOrDefault().StartTime.Date.ToString();
        tournamenSiriustModel.Members = firstSirius.Members.Count;
        tournamenSiriustModel.Courts = GetTournamentCourts(firstSirius);
        tournamentList.Add(tournamenSiriustModel);
      }

      return Json(tournamentList, JsonRequestBehavior.AllowGet);
    }    

    public JsonResult GetClubtournInfos(string tournId)
    {
      var tourGuid = new Guid(tournId);
      var tourn = new AccessorBase<Tournament>().GetFirstOrDefaultWhere(t => t.TournamentId == tourGuid);
      var tournModel = new TournamentViewModel();

      tournModel.InjectFrom(tourn);

      return Json(tournModel, JsonRequestBehavior.AllowGet);
    }

    public void SetInterclubStatus(string tournId)
    {
      var tournAcc = new AccessorBase<Tournament>();
      var tourGuid = new Guid(tournId);
      var tourn = tournAcc.GetFirstOrDefaultWhere(t => t.TournamentId == tourGuid);

      tourn.Closed = !tourn.Closed;

      tournAcc.SaveChanges();
    }

    public void SetTournComment(string tourId, string comment)
    {
      var tournAcc = new AccessorBase<Tournament>();
      var tourGuid = new Guid(tourId);
      var tourn = tournAcc.GetFirstOrDefaultWhere(t => t.TournamentId == tourGuid);

      tourn.TournComment = comment;

      tournAcc.SaveChanges();
    }

    public void SetUrlLink(string tournId, string link)
    {
      var tournAcc = new AccessorBase<Tournament>();
      var tourGuid = new Guid(tournId);
      var tourn = tournAcc.GetFirstOrDefaultWhere(t => t.TournamentId == tourGuid);

      tourn.LinkUrl = link;

      tournAcc.SaveChanges();
    }

    //all = false: Get only the First one.
    public JsonResult GetInterclubTournament(bool all = false)
    {
      var interclubTourn = new TournamentViewModel();
      List<Tournament> breakTourn = new List<Tournament>();
      var tourn = new AccessorBase<Tournament>().GetAllWhere(t => t.TournamentType == 2 && t.TournamentOpen == null);

      foreach (var inTourn in tourn)
      {
       var bases =  inTourn.BookingBases.OrderByDescending(b => b.EndTime.Date);
       if ((bases.First().EndTime.Year == DateTime.Now.Year
         && bases.First().EndTime.Month == DateTime.Now.Month
         && bases.First().EndTime.Day == DateTime.Now.Day)
         || bases.First().EndTime > DateTime.Now)
       {
         breakTourn.Add(inTourn);
       }
      }

      List<Tournament> tournList = new List<Tournament>();
      tournList = breakTourn.OrderBy(t => t.BookingBases.FirstOrDefault().StartTime.Date).ToList();

      if (!all)
      {
        if (tournList.Count != 0)
        {
          interclubTourn.InjectFrom(tournList[0]);
          interclubTourn.TournamentType = tournList[0].TournamentType.ToString();
          interclubTourn.Mode = tournList[0].Mode.ToString();
          interclubTourn.Start = tournList[0].BookingBases.FirstOrDefault().StartTime.Date.ToString();
          interclubTourn.Members = tournList[0].Members.Count;
          interclubTourn.Courts = GetTournamentCourts(tournList[0]);
          interclubTourn.TournComment = tournList[0].TournComment;
          interclubTourn.Comment = tournList[0].BookingBases.FirstOrDefault().Comment;
          interclubTourn.LinkUrl = tournList[0].LinkUrl;
        }

        return Json(interclubTourn, JsonRequestBehavior.AllowGet);
      }
      else
      {
        List<TournamentViewModel> viewList = new List<TournamentViewModel>();

        foreach (var tour in tournList)
        {
          var nTournView = new TournamentViewModel();
          nTournView.InjectFrom(tour);
          nTournView.TournamentType = tour.TournamentType.ToString();
          nTournView.Mode = tour.Mode.ToString();
          nTournView.Start = tour.BookingBases.FirstOrDefault().StartTime.ToString();
          nTournView.Members = tour.Members.Count;
          nTournView.Courts = GetTournamentCourts(tour);
          nTournView.TournComment = tour.TournComment;
          nTournView.Comment = tour.BookingBases.FirstOrDefault().Comment;
          nTournView.LinkUrl = tour.LinkUrl;
          viewList.Add(nTournView);
        }

        return Json(viewList, JsonRequestBehavior.AllowGet);
      }
    }

    //returns all Tournaments which were ended by a Admin
    public JsonResult GetOldTournaments()
    {
      List<TournamentViewModel> tournamentModel = new List<TournamentViewModel>();
      var tournaments = new AccessorBase<Tournament>().GetAllWhere(t => t.TournamentOpen != null);

      foreach (var tournament in tournaments)
      {
        var mode = tournament.Mode == 0 ? "Einzel" : "Doppel";
        var type = tournament.TournamentType == 0 ? "Plausch" : "Klassisch";

        var nTournament = new TournamentViewModel();

        nTournament.InjectFrom(tournament);
        nTournament.TournamentType = type;
        nTournament.Mode = mode;
        nTournament.Start = tournament.BookingBases.FirstOrDefault().StartTime.Date.ToString();
        nTournament.Members = tournament.Members.Count;
        nTournament.Courts = GetTournamentCourts(tournament);
        tournamentModel.Add(nTournament);
      }

      return Json(tournamentModel, JsonRequestBehavior.AllowGet);
    }

    //gets all Tournaments which are still open
    public JsonResult GetAllTournaments()
    {
      List<TournamentViewModel> tournamentModel = new List<TournamentViewModel>();
      var tournaments = new AccessorBase<Tournament>().GetAllWhere(t => t.TournamentOpen == null && (t.TournamentType == 0 || t.TournamentType ==  1));

      foreach (var tournament in tournaments)
      {
        var mode = tournament.Mode == 0 ? "Einzel" : "Doppel";
        var type = tournament.TournamentType == 0 ? "Plausch" : "Klassisch";

        var nTournament = new TournamentViewModel();

        nTournament.InjectFrom(tournament);
        nTournament.TournamentType = type;
        nTournament.Mode = mode;
        nTournament.Start = tournament.BookingBases.FirstOrDefault().StartTime.Date.ToString();
        nTournament.Members = tournament.Members.Count;
        nTournament.Courts = GetTournamentCourts(tournament);
        tournamentModel.Add(nTournament);
      }

      return Json(tournamentModel, JsonRequestBehavior.AllowGet);
    }

    //returns all future Tournaments which weren't ended by a admin
    public JsonResult GetNewTournaments()
    {
      List<TournamentViewModel> tournamentModel = new List<TournamentViewModel>();
      var tournaments = new AccessorBase<Tournament>().GetAllWhere(t => t.BookingBases.FirstOrDefault().StartTime > DateTime.Now && t.TournamentOpen == null && (t.TournamentType == 0 || t.TournamentType == 1));

      foreach (var tournament in tournaments)
      {
        var mode = tournament.Mode == 0 ? "Einzel" : "Doppel";
        var type = tournament.TournamentType == 0 ? "Plausch" : "Klassisch";

        var nTournament = new TournamentViewModel();

        nTournament.InjectFrom(tournament);
        nTournament.TournamentType = type;
        nTournament.Mode = mode;
        nTournament.Start = tournament.BookingBases.FirstOrDefault().StartTime.Date.ToString();
        nTournament.Members = tournament.Members.Count;
        nTournament.Courts = GetTournamentCourts(tournament);
        tournamentModel.Add(nTournament);
      }

      return Json(tournamentModel, JsonRequestBehavior.AllowGet);
    }

    //Creates the ladder, which is save as a singel string in the db
    public void CreateLadder(Guid tourId)
    {
      var ladderAcc = new AccessorBase<TournamentLadder>();
      var memberString = "";
      var indexHelper = 1;
      var members = new AccessorBase<Member>().GetAllWhere(m => m.TournamentId == tourId && m.TournamentPoints != null).OrderByDescending(m => m.TournamentPoints);
      var ladder = ladderAcc.GetFirstOrDefaultWhere(l => l.LadderId == tourId);

      foreach (var member in members)
      {
        memberString += indexHelper + ". " + member.FullName + ";";
        indexHelper++;
      }

      ladder.List = memberString;
      ladderAcc.SaveChanges();

      return;
    }

    //Returns the ladder
    public JsonResult GetLadder(string tourId)
    {
      var ladderGiud = new Guid(tourId);
      var ladder = new AccessorBase<TournamentLadder>().GetFirstOrDefaultWhere(l => l.LadderId == ladderGiud);
      var ladderModel = new LadderModel();

      ladderModel.InjectFrom(ladder);

      return Json(ladderModel, JsonRequestBehavior.AllowGet);
    }

    //Ends the Tournament, deletes the TournamentId of all relevant Members and calls function which generates the ladder
    public JsonResult EndTournament(string tourId)
    {
      var tourGuid = new Guid(tourId);

      var memberAcc = new MemberAccessor();
      var tourAcc = new AccessorBase<Tournament>();
      var bookingAcc = new AccessorBase<Booking>();
      var members = memberAcc.GetAllWhere(m => m.TournamentId == tourGuid);
      var tournament = tourAcc.GetFirstOrDefaultWhere(t => t.TournamentId == tourGuid);
      var bookings = bookingAcc.GetAllWhere(b => b.TournamentId == tourGuid);

      CreateLadder(tourGuid);

      //releases the member from the Tournament
      foreach (var member in members)
      {
        member.TournamentId = null;
        member.TournamentPoints = null;
      }

      //in the next Infoboard tick the booking will be gone
      if (bookings.First().EndTime > DateTime.Now)
      {
        foreach (var booking in bookings)
        {
          booking.EndTime = DateTime.Now.AddSeconds(1);
        }
      }

      //Sets the Open Status to closed
      tournament.TournamentOpen = 1;

      memberAcc.SaveChanges();
      tourAcc.SaveChanges();
      bookingAcc.SaveChanges();

      return Json(new { status = "succes" });
    }

    //This is used to select which method for winner selection should be called based on the TournamentType
    public void WinnerDist(string[] memberIds)
    {
      //indexHelper is used because foreach holds no index on it's own
      var indexHelper = 1;
      foreach (var member in memberIds)
      {
        var consolidate = false;

        if (indexHelper == memberIds.Length)
        {
          consolidate = true;
        }

        var memberGuid = new Guid(member);
        var tournamentType = new AccessorBase<Member>().GetFirstOrDefaultWhere(m => m.MemberKey == memberGuid).Tournament.TournamentType;

        if (tournamentType == 0)
        {
          SelectWinner(member);
        }
        else
        {
          SelectTournWinner(member, consolidate);
        }
        indexHelper++;
      }

      return;
    }

    //returns all the Bookings, which are asociated with the Tournament in tourId
    public JsonResult GetGames(string tourId)
    {
      var tourGuid = new Guid(tourId);
      var tournament = new AccessorBase<Tournament>().GetFirstOrDefaultWhere(t => t.TournamentId == tourGuid);
      List<GamesViewModel> gamesList = new List<GamesViewModel>();

      foreach (var booking in tournament.BookingBases)
      {
        var nGame = new GamesViewModel();
        nGame.InjectFrom(booking);
        nGame.Court = booking.Court.Name;
        nGame.Member0FullName = booking.Member0.FullName;
        if (tournament.Mode == 1)
        {
          //only add to booking if the game is actually played, a double booking where there is no second team will not be played
          if (nGame.Member2Fk != null)
          {
            nGame.Member1FullName = booking.Member1.FullName;
            nGame.Member2FullName = booking.Member2.FullName;
            nGame.Member3FullName = booking.Member3.FullName;
            gamesList.Add(nGame);
          }
        }
        else
        {
          //only add to booking if the game is actually played, a single booking where there is no second player will ot be played
          if (nGame.Member1Fk != null)
          {
            nGame.Member1FullName = booking.Member1.FullName;
            gamesList.Add(nGame);
          }
        }
      }

      return Json(gamesList, JsonRequestBehavior.AllowGet);
    }

    //Slects the winner for a plauschturnier, meaning this will only add points to the TournamentPoints field of the respective player
    public JsonResult SelectWinner(string memberId)
    {
      var memberGuid = new Guid(memberId);
      var memberAcc = new AccessorBase<Member>();
      var bookingAcc = new AccessorBase<Booking>();

      var member = memberAcc.GetFirstOrDefaultWhere(m => m.MemberKey == memberGuid);
      var booking = bookingAcc.GetFirstOrDefaultWhere(b => (b.Member0Fk == memberGuid
                                                            || b.Member1Fk == memberGuid
                                                            || b.Member2Fk == memberGuid
                                                            || b.Member3Fk == memberGuid)
                                                            && b.EndTime > DateTime.Now && b.TournamentId == member.TournamentId);
      var tournament = new AccessorBase<Tournament>().GetFirstOrDefaultWhere(t => t.TournamentId == member.TournamentId);

      //Setting the Tournaments Points to zero, so that adding can take place
      if (member.TournamentPoints == null)
      {
        member.TournamentPoints = 0;
      }

      member.TournamentPoints++;

      //TODO: must be reworked with switch and a function to set the points
      //If game is double then gives points to the members team partner
      if (tournament.Mode == 1)
      {
        if (booking.Member0Fk == memberGuid)
        {
          if (booking.Member1.TournamentPoints == null)
          {
            booking.Member1.TournamentPoints = 0;
          }
          booking.Member1.TournamentPoints++;
        }
        if (booking.Member1Fk == memberGuid)
        {
          if (booking.Member0.TournamentPoints == null)
          {
            booking.Member0.TournamentPoints = 0;
          }
          booking.Member0.TournamentPoints++;
        }
        if (booking.Member2Fk == memberGuid)
        {
          if (booking.Member3.TournamentPoints == null)
          {
            booking.Member3.TournamentPoints = 0;
          }
          booking.Member3.TournamentPoints++;
        }
        if (booking.Member3Fk == memberGuid)
        {
          if (booking.Member2.TournamentPoints == null)
          {
            booking.Member2.TournamentPoints = 0;
          }
          booking.Member2.TournamentPoints++;
        }
      }

      memberAcc.SaveChanges();
      bookingAcc.SaveChanges();

      return Json(new { status = "succes" });
    }

    //Gets all Players in the specified Tournament
    public JsonResult GetTournamentPlayers(string tournId)
    {
      var tournGiud = new Guid(tournId);
      var tourMembers = new AccessorBase<TournamentMember>().GetAllWhere(tm => tm.TournamentFk == tournGiud);
      List<TournamentMemberModel> membersList = new List<TournamentMemberModel>();
      List<Member> members = new List<Member>();

      foreach (var member in tourMembers)
      {
        var nMember = new TournamentMemberModel();
        nMember.InjectFrom(member.Member);
        nMember.TeamId = member.TeamId;
        membersList.Add(nMember);
      }

      membersList.OrderBy(m => m.TeamId);

      return Json(membersList, JsonRequestBehavior.AllowGet);
    }

    //returns the courts which are contected to a tournament
    public JsonResult GetAssignCourts(string tournId)
    {
      var tournGuid = new Guid(tournId);
      var bases = new AccessorBase<Booking>().GetAllWhere(b => b.TournamentId == tournGuid);
      List<TournBookingSimple> courts = new List<TournBookingSimple>();
      bases = bases.OrderBy(b => b.Court.Id);

      foreach (var booking in bases)
      {
        var courtSim = new TournBookingSimple();
        courtSim.InjectFrom(booking.Court);
        courtSim.InjectFrom(booking);
        courts.Add(courtSim);
      }

      return Json(courts, JsonRequestBehavior.AllowGet);
    }

    //returns all players who are set in a game in a specified tournament
    public JsonResult GetNotSetPlayers(string tournId)
    {
      var tournGuid = new Guid(tournId);
      var memberAcc = new AccessorBase<Member>();
      var tournamentAcc = new AccessorBase<Tournament>();
      var bookingAcc = new AccessorBase<Booking>();
      List<Member> membersInBooking = new List<Member>();
      List<Member> membersNotInBooking = new List<Member>();
      List<NotSetPlayerModel> notSetMember = new List<NotSetPlayerModel>();

      var members = memberAcc.GetAllWhere(m => m.TournamentId == tournGuid);
      var bookings = bookingAcc.GetAllWhere(b => b.TournamentId == tournGuid);


      foreach (var member in members)
      {
        foreach (var booking in bookings)
        {
          //checks if the member is in the booking
          if (booking.Member0Fk == member.MemberKey || booking.Member1Fk == member.MemberKey || booking.Member2Fk == member.MemberKey || booking.Member3Fk == member.MemberKey)
          {
            //checks if the booking is actually played
            if (booking.Member1Fk != null)
            {
              membersInBooking.Add(member);
            }
          }
        }

        if (!membersInBooking.Contains(member))
        {
          membersNotInBooking.Add(member);
        }
      }

      foreach (var notMember in membersNotInBooking)
      {
        var nModel = new NotSetPlayerModel();
        nModel.FullName = notMember.FullName;
        notSetMember.Add(nModel);
      }

      return Json(notSetMember, JsonRequestBehavior.AllowGet);
    }

    //Creates a new TournamentMember
    public void AddToMemberToTournament(string guid, string tourGuid)
    {
      var memberGuid = new Guid(guid);
      var tournGuid = new Guid(tourGuid);
      var memberAcc = new AccessorBase<Member>();
      var tournMAcc = new AccessorBase<TournamentMember>();
      var tournMemb = new TournamentMember();
      var member = memberAcc.GetFirstOrDefaultWhere(m => m.MemberKey == memberGuid);

      tournMemb.TMemberId = Guid.NewGuid();
      tournMemb.MemberFk = memberGuid;
      tournMemb.TournamentFk = tournGuid;

      member.TournamentId = tournGuid;

      tournMAcc.Add(tournMemb);
      memberAcc.SaveChanges();
    }

    //returns members which are not in the specified tournament
    public JsonResult GetNotInTournament(string tourId)
    {
      var tourGuid = new Guid(tourId);
      var tournMembers = new AccessorBase<TournamentMember>().GetAllWhere(tm => tm.TournamentFk == tourGuid);
      List<Member> tournMemberList = new List<Member>();
      var allMembers = new AccessorBase<Member>().GetAll().ToList();

      foreach (var intourn in tournMembers)
      {
        tournMemberList.Add(intourn.Member);
      }

      allMembers.RemoveAll(x => tournMemberList.Any(y => y.MemberKey == x.MemberKey));

      List<TournamentMemberModel> membersList = new List<TournamentMemberModel>();

      membersList.InjectFrom(allMembers);

      return Json(membersList, JsonRequestBehavior.AllowGet);
    }

    //Is used to find all Bookings of a tournament where there is only one or two players  (depending on type), and merges them together
    //TODO: Must be reworked, and redundant parts added to a seperate function.
    public void ConsolidateBookings(bool mode, Tournament tournament)
    {
      var indexHelper = 0;
      var bookingAcc = new AccessorBase<Booking>();
      List<Booking> bookingList = new List<Booking>();

      if (mode)
      {
        var bookings = bookingAcc.GetAllWhere(b => b.TournamentId == tournament.TournamentId && b.Member1Fk == null);
        int? round = 0;

        //finds the current round
        foreach (var booking in bookings)
        {
          if (booking.TournamentRound == null)
          {
            booking.TournamentRound = 0;
          }

          if (booking.TournamentRound > round)
          {
            round = booking.TournamentRound;
          }
        }

        //only bookings which are in the current round get added
        foreach (var booking in bookings)
        {
          if (booking.TournamentRound == round)
          {
            bookingList.Add(booking);
          }
        }

        foreach (var booking in bookingList)
        {
          //this is needed in case the round is a odd number. floating points get just cut of in int
          if (round == 0)
          {
            if (indexHelper > (bookingList.Count / 2))
            {
              break;
            }
          }
          else
          {
            if (indexHelper >= (bookingList.Count / 2))
            {
              break;
            }
          }

          //if the index Helper is an uneven number the booking gets skiped,
          //because the booking already has been used to set the game from the last booking
          if (indexHelper % 2 != 0)
          {
            indexHelper++;
            continue;
          }

          booking.Member1Fk = bookingList[indexHelper + 1].Member0Fk;
          booking.TournamentRound++;
          indexHelper++;
        }

      }
      else
      {
        var bookings = bookingAcc.GetAllWhere(b => b.TournamentId == tournament.TournamentId && b.Member2Fk == null);
        int? round = 0;

        //works the same as the code above, must be outsourced to seperate function
        foreach (var booking in bookings)
        {
          if (booking.TournamentRound == null)
          {
            booking.TournamentRound = 0;
          }

          if (booking.TournamentRound > round)
          {
            round = booking.TournamentRound;
          }
        }

        foreach (var booking in bookings)
        {
          if (booking.TournamentRound == round)
          {
            bookingList.Add(booking);
          }
        }

        foreach (var booking in bookingList)
        {
          if (round == 0)
          {
            if (indexHelper > (bookingList.Count / 2))
            {
              break;
            }
          }
          else
          {
            if (indexHelper >= (bookingList.Count / 2))
            {
              break;
            }
          }

          if (indexHelper % 2 != 0)
          {
            indexHelper++;
            continue;
          }

          booking.Member2Fk = bookingList[indexHelper + 1].Member0Fk;
          booking.Member3Fk = bookingList[indexHelper + 1].Member1Fk;
          booking.TournamentRound++;
          indexHelper++;
        }

      }
      bookingAcc.SaveChanges();

      return;
    }

    //Is used to select the winner of a game where the Mode is 1
    //TODO: rework with switch, and rethink function logic, this can be solved better
    public ActionResult SelectTournWinner(string memberId, bool consolidate)
    {
      var memberGuid = new Guid(memberId);
      var memberAcc = new AccessorBase<Member>();
      var bookingAcc = new AccessorBase<Booking>();

      var member = memberAcc.GetFirstOrDefaultWhere(m => m.MemberKey == memberGuid);
      var booking = bookingAcc.GetFirstOrDefaultWhere(b => (b.Member0Fk == memberGuid || b.Member1Fk == memberGuid
                                                      || b.Member2Fk == memberGuid
                                                      || b.Member3Fk == memberGuid)
                                                      && b.EndTime > DateTime.Now && b.TournamentId == member.TournamentId);
      var tournament = new AccessorBase<Tournament>().GetFirstOrDefaultWhere(t => t.TournamentId == member.TournamentId);

      if (tournament.Mode == 1)
      {
        //sets the Points from null to 0, so that adding is possible
        if (member.TournamentPoints == null)
        {
          member.TournamentPoints = 0;
        }

        if (booking.Member1.TournamentPoints == null)
        {
          booking.Member1.TournamentPoints = 0;
        }

        if (booking.Member2Fk != null)
        {
          if (booking.Member2.TournamentPoints == null)
          {
            booking.Member2.TournamentPoints = 0;
          }

          if (booking.Member3.TournamentPoints == null)
          {
            booking.Member3.TournamentPoints = 0;
          }
        }

        member.TournamentPoints++;

        //gives points to the correct players and if necessary replaces the first team with the second
        //the loosers get deleted from the booking
        if (booking.Member0Fk == memberGuid)
        {
          booking.Member1.TournamentPoints++;
          booking.Member3Fk = null;
          booking.Member2Fk = null;
        }
        if (booking.Member1Fk == memberGuid)
        {
          booking.Member0.TournamentPoints++;
          booking.Member3Fk = null;
          booking.Member2Fk = null;
        }
        if (booking.Member2Fk == memberGuid)
        {
          booking.Member3.TournamentPoints++;
          booking.Member1Fk = booking.Member3.MemberKey;
          booking.Member0Fk = memberGuid;
          booking.Member3Fk = null;
          booking.Member2Fk = null;
        }
        if (booking.Member3Fk == memberGuid)
        {
          booking.Member2.TournamentPoints++;
          booking.Member0Fk = memberGuid;
          booking.Member1Fk = booking.Member2.MemberKey;
          booking.Member3Fk = null;
          booking.Member2Fk = null;
        }
        bookingAcc.SaveChanges();
        memberAcc.SaveChanges();

        //once all members have been iterated through
        if (consolidate)
        {
          ConsolidateBookings(false, tournament);
        }
      }
      else
      {
        //setting the points so that adding can take place
        if (member.TournamentPoints == null)
        {
          member.TournamentPoints = 0;
        }

        member.TournamentPoints++;
        //deletes the looser of the booking
        if (booking.Member0Fk == memberGuid)
        {
          booking.Member1Fk = null;
        }
        else
        {
          booking.Member1Fk = null;
          booking.Member0Fk = memberGuid;
        }

        bookingAcc.SaveChanges();
        memberAcc.SaveChanges();

        //once all winners have been iterated through
        if (consolidate)
        {
          ConsolidateBookings(true, tournament);
        }
      }

      return RedirectToAction("TournamentStart");
    }

    //takes a list of all Players in a tournament, and randomises it
    public JsonResult RandomizePlayers(string tourId)
    {
      var tourGuid = new Guid(tourId);

      var indexHelper = 0;
      var bookingAcc = new AccessorBase<Booking>();
      var bookings = bookingAcc.GetAllWhere(b => b.TournamentId == tourGuid);
      var members = new AccessorBase<TournamentMember>().GetAllWhere(m => m.TournamentFk == tourGuid);
      var tournament = new AccessorBase<Tournament>().GetFirstOrDefaultWhere(t => t.TournamentId == tourGuid);
      List<Member> memberList = new List<Member>();

      //randomizing only works for lists
      foreach (var member in members)
      {
        memberList.Add(member.Member);
      }

      //NOT MY CODE src:http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp
      var rng = new Random();
      var n = memberList.Count;

      while (n > 1)
      {
        n--;
        int k = rng.Next(n + 1);
        Member value = memberList[k];
        memberList[k] = memberList[n];
        memberList[n] = value;
      }
      //MY CODE AGAIN

      foreach (var booking in bookings)
      {
        //fills the bookings with the members in the list
        if (!(indexHelper >= (memberList.Count - 1)))
        {
          booking.Member0Fk = memberList[indexHelper].MemberKey;
          booking.Member1Fk = memberList[indexHelper + 1].MemberKey;
          if (tournament.Mode == 1)
          {
            booking.Member2Fk = memberList[indexHelper + 2].MemberKey;
            booking.Member3Fk = memberList[indexHelper + 3].MemberKey;
            indexHelper = indexHelper + 4;
          }
          else
          {
            indexHelper = indexHelper + 2;
          }
        }
        booking.TournamentRound = 0;
      }

      bookingAcc.SaveChanges();

      return Json(new { status = "succes" });
    }

    //IPA RELEVANT END

    //public void AssignPlayerLanding(string courtId) {
    //}

    //Assigns players in a tournament to a game
    public void AssignPlayer(string memberId, string courtId, string tournId)
    {
      //all members are one string as a parameter, and get split up into a their respective entities
      string[] members = memberId.Split(',');
      List<Member> toSetMembers = new List<Member>();
      var indexHelper = 0;
      var bookingAcc = new AccessorBase<Booking>();

      foreach (var m in members)
      {
        //checks if a member is actually set
        if (!(String.IsNullOrEmpty(m) || m == "undefined"))
        {
          var memberGuid = new Guid(m);
          var toSetMember = new AccessorBase<Member>().GetFirstOrDefaultWhere(mb => mb.MemberKey == memberGuid);
          toSetMembers.Add(toSetMember);
        }
        else
        {
          toSetMembers.Add(null);
        }
      }

      var courtGuid = new Guid(courtId);
      var tournGiud = new Guid(tournId);

      var toSetBooking = bookingAcc.GetFirstOrDefaultWhere(b => b.CourtFk == courtGuid && b.TournamentId == tournGiud);
      List<Member> courtMember = new List<Member>();

      courtMember.Add(toSetBooking.Member0);
      courtMember.Add(toSetBooking.Member1);
      courtMember.Add(toSetBooking.Member2);
      courtMember.Add(toSetBooking.Member3);

      //sets all the members, in their correct position
      foreach (var member in toSetMembers)
      {
        if (member != null)
        {
          courtMember[indexHelper] = member;
        }
        else
        {
          courtMember[indexHelper] = null;
        }
        indexHelper++;
      }

      if (courtMember[0] != null)
      {
        toSetBooking.Member0Fk = courtMember[0].MemberKey;
      }
      else
      {
        //Sets member to special Member whose name is NULL, so it appears that the booking is complety empty
        toSetBooking.Member0Fk = new Guid("94e13f9a-823d-4217-932d-c57465cbacb4");
      }
      if (courtMember[1] != null)
      {
        toSetBooking.Member1Fk = courtMember[1].MemberKey;
      }
      else
      {
        toSetBooking.Member1Fk = null;
      }
      if (courtMember[2] != null)
      {
        toSetBooking.Member2Fk = courtMember[2].MemberKey;
      }
      else
      {
        toSetBooking.Member2Fk = null;
      }
      if (courtMember[3] != null)
      {
        toSetBooking.Member3Fk = courtMember[3].MemberKey;
      }
      else
      {
        toSetBooking.Member3Fk = null;
      }

      bookingAcc.SaveChanges();
    }

    //Closes the gaps in the rankings
    public bool CascadeRanked(int? newRank, int? formerRank, bool directionUp, short? gender)
    {
      var accessor = new AccessorBase<RankedMember>();
      var ranks = accessor.GetAllWhere(rm => rm.ClubRank >= newRank && rm.ClubRank < formerRank && rm.UsersInClub.User.Member.TitleFk == gender);

      if (formerRank == null)
      {
        ranks = accessor.GetAllWhere(rm => rm.ClubRank > newRank);
      }

      //foreach rank that is between formerRank and newRank move the rank one up or down
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

    //returns all posible players a player can challenge by a string of the full name
    public ActionResult GetChallFromName(string fullname)
    {
      var member = new AccessorBase<Member>().GetFirstOrDefaultWhereFunc(m => m.FullName == fullname);
      return GetChalPlayers(member.User.UserName, member.TitleFk);
    }

    //returns if player has a ranked game or not
    public bool PlayerHasGame(string member)
    {
      var rGames = new AccessorBase<RankedGame>().GetAllWhere(rg => rg.BookingBase.StartTime > DateTime.Now);

      foreach (var game in rGames)
      {
        if (game.BookingBase.Member0.FullName == member || game.BookingBase.Member1.FullName == member)
        {
          return true;
        }
      }
      return false;
    }

    //returns the other players in the ranking a player can challenge, is the same funtion as in the MemberController, copied because of lazyness
    public JsonResult GetChalPlayers(string name, int? gender)
    {
      var accessorRanked = new AccessorBase<RankedMember>();
      var accessorUic = new AccessorBase<UsersInClub>();
      List<RankedMemberSimple> memberModel = new List<RankedMemberSimple>();

      var clubId = accessorUic.GetFirstOrDefaultWhere(uic => uic.User.UserName == name).ClubFK;
      var userFk = accessorUic.GetFirstOrDefaultWhere(uic => uic.User.UserName == name && uic.ClubFK == clubId).UsersInClubsKey;
      var rank = accessorRanked.GetFirstOrDefaultWhere(rm => rm.RankedMemberFk == userFk).ClubRank;

      //This part finds out which player can be challenged.
      //The edge Numbers are the Numbers last in their row on the pyramid. max amount of players is 210
      int[] edgeNumbers = { 0, 1, 3, 6, 10, 15, 21, 28, 36, 45, 55, 66, 78, 91, 105, 120, 136, 153, 171, 190, 210 };
      var row = 0;

      //the index is used to determine in which row the number is. ex. rank=5 it goes through the loop three times.
      //  the fourth time it sets the row as 3 - 1 = 2, means the player on rank five can challenge 2 people down.
      for (int i = 0; i < edgeNumbers.Count(); i++)
      {
        if (rank <= edgeNumbers[i])
        {
          row = i - 1;
          break;
        }
      }

      var players = accessorRanked.GetAllWhere(rm => rm.ClubRank < rank && rm.ClubRank >= rank - row && rm.UsersInClub.User.Member.TitleFk == gender).OrderBy(rm => rm.ClubRank);

      foreach (var player in players)
      {
        var nMemberSimple = new RankedMemberSimple();
        nMemberSimple.Id = player.UsersInClub.User.Member.MemberKey;
        nMemberSimple.FullName = player.UsersInClub.User.Member.FullName;
        nMemberSimple.Rank = player.ClubRank;
        nMemberSimple.Email = player.UsersInClub.User.UserName;
        nMemberSimple.HasGame = PlayerHasGame(nMemberSimple.FullName);
        //makes sure that some phone number is displayed
        if (player.UsersInClub.User.Member.MobilePhone == null)
        {
          if (player.UsersInClub.User.Member.PrivatePhone == null)
          {
            nMemberSimple.TelNr = player.UsersInClub.User.Member.BusinessPhone;
          }
          else
          {
            nMemberSimple.TelNr = player.UsersInClub.User.Member.PrivatePhone;
          }
        }
        else
        {
          nMemberSimple.TelNr = player.UsersInClub.User.Member.MobilePhone;
        }
        memberModel.Add(nMemberSimple);
      }

      return Json(memberModel, JsonRequestBehavior.AllowGet);
    }

    //returns all ranked members
    public ActionResult GetPlayers(string userName, int gender)
    {
      var clubId = new AccessorBase<UsersInClub>().GetFirstOrDefaultWhere(uic => uic.User.UserName == userName).ClubFK;
      var members = new AccessorBase<RankedMember>().GetAllWhere(rm => /*rm.UsersInClub.User.Member.TitleFk == gender &&*/ rm.UsersInClub.ClubFK == clubId).OrderBy(rm => rm.ClubRank);
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

    //returns all courts
    public ActionResult GetCourts()
    {
      var courts = new AccessorBase<Court>().GetAll().OrderBy(c => c.Name);
      List<CourtSimpleRanked> courtModel = new List<CourtSimpleRanked>();

      foreach (var court in courts)
      {
        var nCourt = new CourtSimpleRanked();
        nCourt.CourtKey = court.Id;
        nCourt.Name = court.Name;
        courtModel.Add(nCourt);
      }

      return Json(courtModel, JsonRequestBehavior.AllowGet);
    }

    //returns all ranked games
    public ActionResult GetRankedGames()
    {
      var games = new AccessorBase<RankedGame>().GetAllWhere(g => g.WinnerFk == null);
      List<RankedDisplayModel> rankedGames = new List<RankedDisplayModel>();

      foreach (var game in games)
      {
        var nGame = new RankedDisplayModel();
        nGame.GameId = game.RankedGameKey;
        nGame.Challenger = game.BookingBase.Member0.FullName;
        nGame.Defender = game.BookingBase.Member1.FullName;
        var user = new AccessorBase<UsersInClub>().GetFirstOrDefaultWhere(uic => uic.UserFK == game.BookingBase.Member1.MemberKey).UsersInClubsKey;
        var rank = new AccessorBase<RankedMember>().GetFirstOrDefaultWhere(rm => rm.RankedMemberFk == user).ClubRank;
        nGame.ContestedRank = rank;
        nGame.Date = game.BookingBase.StartTime.Date.Day + "." + game.BookingBase.StartTime.Date.Month;
        nGame.Start = game.BookingBase.StartTime.TimeOfDay.ToString();
        nGame.End = game.BookingBase.EndTime.TimeOfDay.ToString();
        nGame.Court = game.BookingBase.Court.Name;
        rankedGames.Add(nGame);
      }

      return Json(rankedGames, JsonRequestBehavior.AllowGet);
    }

    //returns all ranked games which have been played
    public ActionResult GetPlayedGames()
    {
      var mAcc = new AccessorBase<Member>();
      var games = new AccessorBase<RankedGame>().GetAllWhere(g => g.WinnerFk != null);
      List<FinnishedGame> rankedGames = new List<FinnishedGame>();

      games.OrderBy(g => g.BookingBase.StartTime);

      foreach (var game in games)
      {
        var nGame = new FinnishedGame();
        nGame.Player1 = game.BookingBase.Member0.FullName;
        nGame.Player2 = game.BookingBase.Member1.FullName;
        nGame.Winner = mAcc.GetFirstOrDefaultWhere(m => m.MemberKey == game.WinnerFk).FullName;
        nGame.Player1Score1 = game.Player1ScoreFirst;
        nGame.Player1Score2 = game.Player1ScoreSecond;
        nGame.Player1ScoreTie = game.Player1ScoreTie;
        nGame.Player2Score1 = game.Player2ScoreFirst;
        nGame.Player2Score2 = game.Player2ScoreSecond;
        nGame.Player2ScoreTie = game.Player2ScoreTie;
        nGame.ContestedRank = new AccessorBase<RankedMember>().GetFirstOrDefaultWhere(rm => rm.UsersInClub.UserFK == game.WinnerFk).ClubRank;
        rankedGames.Add(nGame);
      }

      return Json(rankedGames, JsonRequestBehavior.AllowGet);
    }

    //changes the rank of a player specified by an the id
    public ActionResult ChangeRank(Guid id, int? ClubRank)
    {
      var user = new AccessorBase<UsersInClub>().GetFirstOrDefaultWhere(uic => uic.UserFK == id);
      var uicId = user.UsersInClubsKey;
      var accessor = new AccessorBase<RankedMember>();
      var gender = user.User.Member.TitleFk;
      var winner = accessor.GetFirstOrDefaultWhere(rm => rm.RankedMemberFk == uicId);
      int? formerRankWinner = winner.ClubRank;
      var loser = accessor.GetFirstOrDefaultWhere(rm => rm.ClubRank == ClubRank);

      CascadeRanked(ClubRank, formerRankWinner, true, gender);
      winner.ClubRank = ClubRank;
      winner.FormerClubRank = formerRankWinner;
      if (loser != null)
      {
        loser.FormerClubRank = ClubRank;
      }
      else
      {
        CascadeRanked(ClubRank, null, false, gender);
      }
      accessor.SaveChanges();

      return RedirectToAction("EnterResult");
    }

    //converts a string date into a DateTime
    public ActionResult ConvertDate(string start, bool end)
    {
      var startDate = start.Split(';')[0];
      var startTime = start.Substring(start.Length - 5);
      var startDay = Convert.ToInt32(startDate.Substring(0, 2));
      var startMonth = Convert.ToInt32(startDate.Substring(3, 2));
      var startYear = Convert.ToInt32(startDate.Substring(6, 4));
      var startHour = Convert.ToInt32(startTime.Substring(0, 2));
      var startMin = Convert.ToInt32(startTime.Substring(3, 2));

      DateTime startDTime = new DateTime(startYear, startMonth, startDay, startHour, startMin, 0);

      //if the time is the end of a booking 90min, the standart lenght for a ranked game, gets added
      if (end)
      {
        startDTime = startDTime.AddMinutes(90);
      }

      return Json(startDTime, JsonRequestBehavior.AllowGet);
    }

    //creates a new ranked game
    public ActionResult CreateRankedGame(BookingRequest bookingRequest)
    {
      var bookings = new AccessorBase<Booking>().GetAllWhere(b => b.Court.Id == bookingRequest.CourtId);
      foreach (var nBooking in bookings)
      {
        if (Calc.DateRangesIntersect(bookingRequest.StartTime, bookingRequest.EndTime, nBooking.StartTime, nBooking.EndTime))
        {
          return Json(new { success = false });
        }
      }

      var courtKey = new AccessorBase<Court>().GetFirstOrDefaultWhere(c => c.Id == bookingRequest.CourtId).CourtKey;

      var booking = new Booking { BookingKey = Guid.NewGuid(), CourtFk = courtKey, BookingTypeEnum = EBookingType.Match };
      var bookingKey = booking.BookingKey;
      var rankedGame = new RankedGame { RankedGameKey = Guid.NewGuid(), BookingFk = bookingKey };
      var member1 = new AccessorBase<Member>().GetFirstOrDefaultWhere(m => m.MemberKey == bookingRequest.Member0Fk).FullName;
      var member2 = new AccessorBase<Member>().GetFirstOrDefaultWhere(m => m.MemberKey == bookingRequest.Member1Fk).FullName;

      booking.InjectFrom(bookingRequest);
      booking.Comment = "Ranglistenspiel: " + member1 + " vs. " + member2;

      new BookingAccessor().Add(booking);
      new AccessorBase<RankedGame>().Add(rankedGame);

      RedirectToAction("RankedAdmin");
      return Json(new { success = true });
    }

    //removes a ranked game
    public ActionResult DeleteGame(string gameId)
    {
      Guid gameGuid = new Guid(gameId);
      var gameAcc = new AccessorBase<RankedGame>();
      var bookingAcc = new AccessorBase<Booking>();
      var game = gameAcc.GetFirstOrDefaultWhere(rm => rm.RankedGameKey == gameGuid);
      var booking = bookingAcc.GetFirstOrDefaultWhere(b => b.BookingKey == game.BookingFk);

      gameAcc.Remove(game);
      bookingAcc.Remove(booking);

      return RedirectToAction("RankedAdmin");
    }

    public ActionResult EditRanked(RankedGameAdmin rankedGame)
    {
      var gameAccessor = new AccessorBase<RankedGame>();
      var game = gameAccessor.GetFirstOrDefaultWhere(rg => rg.RankedGameKey == rankedGame.GameId);
      var winnerFk = new AccessorBase<Member>().GetFirstOrDefaultWhere(m => m.FullName == rankedGame.Winner).MemberKey;

      game.WinnerFk = winnerFk;
      game.Player1ScoreFirst = rankedGame.Player1Score1;
      game.Player1ScoreSecond = rankedGame.Player1Score2;
      game.Player1ScoreTie = rankedGame.Player1ScoreTie;
      game.Player2ScoreFirst = rankedGame.Player2Score1;
      game.Player2ScoreSecond = rankedGame.Player2Score2;
      game.Player2ScoreTie = rankedGame.Player2ScoreTie;

      gameAccessor.SaveChanges();

      if (game.BookingBase.Member0.MemberKey == winnerFk)
      {
        var memberAccessor = new AccessorBase<RankedMember>();
        var winnerKey = new AccessorBase<Member>().GetFirstOrDefaultWhere(m => m.MemberKey == winnerFk).UserInClubs.First().UsersInClubsKey;
        var looserKey = new AccessorBase<Member>().GetFirstOrDefaultWhere(m => m.MemberKey == game.BookingBase.Member1.MemberKey).UserInClubs.First().UsersInClubsKey;
        var rWinner = memberAccessor.GetFirstOrDefaultWhere(rm => rm.RankedMemberFk == winnerKey).ClubRank;
        var gender = new AccessorBase<Member>().GetFirstOrDefaultWhere(m => m.MemberKey == winnerFk).TitleFk;
        var rLooser = memberAccessor.GetFirstOrDefaultWhere(rm => rm.RankedMemberFk == looserKey).ClubRank;
        ChangeRank(winnerFk, rLooser);
      }

      return RedirectToAction("RankedAdmin");
    }
  }
}
