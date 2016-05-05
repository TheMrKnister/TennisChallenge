using System;
using System.Collections.Generic;
using System.Linq;

namespace TennisChallenge.Dal
{
  public class AdvertisementShowingAccessor : AccessorBase<AdvertisementShowing>
  {
    public IEnumerable<AdvertisementShowing> GetNewestAdvertisementShowings(Guid clubId, int numberOfShowingsPerAdvertisement)
    {
      var baseQuery = this.GenerateQueryable<AdvertisementShowing>(a => a.Advertisement);

      var query = baseQuery.Where(a => a.Advertisement.ClubKey == clubId)
                           .GroupBy(a => a.AdvertisementFk)
                           .SelectMany(g => g.OrderByDescending(a => a.Showed).Take(numberOfShowingsPerAdvertisement));

      return query.ToArray();
    }
  }
}
