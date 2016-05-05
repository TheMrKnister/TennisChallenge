using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Security;

namespace TennisChallenge.Dal
{
  public class MemberAccessor : AccessorBase<Member>
  {
    public MembershipCreateStatus CreateMember(string username, string password, Member member)
    {
      if (String.IsNullOrEmpty(username))
        throw new ArgumentException("The parameter must not be null or empty.", "username");
      if (member == null)
        throw new ArgumentNullException("member");

      MembershipCreateStatus status;

      var existingUser = Membership.GetUser(username);
      if (existingUser != null && !existingUser.IsApproved)
      {
        Membership.DeleteUser(username, false);
      }

      var membershipUser = Membership.CreateUser(username, password, null, null, null, false, out status);

      if ((membershipUser == null) || (status != MembershipCreateStatus.Success))
        return status;

      try
      {
        member.MemberKey = (Guid)membershipUser.ProviderUserKey;

        Add(member);
      }
      catch (Exception e)
      {
        var message = e.Message;
        Membership.DeleteUser(username);
      }

      return status;
    }

    public Member GetByUserName(string userName, params Expression<Func<Member, object>>[] includes)
    {
      // aspnt_Users.UserName ist eigentlich nicht eindeutig. Wurde aber nicht im Detail geklärt.
      // Müsste evtl. durch ApplicationId ergänzt werden, falls DB für versch. Anwendungen genutzt wird.
      return GetFirstOrDefaultWhere(m => userName.Equals(m.User.UserName, StringComparison.InvariantCultureIgnoreCase), includes);
    }

    /// <summary>
    /// Gets all the members that are in the same club as the Infoboard
    /// </summary>
    /// <param name="infoboardUserName">Username of the Infoboard</param>
    /// <param name="filter">String used to filter the full names of the members to get</param>
    /// <param name="includes"></param>
    /// <returns>An IEnumerable of the filtered Members </returns>
    public IEnumerable<Member> GetMembersClub(Guid clubId, string filter, params Expression<Func<Member, object>>[] includes)
    {
      if (string.IsNullOrEmpty(filter))
        return GetAllWhereFunc(m => m.User.Clubs.Any(c => c.ClubKey == clubId),
          includes);

      var lowerCaseFilter = filter.ToLowerInvariant();

      return GetAllWhereFunc(m => m.FullName.ToLowerInvariant().Contains(lowerCaseFilter) &&
                              m.Active &&
                              m.User.Clubs.Any(c => c.ClubKey == clubId),
                              includes);
    }

    /// <summary>
    /// Prüft, ob die Rfid gültig und einem Benutzer zugeordnet ist, welcher Mitglied des Tennisklubs ist, in welchem 
    /// das angegebene Infoboard steht.
    /// </summary>
    /// <param name="infoboardUserName">Benutzername des Infoboards.</param>
    /// <param name="rfid">Rfid welcher geprüft wird.</param>
    /// <returns>Eine Instanz von Member mit den Daten des Benutzers, falls die Prüfung bestanden ist. <i>Null</i> sonst.</returns>
    public Member InfoboardVerifyUser(string rfid)
    {
      if (rfid == null)
        throw new ArgumentNullException();

      var member = new AccessorBase<Rfid>().GetAllWhere(r => r.Id == rfid && r.Active && r.Member != null && r.Member.Active,
        r => r.Member,
        r => r.Member.User)
        .Select(r => r.Member).FirstOrDefault();

      if (member == null || member.User.Clubs == null || !member.User.Clubs.Any())
        return null;

      return member;
    }

    /// <summary>
    /// verfies the user with a password and a infoboard username
    /// </summary>
    /// <param name="infoboardUserName">Username of the infoboard</param>
    /// <param name="memberUserName">Username of the member</param>
    /// <param name="memberPassword">Password of the member</param>
    /// <returns>An instance of Member of the verfication succeeded else <i>Null</i></returns>
    public Member InfoboardVerifyUser(string memberUserName, string memberPassword, Guid clubId)
    {
      if (memberUserName == null || memberPassword == null)
        throw new ArgumentNullException();

      if (!Membership.ValidateUser(memberUserName, memberPassword))
        return null;

      var member = GetByUserName(memberUserName, m => m.User.UsersInClubs);

      if (member == null || member.User.Clubs == null || !member.User.Clubs.Any())
        return null;

      if (!member.User.ClubFks.Contains(clubId))
        return null;

      return member;
    }
  }
}
