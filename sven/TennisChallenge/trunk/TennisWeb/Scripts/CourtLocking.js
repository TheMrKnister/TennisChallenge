"use strict"
$(function () {
  if (window.tc == null || window.tc.CourtLockingAPI == null) {
    return
  }

  function UpdateCheckbox() {
    _.every(Courts, function (elem) { return $(elem).hasClass(LockedClass) })
      ? Checkbox.prop("checked", true)
      : Checkbox.prop("checked", false)
  }

  function UpdateCourtLocking(id, lock) {
    $.ajax({
      type: 'PUT',
      url: window.tc.CourtLockingAPI + "/" + id,
      data: { lock: lock },
      error: function (jqXHR, textStatus, errorThrown) {
        alert("Etwas scheint schiefgegangen zu sein")
      }
    })
    UpdateCheckbox()
  }

  var LockedClass = "court-locked",
      Checkbox = $("[name=lock-all-courts]"),
      Courts = $(".court-image")

  Checkbox.change(function (event) {
    var lock = $(this).prop("checked")
    _.each(_.filter(Courts, function (i) { return $(i).hasClass(LockedClass) !== lock }), function (elem) {
      lock ? $(elem).addClass(LockedClass)
           : $(elem).removeClass(LockedClass)
      UpdateCourtLocking($(elem).data("court-id"), lock)
    })
  })

  $(".court-image").click(function (event) {
    var courtId = $(this).data("court-id")
    $(this).toggleClass(LockedClass)
    var lock = $(this).hasClass(LockedClass)
    UpdateCourtLocking(courtId, lock)
  })

  $.ajax({
    type: 'GET',
    url: tc.CourtLockingAPI,
    success: function (data, textStatus, jqXHR) {
      _.each(data, function (elem) {
        $("[data-court-id='" + elem + "']").addClass(LockedClass)
      })
      UpdateCheckbox()
    }
  })
})