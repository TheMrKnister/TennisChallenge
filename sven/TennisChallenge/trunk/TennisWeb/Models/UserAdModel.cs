using System;

namespace TennisWeb.Models
{
  public class UserAdModelSimple
  {
    public Guid AdId { get; set; }
    public string AdText { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string TelNr { get; set; }
  }
}