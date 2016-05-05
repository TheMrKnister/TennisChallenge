using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TennisChallenge.Dal;
using TennisWeb.Models;

namespace TennisWeb.Models
{
  public class RankedDisplayModel
  {
    public Guid GameId { get; set; }
    public String Challenger { get; set; }
    public String Defender { get; set; }
    public String Date { get; set; }
    public String Start { get; set; }
    public String End { get; set; }
    public String Court { get; set; }
    public int? ContestedRank { get; set; }
  }

  public class FinnishedGame
  {
    public String Player1 { get; set; }
    public String Player2 { get; set; }
    public String Winner { get; set; }
    public int? Player1Score1 { get; set; }
    public int? Player1Score2 { get; set; }
    public int? Player1ScoreTie { get; set; }
    public int? Player2Score1 { get; set; }
    public int? Player2Score2 { get; set; }
    public int? Player2ScoreTie { get; set; }
    public int? ContestedRank { get; set; }
  }

  public class RankedGameAdmin
  {
    public Guid GameId { get; set; }
    public String Winner { get; set; }
    public int Player1Score1 { get; set; }
    public int Player2Score1 { get; set; }
    public int Player1Score2 { get; set; }
    public int Player2Score2 { get; set; }
    public int Player1ScoreTie { get; set; }
    public int Player2ScoreTie { get; set; }
  }

  public class CourtSimpleRanked
  {
    public int CourtKey { get; set; }
    public string Name { get; set; }
  }
}