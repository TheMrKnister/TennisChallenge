var state = "default";
var tournIdOut;
var exFill = true;

function updatememberCount() {
  var count = $('#assign-member-list ul li.assing-selectable').length;
  $('#memb-count').text(count);
  count = $('input.assign-input-inp').length;
 
  count = count - $('input.assign-input-inp.gender-1').length;
  count = count - $('input.assign-input-inp.gender-2').length;

  $('#memb-court-count').text(count);
}

function clearSetPlayer(elem) {
  var name = elem.val();

  $('#assign-member-list ul li.selected').each(function (ind, val) {
    if ($(this).text() == name) {
      $(this).removeClass("selected");
      $(this).addClass("assing-selectable");
      $(this).show();
    }
  });
  elem.val("");
  elem.data("data-member-key", "");
  updatememberCount();
}

function setPlayer(elem, input) {
  if (elem.hasClass('assing-selectable')) {
    elem.removeClass("assing-selectable");
    input.val(elem.text());
    if (elem.hasClass('gender-1')) {
      input.addClass('gender-1');
    } else {
      input.addClass('gender-2');
    }

    input.data("data-member-key", elem.attr("data-user-id"));
    input.removeClass('assign-selected');

    elem.addClass('selected');
    elem.hide();
  }
}

function fillPreviouslySet() {
  $('input.assign-input-inp').each(function (ind, val) {
    var id = $(this).parent().data("data-member-key");
    var playerElem = $('#assign-member-list ul li.assing-selectable[data-user-id="' + id + '"]')
    setPlayer(playerElem, $(this));
  });
  updatememberCount();
}

function assignPlayerDisplay(tournId) {
  if (exFill) {
    tournIdOut = tournId
    $.get(TennisChallenge.Constants.getTournPlayers, { tournId: tournId }).success(function (data) {

      var list = $('#assign-member-list ul');
      var prevTeam = "";
      var currentTeam = 0;
      list.empty();

      $.each(data, function (index, item) {
        list.append('<li class="assing-selectable gender-' + item.TitleFk + '" data-user-id="' + item.MemberKey + '" data-is-set="' + item.isSet + '" data-team-Id="' + item.TeamId + '">' + item.FullName + '</li>');
      });

      $('.assing-selectable').each(function (ind, val) {
        if ($(val).data('team-id') != null) {
          if ($(val).data('team-id') == prevTeam) {
            $(val).append(', Team ' + currentTeam)
          }
          else {
            currentTeam++;
            $(val).append(', Team ' + currentTeam)
            prevTeam = $(val).data('team-id')
          }
        }
      });

      $('#assign-member-list ul li').click(function () {
        if ($('#assign-reserve').hasClass('reserve-selected')) {
          if ($(this).hasClass('reserve')) {
            $(this).addClass("assing-selectable")
            $(this).removeClass('selected reserve')
          } else {
            $(this).removeClass("assing-selectable");
            $(this).addClass('selected reserve');
          }
        } else {
          if ($('input.assign-input-inp').hasClass('assign-selected')) {
            if ($(this).hasClass('assing-selectable')) {
              $(this).removeClass("assing-selectable");
              $('input.assign-input-inp.assign-selected').val($(this).text());
              if ($(this).hasClass('gender-1')) {
                $('input.assign-input-inp.assign-selected').addClass('gender-1');
              } else {
                $('input.assign-input-inp.assign-selected').addClass('gender-2');
              }
              $('input.assign-input-inp.assign-selected').data("data-member-key", $(this).attr("data-user-id"));
              $('input.assign-input-inp.assign-selected').removeClass('assign-selected');
              $(this).addClass('selected');
              $(this).hide();
            }
          }
        }
        updatememberCount();
      });

      $('#assign-member-list ul li.assing-selectable').click(function () {

      });
      updatememberCount();
    }).done(function () {
      fillPreviouslySet()
    });

    $.get(TennisChallenge.Constants.getAssigncourts, { tournId: tournId }).success(function (data) {

      $('#assign-player-court-bsp').hide();

      $.each(data, function (index, item) {
        var input1 = $("<div />", { class: "assing-player-1 assign-input" }).append($("<input />", { class: "assign-input-inp", placeholder: "Spieler 1" })).data('data-member-key', item.Member0Fk),
          input2 = $("<div />", { class: "assing-player-2 assign-input" }).append($("<input />", { class: "assign-input-inp", placeholder: "Spieler 2" })).data('data-member-key', item.Member1Fk),
          input3 = $("<div />", { class: "assing-player-3 assign-input" }).append($("<input />", { class: "assign-input-inp", placeholder: "Spieler 3" })).data('data-member-key', item.Member2Fk),
          input4 = $("<div />", { class: "assing-player-4 assign-input" }).append($("<input />", { class: "assign-input-inp", placeholder: "Spieler 4" })).data('data-member-key', item.Member3Fk),
          courtName = $("<div />", { class: "assign-court-name", text: item.Name })

        $("<div />", { class: "assing-court-" + index + " assign-court" }).append(input1, input2, input3, input4, courtName).appendTo($('#assign-player-court-wrapper'));
        $(".assing-court-" + index).data('data-court-key', item.CourtKey);
      });

      $('input.assign-input-inp').on('focus', function () {
        var input = $(this);
        if (input.val() == input.attr('placeholder')) {
          input.val('');
          input.removeClass('placeholder');
        }
      }).on('blur', function () {
        var input = $(this);
        if (input.val() == '' || input.val() == input.attr('placeholder')) {
          input.addClass('placeholder');
          input.val(input.attr('placeholder'));
        }
      }).blur();

      $('input.assign-input-inp').click(function () {
        $('#assign-reserve').removeClass('reserve-selected');
        $(this).removeClass('gender-1 gender-2')
        if ($(this).val() != "") {
          clearSetPlayer($(this))
        }
        $('input.assign-input-inp.assign-selected').removeClass('assign-selected');
        $(this).addClass('assign-selected');
      });
    }).done(function () {
      fillPreviouslySet()
    });

    $('#assign-players-view').show();
    exFill = false;
  }
}

$('#assign-reserve').click(function () {
  if ($(this).hasClass('reserve-selected')) {
    $(this).removeClass('reserve-selected')
  } else {
    $(this).addClass('reserve-selected')
  }
});

$('#close-assign-player').click(function () {
  $("#assign-player-court-wrapper").empty();
  $('#assign-players-view').hide();
});

$('#delete-assign-player').click(function () {
  $('.assign-input-inp').each(function () {
    if ($(this).val() != "") {
      clearSetPlayer($(this))
      $(this).removeClass('gender-1 gender-2')
    }
  });
});

$('#fill-assign-player').click(function () {  
  $.each($('.assign-input-inp'), function (ind, val) {
    var elemList = $('#assign-member-list ul li.assing-selectable')
    
    elemList.sort(function () {
      return Math.round(Math.random()) - 0.5;
    })

    var value = $(this).val();
    if (value == "Spieler 1" || value == "Spieler 2" || value == "Spieler 3" || value == "Spieler 4" || value == "") {
      var firstElem = $(elemList[0]);
      setPlayer(firstElem, $(val));
    }
  })
  updatememberCount();
});

$('#save-assign-player').click(function () {
  //$("#assign-player-court-wrapper").empty();
  $('.assign-court').each(function (ind, item) {
    var courtClass = ".assing-court-" + ind;
    var members = "";
    var tournament = tournIdOut;

    var courtId = $(this).data("data-court-key");
    members += ($(courtClass + " .assing-player-1 .assign-input-inp").data("data-member-key")) + ","
    members += ($(courtClass + " .assing-player-2 .assign-input-inp").data("data-member-key")) + ","
    members += ($(courtClass + " .assing-player-3 .assign-input-inp").data("data-member-key")) + ","
    members += ($(courtClass + " .assing-player-4 .assign-input-inp").data("data-member-key"))

    $.post(TennisChallenge.Constants.AssignPlayers, { memberId: members, courtId: courtId, tournId: tournament })
  });

  $("#assign-player-court-wrapper").empty();
  $('#assign-players-view').hide();
  //$.post(TennisChallenge.Constants.AssignPlayers, { games: JSON.stringify(games) })
});