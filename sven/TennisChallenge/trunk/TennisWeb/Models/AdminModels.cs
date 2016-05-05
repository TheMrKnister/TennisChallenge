using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using TennisChallenge.Dal;
using TennisWeb.Properties;
using TennisWeb.Controllers;

using CompareAttribute = System.Web.Mvc.CompareAttribute;

namespace TennisWeb.Models
{
  public class UserListModel
  {
    public UserListModel()
      : this(new List<Club>()) { }

    public UserListModel(IEnumerable<Club> clubs)
    {
      Clubs = clubs;
    }

    [UIHint("Clubs")]
    public IEnumerable<Club> Clubs { get; set; }
  }

  public class EditUserInClubModel
  {
    public EditUserInClubModel()
    {
      AllRoles = new List<Role>();
      Roles = new List<Guid>();
    }

    public Guid UsersInClubsKey { get; set; }
    public IEnumerable<Role> AllRoles { get; set; }
    public IEnumerable<Guid> Roles { get; set; }
  }

  public class AdminDeleteViewModel
  {
    public string Name { get; set; }
    public Guid UserKey { get; set; }
    public Guid ClubKey { get; set; }
  }

  public class CreateUserModel : MemberModel
  {
    [Display(Name = "Benutzername")]
    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    public string UserName { get; set; }

    [Display(Name = "Passwort")]
    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    [StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MinLengthPassword", MinimumLength = TennisWeb.Utils.Parameters.MinPasswordLength)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Passwort wiederholen")]
    [Compare("Password", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PasswordsDoNotMatch")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
  }

  public class FullcalendarEvent
  {
    public FullcalendarEvent()
    {
      Title = "";
      ClassName = "";
    }
        
    public Guid Id { get; set; }
    [Display(Name = "Kommentar")]
    public string Title { get; set; }
    [Display(Name = "Turnierkommentar")]
    public string TournComment { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    [Display(Name = "Plätze")]
    public Guid CourtFk { get; set; }
    public EBookingType BookingTypeEnum { get; set; }
    public string ClassName { get; set; }
    public bool Editable { get; set; }
    [Display(Name = "Dauer")]
    public TimeSpan Duration { get; set; }
    //IPA Relevat Begin
    [Display(Name = "Turniere")]
    public Guid? TournamentId { get; set; }
    [Display(Name = "Modus")]
    public TourModeScheme Mode { get; set; }
    [Display(Name = "Typ")]
    public TourTypeScheme Type { get; set; }
    public List<Guid> TournamentCourts { get; set; }
  }

  public enum TourModeScheme
  {
    [Description("Einzel")]
    Single = 0,
    [Description("Doppel")]
    Double = 1
  }

  public enum TourTypeScheme
  {
    [Description("Plausch")]
    Fun = 0,
    [Description("Klassisch")]
    Classic = 1,
    [Description("Clubmeisterschaft")]
    Interclub = 2
  }

  //IPA Relevant End

  public class CourtSimple
  {
    public CourtSimple()
    {
      Name = "";
    }

    public Guid CourtKey { get; set; }
    public string Name { get; set; }
  }

  public class TournSimple
  {
    public TournSimple()
    {
      TournName = "";
    }

    public Guid TournamentId { get; set; }
    public string TournName { get; set; }
  }

  public class MemberSimple
  {
    public MemberSimple()
    {
      FullName = "";
    }

    public Guid Id { get; set; }
    public string FullName { get; set; }
    public int Rfid { get; set; }
    public short? Gender { get; set; }
  }

  public class RankedMemberSimple
  {
    public RankedMemberSimple()
    {
      FullName = "";
    }

    public Guid Id { get; set; }
    public string FullName { get; set; }
    public int? Rank { get; set; }
    public string Email { get; set; }
    public string TelNr { get; set; }
    public bool HasGame { get; set; }
  }

  public class RankedMemberModel
  {
    public Guid RankedMemberKey { get; set; }
    public Guid RankedMemberFk { get; set; }
    public int ClubRank { get; set; }
    public int FormerClubRank { get; set; }
    public int SwissTennisRank { get; set; }
  }

  public class FullcalendarEventEditModel : FullcalendarEvent
  {
    public FullcalendarEventEditModel()
      : base()
    {
      AllCourts = new List<CourtSimple>();
      AllTournaments = new List<TournSimple>();
    }
    public IEnumerable<CourtSimple> AllCourts { get; set; }
    public IEnumerable<TournSimple> AllTournaments { get; set; }
  }

  public class BookingSeriesModel : IValidatableObject
  {
    public BookingSeriesModel()
    {
      Title = "";
    }

    [Display(Name = "Name")]
    public string Name { get; set; }
    [Display(Name = "Kommentar")]
    public string Title { get; set; }
    [Required]
    [DataType(DataType.Time)]
    [Display(Name = "Anfang der Lektionen")]
    public DateTime? Start { get; set; }
    [Required]
    [DataType(DataType.Time)]
    [Display(Name = "Ende der Lektionen")]
    public DateTime? End { get; set; }
    [Required]
    [Display(Name = "Platz")]
    public Guid? CourtFk { get; set; }
    [Required]
    public EBookingType? BookingTypeEnum { get; set; }
    [Required]
    [Display(Name = "Schema")]
    public SerialBookingSchemes? Scheme { get; set; }
    [Required]
    [Display(Name = "Tag(e)")]
    public IEnumerable<DayOfWeek> DaysOfWeek { get; set; }
    [Display(Name = "Woche des Monats")]
    [Range(1, 4)]
    public int? WeekOfMonth { get; set; }
    [Required]
    [Display(Name = "Anfang der Serie (nur die Woche, bzw der Monat zählt)")]
    [DataType(DataType.Date)]
    public DateTime? SeriesStart { get; set; }
    [Required]
    [Display(Name = "Ende der Serie (nur die Woche, bzw der Monat zählt)")]
    [DataType(DataType.Date)]
    public DateTime? SeriesEnd { get; set; }

    public IEnumerable<CourtSimple> AllCourts { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      if (Scheme == SerialBookingSchemes.Monthly && !WeekOfMonth.HasValue)
      {
        yield return new ValidationResult("WeekOfMonth is empty", new[] { "WeekOfMonth" });
      }
    }
  }

  public enum SerialBookingSchemes
  {
    [Description("Wöchentlich")]
    Weekly = 0,
    [Description("Monatlich")]
    Monthly = 1
  }

  public class AdvertisementModel
  {
    public Guid? Id { get; set; }
    [Required]
    [Display(Name = "Name")]
    public string Name { get; set; }
    [DataType(DataType.ImageUrl)]
    public string ImageUrl { get; set; }
    [Display(Name = "Bild")]
    public HttpPostedFileBase Image { get; set; }
    [Required]
    [Display(Name = "Von")]
    [DataType(DataType.Time)]
    public DateTime? StartTime { get; set; }
    [Required]
    [Display(Name = "Bis")]
    [DataType(DataType.Time)]
    public DateTime? EndTime { get; set; }
    [Display(Name = "Montag")]
    public bool Monday { get; set; }
    [Display(Name = "Dienstag")]
    public bool Tuesday { get; set; }
    [Display(Name = "Mittwoch")]
    public bool Wednesday { get; set; }
    [Display(Name = "Donnerstag")]
    public bool Thursday { get; set; }
    [Display(Name = "Freitag")]
    public bool Friday { get; set; }
    [Display(Name = "Samstag")]
    public bool Saturday { get; set; }
    [Display(Name = "Sonntag")]
    public bool Sunday { get; set; }
    [Display(Name = "Aktiv")]
    public bool Active { get; set; }
    [Required]
    [Range(1, Int32.MaxValue)]
    [Display(Name = "Anzeigedauer in sekunden")]
    [DataType(DataType.Duration)]
    public int? Duration { get; set; }
    [Required]
    public Guid? ClubKey { get; set; }
    public bool Delete { get; set; }
  }

  public class NewsFeedModel
  {
    [Required]
    public Guid Id { get; set; }
    [Required]
    [Display(Name = "Name")]
    public string Name { get; set; }
    [Required]
    [DataType(DataType.Url)]
    public string Url { get; set; }
    public bool Active { get; set; }
    [Required]
    public Guid? ClubKey { get; set; }
    [Required]
    [Range(1, Int32.MaxValue)]
    [Display(Name = "Anzeigedauer in sekunden")]
    [DataType(DataType.Duration)]
    public int? Duration { get; set; }
    public bool Delete { get; set; }
  }

  public class NewsFeedsModel
  {
    [Required]
    public Guid? ClubKey { get; set; }
    public IEnumerable<NewsFeedModel> NewsFeeds { get; set; }
    [Required]
    [Display(Name = "Anzahl Werbungen pro News Segment")]
    public int? AdvertisementsPerNewsFeed { get; set; }
  }

  public class ResetPasswordModel
  {
    [Required]
    [StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MinLengthPassword", MinimumLength = TennisWeb.Utils.Parameters.MinPasswordLength)]
    [DataType(DataType.Password)]
    [Display(Name = "Neues Passwort")]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Neues Passwort bestätigen")]
    [Compare("Password", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PasswordsDoNotMatch")]
    public string ConfirmPassword { get; set; }
  }

  public class BookingSeriesSimple
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime Day
    {
      get
      {
        return AdminController.GetFirstDateOfBooking(Id);
      }
    }
    public TimeSpan StartTime
    {
      get
      {
        return AdminController.GetStartTime(Id);
      }
    }
    public String CourtName
    {
      get
      {
        return AdminController.GetBookingSeriesCourt(Id);
      }
    }
  }
}