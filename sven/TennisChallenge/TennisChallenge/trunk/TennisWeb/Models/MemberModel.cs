using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using TennisChallenge.Dal;
using TennisWeb.Properties;
using TennisWeb.Utils;

namespace TennisWeb.Models
{
  public class MemberModel : IValidatableObject
  {
    public MemberModel()
    {
      Birthday = new DateTime(1970, 1, 1);
    }

    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    [Display(Name = "Anrede")]
    public Int16? TitleFk { get; set; }

    [Display(Name = "Anrede")]
    public string TitleName { get; set; }

    public IEnumerable<Title> Titles { get; protected set; }

    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    [Display(Name = "Vorname")]
    public string Firstname { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    [Display(Name = "Nachname")]
    public string Lastname { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    [Display(Name = "Geburtsdatum")]
    [DateRange("1880-01-01", "now", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "BirthdayRange")]
    public DateTime? Birthday { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    [Display(Name = "Strasse")]
    public string Street { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    [Display(Name = "PLZ")]
    public string Zip { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
    [Display(Name = "Ort")]
    public string City { get; set; }

    [Display(Name = "Telefonnummer privat")]
    [RegularExpression(@"^ *0 *(\d *){9}$", ErrorMessage = "Telefonnummer im Format 0xx xxx xx xx eingeben.")]
    public string PrivatePhone { get; set; }

    [Display(Name = "Telefonnummer geschäftlich")]
    [RegularExpression(@"^ *0 *(\d *){9}$", ErrorMessage = "Telefonnummer im Format 0xx xxx xx xx eingeben.")]
    public string BusinessPhone { get; set; }

    [Display(Name = "Telefonnummer mobil")]
    [RegularExpression(@"^ *0 *(\d *){9}$", ErrorMessage = "Telefonnummer im Format 0xx xxx xx xx eingeben.")]
    public string MobilePhone { get; set; }

    [Display(Name = "Lizenznummer")]
    [RegularExpression(@"^(\d){3}.(\d){2}.(\d){3}.(\d)", ErrorMessage = "Lizenznummer im Format xxx.xx.xxx.x eingeben.")]
    public string LicenseNumber { get; set; }

    [DataType(DataType.ImageUrl)]
    [Display(Name = "Profilbild")]
    public string PictureUrl { get; set; }

    [UIHint("Clubs")]
    [Display(Name = "Klubs")]
    public IEnumerable<Guid> ClubFks { get; set; }

    [UIHint("Clubs")]
    [Display(Name = "Klubs")]
    public IEnumerable<Club> Clubs { get; set; }

    [UIHint("Clubs")]
    [Display(Name = "Klubs")]
    public IEnumerable<Club> AllClubs { get; set; }

    public void FillTitles()
    {
      Titles = new AccessorBase<Title>().GetAll();
    }

    public void FillAllClubs(params string[] clubs)
    {
      AllClubs = clubs.Length > 0 ? new AccessorBase<Club>().GetAllWhere(c => clubs.Contains(c.Name))
                                  : new AccessorBase<Club>().GetAll();
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      if (String.IsNullOrWhiteSpace(PrivatePhone) &&
        String.IsNullOrWhiteSpace(BusinessPhone) &&
        String.IsNullOrWhiteSpace(MobilePhone))
      {
        yield return new ValidationResult(
          "Bitte mindestens eine Telefonnummer eingeben.",
          new[] { "PrivatePhone", "BusinessPhone", "MobilePhone" });
      }
    }
  }
}