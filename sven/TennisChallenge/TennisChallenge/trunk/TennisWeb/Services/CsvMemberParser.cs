using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using TennisChallenge.Dal;
using TennisWeb.Models;
using Omu.ValueInjecter;
using System.Web.Security;

namespace TennisWeb.Services
{
  public class CsvMemberParser
  {
    const string Format = "dd.MM.yyyy";
    readonly CultureInfo _cultureProvider = CultureInfo.InvariantCulture;

    public void CreateFromCurrentReadLine(string[] line, Dictionary<string, int> mapping, Guid? clubFk)
    {
            var createUserModel = new CreateUserModel()
      {
        Birthday = mapping.ContainsKey("BirthDay") ? DateTime.ParseExact(line[mapping["Birthday"]], Format, _cultureProvider) : (DateTime?)null,
        Zip = mapping.ContainsKey("Zip") ? line[mapping["Zip"]] : null,
        City = mapping.ContainsKey("City") ? line[mapping["City"]] : null,
        Lastname = mapping.ContainsKey("LastName") ? line[mapping["LastName"]] : null,
        Firstname = mapping.ContainsKey("FirstName") ? line[mapping["FirstName"]] : null,
        MobilePhone = mapping.ContainsKey("MobilePhone") ? line[mapping["MobilePhone"]] : null,
        Password = mapping.ContainsKey("Password") ? line[mapping["Password"]] : null,
        UserName = mapping.ContainsKey("UserName") ? line[mapping["UserName"]] : null,
        Street = mapping.ContainsKey("Street") ? line[mapping["Street"]] : null,
        PrivatePhone = mapping.ContainsKey("PrivatePhone") ? line[mapping["PrivatePhone"]] : null,
        BusinessPhone = mapping.ContainsKey("BusinessPhone") ? line[mapping["BusinessPhone"]] : null,
        TitleName = mapping.ContainsKey("Title") ? line[mapping["Title"]] : null,
      };

      var memberAccessor = new MemberAccessor();
      var member = TennisChallenge.Dal.Member.CreateMember(Guid.NewGuid(), true);
      //var userName = mapping.ContainsKey("Email") && !String.IsNullOrEmpty(line[mapping["Email"]]) ? line[mapping["Email"]] : line[mapping["LastName"]] + line[mapping["FirstName"]];

      member.InjectFrom(createUserModel);
      var createStatus = memberAccessor.CreateMember(createUserModel.UserName, createUserModel.Password, member);
      if (createStatus == MembershipCreateStatus.Success)
      {
        var user = Membership.GetUser(createUserModel.UserName);
        user.IsApproved = true;
        Membership.UpdateUser(user);

        //Add this line when clubKeys are given by the upload
        //member.UserInClubs = createUserModel.ClubFks.Select(c => UsersInClub.CreateUsersInClub(Guid.NewGuid(), member.MemberKey, c));
        memberAccessor.SaveChanges();
      }
      //return ImportUserModel;
    }
  }
}