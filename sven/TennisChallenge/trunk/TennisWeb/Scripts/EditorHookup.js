
$(function () {

  function getDateYymmdd(value) {
    if (value == null)
      return null;
    return $.datepicker.parseDate("yy-mm-dd", value);
  }

  $('input.date-picker, #BookingStartTime, #BookingEndTime').each(function () {
    var $this = $(this),
        minDate = getDateYymmdd($this.data("val-rangedate-min")),
        maxDate = getDateYymmdd($this.data("val-rangedate-max"));
    $this.datepicker({
      minDate: minDate,
      maxDate: maxDate
    });
  });

  $('input.time-picker, #LessonStartTime, #LessonEndTime').each(function () {
    var $this = $(this);
    $this.timepicker();
  });
});