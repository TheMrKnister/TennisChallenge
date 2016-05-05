using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace TennisWeb.Utils
{
  public class DateRangeAttribute : ValidationAttribute, IClientValidatable, IMetadataAware
  {
    public const string DateFormat = "yyyy-MM-dd";
    private const string DefaultErrorMessage = "'{0}' must be a date between {1:d} and {2:d}.";

    public DateTime MinDate { get; set; }

    private DateTime? _maxDate;
    public DateTime MaxDate
    {
      get
      {
        if (_maxDate.HasValue)
          return _maxDate.Value;

        return DateTime.Now;
      }
    }

    public DateRangeAttribute(string minDate, string maxDate)
      : base(DefaultErrorMessage)
    {
      MinDate = ParseDate(minDate);
      if(maxDate != "now")
        _maxDate = ParseDate(maxDate);
    }

    public override bool IsValid(object value)
    {
      if (value == null || !(value is DateTime))
      {
        return true;
      }
      var dateValue = (DateTime)value;
      return MinDate <= dateValue && dateValue <= MaxDate;
    }

    public override string FormatErrorMessage(string name)
    {
      return String.Format(CultureInfo.CurrentCulture, ErrorMessageString,
          name, MinDate, MaxDate);
    }

    private static DateTime ParseDate(string dateValue)
    {
      return DateTime.ParseExact(dateValue, DateFormat,
        CultureInfo.InvariantCulture);
    }

    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
      return new[]
               {
                 new ModelClientValidationRangeDateRule(
                   FormatErrorMessage(metadata.GetDisplayName()), MinDate, MaxDate)
               };
    }

    public void OnMetadataCreated(ModelMetadata metadata)
    {
      metadata.DataTypeName = "Date";
    }
  }

  public class ModelClientValidationRangeDateRule : ModelClientValidationRule
  {
    public ModelClientValidationRangeDateRule(string errorMessage, DateTime minValue, DateTime maxValue)
    {
      ErrorMessage = errorMessage;
      ValidationType = "rangedate";

      ValidationParameters["min"] = minValue.ToString(DateRangeAttribute.DateFormat);
      ValidationParameters["max"] = maxValue.ToString(DateRangeAttribute.DateFormat);
    }
  }
}