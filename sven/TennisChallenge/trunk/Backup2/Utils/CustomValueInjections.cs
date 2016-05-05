using System;
using Omu.ValueInjecter;

namespace TennisWeb.Utils
{
  public class FullcalendarEventToBookingInjection : ConventionInjection
  {
    protected override bool Match(ConventionInfo c)
    {
      return
        c.SourceProp.Value != null &&
        (c.SourceProp.Name == "Id" && c.TargetProp.Name == "BookingKey" ||
        c.SourceProp.Name == "Start" && c.TargetProp.Name == "StartTime" ||
        c.SourceProp.Name == "End" && c.TargetProp.Name == "EndTime" ||
        c.SourceProp.Name == "Title" && c.TargetProp.Name == "Comment");
    }
  }

  public class BookingToFullcalendarEventInjection : ConventionInjection
  {
    protected override bool Match(ConventionInfo c)
    {
      return
        c.SourceProp.Value != null &&
        (c.SourceProp.Name == "BookingKey" && c.TargetProp.Name == "Id" ||
        c.SourceProp.Name == "StartTime" && c.TargetProp.Name == "Start" ||
        c.SourceProp.Name == "EndTime" && c.TargetProp.Name == "End" ||
        c.SourceProp.Name == "Comment" && c.TargetProp.Name == "Title");
    }
  }

  public class NullableToNormal : ConventionInjection
  {
    protected override bool Match(ConventionInfo c)
    {
      return c.SourceProp.Name == c.TargetProp.Name &&
             Nullable.GetUnderlyingType(c.SourceProp.Type) == c.TargetProp.Type;
    }
  }

  public class NormalToNullable : ConventionInjection
  {
    protected override bool Match(ConventionInfo c)
    {
      return c.SourceProp.Name == c.TargetProp.Name &&
             c.SourceProp.Type == Nullable.GetUnderlyingType(c.TargetProp.Type);
    }
  }
}