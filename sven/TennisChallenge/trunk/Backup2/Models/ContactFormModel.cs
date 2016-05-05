using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TennisChallenge.Dal;
using TennisWeb.Properties;
using TennisWeb.Utils;
using CaptchaLib;

namespace TennisWeb.Models
{
  public class ContactFormModel : Model
  {
    [Display(Name = "Anrede")]
    public string Title { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    [Display(Name = "Vorname")]
    public string Firstname { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    [Display(Name = "Nachname")]
    public string Name { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    [Display(Name = "E-Mail-Adresse")]
    [DataType(DataType.EmailAddress)]
    [Email(ErrorMessage = "Ungültige E-Mail-Adresse")]
    public string EMailAddress { get; set; }

    public string Phone { get; set; }

    //[Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    //[Display(Name = "Telefonnummer")]
    //[RegularExpression(@"^ *0 *(\d *){9}$", ErrorMessage = "Telefonnummer im Format 0xx xxx xx xx eingeben.")]
    //public string PhoneLocal
    //{
    //  get { return MemberModel.PrivatePhone; }
    //  set { Phone = UserModel.PhoneLocalToPhone(value); }
    //}

    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    [Display(Name = "Betreff")]
    public string Subject { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Text")]
    public string Message { get; set; }

    [ValidateCaptcha(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "CaptchaInvalid")]
    public string Captcha { get; set; }
  }
}