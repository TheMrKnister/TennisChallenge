using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace TennisWeb.Utils
{
  public static class SelectListUtils
  {
    /// <summary>
    /// Generates a select list from the enum given as a type parameter.
    /// Uses <see cref="DescriptionAttribute"/> if available
    /// </summary>
    /// <typeparam name="TEnum">The enum from which a SelectList should be generated.</typeparam>
    /// <returns>A SelectList</returns>
    public static IEnumerable<SelectListItem> EnumSelectList<TEnum>()
      where TEnum : struct, IConvertible
    {
      Debug.Assert(typeof(TEnum).IsEnum, "TEnum has to be an enum");

      foreach (TEnum e in Enum.GetValues(typeof(TEnum)))
      {
        var descriptionAttribute = typeof(TEnum).GetMember(e.ToString()).FirstOrDefault().GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
        yield return new SelectListItem { Text = descriptionAttribute == null ? e.ToString() : descriptionAttribute.Description, Value = e.ToInt32(CultureInfo.InvariantCulture).ToString() };
      }
    }

    public static IEnumerable<SelectListItem> DaysOfWeek()
    {
      return DaysOfWeek(CultureInfo.InvariantCulture);
    }

    public static IEnumerable<SelectListItem> DaysOfWeek(CultureInfo cultureInfo)
    {
      return from DayOfWeek d in Enum.GetValues(typeof(DayOfWeek))
             select new SelectListItem { Text = cultureInfo.DateTimeFormat.DayNames[(int)d], Value = ((int)d).ToString() };
    }
  }
}