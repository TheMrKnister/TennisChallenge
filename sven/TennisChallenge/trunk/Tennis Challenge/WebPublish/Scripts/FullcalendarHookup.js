$(function () {
  function Event(start, end, courtFk) {
    this.start = start
    this.end = end
    this.courtFk = courtFk;
    this.title = ""
    this.editable = true
    this.allDay = false
    this.duration = ""
  }

  function LowerCaseFirst(str) {
    var head = str.charAt(0).toLowerCase()
    var tail = str.substring(1)
    return head + tail
  }

  function SerializedArrayToObject(array) {
    var o = {}
    _.each(array, function (e) {
      var propertyName = LowerCaseFirst(e.name)
      o[propertyName] = e.value
    })
    return o
  }

  function CreateEvent(event) {
    var durationMin = parseInt(event.duration.substr(3, 5));
    var durationHour = parseInt(event.duration.substr(0, 2));
    if (durationHour == 0)
      durationHour = 1
    event.end.setHours(event.start.getHours() + durationHour)
    event.end.setMinutes(event.start.getMinutes() + durationMin)
    var data = JSON.parse(JSON.stringify(event)) // work around the bad JSONification of JQuery
    $.ajax({ type: 'POST', data: data })
    calendar.fullCalendar('renderEvent', event, false)
  }

  function CalcTimeDif(event) {
    var dif = event.end - event.start;
    var difH = dif / (3600 * 1000)
    difH = parseInt(difH);
    var difM = dif / (60000) - (difH * 60)

    var durationMin = parseInt(event.duration.substr(3, 5));
    var durationHour = parseInt(event.duration.substr(0, 2));

    if (difH != durationHour) {
      var hour = durationHour - difH
      event.end.setHours(event.end.getHours() + hour);
    }

    if (difM != durationMin) {
      var min = durationMin - difM
      event.end.setMinutes(event.end.getMinutes() + min);
    }

    return event;
  }

  function GetDuration(event) {
    var dif = event.end - event.start;
    var difH = dif / (3600 * 1000)
    difH = parseInt(difH);
    var difM = dif / (60000) - (difH * 60)

    if (difH < 10)
      difH = "0" + difH
    if (difM < 10)
      difM = "0" + difM

    if (difH == 0 && difM == 0)
      return "01:00";

    return difH + ":" + difM;
  }

  function UpdateEventClick(event, isClick) {
    CalcTimeDif(event);
    var data = JSON.parse(JSON.stringify(event)) // work around the bad JSONification of JQuery
    $.ajax({ type: 'PUT', data: data })
  }

  function UpdateEvent(event, isClick) {
    var data = JSON.parse(JSON.stringify(event)) // work around the bad JSONification of JQuery
    $.ajax({ type: 'PUT', data: data })
  }

  function DeleteEvent(eventId) {
    $.ajax({
      type: 'DELETE',
      contentType: 'application/json; charset=utf-8',
      data: JSON.stringify(eventId),
      success: function () { calendar.fullCalendar('removeEvents', eventId) }
    })
  }

  function UpdateEventSource(event) {
    var calendarOptions = event.data
    // fullCalendar has no setting or getting of individual options
    calendar.fullCalendar('destroy')
    calendarOptions.events = window.tc.calenderAPI + '/' + $(this).val()
    $.ajaxSetup({
      url: calendarOptions.events,
      statusCode: {
        403: function () {
          alert("Sie haben nicht die Berechtigung das zu tun")
        }
      },
      success: function (data, textStatus, jqXHR) {
        calendar.fullCalendar('refetchEvents')
      }
    })
    calendar.fullCalendar(calendarOptions)
  }

  function EventEditDialog(calEvent, saveEvent, deleteEvent) {
    var buttons = {
      'Speichern': function () {
        //IPA Relevant Start
        var tournamentCourtsList = $('#tournament-courts select option:selected');
        if (tournamentCourtsList.length != 0) {
          var tournamentCourts = [];
          tournamentCourtsList.each(function (_, court) {
            tournamentCourts.push(court.value);
          })
        }
        //IPA Relevant End
        var obj = SerializedArrayToObject($(this).find('input, select').serializeArray())
        $.extend(calEvent, obj)
        //IPA Relevant Start
        var tournamentCourt = { "TournamentCourts": tournamentCourts };
        $.extend(calEvent, tournamentCourt)
        //IPA Relevant End
        saveEvent(calEvent)
        $(this).dialog('close')
      },
      'Abbrechen': function () {
        $(this).dialog('close')
      }
    }

    if (typeof (deleteEvent) === 'function') {
      buttons.Löschen = function () {
        deleteEvent(calEvent.id)
        $(this).dialog('close')
      }
    }

    $('#dialog').dialog({
      modal: true,
      open: function (event, ui) {
        $(this).find('#Title').val(calEvent.title)
        $(this).find('#courtFk').val(calEvent.courtFk)
        $(this).find('#Duration').val(GetDuration(calEvent))
        //IPA Relevant Beginn
        $(this).find('#tournament-courts option').removeAttr("selected");
        $(this).find('#tournament-courts select').val(calEvent.courtFk).attr('selected', 'selected')
        //IPA Relevant End
      },
      buttons: buttons
    })
  }

  function SchemeChanged() {
    var scheme = $(this).val()
    var weekOfMonth = $(".week-of-month")
    if (scheme == TennisChallenge.Utils.SerialBookingSchemes.Weekly) {
      weekOfMonth.find("input").attr("disabled", "disabled")
      weekOfMonth.hide()
    }
    else if (scheme == TennisChallenge.Utils.SerialBookingSchemes.Monthly) {
      weekOfMonth.show()
      weekOfMonth.find("input").removeAttr("disabled")
    }
  }

  var calendar = $('#calendar')
  var selectEventSource = $('#courtSelect')
  var calendarOptions = {
    allDayDefault: false,
    buttonText: {
      today: 'Heute',
      month: 'Monatsansicht',
      day: 'Tagesansicht',
      week: 'Woche'
    },
    allDayText: "Ganztägig",
    monthNames: ['Januar', 'Februar', 'März', 'April', 'Mai', 'Juni', 'Juli', 'August', 'September', 'Oktober', 'November', 'Dezember'],
    monthNamesShort: ['Jan', 'Feb', 'Mär', 'Apr', 'Mai', 'Jun', 'Jul', 'Aug', 'Sept', 'Okt', 'Nov', 'Dez'],
    dayNames: ['Sonntag', 'Montag', 'Dienstag', 'Mittwoch', 'Donnerstag', 'Freitag', 'Samstag'],
    dayNamesShort: ['So', 'Mo', 'Di', 'Mi', 'Do', 'Fr', 'Sa'],
    firstDay: 1,
    timeFormat: "H:mm",
    axisFormat: "H:mm",
    editable: true,
    header: {
      left: 'title',
      center: 'agendaDay month',
      right: 'today prev,next'
    },
    dayClick: function (date, allDay, jsEvent, view) {
      // if you click on a date; change to the view
      // and go to the specific day you clicked on
      switch (view.name) {
        case 'month':
          calendar.fullCalendar('changeView', 'agendaDay')
                  .fullCalendar('gotoDate', date)
          break
        case 'agendaDay':
          var start = new Date(date)
          var end = new Date(date)
          var eventObject = new Event(start, end, selectEventSource.val())
          EventEditDialog(eventObject, CreateEvent)
          break
      }
    },
    eventClick: function (calEvent, jsEvent, view) {
      if (calEvent.editable) {
        EventEditDialog(calEvent, UpdateEventClick, DeleteEvent)
      }
    },
    eventDrop: UpdateEvent,
    eventResize: UpdateEvent
  }

  selectEventSource.change(calendarOptions, UpdateEventSource)
  selectEventSource.change()

  $("[name=open-create-booking-series]").click(function () {
    $("#create-booking-series").dialog({
      modal: true,
      width: "auto",
      height: "auto"
    })

    $("#Scheme").change(SchemeChanged)
    SchemeChanged.call($("#Scheme"))
  })

})