$(function () {

  //sets up caching for IExplorer
  $.ajaxSetup({
    cache: false
  });

  //sets up the ranked games for the website
  $.getJSON(GetGames, function (data) {
    $.each(data, function (i, val) {
      $game = $("<div id='game-" + i + "' class='rGame'></div>");
      $game.data('gameId', { id: val.GameId });
      $game.append("<div class='game-players'>" + val.Defender + " vs. " + val.Challenger + "</div>")
      $game.append("<div class='select-winner'>Gewinner: <select id='winner'><option>" + val.Challenger + "</option><option>" + val.Defender + "</option></select></div>")
      $game.append("<div class='enter-game-1'>Satz 1: <input type='text' class='game1-score1 score-field' /> : <input type='text' class='game1-score2 score-field'/></div>")
      $game.append("<div class='enter-game-2'>Satz 2: <input type='text' class='game2-score1 score-field'/> : <input type='text' class='game2-score2 score-field'/></div>")
      $game.append("<div class='enter-game-3'>Tie Break: <input type='text' class='game3-score1 score-field'/> : <input type='text' class='game3-score2 score-field'/></div>")
      $game.append("<input class='save-game-changes' type='button' value='Spiel Speichern' />")
      $game.append("<input class='delete-game' type='button' value='Spiel löschen' />")
      $('#ranked-games-list').append($game);

      //clik setup to save the changes to the game
      $game.find('.save-game-changes').click(function () {
        var $this = $(this).parent();
      
        var game = {
          GameId: $this.data('gameId').id,
          Winner: $this.find('select option:selected').text(),
          Player1Score1: $this.find('.game1-score1').val(),
          Player1Score2: $this.find('.game2-score1').val(),
          Player1ScoreTie: $this.find('.game3-score1').val(),
          Player2Score1: $this.find('.game1-score2').val(),
          Player2Score2: $this.find('.game2-score2').val(),
          Player2ScoreTie: $this.find('.game3-score2').val()     
        };

        $.post(EditGame, game, function (data) { });
        location.reload();
      })

      //deletes the game
      $game.find('.delete-game').click(function () {
        var $this = $(this).parent();
        var gameId = $this.data('gameId').id;

        $.post(DeleteGame, { gameId: gameId }, function (data) { });
        location.reload();
      });

    });
  });
});