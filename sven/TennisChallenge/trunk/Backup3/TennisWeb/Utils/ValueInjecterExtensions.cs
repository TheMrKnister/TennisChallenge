using System.Collections.Generic;
using Omu.ValueInjecter;

namespace TennisWeb.Utils
{
  public static class ValueInjecterExtensions
  {
    public static ICollection<TTo> InjectFrom<TFrom, TTo>(this ICollection<TTo> to, IEnumerable<TFrom> from)
      where TTo : new()
    {
      foreach (var source in from)
      {
        var target = new TTo();
        target.InjectFrom(source);
        to.Add(target);
      }
      return to;
    }

  }
}