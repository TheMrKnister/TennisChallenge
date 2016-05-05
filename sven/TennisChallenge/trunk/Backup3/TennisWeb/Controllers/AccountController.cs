using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using TennisWeb.Models;
using TennisChallenge.Dal;
using Omu.ValueInjecter;
using TennisWeb.Properties;
using TennisWeb.Utils;

namespace TennisWeb.Controllers
{
  public class AccountController : Controller
  {

    //
    // GET: /Account/LogOn

    public ActionResult LogOn()
    {
      if (User.Identity.IsAuthenticated)
        return RedirectToAction("Index", "Member");
      return View();
    }

    //
    // POST: /Account/LogOn

    [HttpPost]
    public ActionResult LogOn(LogOnModel model, string returnUrl)
    {
      if (ModelState.IsValid)
      {
        model.UserName = model.UserName.ToLower();
        if (Membership.ValidateUser(model.UserName, model.Password))
        {
          FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
          if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
              && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
          {
            return Redirect(returnUrl);
          }
          else
          {
            return RedirectToAction("Index", "Member");
          }
        }
        else
        {
          ModelState.AddModelError("", Resources.AccountController_LogOn_The_user_name_or_password_provided_is_incorrect_);
        }
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    [HttpPost]
    public ActionResult InfoBoardLogOn(LogOnModel model)
    {
      var clubId = new Guid(Request.Cookies["clubId"].Value);
      if (ModelState.IsValid)
      {
        var club = new AccessorBase<Club>().GetFirstOrDefaultWhere(c => c.UsersInClubs.Any(uic => uic.User.UserName == model.UserName) && c.ClubKey == clubId);
        if (Membership.ValidateUser(model.UserName, model.Password) && club != null)
        {
          FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

          return RedirectToAction("Index", "InfoBoard");
        }
      }

      return Content("Fehler");
    }

    //
    // GET: /Account/LogOff

    public ActionResult LogOff()
    {
      FormsAuthentication.SignOut();

      return RedirectToAction("Index", "Home");
    }

    //
    // GET: /Account/Register

    public ActionResult Register()
    {
      var model = new RegisterModel();
      model.FillTitles();
      model.FillAllClubs();

      return View(model);
    }

    //
    // POST: /Account/Register

    [HttpPost]
    public ActionResult Register(RegisterModel model, float crop_x = 0, float crop_y = 0, float crop_w = 0, float crop_h = 0)
    {
      if (ModelState.IsValid)
      {
        if (!String.IsNullOrWhiteSpace(model.PictureUrl))
        {
          try
          {
            Picture.ProcessPicture(Server.MapPath(String.Concat(Picture.UploadImagePath, model.PictureUrl)), crop_x, crop_y, crop_w, crop_h);
          }
          catch (FileNotFoundException)
          {
            model.PictureUrl = "";
          }
        }
        var member = new Member();
        member.InjectFrom(model);
        var memberAccessor = new MemberAccessor();
        var createStatus = memberAccessor.CreateMember(model.Email, model.Password, member);

        if (createStatus == MembershipCreateStatus.Success)
        {
          try
          {
            foreach (var clubFk in model.ClubFks)
            {
              member.User.UsersInClubs.Add(UsersInClub.CreateUsersInClub(Guid.NewGuid(), member.User.UserId, clubFk));
            }

            memberAccessor.SaveChanges();

            var url = Request.Url.GetLeftPart(UriPartial.Authority) +
                              Url.Action("Verify", new { id = member.MemberKey });

            new MailHelper().SendConfirmation(model, url);

            return RedirectToAction("Confirmation");
          }
          catch (Exception)
          {
            ModelState.AddModelError("", "Die Bestätigungs E-Mail konnte nicht versandt werden.");
          }
        }
        else
        {
          ModelState.AddModelError("", ErrorCodeToString(createStatus));
        }
      }

      // If we got this far, something failed, redisplay form
      model.FillTitles();
      model.FillAllClubs();

      return View(model);
    }

    public ActionResult Confirmation()
    {
      return View();
    }

    public ActionResult Recovery()
    {
      return View(new PasswordResetModel());
    }    

    [HttpPost]
    public ActionResult Recovery(PasswordResetModel passwordResetModel)
    {
      if (ModelState.IsValid)
      {
        try
        {
          var membershipUser = Membership.GetUser(passwordResetModel.EMailAddress);
          var user =
            new MemberAccessor().GetFirstOrDefaultWhere<Member>(
              u => u.MemberKey == (Guid)membershipUser.ProviderUserKey);

          if ((membershipUser != null) && (user != null))
          {
            var newPassword = membershipUser.ResetPassword();

            try
            {
              var mailHelper = new MailHelper();
              var userModel = new MemberModel();

              userModel.InjectFrom<FlatLoopValueInjection>(user);
              mailHelper.SendNewPassword(passwordResetModel.EMailAddress, userModel, newPassword);

              return RedirectToAction("RecoverySuccess");
            }
            catch (Exception)
            {
              ModelState.AddModelError("", "Fehler beim Versand der E-Mail.");
            }
          }
        }
        catch (Exception)
        {
          ModelState.AddModelError("", "Konnte das Passwort nicht zurücksetzen.");
        }
      }

      return View(passwordResetModel);
    }

    public ActionResult RecoverySuccess()
    {
      return View();
    }

    public ActionResult Verify(Guid id)
    {
      var membershipUser = Membership.GetUser(id);
      var status = VerificationStatus.Invalid;

      if (membershipUser != null)
      {
        if (!membershipUser.IsApproved)
        {
          membershipUser.IsApproved = true;
          Membership.UpdateUser(membershipUser);

          try
          {
            var memberAccessor = new MemberAccessor();
            var member = memberAccessor.GetEntity(membershipUser.ProviderUserKey);
            member.Active = true;
            memberAccessor.SaveChanges();

            status = VerificationStatus.Success;
          }
          catch (Exception)
          {
            membershipUser.IsApproved = false;
            Membership.UpdateUser(membershipUser);
          }

          FormsAuthentication.SetAuthCookie(membershipUser.UserName, false);
        }
        else
        {
          status = VerificationStatus.AlreadyConfirmed;
        }
      }

      return View(status);
    }

    public enum VerificationStatus
    {
      Success,
      AlreadyConfirmed,
      Invalid
    }

    //
    // GET: /Account/ChangePassword

    [MyAuthorize]
    public ActionResult ChangePassword()
    {
      return View();
    }

    //
    // POST: /Account/ChangePassword

    [MyAuthorize]
    [HttpPost]
    public ActionResult ChangePassword(ChangePasswordModel model)
    {
      if (ModelState.IsValid)
      {

        // ChangePassword will throw an exception rather
        // than return false in certain failure scenarios.
        bool changePasswordSucceeded;
        try
        {
          MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
          changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
        }
        catch (Exception)
        {
          changePasswordSucceeded = false;
        }

        if (changePasswordSucceeded)
        {
          return RedirectToAction("ChangePasswordSuccess");
        }
        else
        {
          ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
        }
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    //
    // GET: /Account/ChangePasswordSuccess

    public ActionResult ChangePasswordSuccess()
    {
      return View();
    }

    #region Status Codes
    private static string ErrorCodeToString(MembershipCreateStatus createStatus)
    {
      // See http://go.microsoft.com/fwlink/?LinkID=177550 for
      // a full list of status codes.
      switch (createStatus)
      {
        case MembershipCreateStatus.DuplicateUserName:
        return "Es wurde bereits ein Konto auf diese E-Mail-Adresse erstellt. Bitte verwenden Sie eine andere E-Mail-Adresse.";

        case MembershipCreateStatus.DuplicateEmail:
        return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

        case MembershipCreateStatus.InvalidPassword:
        return "Das angegebene Passwort ist ungültig. Bitte geben Sie ein gültiges Passwort ein.";

        case MembershipCreateStatus.InvalidEmail:
        return "The e-mail address provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidAnswer:
        return "The password retrieval answer provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidQuestion:
        return "The password retrieval question provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidUserName:
        return "The user name provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.ProviderError:
        return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        case MembershipCreateStatus.UserRejected:
        return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        default:
        return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
      }
    }
    #endregion
  }
}
