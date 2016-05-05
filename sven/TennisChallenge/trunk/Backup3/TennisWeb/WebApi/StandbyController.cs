using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Omu.ValueInjecter;
using TennisChallenge.Dal;
using TennisWeb.Models;

namespace TennisWeb.WebApi
{
  public class StandbyController : ApiController
  {
    private static bool AdvertisementIsActiveOnDay(Advertisement advertisement, DayOfWeek dayOfWeek)
    {
      switch (dayOfWeek)
      {
        case DayOfWeek.Monday:
          return advertisement.Monday;
        case DayOfWeek.Tuesday:
          return advertisement.Tuesday;
        case DayOfWeek.Wednesday:
          return advertisement.Wednesday;
        case DayOfWeek.Thursday:
          return advertisement.Thursday;
        case DayOfWeek.Friday:
          return advertisement.Friday;
        case DayOfWeek.Saturday:
          return advertisement.Saturday;
        case DayOfWeek.Sunday:
          return advertisement.Sunday;
      }

      return false;
    }

    private static StandbyContentModel GetAdvertisement(IEnumerable<Advertisement> advertisements, IEnumerable<AdvertisementShowing> advertisementShowings)
    {
      var now = DateTime.Now;
      var timeOfDay = now.TimeOfDay;

      var possibleAdvertisements = advertisements.Where(a => a.Active &&
                                                             a.StartTime.TimeOfDay <= timeOfDay &&
                                                             a.EndTime.TimeOfDay >= timeOfDay &&
                                                             !String.IsNullOrWhiteSpace(a.ImageUrl) &&
                                                             AdvertisementIsActiveOnDay(a, now.DayOfWeek));

      var advertisementWithoutShowing = possibleAdvertisements
        .Where(a => !advertisementShowings.Where(ads => ads.AdvertisementFk == a.Id).Any())
        .FirstOrDefault();

      var advertisementWithOldestShowing = possibleAdvertisements
        .Where(a => advertisementShowings.Where(ads => ads.AdvertisementFk == a.Id).Any())
        .OrderBy(a => advertisementShowings.Where(ads => ads.AdvertisementFk == a.Id).Max(s => s.Showed))
        .FirstOrDefault();

      var selectedAdvertisement = advertisementWithoutShowing ?? advertisementWithOldestShowing;

      var model = new StandbyContentModel();
      model.InjectFrom(selectedAdvertisement);
      model.ImageUrl = VirtualPathUtility.ToAbsolute(selectedAdvertisement.ImageUrl);

      //Vermutung: Eintrag in der Datenbank Tabelle AdvertisementShowings (Mesut)
      var showing = AdvertisementShowing.CreateAdvertisementShowing(Guid.NewGuid(), now, selectedAdvertisement.Id);
      new AccessorBase<AdvertisementShowing>().Add(showing);

      return model;
    }

    private static StandbyContentModel GetNewsFeed(IEnumerable<NewsFeed> newsFeeds, IEnumerable<NewsFeedShowing> newsFeedShowings)
    {
      var newsFeedWithoutShowing = newsFeeds
        .Where(a => !newsFeedShowings.Any(nfs => nfs.NewsFeedFk == a.Id))
        .FirstOrDefault();

      var newsFeedWithOldestshowing = newsFeeds
        .Where(a => newsFeedShowings.Any(nfs => nfs.NewsFeedFk == a.Id))
        .OrderBy(a => newsFeedShowings.Where(nfs => nfs.NewsFeedFk == a.Id).Max(nfs => nfs.Showed))
        .FirstOrDefault();

      var selectedNewsFeed = newsFeedWithoutShowing ?? newsFeedWithOldestshowing;

      //Vermutung: Eintrag in der Datenbank Tabelle NewsFeedShowings (Mesut)
      var newsFeedShowing = NewsFeedShowing.CreateNewsFeedShowing(Guid.NewGuid(), DateTime.Now, selectedNewsFeed.Id);
      new AccessorBase<NewsFeedShowing>().Add(newsFeedShowing);

      var model = new StandbyContentModel();
      model.InjectFrom(selectedNewsFeed);

      return model;
    }

    // GET api/standby/id
    public StandbyContentModel Get(Guid id)
    {
      try
      {
        var clubId = id;

        var advertisementsPerNewsFeed = new AccessorBase<Club>().GetEntity(clubId).AdvertisementsPerNewsFeed;

        var advertisements = new AccessorBase<Advertisement>().GetAllWhere(a => a.ClubKey == clubId);
        var advertisementShowing = new AdvertisementShowingAccessor().GetNewestAdvertisementShowings(clubId, advertisementsPerNewsFeed);
        var newsFeeds = new AccessorBase<NewsFeed>().GetAllWhere(n => n.ClubKey == clubId);
        var newsFeedShowings = new NewsFeedShowingAccessor().GetNewestNewsFeedShowings(clubId);

        var showings = advertisementShowing
          .Cast<IShowing>()
          .Union(newsFeedShowings)
          .OrderByDescending(s => s.Showed);

        var anyNewsFeeds = newsFeeds.Any();
        var anyAdvertisements = advertisements.Any();
        var lastXShowings = showings.Take(advertisementsPerNewsFeed);
        var enoughAdvertisementsShowedForNewsFeed = showings.Count() >= advertisementsPerNewsFeed && lastXShowings.All(s => s is AdvertisementShowing);

        if (anyNewsFeeds && (!anyAdvertisements || enoughAdvertisementsShowedForNewsFeed))
        {
          return GetNewsFeed(newsFeeds, newsFeedShowings);
        }

        else if (anyAdvertisements)
        {
          return GetAdvertisement(advertisements, advertisementShowing);
        }
      }
      catch
      {
      }

      return null;
    }
  }
}
