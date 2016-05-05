$(function () {

  function updateMemberList(tournId) {
    $('#member-list li').remove();
    $.get(GetPlayers, { tournId: tournId }).success(function (data) {
      $.each(data, function (_, member) {
        $('#member-list').append(
        $("<li></li>")
        .text(member.FullName)
        );
      });
    });
  }

  function setupTournButtons() {
    $('.tourn-expand > button').click(function () {
      var parent = $(this).parent().parent();

      if (parent.hasClass('display-closed')) {
        parent.removeClass('display-closed')
        parent.addClass('display-open')
        $(this).text("-");        
        updateMemberList(parent.attr('tourn-id'))
      } else if (parent.hasClass('display-open')) {
        parent.removeClass('display-open')
        parent.addClass('display-closed')
        $(this).text("+");
        $('#member-list li').remove();
      }
    });

    $('.cancel-clubtourn').click(function () {
      var tourId = $(this).parent().parent().attr("tourn-id");
      $.get(DeleteTournament, { tourId: tourId });
      intercSetup()
    });

    $('.clubtourn-close input').click(function () {
      var tourId = $(this).parent().parent().attr("tourn-id");
      $.post(SetIntStatus, { tournId: tourId });      
    });

    $('.plan-link').click(function () {
      var tourId = $(this).parent().parent().attr("tourn-id");
      var link = $(this).parent().parent().find('.clubtourn-link input').val();
      $.post(SetLink, { tournId: tourId, link: link }).success(function () {
        alert("Link gespeichert")
      });
    });

    $('.clubtourn-comment textarea').keyup(function () {
      var tourId = $(this).parent().parent().attr("tourn-id");
      var comment = $(this).val();
      $.post(SetComment, { tourId: tourId, comment: comment });
    });
  }

  function appendTourn(val) {
    var toAppend = $("#display-default").clone(true).removeAttr("id");
    toAppend.attr("tourn-id", val.TournamentId)
    toAppend.addClass('not-default')
    $('#clubtournament-container').append(toAppend)
  }

  function appendInfos(val) {
    var divSelect = '.clubtourn-display[tourn-id=' + val.TournamentId + ']'

    if (val.Comment == null) {
      val.Comment = "Kein Name"
    }

    $(divSelect + ' .name-label').text(val.Comment)
    $(divSelect + ' .date-label').text(val.Start)

    $.get(GetTournInfos, { tournId: val.TournamentId }).success(function (data) {      
      $(divSelect + ' .clubtourn-close input').prop('checked', data.Closed);
      $(divSelect + ' .clubtourn-link input').val(data.LinkUrl)
      $(divSelect + ' .clubtourn-comment textarea').val(data.TournComment)
    });
  }

  function intercSetup() {
    $('#clubtournament-container .not-default').remove();
    $.get(GetInterclub, { all: true }).success(function (data) {
      $.each(data, function (ind, val) {
        appendTourn(val)
        appendInfos(val)
      });
    }).done(function () {
      setupTournButtons();
    });
  }

  intercSetup();
});