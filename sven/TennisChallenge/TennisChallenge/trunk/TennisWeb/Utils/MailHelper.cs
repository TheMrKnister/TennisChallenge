using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using TennisWeb.Models;

namespace TennisWeb.Utils
{
  public class MailHelper
  {
    private readonly SmtpClient _smtpClient;
    private const string From = "no-reply@tennis-challenge.ch";
    private const string ReplyTo = "hilfe@tennis-challenge.ch";
    private const string Contact = "hilfe@tennis-challenge.ch";

    public MailHelper()
    {
      var host = ConfigurationManager.AppSettings["SmtpServer"];
      var username = ConfigurationManager.AppSettings["SmtpUser"];
      var password = ConfigurationManager.AppSettings["SmtpPassword"];

      _smtpClient = new SmtpClient(host) { Credentials = new NetworkCredential(username, password) };
    }

    public void SendConfirmation(RegisterModel registerModel, string confirmationUrl)
    {
      var mailMessage = new MailMessage(From, registerModel.Username);

      mailMessage.ReplyToList.Add(ReplyTo);
      mailMessage.Subject = "Tennis Challenge - Aktivierungslink";

      var body = new StringBuilder("Hallo ");

      body.Append(registerModel.Firstname).Append(" ").AppendLine(registerModel.Lastname);
      body.AppendLine();
      body.Append("Danke für deine Registrierung. Um unseren Service nutzen zu können, bestätige deine ");
      body.AppendLine("Anmeldung mit folgendem Link.");
      body.AppendLine();
      body.AppendLine(confirmationUrl);
      body.AppendLine();
      body.AppendLine("Deine persönlichen Daten").AppendLine();
      body.Append("E-Mail: ").AppendLine(registerModel.Username);
      body.Append("Passwort: ").AppendLine(registerModel.Password);
      body.AppendLine();
      body.Append("Bitte drucke oder speichere diese E-Mail für den Fall, dass du deine Daten ");
      body.AppendLine("später ändern möchtest.");

      mailMessage.Body = body.ToString();
      mailMessage.IsBodyHtml = false;

      _smtpClient.Send(mailMessage);
    }

    public void SendNewPassword(string eMailAddress, MemberModel user, string newPassword)
    {
      var body = new StringBuilder("Sehr geehrte");
      if (user.TitleName.Equals("Frau", StringComparison.InvariantCultureIgnoreCase))
        body.Append(" ");
      else if (user.TitleName.Equals("Herr", StringComparison.InvariantCultureIgnoreCase))
        body.Append("r ");
      else
        body.Append("(r) ");

      body.Append(user.TitleName).Append(" ");
      body.Append(user.Firstname).Append(" ").AppendLine(user.Lastname);
      body.AppendLine();
      body.Append("Das Passwort für Ihr Konto bei \"Tennis Challenge\" wurde zurückgesetzt. Bitte melden Sie sich mit ");
      body.AppendLine("dem folgenden Passwort an und änderen Sie das Passwort nach Ihren Wünschen.").AppendLine();
      body.Append("Neues Passwort: ").AppendLine(newPassword).AppendLine();
      body.AppendLine("http://www.tennis-challenge.ch");

      var mailMessage = new MailMessage(From, eMailAddress)
      {
        Subject = "Neues Passwort",
        IsBodyHtml = false,
        Body = body.ToString()
      };

      _smtpClient.Send(mailMessage);
    }

    public void SendContactForm(ContactFormModel contactFormModel)
    {
      var body = new StringBuilder(contactFormModel.Message);
      body.AppendLine().AppendLine("----------------------");
      body.AppendLine(contactFormModel.Title);
      body.Append(contactFormModel.Firstname).Append(" ").AppendLine(contactFormModel.Name);
      body.AppendLine(contactFormModel.EMailAddress);
      //body.AppendLine(contactFormModel.PhoneLocal);

      var mailMessage = new MailMessage(contactFormModel.EMailAddress, Contact)
      {
        Subject = contactFormModel.Subject,
        IsBodyHtml = false,
        Body = body.ToString()
      };

      _smtpClient.Send(mailMessage);
    }
  }
}