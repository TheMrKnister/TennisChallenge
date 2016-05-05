$(function () {

  function updateList(members) {
    $.each(members, function (_, member) {
      $('#rankning-list').append(
        $("<li></li>")
        .text(member.Rank + ". " + member.FullName)
        .addClass('hasGame-' + member.HasGame),
        $('<li><a href="mailto:' + member.Email + '">Email Me</a></li>'),       
        $("<li></li>")
        .text(member.TelNr)
        );
    });
  }

  var userName = $('#title-name').attr('data-user');
  var gender = parseInt($('#title-name').attr('data-gender'))

  $.ajaxSetup({
    cache: false
  })

  $.getJSON(GetPlayers, { name: userName, gender: gender }, function () {
  }).success(function (data) {
    var members = data;
    updateList(members);
  });

  $('#back-ranked').click(function () {
    parent.history.back();
    return false;
  });
});