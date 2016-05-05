using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity.Validation;
using Omu.ValueInjecter;
using TennisChallenge.Dal;
using TennisWeb.Models;
using TennisWeb.Utils;

namespace TennisWeb.Controllers
{
  public class UserAdsController : Controller
  {
    //
    // GET: /UserAds/

    public ActionResult Index()
    {
      return View();
    }

    public ActionResult TennisMarket()
    {
      return View();
    }

    public ActionResult LookingForPlayer()
    {
      return View();
    }

    //filters the ads according to certain criteria
    public ActionResult FilterAds(bool ranked, int ageMin, int ageMax, bool male, bool female, int? ranking)
    {
      var adAccessor = new AccessorBase<UserAd>();
      var rankedMembers = new AccessorBase<RankedMember>().GetAll();
      var ads = adAccessor.GetAllWhere(ad => ad.AdType == 2);
      List<Guid> filteredAds = new List<Guid>();

      foreach (var ad in ads)
      {
        Member member = ad.UsersInClub.User.Member;
        int userAge = (DateTime.Today.Year - member.Birthday.Value.Year);
        bool isMale = member.Title.TitleKey == 2 ? true : false;
        bool isFemale = member.Title.TitleKey == 1 ? true : false;
        var temp = rankedMembers.Where(rm => rm.RankedMemberFk == member.User.UsersInClubs.First().UsersInClubsKey);
        bool isRanked = temp.FirstOrDefault() != null ? true : false;

        //in case both genders are unchecked, the search wont get filtered by gender
        //if (!male && !female)
        //{
        //  male = true;
        //  female = true;
        //}

        if (!ranked)
        {
          isRanked = false;
        }

        if (isMale)
        {
          if ((userAge >= ageMin && userAge <= ageMax) && isMale == male && ranked == isRanked)
          {
            if (filteredAds.IndexOf(ad.UserAdId) == -1)
            {
              filteredAds.Add(ad.UserAdId);
            }
          }
        }

        if (isFemale)
        {
          if ((userAge >= ageMin && userAge <= ageMax) && isFemale == female && ranked == isRanked)
          {
            if (filteredAds.IndexOf(ad.UserAdId) == -1)
            {
              filteredAds.Add(ad.UserAdId);
            }
          }
        }
      }

      return Json(filteredAds, JsonRequestBehavior.AllowGet);
    }

    public Guid GetUicId(string userName)
    {
      var uicId = new AccessorBase<UsersInClub>().GetFirstOrDefaultWhere(uic => uic.User.UserName == userName).UsersInClubsKey;
      return uicId;
    }

    //returns all userAds of a type
    public ActionResult GetUserAds(int type)
    {
      var ads = new AccessorBase<UserAd>().GetAllWhere(ad => ad.AdType == type);
      ads = ads.OrderBy(ad => ad.CreationTime);
      List<UserAdModelSimple> adsModel = new List<UserAdModelSimple>();
      foreach (var ad in ads)
      {
        var adModel = new UserAdModelSimple();
        adModel.AdId = ad.UserAdId;
        adModel.AdText = ad.AdText;
        adModel.UserName = ad.UsersInClub.User.UserName;
        adModel.FullName = ad.UsersInClub.User.Member.FullName;
        //makes sure some phone number is returned
        if (ad.UsersInClub.User.Member.MobilePhone == null)
        {
          if (ad.UsersInClub.User.Member.PrivatePhone == null)
          {
            adModel.TelNr = ad.UsersInClub.User.Member.BusinessPhone;
          }
          else
          {
            adModel.TelNr = ad.UsersInClub.User.Member.PrivatePhone;
          }
        }
        else
        {
          adModel.TelNr = ad.UsersInClub.User.Member.MobilePhone;
        }

        adsModel.Add(adModel);
      }
      return Json(adsModel, JsonRequestBehavior.AllowGet);
    }

    //creates a new userAd, type 1 = tennisbörse, type 2 = spielersuche
    public ActionResult CreateUserAd(string userName, string text, int type)
    {
      var id = GetUicId(userName);
      var model = new UserAd();
      var UserAdAcc = new AccessorBase<UserAd>();

      model.UserAdId = Guid.NewGuid();
      model.AdText = text;
      model.AdType = type;
      model.UserInClubFk = id;
      model.CreationTime = DateTime.Now;

      UserAdAcc.Add(model);

      return RedirectToAction("TennisMarket");
    }

    //removes a userAd, specified by a Id
    public ActionResult DeleteUserAd(Guid id)
    {
      new AccessorBase<UserAd>().RemoveAllWhere(ad => ad.UserAdId == id);
      return RedirectToAction("TennisMarket");
    }
  }
}
