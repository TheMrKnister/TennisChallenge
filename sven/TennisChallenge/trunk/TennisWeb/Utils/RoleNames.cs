using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TennisWeb.Utils
{
  public static class RoleNames
  {
    public const string InfoBoard = "infoboard";
    public const string ApplicationAdmin = "applicationadmin";
    public const string ClubAdmin = "clubadmin";
    public const string TennisTeacher = "tennisteacher";
    public const string InterClubOrganizer = "intercluborganizer";
    public const string CasualTournamentOrganizer = "casualtournamentorganizer";
    public const string Janitor = "janitor";
    public const string EventManager = "eventmanager";
    public const string Host = "host";
    public const string AdvertisementManager = "advertisementmanager";
    public const string TopicalityManger = "topicalitymanger";

    public static readonly IEnumerable<String> AllRoles = new[] { ClubAdmin, TennisTeacher, InterClubOrganizer, CasualTournamentOrganizer, Janitor, AdvertisementManager, TopicalityManger, EventManager, Host };
  }
}