$(function () {
  function updateList(members) {
    $.each(members, function (_, member) {
      $('#rankning-list').append(
        $("<li></li>")
        .text(member.Rank + ". " + member.FullName)
        .attr("data-id", member.Id)
        .attr("data-rank", member.Rank)
        .click(SelectUser)
        );

    });
  }

  function updateLinks(user) {
    var link = $("#change-rank")
    link.show()

    var oldUrl = link.prop("href")
    var newUrl = oldUrl.replace(/[^\/]+$/, user)

    var indexOfQm = oldUrl.indexOf('?');
    if (indexOfQm > 0) {
      var oldUrlPart = oldUrl.substr(indexOfQm);
      newUrl = newUrl + oldUrlPart;
    }

    link.prop("href", newUrl)
  }

  function SelectUser() {
    $('#former-rank').show();
    $('#new-rank').show();
    var $this = $(this);
    var fullName = $this.text().split(". ").pop();
    var id = $this.attr('data-id');
    $('#selected-player h3').text(fullName)
    $('#former-rank-input').val($this.attr('data-rank'));
    updateLinks(id);
  }

  var userName = $('#title-name').attr('data-user');
  var gender = parseInt($('#title-name').attr('data-gender'))

  $.ajaxSetup({
    cache: false
  })

  $.getJSON(GetPlayers, { userName: userName, gender: gender }, function () {
  }).success(function (data) {
    var members = data;
    updateList(members);
  });

  $('#back-ranked').click(function () {
    parent.history.back();
    return false;
  });

  $('#new-rank').keyup(function () {
    $('#rank-link').show();
    var link = $("#change-rank");
    var oldUrl = link.prop("href")
    var clubrank = parseInt($('#new-rank-input').val());
    var oldRank = parseInt($('#former-rank-input').val());
    var ind = oldUrl.indexOf("=");
    if (oldUrl.length >= ind) {
      var i = oldUrl.indexOf("=");
      var slice = (oldUrl.length - i) - 1;
      var newUrl = oldUrl.slice(0, -slice);
    } else {
      newUrl = oldUrl;
    }
    var oldUrl = link.prop("href", "")
    newUrl += clubrank;
    link.prop("href", newUrl)

    if (link.prop("href").length < ind || clubrank < 1 || isNaN(clubrank) || clubrank > oldRank) {
      $('#rank-link').hide();
    }
  });
});