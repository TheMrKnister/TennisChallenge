//IPA Relevant Beginn
$(function () {

  //caching for IExplorer
  $.ajaxSetup({ cache: false });

  //creates and appends the div structure, depending on the site on which it gets executed it changes some elemnts
  function displayTournament(tournament, site) {
    var tournMode = ""
    var tournType = ""

    var dateInput = $("<input />", { class: "tournament-input date-input", value: tournament.Start, readonly: "true" }),
      memberInput = $("<input />", { class: "tournament-input member-input", value: tournament.Members, readonly: "true" }),
      courtsInput = $("<input />", { class: "tournament-input courts-input", value: tournament.Courts, readonly: "true" }),
      modeInput = $("<input />", { class: "tournament-input mode-input", value: tournament.Mode, readonly: "true" }),
      typeInput = $("<input />", { class: "tournament-input type-input", value: tournament.TournamentType, readonly: "true" })

    var date = $("<div />", { class: "tournament-label", text: "Datum:" }).append(dateInput),
     member = $("<div />", { class: "tournament-label", text: "Anzahl Angemeldeter:" }).append(memberInput),
     courts = $("<div />", { class: "tournament-label", text: "Plätze:" }).append(courtsInput),
     mode = $("<div />", { class: "tournament-label", text: "Modus:" }).append(modeInput),
     type = $("<div />", { class: "tournament-label", text: "Typ:" }).append(typeInput)

    if (site == "admin") {
      var cancel = $("<button />", { class: "tournament-button cancel-tournament", text: "Turnier absagen" }).data("data-id", tournament.TournamentId),
        finish = $("<button />", { class: "tournament-button end-tournament", text: "Turnier abschliessen" }).data("data-id", tournament.TournamentId)

      $("<div />", { class: "tournament-display" }).append(date, member, courts, mode, type, cancel, finish).appendTo("#tournament-container");
    }

    if (site == "old") {
      var rankes = $("<button />", { class: "tournament-button ranked-tournament", text: "Rangliste" }).data("data-id", tournament.TournamentId)

      $("<div />", { class: "tournament-display old" }).append(date, courts, mode, type, rankes).appendTo("#tournament-container");
    }
    if (site == "new" || site == "beginn") {
      $("<div />", { class: "tournament-display no-buttons" }).append(date, member, courts, mode, type).appendTo("#tournament-container");
    }
  }

  //Gets, fills and appends the tournament ladder
  function fillLadder(tournament) {
    $.getJSON(GetLadder, { tourId: tournament }, function (data) {
      var members = data.List.split(";");

      $.each(members, function (ind, val) {
        $("<div>", { class: "member", text: val }).appendTo("#ladder-container").before($('#close-ladder'));
      });
    });
  }

  function initiateButtons() {
    $(".cancel-tournament").click(function () {
      $.post(DeleteTournament, { tourId: $(this).data("dataId") });
      location.reload();
    });

    $(".end-tournament").click(function () {
      $.get(EndTournament, { tourId: $(this).data("dataId") });
      location.reload();
    });

    $(".ranked-tournament").click(function () {
      $('#tournament-container').hide();
      fillLadder($(this).data("data-id"));
      $("#ladder-container").show();
    });

    $("#close-ladder").click(function () {
      $("#ladder-container").hide();
      $(".member").remove();
      $('#tournament-container').show();
    });
  }

  function buildTournaments() {
    var tournaments = [];

    //Gets the correct tournaments
    $.getJSON(GetAllTournaments, function () { }).done(function (data) {
      $.each(data, function (_, tourn) {
        displayTournament(tourn, Site)
      })
      initiateButtons()
    });
  }
  buildTournaments()
});
//IPA Relevant End