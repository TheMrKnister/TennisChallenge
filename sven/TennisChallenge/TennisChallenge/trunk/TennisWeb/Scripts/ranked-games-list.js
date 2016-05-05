$(function () {
  $.ajaxSetup({
    cache: false
  });

  $.getJSON(GetGames, function (data) {
    $.each(data, function (i, val) {
      $game = $("<div id='game-" + i + "' class='rGame future-game'></div>");
      $game.data('gameId', { id: val.GameId });
      $game.append("<div class='game-chall'>Forderer: <input type='text' value='" + val.Challenger + "' readonly/></div>")
      $game.append("<div class='game-def'>Geforderter: <input type='text' value='" + val.Defender + "' readonly/></div>")
      $game.append("<div class='game-date'>Datum: <input type='text' value='" + val.Date + "' readonly/></div>")
      $game.append("<div class='game-start'>Start: <input type='text' value='" + val.Start + "' readonly/></div>")
      $game.append("<div class='game-end'>Ende: <input type='text' value='" + val.End + "' readonly/></div>")
      $game.append("<div class='game-court'>Platz: <input type='text' value='" + val.Court + "' readonly/></div>")
      $game.append("<div class='game-rank'>Umspielter Rang: <input type='text' value='" + val.ContestedRank + "' readonly/></div>")
      $('#ranked-games-list').append($game);
    });
  });
});