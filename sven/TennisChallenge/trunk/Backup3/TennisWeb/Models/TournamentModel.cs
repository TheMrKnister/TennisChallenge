//IPA Relevant Beginn
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TennisChallenge.Dal;

namespace TennisWeb.Models
{
  public class TournamentViewModel
  {
    public Guid TournamentId { get; set; }
    public string TournamentType { get; set; }
    public string Mode { get; set; }
    public int Members { get; set; }
    public string Start { get; set; }
    public string Courts { get; set; }
    public string TournComment { get; set; }
    public string Comment { get; set; }
    public bool Closed { get; set; }
    public string LinkUrl { get; set; }
  }

  public class NotSetPlayerModel
  {
    public string FullName { get; set; }
  }

  public class GamesViewModel
  {
    public Guid Member0Fk { get; set; }
    public Guid? Member1Fk { get; set; }
    public Guid? Member2Fk { get; set; }
    public Guid? Member3Fk { get; set; }
    public string Member0FullName { get; set; }
    public string Member1FullName { get; set; }
    public string Member2FullName { get; set; }
    public string Member3FullName { get; set; }
    public string Court { get; set; }
  }

  public class LadderModel
  {
    public Guid LadderId { get; set; }
    public string List { get; set; }
  }

  public class TournamentMemberModel
  {
    public Guid MemberKey { get; set; }
    public string FullName { get; set; }
    public Int16? TitleFk { get; set; }
    public bool isSet { get; set; }
    public Guid? TeamId { get; set; }
  }

  public class TournBookingSimple
  {
    public TournBookingSimple()
    {
      Name = "";     
    }

    public Guid CourtKey { get; set; }
    public string Name { get; set; }
    public Guid Member0Fk{ get; set; }
    public Guid? Member1Fk { get; set; }
    public Guid? Member2Fk { get; set; }
    public Guid? Member3Fk { get; set; }
  }
}
//IPA Relevant End