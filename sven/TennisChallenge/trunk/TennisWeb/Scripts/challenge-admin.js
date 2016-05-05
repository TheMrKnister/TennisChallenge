$(function () {
  //sets up caching for IExplorer
  $.ajaxSetup({
    cache: false
  })

  //converts date from json string to usable format
  function aspNetJsonToDate(dateString) {
    var date = new Date(parseInt(dateString.replace(/\/Date\((\d+)\)\//gi, "$1"), 10));
    return date;
  }

  //sets the style pickers form jquerui
  $('#challenge-date-pick').datepicker();
  $('.timepicker').timepicker();

  var userName = $('#challenge-player-mask').attr('data-user');
  var gender = parseInt($('#challenge-player-mask').attr('data-gender'))

  //sets up the new game
  $('#send-game').click(function () {
    var member0 = $('#select-challenger option:selected').attr('data-id'),
          member1 = $('#select-defender option:selected').attr('data-id'),
          member2 = null, member3 = null,
          court = $('#select-court option:selected').attr('data-court'),
          start = $('#challenge-date-pick').val(),
          time = $('.timepicker').val(),
          startTime,
          endTime;

    $.get(ConvertDate, { start: start + ":" + time, end: false }, function (data) { }).success(function (data) {
      startTime = aspNetJsonToDate(data).toISOString();

      $.get(ConvertDate, { start: start + ":" + time, end: true }, function (data) { }).success(function (data) {
        endTime = aspNetJsonToDate(data).toISOString();

        var bookingRequest = {
          StartTime: startTime,
          EndTime: endTime,
          CourtId: court,
          Member0Fk: member0,
          Member1Fk: member1,
          Member2Fk: member2,
          Member3Fk: member3,
          bookingType: 0
        };

        $.post(CreateRankedGame, bookingRequest, function () { }).success(function (data) {
          if (data.success == true) {
            alert("Neues Spiel hinzugefügt");
          } else {
            alert("Es ist ein Fehler aufgetreten")
          }
        });
      });
    });
  });

  $.getJSON(GetPlayers, { userName: userName, gender: gender }, function () {
  }).success(function (data) {
    $.each(data, function (_, member) {
      $('#select-challenger').append(
        $("<option></option>")
        .text(member.FullName)
        .attr("data-id", member.Id)
        .attr("data-rank", member.Rank)
        );
    });
  });
  
  //sets up which player the selected player can challenge
  $('#select-challenger').change(function () {
    $('#select-defender option').remove();
    var userName = $('#select-challenger option:selected').val();

    $.getJSON(GetChallPlayers, { fullname: userName }, function () {
    }).success(function (data) {
      $.each(data, function (_, member) {
        $('#select-defender').append(
          $("<option></option>")
          .text(member.FullName)
          .attr("data-id", member.Id)
          .attr("data-rank", member.Rank)
          );
      });
      $('#challenge-defender').show();
    });    
  });

  $.getJSON(GetCourts, function () {
  }).success(function (data) {
    $.each(data, function (_, court) {
      $('#select-court').append(
        $("<option></option>")
        .text(court.Name)
        .attr('data-court', court.CourtKey)
        )
    });
  });
});