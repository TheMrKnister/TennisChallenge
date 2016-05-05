using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TennisChallenge.Dal;
using TennisWeb.Models;
using TennisWeb.Utils;
using TennisWeb.Controllers;


namespace TennisWeb.Controllers
{
  public class MemberController : Controller
  {
    //
    // GET: /Member/
    [MyAuthorize]
    public ActionResult Index()
    {
      if (User.IsInRoles(RoleNames.InfoBoard))
        return RedirectToAction("Index", "InfoBoard");

      var member = new MemberAccessor().GetByUserName(User.Identity.Name,
                                                      m => m.User.UsersInClubs, m => m.Title);

      var memberModel = new MemberModel();
      memberModel.InjectFrom<FlatLoopValueInjection>(member);

      return View(memberModel);
    }

    [MyAuthorize]
    public ActionResult Delete()
    {
      return View();
    }

    [MyAuthorize]
    [HttpPost]
    public ActionResult Delete(FormCollection collection)
    {
      try
      {
        // Achtung später, wenn der Benutzer Reservationen und Geldkonto hat, kann das Konto vermutlich nicht mehr einfach gelöscht werden.
        Membership.DeleteUser(User.Identity.Name);

        return RedirectToAction("LogOff", "Account");
      }
      catch
      {
        return View();
      }
    }

    [MyAuthorize]
    [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
    public JsonResult MemberList(string filter)
    {
      var clubId = Guid.Parse(Request.Cookies["clubId"].Value);
      var members = new MemberAccessor().GetMembersClub(clubId, filter, m => m.User.UsersInClubs);
      var memberModels = new List<BookingMemberModel>();
      memberModels.InjectFrom(members);

      return Json(memberModels, JsonRequestBehavior.AllowGet);
    }

    //returns the other players in the ranking a player can challenge
    public JsonResult GetChalPlayers(string name, int gender)
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

      //the index us used to determine in which row the number is. ex. rank=5 it goes through the loop three times.
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
        nMemberSimple.FullName = player.UsersInClub.User.Member.FullName;
        nMemberSimple.Rank = player.ClubRank;
        nMemberSimple.Email = player.UsersInClub.User.UserName;
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

    //Used to Login a member, return all players said member can challenge
    public JsonResult VerifyMemberRanked(string rfid, string username, string password)
    {
      if ((rfid == null && username == null && password == null) ||
          (rfid != null && (username != null || password != null)))
        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);

      try
      {
        var clubId = Guid.Parse(Request.Cookies["clubId"].Value);

        var member = (rfid == null)
                         ? new MemberAccessor().InfoboardVerifyUser(username, password, clubId)
                         : new MemberAccessor().InfoboardVerifyUser(rfid);

        if (member != null)
        {
          FormsAuthentication.SetAuthCookie(member.User.UserName, false);
          var genderString = member.TitleFk.ToString();
          var gender = int.Parse(genderString);

          var memberModel = GetChalPlayers(member.User.UserName, gender);

          return Json(new { status = "valid", memberModel = memberModel }, JsonRequestBehavior.AllowGet);
        }
        return Json(new { status = "unassigned" }, JsonRequestBehavior.AllowGet);
      }
      catch (Exception)
      {
        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
      }
    }

    //IPA Relevant Beginn
    //Verifies the User and looks if he has the Rights for editing Tournaments
    public JsonResult VerifyAdmin(string rfid, string username, string password)
    {
      //Taken from verfiyUser Method Start
      if ((rfid == null && username == null && password == null) ||
          (rfid != null && (username != null || password != null)))
        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);      
      try
      {
        var clubId = Guid.Parse(Request.Cookies["clubId"].Value);

        var member = (rfid == null)
                         ? new MemberAccessor().InfoboardVerifyUser(username, password, clubId)
                         : new MemberAccessor().InfoboardVerifyUser(rfid);

        //Taken from verfiyUser Method End
        if (member != null)
        {
          var TournRole = new AccessorBase<Role>().GetFirstOrDefaultWhere(r => r.RoleName == "casualtournamentorganizer");
          var userInClub = member.UserInClubs.FirstOrDefault(uic => uic.ClubFK == clubId);
          var isAdmin = false;

          //checks if the selected user has the right Rights by trying to match the role names of the user with the set name in TournRole
          if (userInClub.Roles != null)
          {
            foreach (var role in userInClub.Roles)
            {
              if (role.RoleName == TournRole.RoleName)
              {
                isAdmin = true;
                break;
              }
            }
          }
          
          //returns the correct status
          if (isAdmin)
          {
            return Json(new { status = "valid" }, JsonRequestBehavior.AllowGet);
          }
          else
          {
            return Json(new { status = "notAllowed" }, JsonRequestBehavior.AllowGet);
          }
        }

        //if the member tried to verify per rif but has no card connected to his account
        if (rfid != null)
        {
          return Json(new { status = "unassigned" }, JsonRequestBehavior.AllowGet);
        }

        //Taken from verfiyUser Method Start
        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
      }
      catch
      {
        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
      }
    }
    //Taken from verfiyUser Method End

    //Deletes the TournamentId from the membe where username (eg. oliver.bouvard@hotmail.com) matches, or where the rfid matches
    public JsonResult DeleteFromTournament(string username, string rfid, string tournId, string guid)
    {
      var memberAcc = new AccessorBase<Member>();
      var rfidAcc = new AccessorBase<Rfid>();
      var tournMAcc = new AccessorBase<TournamentMember>();
      var member = new Member();
      var tournGuid = new Guid(tournId);

      //is needed when login was done per username, password
      if (username != "" && username != null)
      {
        member = memberAcc.GetFirstOrDefaultWhere(m => m.User.UserName == username);        
      }
        //is needed when login was done via rfid
      else if (rfid != "")
      {
        member = rfidAcc.GetFirstOrDefaultWhere(rf => rf.Id == rfid).Member;
      }
      else
      {
        var memberGuid = new Guid(guid);
        member = memberAcc.GetFirstOrDefaultWhere(m => m.MemberKey == memberGuid);
      }
      
      tournMAcc.RemoveAllWhere(tm => tm.MemberFk == member.MemberKey && tm.TournamentFk == tournGuid);
      member.TournamentId = null;
      memberAcc.SaveChanges();
      rfidAcc.SaveChanges();
      tournMAcc.SaveChanges();
      DeleteSoloTeam();
      
      return Json(new { status = "valid" }, JsonRequestBehavior.AllowGet);
    }

    //return the TournamentMemberKey of the person which has no team partner
    public Guid? GetSoloTeam()
    {
      var allMembers = new AccessorBase<TournamentMember>().GetAllWhere(t => t.TeamId != null);
      var uniqueIds = allMembers.Select(t => t.TeamId).Distinct();     

      foreach (var id in uniqueIds)
      {
        var count = allMembers.Where(i => i.TeamId == id).Count();
        if (count < 2)
        {
          return id;
        }
      }

      return new Guid?();
    }

    //deletes the Member without a team partner
    public void DeleteSoloTeam()
    {
      var uniqueId = GetSoloTeam();
      var teamAcc = new AccessorBase<TournamentMember>();

      teamAcc.RemoveAllWhere(t => t.TeamId == uniqueId);
      teamAcc.SaveChanges();
    }

    //verifies a Member and adds him to the tournament
    public JsonResult VerifyAndAddTournament(string rfid, string username, string password, string tourId)
    {
      var tourGuid = new Guid(tourId);

      //Taken from verfiyUser Method Start
      if ((rfid == null && username == null && password == null) ||
    (rfid != null && (username != null || password != null)))
        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);

      try
      {
        var clubId = Guid.Parse(Request.Cookies["clubId"].Value);

        var member = (rfid == null)
                         ? new MemberAccessor().InfoboardVerifyUser(username, password, clubId)
                         : new MemberAccessor().InfoboardVerifyUser(rfid);
        //Taken from verrfiyUser Method End

        if (member != null)
        {          
          var memberAcc = new AccessorBase<Member>();
          var tournMAcc = new AccessorBase<TournamentMember>();   
          var tournament = new AccessorBase<Tournament>().GetFirstOrDefaultWhere(t => t.TournamentId == tourGuid);
          var editMember = memberAcc.GetFirstOrDefaultWhere(m => m.MemberKey == member.MemberKey);
          var alreadyExistMember = tournMAcc.GetFirstOrDefaultWhere(m => m.MemberFk == member.MemberKey && m.TournamentFk == tourGuid);

          if (alreadyExistMember == null && tournament.Closed != true)
          {        
            //the member exists two times because it would not register that he already has a tournamentId
            var newMember = new TournamentMember();
            newMember.TMemberId = Guid.NewGuid();
            newMember.TournamentFk = tourGuid;
            newMember.MemberFk = editMember.MemberKey;

            if (tournament.TournamentType == 2 && tournament.Mode == 1)
            {
              var uniqueId = GetSoloTeam();

              if (uniqueId == Guid.Empty ||uniqueId == null)
              {
                newMember.TeamId = Guid.NewGuid();
                tournMAcc.Add(newMember);
                return Json(new { status = "team" }, JsonRequestBehavior.AllowGet);
              }
              else
              {
                newMember.TeamId = uniqueId;
                tournMAcc.Add(newMember);
                return Json(new { status = "added" }, JsonRequestBehavior.AllowGet);
              }              
            }
            else
            {
              tournMAcc.Add(newMember);
              return Json(new { status = "added" }, JsonRequestBehavior.AllowGet);
            }
          }
          else if (tournament.Closed == true)
          {
            return Json(new { status = "tournClosed" }, JsonRequestBehavior.AllowGet);
          }
          //Member already has a is in Tournament asociated with him
          else
          {
            return Json(new { status = "already" }, JsonRequestBehavior.AllowGet);
          }       
        
        }
        //Taken from verfiyUser Method Start
        if (rfid != null)
        {
          return Json(new { status = "unassigned" }, JsonRequestBehavior.AllowGet);
        }

        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
      }
      catch
      {
        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
      }
    }
    //Taken from verfiyUser Method End

    //IPA Relevant End

    [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
    public JsonResult VerifyMember(string rfid, string username, string password)
    {
      if ((rfid == null && username == null && password == null) ||
          (rfid != null && (username != null || password != null)))
        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);

      try
      {
        var clubId = Guid.Parse(Request.Cookies["clubId"].Value);

        var member = (rfid == null)
                         ? new MemberAccessor().InfoboardVerifyUser(username, password, clubId)
                         : new MemberAccessor().InfoboardVerifyUser(rfid);

        if (member != null)
        {
          FormsAuthentication.SetAuthCookie(member.User.UserName, false);

          var bookingMember = CreateBookingMemberModel(clubId, member);

          if (!member.HasOpenBooking)
            return Json(new { status = "valid", member = bookingMember }, JsonRequestBehavior.AllowGet);

          var bookingInfo = new
                              {
                                member.NextOpenBooking.BookingKey,
                                member.NextOpenBooking.StartTime,
                                member.NextOpenBooking.EndTime,
                                CourtName = member.NextOpenBooking.Court.Name
                              };
          return Json(new { status = "valid", member = bookingMember, bookingInfo }, JsonRequestBehavior.AllowGet);
        }

        // Falls die Rfid noch keinem Mitglied zugeordnet ist, oder Rfid noch gar nicht bekannt ist, wird der unassigned status vergeben. 
        // Die wird benötigt um später die Id einem Mitglied zuzuordnen.
        if (rfid != null)
        {
          return Json(new { status = "unassigned" }, JsonRequestBehavior.AllowGet);
        }

        return Json(new { status = "invalid" }, JsonRequestBehavior.AllowGet);
      }
      catch (Exception)
      {
        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
      }
    }

    private static BookingMemberModel CreateBookingMemberModel(Guid clubId, Member member)
    {
      var bookingMember = new BookingMemberModel();
      bookingMember.InjectFrom(member);

      var userInClub = member.UserInClubs.FirstOrDefault(uic => uic.ClubFK == clubId);
      if (userInClub != null)
      {
        bookingMember.Roles = userInClub.Roles.Select(r => r.RoleName).ToArray();
        bookingMember.Club = userInClub.Club.Name;
      }
      return bookingMember;
    }

    [MyAuthorize]

    public JsonResult FindByRfId(string rfid)
    {
      var rfidAccessor = new AccessorBase<Rfid>();
      var rfidEntity = rfidAccessor.GetFirstOrDefaultWhere(r => r.Id == rfid);

      try
      {
        var memAccessor = new AccessorBase<Member>();
        var member = memAccessor.GetFirstOrDefaultWhere(m => m.MemberKey == rfidEntity.MemberFk);

        return Json(new { userId = member.Id }, JsonRequestBehavior.AllowGet);
      }
      catch (Exception e)
      {
        throw new Exception("User has not been found!", e);
      }

    }

    public JsonResult AssignRfid(string rfid, string username, string password)
    {
      if (rfid == null || username == null || password == null)
        return Json(new { status = false });

      try
      {
        var clubId = Guid.Parse(Request.Cookies["clubId"].Value);

        var member = new MemberAccessor().InfoboardVerifyUser(username, password, clubId);

        if (member != null)
        {
          var rfidAccessor = new AccessorBase<Rfid>();
          var rfidEntity = rfidAccessor.GetFirstOrDefaultWhere(r => r.Id == rfid);

          var bookingMember = CreateBookingMemberModel(clubId, member);

          // Rfid steht schon in Datenbank, ist aber mit keinem Mitglied verknüpft und nicht gesperrt.
          if (rfidEntity != null && rfidEntity.MemberFk == null && rfidEntity.Active)
          {
            rfidEntity.MemberFk = member.MemberKey;

          }
          // Normalfall neue Karte wird mit Mitglied verknüpft.
          else if (rfidEntity == null)
          {
            var newRfid = new Rfid { RfidKey = Guid.NewGuid(), Active = true, Id = rfid, MemberFk = member.MemberKey };

            rfidAccessor.Add(newRfid);
          }
          // Karte ist gesperrt oder bereits mit Mitglied verknüpft.
          else
          {
            return Json(new { status = false });
          }

          FormsAuthentication.SetAuthCookie(username, false);

          rfidAccessor.SaveChanges();

          if (!member.HasOpenBooking)
            return Json(new { status = true, member = bookingMember }, JsonRequestBehavior.AllowGet);

          var bookingInfo = new
          {
            member.NextOpenBooking.BookingKey,
            member.NextOpenBooking.StartTime,
            member.NextOpenBooking.EndTime,
            CourtName = member.NextOpenBooking.Court.Name
          };

          return Json(new { status = true, member = bookingMember, bookingInfo }, JsonRequestBehavior.AllowGet);
        }
      }
      catch (Exception)
      { }

      return Json(new { status = false });
    }

    //
    // GET: /Member/Edit
    [MyAuthorize]
    public ActionResult Edit()
    {
      var member = new MemberAccessor().GetByUserName(User.Identity.Name,
                                                      m => m.User.UsersInClubs, m => m.Title);
      var memberModel = new MemberModel();
      memberModel.InjectFrom<FlatLoopValueInjection>(member);
      memberModel.FillTitles();
      memberModel.FillAllClubs();

      return View(memberModel);
    }

    [MyAuthorize]
    [HttpPost]
    public ActionResult Edit(MemberModel memberModel, float crop_x = 0, float crop_y = 0, float crop_w = 0, float crop_h = 0)
    {
      if (ModelState.IsValid)
      {
        if (!String.IsNullOrWhiteSpace(memberModel.PictureUrl))
        {
          try
          {
            Picture.ProcessPicture(
              Server.MapPath(String.Concat(Picture.UploadImagePath, memberModel.PictureUrl)),
              crop_x, crop_y, crop_w, crop_h);
          }
          catch (FileNotFoundException)
          {
            memberModel.PictureUrl = String.Empty;
          }
        }

        var member = new MemberAccessor().GetByUserName(User.Identity.Name,
          m => m.User.UsersInClubs, m => m.Title);

        var userInClubAccessor = new AccessorBase<UsersInClub>();
        var userFk = member.MemberKey;
        var postedClubFks = memberModel.ClubFks ?? new List<Guid>();
        var currentClubFks = member.UserInClubs.Select(uic => uic.ClubFK);
        var newClubFks = postedClubFks.Except(currentClubFks).ToList();
        var delClubFks = currentClubFks.Except(postedClubFks);

        userInClubAccessor.RemoveAllWhere(i => delClubFks.Any(dc => dc == i.ClubFK) && i.UserFK == userFk);
        newClubFks.ForEach(cfk => userInClubAccessor.Add(new UsersInClub { UsersInClubsKey = Guid.NewGuid(), ClubFK = cfk, UserFK = userFk }));
        userInClubAccessor.SaveChanges();

        var memberAccessor = new MemberAccessor();
        member = memberAccessor.GetByUserName(User.Identity.Name,
            m => m.User.UsersInClubs, m => m.Title);

        member.InjectFrom(memberModel);
        memberAccessor.SaveChanges();
        return RedirectToAction("Index", "Member");
      }

      memberModel.FillTitles();
      memberModel.FillAllClubs();

      return View(memberModel);
    }

    [HttpPost]
    public JsonResult EditPicture(HttpPostedFileBase PictureUrlUpload)
    {
      var fileName = Picture.GeneratePictureName(PictureUrlUpload);
      if (!String.IsNullOrWhiteSpace(fileName))
      {
        var pictureUrl = Url.Content(Picture.UploadImagePath + fileName);
        var resultMap = new Dictionary<String, String>();
        resultMap["fileName"] = fileName;
        resultMap["pictureUrl"] = pictureUrl;
        PictureUrlUpload.SaveAs(Server.MapPath(Picture.UploadImagePath + fileName));
        return Json(resultMap, "text/plain"); // the text/plain content type is so that Internet Explorer can understand it too
      }
      Response.StatusCode = (int)HttpStatusCode.BadRequest;
      return Json(String.Empty);
    }
  }
}
