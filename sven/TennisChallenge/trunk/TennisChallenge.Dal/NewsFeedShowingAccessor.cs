using System;
using System.Collections.Generic;
using System.Linq;

namespace TennisChallenge.Dal
{
  public class NewsFeedShowingAccessor : AccessorBase<NewsFeedShowing>
  {
    public IEnumerable<NewsFeedShowing> GetNewestNewsFeedShowings(Guid clubId)
    {
      var query = this.GenerateQueryable<NewsFeedShowing>(nfs => nfs.NewsFeed);
      return query.Where(nfs => nfs.NewsFeed.ClubKey == clubId)
                  .GroupBy(nfs => nfs.NewsFeedFk)
                  .Select(g => g.OrderByDescending(nfs => nfs.Showed).FirstOrDefault())
                  .ToArray();
    }
  }
}
