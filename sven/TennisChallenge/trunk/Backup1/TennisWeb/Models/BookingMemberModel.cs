using System;

namespace TennisWeb.Models
{
  public class BookingMemberModel
  {
    public Guid Id { get; set; }

    public string FullName { get; set; }

    public string Classification { get; set; }

    public string Club { get; set; }

    public string PictureUrl { get; set; }

    public bool HasOpenBooking { get; set; }

    public string[] Roles { get; set; }
  }
}