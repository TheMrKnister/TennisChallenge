using System;
using TennisChallenge.Dal;

namespace TennisWeb.Models
{
  public class StandbyModel
  {
    public Guid ClubKey { get; set; }
  }

  public class StandbyContentModel
  {
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public string Url { get; set; }
    public int Duration { get; set; }
  }

  public class RankedStartModel
  {
    public int gender { get; set; }
  }

  public class InfoboardCalendarModel
  {
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public String Comment { get; set; }
    public String Court { get; set; }
    public String StartTime { get; set; }
    public int Type { get; set; }
    public String Row { get; set; }
  }
}