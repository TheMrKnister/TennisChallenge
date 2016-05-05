//IPA Relevant Beginn
$.ajaxSetup({ cache: false });

  function setupButtons() {
    $('#close-menu').click(function () {
      $('#tournament-admin-menu').hide();
      location.reload();
    });

    $('#randomise-players').click(function () {
      var tourId = $(this).data('data-id')
      $.get(TennisChallenge.Constants.randomizePlayers, { tourId: tourId })
      $('#tournament-admin-menu').hide();
      location.reload();
    });

    $('#end-tournament').click(function () {
      var tourId = $(this).data('data-id');
      $.get(TennisChallenge.Constants.endTournament, { tourId: tourId })
      $('#tournament-admin-menu').hide();
      location.reload();
    });

    $('#set-players').click( function () {
      assignPlayerDisplay($(this).data('data-id'));
    });

    $('#select-winner').click(function () {
      fillGamePlan($(this).data('data-id'), true);
    });   
  }

  //Sets up the Tournament Butons and displays the correct Tournament
  function tournamentSetup() {
    $.getJSON(TennisChallenge.Constants.getNextTournaments).success(function (data) {
      if (data.length != 0) {
        if (data[0].TournamentType == "1") {
          //if the first tournament in data is a classis tournament
          $('#double-fun').hide()
          $('#single-fun').hide()
          if (data[0].Mode == "0") {
            $('#double-tourn').hide()
            //$('#single-tourn').show()
            $('#single-tourn .tourn-date section').text(data[0].Start.substr(0, 10))
          } else {
            $('#single-tourn').hide()
            //$('#double-tourn').show()
            $('#double-tourn .tourn-date section').text(data[0].Start.substr(0, 10))
          }

          $('#tournament-sirius .news-tournament-buttons.sirius button').data('data-id', data[0].TournamentId)
        }
        else {
          if (data[0].Mode == "0") {
            $('#double-fun').hide()
            $('#single-fun').show()
            $('#single-fun .tourn-date section').text(data[0].Start.substr(0, 10))
          } else {
            $('#single-fun').hide()
            $('#double-fun').show()
            $('#double-fun .tourn-date section').text(data[0].Start.substr(0, 10))
          }

          //if there is a classic tournament
          if (data.length > 1) {
            if (data[1].Mode == "0") {
              $('#double-tourn').hide()
              //$('#single-tourn').show()
              $('#single-tourn .tourn-date section').text(data[1].Start.substr(0, 10))
            } else {
              $('#single-tourn').hide()
              //$('#double-tourn').show()
              $('#double-tourn .tourn-date section').text(data[1].Start.substr(0, 10))
            }
            $('#tournament-sirius .news-tournament-buttons.sirius button').data('data-id', data[1].TournamentId)
          } else {
            $('#double-tourn').hide()
            $('#single-tourn').hide()
          }
          $('#tournament-fun .news-tournament-buttons.fun button').data('data-id', data[0].TournamentId)
        }
      } else {
        $('.tournament-display').hide();
      }

      setupButtons();
    })
  }

  function setLoginButton(button, tournId) {
    button.unbind();
    button.text("AN/ ABMELDEN")
    button.click(function () {
      TennisChallenge.InfoBoard.Model.addToGameClick(tournId);
    });
  }

  function openFrame(url) {
    $('#plane-frame').attr("src", url.data.url)
    $('#pdf-view').show();
  }

  function resetButtons(button, url) {
    button.unbind();
    button.text("SPIELPLAN")
    button.click({ url: url }, openFrame)

    $('#close-frame').click(function () {
      $('#pdf-view').hide();
    });
  }

  function clubtournSetup() {
    $.getJSON(TennisChallenge.Constants.getInterclub, { all: true }).done(function (data) {
      for (var i = 0; i <= 2; i++) {
        if (data[i] != null) {
          if (data[i].Mode == 0) {
            $('#double-interclub-' + i).hide();
            $('#single-interclub-' + i).show();
            $('#single-interclub-' + i + ' .tourn-date section').text(data[i].Start.substr(0, 10))
            if (data[i].TournComment != null) {
              $('#single-interclub-' + i + ' .tourn-comment section').text(data[i].TournComment)
            }
            if (data[i].Comment != null) {
              $('#single-interclub-' + i + ' .news-headline section').text(data[i].Comment)
            }
            $('#single-interclub-' + i + ' .news-tournament-buttons.interclub button').data('data-id', data[i].TournamentId)
            if (data[i].Closed) {
              resetButtons($('#single-interclub-' + i + ' .news-tournament-buttons.interclub .add-to-game'), data[i].LinkUrl)
              $('#single-interclub-' + i + ' .news-tournament-buttons.interclub .admin-button').hide()
              $('#single-interclub-' + i + ' .news-tournament-buttons.interclub .single-interc-plan').hide()
            } else {
              setLoginButton($('#single-interclub-' + i + ' .news-tournament-buttons.interclub .add-to-game'), data[i].TournamentId)
              $('#single-interclub-' + i + ' .news-tournament-buttons.interclub .admin-button').show()
              $('#single-interclub-' + i + ' .news-tournament-buttons.interclub .single-interc-plan').show()
            }
          } else {
            $('#single-interclub-' + i + '').hide();
            $('#double-interclub-' + i + '').show();
            $('#double-interclub-' + i + ' .tourn-date section').text(data[i].Start.substr(0, 10));
            if (data[i].TournComment != null) {
              $('#double-interclub-' + i + ' .tourn-comment section').text(data[i].TournComment)
            }
            if (data[i].Comment != null) {
              $('#double-interclub-' + i + ' .news-headline section').text(data[i].Comment)
            }            
            $('#double-interclub-' + i + ' .news-tournament-buttons.interclub button').data('data-id', data[i].TournamentId)
            if (data[i].Closed) {
              resetButtons($('#double-interclub-' + i + ' .news-tournament-buttons.interclub .add-to-game'), data[i].LinkUrl)
              $('#double-interclub-' + i + ' .news-tournament-buttons.interclub .admin-button').hide()
              $('#double-interclub-' + i + ' .news-tournament-buttons.interclub .double-interc-plan').hide()
            } else {
              setLoginButton($('#double-interclub-' + i + ' .news-tournament-buttons.interclub .add-to-game'), data[i].TournamentId)
              $('#double-interclub-' + i + ' .news-tournament-buttons.interclub .admin-button').show()
              $('#double-interclub-' + i + ' .news-tournament-buttons.interclub .double-interc-plan').show()
            }
          }
        } else {
          $('#double-interclub-' + i).hide();
          $('#single-interclub-' + i + '').hide();
        }
      }
    });
    setupButtons();
  }
  //IPA Relevant End