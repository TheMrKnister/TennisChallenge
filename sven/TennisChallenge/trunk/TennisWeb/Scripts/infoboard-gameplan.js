//IPA Relevant Beginn
//ajax setup for IExplorer
$.ajaxSetup({ cache: false });

//modifies the gameplan for the admin
function fillAdmin(tourId) {
  $('.player').addClass('admin');
  $('.player').addClass('selectable');

  var button = $("<button />", { id: "save-winners", class: "gameplan-button", text: "GEWINNER SPEICHERN" });
  button.data('tourId', tourId);
  button.insertBefore("#close-gameplan");

  $('#gameplan .tournament-game .player.admin.selectable').click(function () {
    var $this = $(this);

    //removes all class selects from the gameplan , and adds it to the correct double container
    if ($this.hasClass('double')) {
      $this.parent().parent().children('.double-container').removeClass('selected')
      $this.parent('.double-container').addClass('selected');
    }

    //Does the same for games which arn't double single player
    $this.parent().children('.selectable').removeClass('selected');

    if ($this.hasClass('selected')) {
      $this.removeClass('selected');
    } else {
      $this.addClass('selected');
    }
  })

  $('#save-winners').click(function () {
    var winnerArray = new Array();
    var tourId = $(this).data('tourId')

    //data-id is the MemberKey
    $('.player.selected').each(function (ind, elem) {
      winnerArray.push($(this).data('data-id'))
    });

    //did not use jquery because traditional: true
    $.ajax({
      url: TennisChallenge.Constants.selectWinner,
      type: 'GET',
      data: { memberIds: winnerArray },
      traditional: true
    });

    $('#gameplan-view').hide();
  });
}

//generates the gameplan/Spielplan
function fillGamePlan(guid, isAdmin) {
  $('.tournament-game').remove();
  $('#gameplan-view').show();

  $.get(TennisChallenge.Constants.getTournamentGames, { tourId: guid }).success(function (data) {
    $.each(data, function (ind, val) {
      var court = $("<div />", { class: "game-court", text: val.Court }),
        player1 = $("<div />", { class: "player game-player1", text: val.Member0FullName }).data('data-id', val.Member0Fk),
        player2 = $("<div />", { class: "player game-player2", text: val.Member1FullName }).data('data-id', val.Member1Fk),
        vsSing = $("<div />", { class: "vs-sing", text: "vs." });

      if (val.Member2Fk != null) {
        double1 = $("<div />", { class: "double-container double1" }),
        double2 = $("<div />", { class: "double-container double2" }),
        player1.appendTo(double1),
        player2.appendTo(double1),
        player3 = $("<div />", { class: "player game-player3 double", text: val.Member2FullName }).data('data-id', val.Member2Fk).appendTo(double2),
       player4 = $("<div />", { class: "player game-player4 double", text: val.Member3FullName }).data('data-id', val.Member3Fk).appendTo(double2),
        vsSing.addClass("double");
        player1.addClass("double")
        player2.addClass("double")
        $("<div />", { id: "game-" + ind + "", class: "tournament-game" }).append(court, double1, vsSing, double2).appendTo('#games-view');
      } else {
        $("<div />", { id: "game-" + ind + "", class: "tournament-game" }).append(court, player1, vsSing, player2).appendTo('#games-view');
      }
    })

    //if called by a admin
    if (isAdmin) {
      fillAdmin(guid);
    }
  });

  $.get(TennisChallenge.Constants.notInBooking, { tournId: guid }).success(function (data) {
    var memberString = "";
    $.each(data, function (ind, val) {
      memberString += val.FullName + ", ";
    });

    $("<div />", { id: "not-set-players", text: "Es spielen nicht: " + memberString }).appendTo('#gameplan');
  });
}

//following code used for the Teilnehmer display on the infoboard

function updateMemberCount() {
  var count = $('#tournament-member-list ul li').length;
  $('#in-tourn-count').text(count);
}

//reloads the list of members in a tournament
function refreshTournMembers() {
  var tourId = $('#tournament-members-dialog').data('tourn-id')
  $.get(TennisChallenge.Constants.getTournPlayers, { tournId: $('#tournament-members-dialog').data('tourn-id') }).success(function (data) {
    var list = $('#tournament-member-list ul');
    list.empty();

    $.each(data, function (index, item) {
      list.append('<li class="leave-tourn-member assing-selectable gender-' + item.TitleFk + '" data-user-id="' + item.MemberKey + '" data-is-set="' + item.isSet + '" data-team-Id="' + item.TeamId + '">' + item.FullName + '</li>');
    });
    setupTournButtons();
    updateMemberCount()
  });

  $.get(TennisChallenge.Constants.getNotInTournament, { tourId: tourId }).success(function (data) {
    var list = $('#not-tournament-member-list ul');
    list.empty();

    $.each(data, function (index, item) {
      list.append('<li class="add-tourn-member assing-selectable gender-' + item.TitleFk + '" data-user-id="' + item.MemberKey + '" data-is-set="' + item.isSet + '">' + item.FullName + '</li>');
    });
    setupTournButtons();
  });
  $('#not-member-search').val("");
}

//bind the correct click to the buttons
function setupTournButtons() {
  $('#tournament-member-list ul li').unbind("click");

  $('#not-tournament-member-list ul li').click(function () {
    $.post(TennisChallenge.Constants.addMemberToTourn, { guid: $(this).attr("data-user-id"), tourGuid: $('#tournament-members-dialog').data('tourn-id') }).done(function () {
      refreshTournMembers();
    });
  });

  $('#tournament-member-list ul li').click(function () {
    var tournId = $('#tournament-members-dialog').data('tourn-id');
    $.get(TennisChallenge.Constants.removeFromTournament, { username: null, rfid: null, tournId: tournId, guid: $(this).attr("data-user-id") }).done(function () {
      refreshTournMembers();
    });
  });

  //$('#not-member-search').blur(function () {
  //  attachKeyboard($('#not-member-search'));
  //});

}

//filters the memberList
function filterList(searchedText) {
  $('#not-tournament-member-list ul li').each(function (ind, item) {
    var text = $(this).text();
    if (!(text.toLowerCase().indexOf(searchedText) >= 0)) {
      $(this).hide();
    } else {
      $(this).show();
    }
  });
}

function attachKeyboard(selector) {
  $(selector).keyboard({
    layout: 'search',
    usePreview: false,
    autoAccept: true,
    alwaysOpen: false,
    position: {
      of: null,
      my: "right top",
      at: "left top",
      at2: "left top"
    },
    change: function (e, keyboard, el) {
      if (el != null) {
        filterList(el.value)
      }
    },
    canceled: function (e, keyboard, el) {
      filterList(el.value)
    }
  });
}

//enables switching around members in the teilnehmer view
function displayMembersAdmin() {
  $('#tournament-members-dialog').addClass('admin');
  $('#tourn-arrow-left').show();
  $('#tourn-arrow-right').show();
  $('#not-tournament-member-list').show();
  attachKeyboard($('#not-member-search'));
  setupTournButtons()

  $('#not-tournament-member-list input').keyup(function () {
    var searchedText = $(this).val()
    filterList(searchedText)
  });
}

//Fills the Teilnehmer list, also sets up the admin part.
function displayMembers(tournId) {
  $('#tournament-members-dialog').data('tourn-id', tournId);
  $.get(TennisChallenge.Constants.getTournPlayers, { tournId: tournId }).success(function (data) {

    var list = $('#tournament-member-list ul');
    var prevTeam = "";
    var currentTeam = 0;
    list.empty();

    $.each(data, function (index, item) {
      list.append('<li class="leave-tourn-member assing-selectable gender-' + item.TitleFk + '" data-user-id="' + item.MemberKey + '" data-is-set="' + item.isSet + '" data-team-Id="' + item.TeamId + '">' + item.FullName + '</li>');
    });

    $('.leave-tourn-member').each(function (ind, val) {
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
    updateMemberCount()
  });

  $.get(TennisChallenge.Constants.getNotInTournament, { tourId: tournId }).success(function (data) {

    var list = $('#not-tournament-member-list ul');
    list.empty();

    $.each(data, function (index, item) {
      list.append('<li class="add-tourn-member assing-selectable gender-' + item.TitleFk + '" data-user-id="' + item.MemberKey + '" data-is-set="' + item.isSet + '">' + item.FullName + '</li>');
    });
  });
}

//Setting up Buttons on the board
$(function () {
  //data-id is TournamentId
  $('#single-fun-plan').click(function () {
    //fillGamePlan($(this).data('data-id'), false);
    displayMembers($(this).data('data-id'));
    $('#tournament-members-view').show();
  });

  $('#double-fun-plan').click(function () {
    //fillGamePlan($(this).data('data-id'), false);
    displayMembers($(this).data('data-id'));
    $('#tournament-members-view').show();
  });

  $('#single-tourn-plan').click(function () {
    //fillGamePlan($(this).data('data-id'), false);
    displayMembers($(this).data('data-id'));
    $('#tournament-members-view').show();
  });

  $('#double-tourn-plan').click(function () {
    //fillGamePlan($(this).data('data-id'), false);
    displayMembers($(this).data('data-id'));
    $('#tournament-members-view').show();
  });

  $('.single-interc-plan').click(function () {
    //fillGamePlan($(this).data('data-id'), false);
    displayMembers($(this).data('data-id'));
    $('#tournament-members-view').show();
  });

  $('.double-interc-plan').click(function () {
    //fillGamePlan($(this).data('data-id'), false);
    displayMembers($(this).data('data-id'));
    $('#tournament-members-view').show();
  });

  $('#close-tournament-members').click(function () {
    $('#tournament-members-view').hide();
    $('#not-tournament-member-list').hide();
    if ($('#tournament-members-dialog').hasClass('admin')) {
      $('#not-tournament-member-list input').getkeyboard().destroy();
    }
    $('#tournament-members-dialog').removeClass('admin');
    location.reload();
  });

  $('#close-gameplan').click(function () {
    $('#save-winners').remove();
    $('#not-set-players').remove();
    $('#gameplan-view').hide();
  })
});
//IPA Relevant End