$(function () {
  $.ajaxSetup({
    cache: false
  });

  $.getJSON(PlayerGames, function (data) {}).success(function (data) {
    $.each(data, function (i, val) {
      $game = $("<div id='game-" + i + "' class='rGame'></div>");
      $game.data('gameId', { id: val.GameId });
      $game.append("<div class='game-players'><p>Forderer: " + val.Player1 + ", Geforderter: " + val.Player2 + "</p></div>")
      $game.append("<div class='contested-rank'>Umspielter Rang: <input class='score-field' type='text' value='" + val.ContestedRank + "' readonly/></div>")
      $game.append("<div class='game-winner'>Gewinner: <input class='winner-input' type='text' value='" + val.Winner + "' readonly/></div>")
      $game.append("<div class='game-1score1 player1'>Satz 1: <input class='score-field' type='text' value='" + val.Player1Score1 + "' readonly/> : </div>")
      $game.append("<div class='game-2score1 player2'><input class='score-field' type='text' value='" + val.Player2Score1 + "' readonly/></div>")
      $game.append("<div class='game-1score2 player1'>Satz 2: <input class='score-field' type='text' value='" + val.Player1Score2 + "' readonly/> : </div>")
      $game.append("<div class='game-2score2 player2'><input class='score-field' type='text' value='" + val.Player2Score2 + "' readonly/></div>")
      if (val.Player1ScoreTie != 0 || val.Player2ScoreTie != 0) {
        $game.append("<div class='game-1score3 player1'>Tie Break: <input class='score-field' type='text' value='" + val.Player1ScoreTie + "' readonly/> : </div>")      
        $game.append("<div class='game-2score3 player2'><input class='score-field' type='text' value='" + val.Player2ScoreTie + "' readonly/></div>")
      }
      $('#ranked-games-list').append($game);
    });
  });
});