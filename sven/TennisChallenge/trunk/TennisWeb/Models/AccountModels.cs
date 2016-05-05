using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using TennisWeb.Properties;
using TennisWeb.Utils;

using CompareAttribute = System.Web.Mvc.CompareAttribute; //neu

namespace TennisWeb.Models
{

  public class ChangePasswordModel
  {
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Aktuelles Passwort")]
    public string OldPassword { get; set; }

    [Required]
    [StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MinLengthPassword", MinimumLength = Parameters.MinPasswordLength)]
    [DataType(DataType.Password)]
    [Display(Name = "Neues Passwort")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Wiederholen Sie das neue Passwort")]
    [Compare("NewPassword", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PasswordsDoNotMatch")]
    public string ConfirmPassword { get; set; }
  }

  public class LogOnModel
  {
    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    [Display(Name = "E-Mail-Adresse")]
    public string UserName { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    [DataType(DataType.Password)]
    [Display(Name = "Passwort")]
    public string Password { get; set; }

    [Display(Name = "Angemeldet bleiben?")]
    public bool RememberMe { get; set; }
  }

  public class RegisterModel : MemberModel
  {
    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "E-Mail-Adresse")]
    [Email(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "InvalidEmailAddress")]
    public string Email { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    [StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MinLengthPassword", MinimumLength = Parameters.MinPasswordLength)]
    [DataType(DataType.Password)]
    [Display(Name = "Passwort")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Passwort wiederholen")]
    [Compare("Password", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PasswordsDoNotMatch")]
    public string ConfirmPassword { get; set; }
  }
}
