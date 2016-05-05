namespace TennisChallenge.Dal
{
  public partial class Booking
  {
    public EBookingType BookingTypeEnum
    {
      get { return (EBookingType)BookingType; }
      set { BookingType = (int)value; }
    }

    public string Title { get { return Comment ?? BookingTypeEnum.ToString(); } }

    public string ClassName { get { return BookingTypeEnum.ToString().ToLowerInvariant(); } }
  }
}
