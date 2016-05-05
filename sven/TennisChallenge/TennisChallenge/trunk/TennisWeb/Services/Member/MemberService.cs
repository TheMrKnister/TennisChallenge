using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TennisChallenge.Dal;
using TennisWeb.Models;

namespace TennisWeb.Services.Member
{
  public class MemberService
  {
    public static void ImportMembersIntoDatabase(IEnumerable<MemberModel> memberModels)
    {
      var memberAccesor = new MemberAccessor();
      using (var context = new TennisChallengeEntities())
      {
        foreach (var memberModel in memberModels)
        {
          //Implement check to see if user already exists else create new user
          var userForMember = new TennisChallenge.Dal.User()
          {
            ApplicationId = Guid.NewGuid(),
            IsAnonymous = Convert.ToBoolean(bool.TrueString),
            UserId = Guid.NewGuid(),
            UserName = memberModel.Firstname + " " + memberModel.Lastname,
            LastActivityDate = DateTime.UtcNow,            
          };

          context.AddToUsers(userForMember);

          context.AddToMembers(new TennisChallenge.Dal.Member()
          {
            MemberKey = Guid.NewGuid(),
            Lastname = memberModel.Lastname,
            Firstname = memberModel.Firstname,
            Birthday = memberModel.Birthday,
            Zip = memberModel.Zip,
            City = memberModel.City,
            BusinessPhone = memberModel.BusinessPhone,
            LicenseNumber = memberModel.LicenseNumber,
            MobilePhone = memberModel.MobilePhone,
            PictureUrl = memberModel.PictureUrl,
            PrivatePhone = memberModel.PrivatePhone,
            Street = memberModel.Street,
            TitleFk = memberModel.TitleFk,
            User = userForMember,
            Active = true,
            Classification = null,
            ClassificationUpdate = null,
            Country = null,
            CountryFk = null,
            Title = null,

          });
        }
        context.SaveChanges();
      }

    }

    public static Dictionary<string, int> CreateMappingBasedOnColumnHeader(string[] columnNames)
    {
      var mapping = new Dictionary<string, int>();

      foreach (var columnName in columnNames)
      {
        switch (columnName)
        {
          case "Id":
            mapping.Add("PersonalNumber", Array.IndexOf(columnNames, columnName));
            break;
          case "Vorname":
            mapping.Add("FirstName", Array.IndexOf(columnNames, columnName));
            break;
          case "Nachname":
            mapping.Add("LastName", Array.IndexOf(columnNames, columnName));
            break;
          case "Anrede":
            mapping.Add("Title", Array.IndexOf(columnNames, columnName));
            break;
          case "E-Mail":
            mapping.Add("EMail", Array.IndexOf(columnNames, columnName));
            break;
          case "Benutzername":
            mapping.Add("UserName", Array.IndexOf(columnNames, columnName));
            break;
          case "Passwort":
            mapping.Add("Password", Array.IndexOf(columnNames, columnName));
            break;
          case "Geburtsdatum":
            mapping.Add("Bithday", Array.IndexOf(columnNames, columnName));
            break;
          case "Handy":
            mapping.Add("MobilePhone", Array.IndexOf(columnNames, columnName));
            break;
          case "Strasse":
            mapping.Add("Street", Array.IndexOf(columnNames, columnName));
            break;
          case "PLZ":
            mapping.Add("Zip", Array.IndexOf(columnNames, columnName));
            break;
          case "Ort":
            mapping.Add("City", Array.IndexOf(columnNames, columnName));
            break;
          case "Tel P":
            mapping.Add("PrivatePhone", Array.IndexOf(columnNames, columnName));
            break;
          case "Tel G":
            mapping.Add("BusinessPhone", Array.IndexOf(columnNames, columnName));
            break;
        }
      }

      return mapping;
    }
  }
}