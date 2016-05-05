using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TennisChallenge.Dal;

namespace TennisWeb.Models
{
  public class CourtBookingsModel
  {
    public int CourtId { get; set; }

    public DateTime NextStartTime { get; set; }

    public TimeSpan EndLastGame { get; set; }
    /// <summary>
    /// Name des ersten Spielers der ersten Partie.
    /// </summary>
    public string Game1Player0Name { get; set; }
    /// <summary>
    /// Name des zweiten Spielers der ersten Partie.
    /// </summary>
    public string Game1Player1Name { get; set; }
    /// <summary>
    /// Name des dritten Spielers der ersten Partie. (Nur wenn es ein Doppel ist.)
    /// </summary>
    public string Game1Player2Name { get; set; }
    /// <summary>
    /// Name des vierten Spielers der ersten Partie. (Nur wenn es ein Doppel ist.)
    /// </summary>
    public string Game1Player3Name { get; set; }

    public List<BookingModel> Bookings { get; set; }
  }

  public class BookingModel
  {
    public BookingModel()
    { }

    public BookingModel(EBookingType bookingType, DateTime startTime, DateTime endTime)
    {
      BookingType = bookingType;
      StartTime = startTime;
      EndTime = endTime;
    }

    public String CourtComment { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public EBookingType BookingType { get; set; }

    public int Duration
    {
      get { return (int)EndTime.Subtract(StartTime).TotalMinutes; }
    }
     //IPA Relevant Beginn
    public int? TournamentMode { get; set; }

    public String Player2 { get; set; }

    public String Player3 { get; set; }
    //IPA Relevant End
  }

  public class BookingRequest
  {
    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int CourtId { get; set; }

    public Guid Member0Fk { get; set; }

    public Guid? Member1Fk { get; set; }

    public Guid? Member2Fk { get; set; }

    public Guid? Member3Fk { get; set; }

    public int BookingType { get; set; }
  }

  public class InfoboardViewModel
  {
    public Member Member { get; set; }

    public List<Booking> Bookings { get; set; }

    public List<Court> Courts { get; set; }

    public List<Club> Clubs { get; set; }
  }

  public class BookingInfoModel
  {
    public string CourtComment { get; set; }

    public int TypeOfGame { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public String Player0Name { get; set; }

    public String Player1Name { get; set; }

    public String Player2Name { get; set; }

    public String Player3Name { get; set; }
  }
}