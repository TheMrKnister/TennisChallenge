using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CaptchaLib;
using TennisChallenge.Dal;
using TennisWeb.Models;
using TennisWeb.Utils;

namespace TennisWeb.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }

    [MyAuthorize]
    public ActionResult UserProfile()
    {
      if (User.IsInRoles(RoleNames.InfoBoard))
        return RedirectToAction("Index", "InfoBoard");

      return RedirectToAction("Index", "Member");
    }

    [MyAuthorize]
    public ActionResult Infoboard()
    {
      var currentMember = new MemberAccessor().GetByUserName(User.Identity.Name);
      // check if the club has an infoboard user
      if (!currentMember.User.Clubs.Any(c => c.UsersInClubs.Any(uic => uic.Roles.Any(r => r.RoleName == RoleNames.InfoBoard))))
      {
        return RedirectToAction("NoInfoboard");
      }

      var clubKeys = currentMember.User.Clubs.Select(c => c.ClubKey).ToArray();
      var bookings = new BookingAccessor().GetAllWhere(b =>
        b.Member0 != null &&
        b.Member0.MemberKey == currentMember.MemberKey &&
        b.EndTime > DateTime.Now &&
        clubKeys.Any(ck => ck == b.Court.Club.ClubKey)) // better would be if Bookings would save UsersInClubs
        .ToList();
      var courts = new AccessorBase<Court>().GetAllWhere(c => c.Club.ClubKey == clubKeys.FirstOrDefault()).ToList(); // TODO: make this work with people who have multiple clubs
      var clubs = new AccessorBase<Club>().GetAll().ToList();

      return View(new InfoboardViewModel()
      {
        Member = currentMember,
        Bookings = bookings ?? new List<Booking>(),
        Courts = courts ?? new List<Court>(),
        Clubs = clubs ?? new List<Club>()
      });
    }

    public ActionResult NoInfoboard()
    {
      var currentMember = new MemberAccessor().GetByUserName(User.Identity.Name);
      if (currentMember.User.Clubs.Any(c => c.UsersInClubs.Any(uic => uic.Roles.Any(r => r.RoleName == RoleNames.InfoBoard))))
      {
        return RedirectToAction("Infoboard");
      }

      return View();
    }

    public ActionResult Contact()
    {
      ContactFormModel contactFormModel;

      //if (User.Identity.IsAuthenticated && User.IsInRole("customer"))
      //{
      //  var user = new UserBaseAccessor().GetCurrentUser<User>(User);
      //  contactFormModel = user.ToModel<ContactFormModel>();

      //  contactFormModel.EMailAddress = User.Identity.Name;
      //}
      //else
      //{
      contactFormModel = new ContactFormModel();
      //}

      return View(contactFormModel);
    }

    [HttpPost]
    public ActionResult Contact(ContactFormModel contactFormModel)
    {
      if (ModelState.IsValid)
      {
        try
        {
          var mailHelper = new MailHelper();
          mailHelper.SendContactForm(contactFormModel);

          ViewData["Success"] = true;

          ModelState.Clear();

          contactFormModel = new ContactFormModel();
        }
        catch (Exception)
        {
          ModelState.AddModelError("", "Fehler beim Versenden des Formulars.");
        }
      }

      return View(contactFormModel);
    }

    public ActionResult Faq()
    {
      return View();
    }

    public ActionResult Imprint()
    {
      return View();
    }

    public ActionResult GetCaptcha()
    {
      return this.Captcha();
    }
  }
}
