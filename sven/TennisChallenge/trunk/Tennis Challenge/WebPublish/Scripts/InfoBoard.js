$(function () {
  "use strict"
  // TODO: Use the default game time club property instead of this hardcoded one
  var defaultGameLength = 45
  var squareClass = "infoboard-square booking-square"
  var columnSelector = ".time-table-column"

  function aspNetJsonToDate(dateString) {
    var date = new Date(parseInt(dateString.replace(/\/Date\((\d+)\)\//gi, "$1"), 10));
    return date;
  }

  /*Start of the Infoboard Calendar*/
  $('#back-calendar').click(function () {
    $('#calendar').fullCalendar('destroy');
    $('#back-calendar').hide();
    $('#courts-inf').show();
    initCalendar();
    calendar();
  });

  function getDay() {
    var d = new Date();
    var day = d.getDay();
    return day;
  }

  function getTitle(booking) {
    if (booking.Comment != null) {
      return booking.Comment;
    }
    else {
      switch (booking.Type) {
        case 0:
          return " Match";
          break;
        case 2:
          return " School";
          break;
        case 3:
          return " Interclub";
          break;
        case 4:
          return " Tournament";
          break;
      }
    }
  };

  function getBackgroundColor(booking) {
    switch (booking.Type) {
      case 0:
        return "#dd1010";
        break;
      case 2:
        return "#33689c";
        break;
      case 3:
        return "#d19c1a";
        break;
      case 4:
        return "#d19c1a";
        break;
    }
  };

  function changeView(date) {
    $('#calendar').fullCalendar('changeView', 'agendaDay')
    $('#calendar').fullCalendar('gotoDate', date)
    $('#back-calendar').show();
    $('#courts-inf').hide();
  }

  function wrapDivs() {
    for (var i = 0; i < 5; i++) {
      $('.week' + i).wrap('<div class="week' + i + '-event" />')
      var height = $('.week' + i).height();
      $('.week' + i + '-event').css('height', height);
    }

    $('.week0-event').parent().addClass('events-pos');

    for (var i = 0; i < 5; i++) {
      var newDiv = $('<div/>').addClass('week' + i + '-wrapper');
      $('.events-pos').append(newDiv);
    }

    for (var i = 0; i < 5; i++) {
      $('.week' + i + '-event').appendTo('.week' + i + '-wrapper')
    }

    $('.events-pos').css('z-index', '-10');
  }

  //function initLegend() {
  //  $('.court-div').each(function (ind, val) {
  //    var height = $('#calendar.infoboard .fc-week' + ind).height();
  //    $('#court-l-' + ind).css('height', height);
  //  });
  //}

  function initCalendar() {    
    $('#calendar').fullCalendar({
      weekends: true,
      height: 465,
      timeFormat: 'H:mm',
      axisFormat: 'H:mm',
      firstDay: getDay(),
      dayNames: ['Sonntag','Montag','Dienstag','Mittwoch','Donnerstag','Freitag','Samstag'],
      dayNamesShort: ['So','Mo','Di','Mi','Do','Fr','Sa'],
      dayClick: function (date) {
        changeView(date);
      },
      eventClick: function (event) {
        changeView(event.start);
      }
    });

    for (var day = 5; day < 35; day += 6) {
      $('.fc-day' + day).css('display', 'none');
      day++;
      $('.fc-day' + day).css('display', 'none');
    }

    $('.fc-week5').hide();
    $('.fc-first.fc-last th:nth-child(n+6)').hide();
    $('.fc-state-highlight').removeClass("fc-state-highlight")
  };

  function calendar() {
    $.getJSON(GetCalendarBookings, function (data) {
    }).success(function (data) {
      $.each(data, function (ind, val) {
        var title = getTitle(val);
        var color = getBackgroundColor(val)
        var event = {
          allDay: false,
          start: aspNetJsonToDate(val.Start),
          end: aspNetJsonToDate(val.End),
          title: title,
          color: color,
          className: val.Row
        };
        $('#calendar').fullCalendar('renderEvent', event);        
      })    
      wrapDivs()
    });
  }

  /*End of the Infoboard Calendar*/

  //Berechnet die Intervaszeiten, der auuskommentierte teil würde noch die minuten ändern falls dies mal benötigt wird.
  function intervalTime(hours, minutes) {
    var firstIntH = parseInt(hours) + 2;

    if (firstIntH > 24) {
      firstIntH = firstIntH - 24;
    }

    //var firstIntM = parseInt(minutes) + 30;
    //if (firstIntM > 60) {
    //  firstIntM = firstIntM - 60;
    //  firstIntM = firstIntM > 9 ? firstIntM.toString() : "0" + firstIntM
    //  firstIntH++;
    //}
    //if (firstIntM == 60) {
    //  firstIntM = "0" + 0;
    //}

    return (firstIntH + ":" + minutes);
  }

  function updateTime() {
    var now = new Date()
    // JS can't format dates natively
    var minutes = now.getMinutes()
    var minutestring = minutes > 9 ? minutes.toString() : "0" + minutes
    var hours = now.getHours()
    var hourstring = hours > 9 ? hours.toString() : "0" + hours
    var sixHours = parseInt(hourstring) + 6;
    if (sixHours > 24) {
      sixHours = sixHours - 24;
    }
    $("#current-time > #time").text(hourstring + ":" + minutestring)
    $("#six-time").text(sixHours + ":" + minutestring);
    $("#fint-time").text(intervalTime(hourstring, minutestring));
    var firstInt = $("#fint-time").text();
    var firstIntH = firstInt.substr(0, 2);
    $("#sint-time").text(intervalTime(firstIntH, minutestring));
  }

  function fillTime(row, duration) {
    var durationDate = new Date(duration)
    var durationMinutesToAdd = (durationDate.getHours() - 1) * 60;
    var durationMinutes = durationDate.getMinutes()
    durationMinutes = durationMinutes + durationMinutesToAdd;
    for (var rest = durationMinutes; rest > 0; rest -= defaultGameLength) {
      var height = rest > 0 ? Math.min(rest, defaultGameLength) : 0
      var filler = $("<div>", { "class": "infoboard-square booking-square free", "style": "height: " + height + "px" })
      row.append(filler)
    }
  }

  function updateInfoboard() {
    var now = new Date()
    $(".booking-square").remove()

    $.get(CURRENTBOOKINGSURL, function (data) {
      $(columnSelector).each(function () {
        var currentRow = $(this)
        var rowCourtId = currentRow.data("court-id")
        var restTime = 6 * 60 // six hours in minutes        

        $.each(data, function (i, item) {
          var endLastRes = now;
          if (item.CourtId == rowCourtId) {
            for (var i = 0; i < item.Bookings.length; i++) {
              var startTime = aspNetJsonToDate(item.Bookings[i].StartTime)
              var endTime = aspNetJsonToDate(item.Bookings[i].EndTime)
              // get the time difference in minutes and use it as pixels for the height of the box
              var gameRestTime = 0

              if (endTime < now) { // game already over
                continue;
              }

              if (startTime < now && endTime > now) { // if game has already begun
                gameRestTime = Math.round((endTime.getTime() - now.getTime()) / 60000)
              }

              else { // if game has not begun
                fillTime(currentRow, startTime.getTime() - endLastRes.getTime())
                gameRestTime = Math.round((endTime.getTime() - startTime.getTime()) / 60000)
              }

              var height = gameRestTime
              var divToAppend = $("<div>", { "class": squareClass })

              switch (item.Bookings[i].BookingType) {
                case TennisChallenge.Utils.EBookingType.Match:
                  divToAppend.addClass("match")
                  break
                case TennisChallenge.Utils.EBookingType.Closed:
                  divToAppend.addClass("closed")
                  break
                case TennisChallenge.Utils.EBookingType.School:
                  divToAppend.addClass("school")
                  break
                case TennisChallenge.Utils.EBookingType.Interclub:
                case TennisChallenge.Utils.EBookingType.Tournament:
                  divToAppend.addClass("tournament")
                  break
              }

              divToAppend.height(height).css("line-height", height)
              restTime -= divToAppend.height()
              currentRow.append(divToAppend)
              if (endTime > now) {
                endLastRes = endTime;
              }
            }
          }
        })

        while (restTime > 0) {
          var freeTimeDiv = $("<div>", { "class": squareClass + " free", style: "height: " + defaultGameLength + "px" })
          restTime -= defaultGameLength
          currentRow.append(freeTimeDiv)
        }

      })
    })
  }

  initCalendar()
  calendar()
  updateInfoboard()
  updateTime()
  setInterval(updateInfoboard, 30000)
  setInterval(updateTime, 60000)
})
