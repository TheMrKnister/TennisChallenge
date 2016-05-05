using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TennisWeb.Utils;
using CaptchaLib;
using TennisWeb.Properties;

namespace TennisWeb.Models
{
  public class PasswordResetModel
  {
    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    [Display(Name = "E-Mail-Adresse")]
    [DataType(DataType.EmailAddress)]
    [Email(ErrorMessage = "Ungültige E-Mail-Adresse")]
    public string EMailAddress { get; set; }

    [ValidateCaptcha(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "CaptchaInvalid")]
    public string Captcha { get; set; }
  }
}