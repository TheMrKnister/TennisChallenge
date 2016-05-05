using System.ComponentModel.DataAnnotations;

namespace TennisWeb.Utils
{
  public class EmailAttribute : RegularExpressionAttribute
  {
    public EmailAttribute()
      : base("^..*@.*.$") { }
  }
}