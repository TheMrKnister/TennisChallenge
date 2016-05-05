TennisChallenge.InfoBoard.Ui = (function ($) {
    var model = TennisChallenge.InfoBoard.Model,
        utils = TennisChallenge.Utils,
        error = TennisChallenge.InfoBoard.Error,
        viewStates = { DEFAULT: "default", LOGIN: "login", BOOKING: "booking", COURT_DETAILS: "courtDetails" },
        courtIds = [],
        intervallID = null,
        DEFAULT_BLOCK_TIME = 45,
        // SASACOM za stimanje visinu
        PIXEL_PER_MINUTE = 0.66,
        dataUserId = "data-user-id",
        bookingRequestInfo = null,
        loginViewVisible = false,
        bookingViewVisible = false,
        assignRfidViewVisible = false,
        alreadyBookedViewVisible = false,
        rankingViewVisible = false,
        challengePlayerVisible = false,
        //IPA Relevant Begin
        adminMenuVisible = false,
        alreadyTournamentVisible = false,
        //IPA Relevant End
        demoIndex = 0,
        longestTimeSince = 0,
        NextAvailableH = 23,
        NextAvailableM = 59,
        ShowNextIsActive = false,
        rankedGames,
        rankedGameIndex = 0,

        getNumberOfCourts = function () { return courtIds.length; },
        attachEmailKeyboard = function (selector) {
            $(selector).keyboard({
                layout: 'e-mail-input',
                tabNavigation: true,
                usePreview: false,
                autoAccept: true,
                position: {
                    of: null,
                    my: "center top",
                    at: "center bottom",
                    at2: "center bottom",
                    offset: "0 80"
                }
            });
        },
        attachDefaultKeyboard = function (selector) {
            $(selector).keyboard({
                layout: 'german-qwertz-2',
                tabNavigation: true,
                usePreview: false,
                autoAccept: true,
                position: {
                    of: null,
                    my: "center top",
                    at: "center bottom",
                    at2: "center bottom",
                    offset: "0 21"
                }
            });
        },
        attachFrameKeyboard = function (selector) {
            $(selector).keyboard({
                layout: 'german-qwertz-2',
                tabNavigation: false,
                usePreview: false,
                autoAccept: true,
                position: {
                    of: null,
                    my: "top top",
                    at: "top center",
                    at2: "top top",
                }
            });
        },
        attachbookingSearchKeyboard = function () {
            $('#booking-view #member-list input').keyboard({
                layout: 'search',
                usePreview: false,
                autoAccept: true,
                position: {
                    of: null,
                    my: "right top",
                    at: "left top",
                    at2: "left top"
                },
                change: filterMemberList,
                canceled: filterMemberList
            });
        },
        showBookingView = function (roles) {
            var selectedCourt = courtIds[model.getStateData().selectedCourtNb];

            $('#booking-view #member-list input').val('');
            attachbookingSearchKeyboard();
            resetMemberList();
            loadMemberList(null);
            showAdminButton(roles);

            $('#window-overlay, #booking-view').show();

            if (selectedCourt !== null) {
                $('#' + selectedCourt + ' .tennis-court').css('z-index', 900);
                $('#' + selectedCourt + ' .booking-bar').css('z-index', 900);
            }

            bookingViewVisible = true;
        },
        hideBookingView = function () {
            $('#window-overlay, #booking-view').hide();

            $('#booking-view #member-list input').getkeyboard().destroy();

            $('.court-container .tennis-court').css('z-index', 'auto');
            $('.court-container .booking-bar').css('z-index', 'auto');

            bookingViewVisible = false;
        },
        showLoginView = function () {
            $('#login-view').show();
            $('#login-dialog input').val('');
            attachEmailKeyboard('#login-dialog input[name="email-address"]');
            attachDefaultKeyboard('#login-dialog input[name="password"]');
            loginViewVisible = true;
        },
        hideLoginView = function () {
            $('#login-view').hide();
            $('#login-dialog input[name="email-address"]').getkeyboard().destroy();
            $('#login-dialog input[name="password"]').getkeyboard().destroy();
            loginViewVisible = false;
        },
        showAssignRfidView = function () {
            $('#assign-rfid-view').show();
            $('#assign-rfid-dialog input').val('');
            attachEmailKeyboard('#assign-rfid-dialog input[name="rfid-email-address"]');
            attachDefaultKeyboard('#assign-rfid-dialog input[name="rfid-password"]');
            assignRfidViewVisible = true;
        },
        hideAssignRfidView = function () {
            $('#assign-rfid-view').hide();
            $('#assign-rfid-dialog input[name="rfid-email-address"]').getkeyboard().destroy();
            $('#assign-rfid-dialog input[name="rfid-password"]').getkeyboard().destroy();
            assignRfidViewVisible = false;
        },
        showAlreadyBookedView = function (message) {
            $('#already-booked-view').show();
            $('#already-booked-dialog p').text(message);
            alreadyBookedViewVisible = true;
        },
        hideAlreadyBookedView = function () {
            $('#already-booked-view').hide();
            $('#already-booked-dialog p').text('');
            alreadyBookedViewVisible = false;
        },
        showRankingView = function (gender) {
            fillRankedPlayerList(gender);
            fillRankedGames();
            hideAllRankedInfo();
            $('#ranking-list-wrap').show();
            $('#ranking-view').show();
            rankingViewVisible = true;
        },
        hideRankingView = function () {
            $('#ranking-view').hide();
            rankingViewVisible = false;
        },
        showChallengePlayer = function () {
            $('#challenge-view').show();
            challengePlayerVisible = true;
        },
        hideChallengePlayer = function () {
            $('#challenge-view').hide();
            challengePlayerVisible = false;
        },
        //IPA Relevant Begin 
        //shows admin Menu and sets the boolean which is used for hideAllViews
        showTournamentAdmin = function () {
            $('#tournament-admin-menu').show();
            adminMenuVisible = true;
        },
        //hides admin Menu and sets the boolean which is used for hideAllViews
        hideTournamentAdmin = function () {
            $('#tournament-admin-menu').hide();
            adminMenuVisible = false;
        },
        //shows the you are alredy in a tournament view and sets the boolean which is used for hideAllViews
        showAlreadyTournament = function () {
            $('#already-tournament-view').show();
            alreadyTournamentVisible = true;
        },
        //hides admin Menu and sets the boolean which is use for hideAllViews
        hideAlreadyTournament = function () {
            $('#already-tournament-view').hide();
            alreadyTournamentVisible = false;
        },
        //IPA Relevant End
        hideAllViews = function () {
            if (loginViewVisible)
                hideLoginView();
            if (bookingViewVisible)
                hideBookingView();
            if (assignRfidViewVisible)
                hideAssignRfidView();
            if (alreadyBookedViewVisible)
                hideAlreadyBookedView();
            if (rankingViewVisible)
                hideRankingView();
            if (challengePlayerVisible)
                hideChallengePlayer();
            //IPA relevant Begin
            if (adminMenuVisible)
                hideTournamentAdmin();
            if (alreadyTournamentVisible) {
                hideAlreadyTournament();
            }
            //IPA Relevant End
        },
        //SASACOM
         bookingLiReset = function (jQueryLi, height) {
             if (typeof height === 'undefined') {
                 height = DEFAULT_BLOCK_TIME * PIXEL_PER_MINUTE;
             }
             jQueryLi.removeClass("booked school tournament tournamentFun").addClass("free");
         },
        setAvilableTime = function (courtId, time) {
            var timeString = null;
            var selectorBase = '#' + courtId + ' .available-time';

            if (time.Days == 0) {
                timeString = time.Minutes + " Minuten";
                $(selectorBase + ' .time-declare').text('Ende letzter Reservation vor: ');
                $(selectorBase).addClass('time-since');

                if (time.Minutes >= longestTimeSince && ShowNextIsActive) {
                    $('.time-since').removeClass('next-court');
                    $(selectorBase).addClass('next-court');
                    longestTimeSince = time.Minutes;
                }
            } else {
                $(selectorBase).addClass('next-reservation');

                timeString = utils.toHhMmString(time);
                var nextTimeH = timeString.substr(0, 2);
                nextTimeH = parseInt(nextTimeH);

                var nextTimeM = timeString.substr(3, 5);
                nextTimeM = parseInt(nextTimeM);
                var isNext = false;

                if (nextTimeH <= NextAvailableH && ShowNextIsActive) {
                    if (nextTimeH == NextAvailableH) {
                        if (nextTimeM <= NextAvailableM) {
                            $('.available-time.next-reservation').removeClass('next-court');
                            isNext = true;
                        }
                    } else {
                        $('.available-time.next-reservation').removeClass('next-court');
                        isNext = true;
                    }
                }

                if (isNext) {
                    NextAvailableH = nextTimeH;
                    NextAvailableM = nextTimeM;

                    $(selectorBase).addClass('next-court');
                } else {
                    $(selectorBase).removeClass('next-court');
                }

                $(selectorBase + ' .time-declare').text('Nächste Reservationsmöglichkeit: ');
            }

            $(selectorBase + ' .time').text(timeString);
            $(selectorBase).show();
        },

        fillRankedPlayerList = function (gender) {

            $.getJSON(TennisChallenge.Constants.getRankedMember, { gender: gender, userName: "null" }, function () {
            }).success(function (data) {
                $('#ranking-list').children('li').remove();
                var members = data;
                updateRankedList(members);
            });
            $('ranking-list-wrap').show();
        },

        fillRankedGames = function () {
            $.getJSON(TennisChallenge.Constants.getRankedGames, function () { }).success(function (data) {
                rankedGames = data;
                updateRankedGames(rankedGames[0]);
            });
        },

        nextRankedGame = function () {
            rankedGameIndex++;
            if (rankedGameIndex >= rankedGames.length) {
                rankedGameIndex = 0;
            }
            updateRankedGames(rankedGames[rankedGameIndex]);
        },

        previousRankedGame = function () {
            rankedGameIndex--;
            if (rankedGameIndex < 0) {
                rankedGameIndex = rankedGames.length - 1;
            }
            updateRankedGames(rankedGames[(rankedGameIndex)]);
        },

        updateRankedGames = function (game) {
            $('.game-chall input').val(game.Challenger);
            $('.game-def input').val(game.Defender);
            $('.game-end input').val(game.End);
            $('.game-start input').val(game.Start);
            $('.game-rank input').val(game.ContestedRank);
            $('.game-court input').val(game.Court);
            $('#ranked-index').text((rankedGameIndex + 1) + "/" + rankedGames.length);
        },

        updateRankedList = function (members) {
            $.ajaxSetup({
                cache: false
            });
            $.each(members, function (_, member) {
                $('#ranking-list').append(
                  $("<li></li>")
                  .text(member.Rank + ". " + member.FullName)
                  );
            });
        },

        resetCourt = function (courtIndex) {
            $('.row-infos').remove();
            $('.available-time.next-reservation').removeClass('next-court');
            var courtId = courtIds[courtIndex];
            var selector = '#' + courtId + ' img.ongoing-game';
            $(selector).hide();

            $('#' + courtId + ' .caption').text("TENNISPLATZ #" + (courtIndex + 1));
            $('#' + courtId + ' .tennis-court').removeClass('school tournament tournamentFun locked');

            selector = '#' + courtId + ' ul.booking-bar';
            bookingLiReset($(selector + ' li'));
            $(selector).show();

            selector = '#' + courtId + ' .available-time';
            $(selector).hide();
        },

        courtIsEmpty = function (bookingInfo, bookingCount, now) {
            for (var i = 0; i < bookingCount; i++) {

                var currentStart = bookingInfo.Bookings[i].StartTime;
                currentStart = currentStart.substr(6, 13);
                currentStart = parseInt(currentStart);

                var currentEnd = bookingInfo.Bookings[i].EndTime;
                currentEnd = currentEnd.substr(6, 13);
                currentEnd = parseInt(currentEnd);

                if (currentStart <= now && currentEnd >= now) {
                    return false;
                }
            }
            return true;
        },
        determineBookingType = function (typeOfgame) {
            switch (typeOfgame) {
                case 0:
                    return "";
                    break;
                case 1:
                    return "GESPERT";
                    break;
                case 2:
                    return "Tennisschule";
                    break;
                case 3:
                    return "Interclub";
                    break;
                case 4:
                    return "Plauschturnier";
                    break;
            }
        },
        setTypeOfGame = function (booking, index, bookingTypeString, selectorBase) {
            if (booking.TypeOfGame != 0) {
                $(selectorBase + ' .row-' + index).append("<div class='gametype-infos'>" + bookingTypeString + "</div>");
                var width = $(selectorBase + ' .row-' + index + ' .player0-infos').width;
                $('.row-' + index + ' .gametype-infos').css("margin-left", (170 + width + 30) + "px");
                if (booking.CourtComment != null) {
                    $(selectorBase + ' .row-' + index + ' .player0-infos').text(booking.CourtComment);
                } else {
                    $(selectorBase + ' .row-' + index + ' .player0-infos').text(bookingTypeString);
                }
            }
        },
        showPastGames = function (courtId, courtSelector, counter) {
            $('#' + courtSelector + ' .show-booking-infos .row-infos').remove();
            $('#' + courtSelector + ' .present-button > button').show();
            $.get(TennisChallenge.Constants.getPastBookings, { courtId: courtId }).success(function (data) {

                if (parseInt(data.length / 6) == counter) {
                    $('#' + courtSelector + ' .past-button > button').hide();
                } else {
                    $('#' + courtSelector + ' .past-button > button').show();
                }

                $.each(data, function (ind, game) {
                    var start = utils.toHhMmString(utils.aspNetJsonToDate(game.StartTime)).substr(0, 5),
                      end = utils.toHhMmString(utils.aspNetJsonToDate(game.EndTime)).substr(0, 5),
                      gameType = determineBookingType(game.TypeOfGame),
                      prevPos = 0,
                      preInd = ind - 1;
                    if (ind > 0) {
                        prevPos = parseInt($('#' + courtSelector + ' .row-' + preInd).css("margin-top"));
                    } else {
                        prevPos = -(((data.length / 6 - 1) * 300) - (data.length % 6 * 50));
                        prevPos -= ((data.length % 6) * 50) - counter * 300;
                    }

                    $('#' + courtSelector + ' .show-booking-infos').append("<div class='row-" + ind + " row-infos'></div>");
                    $('#' + courtSelector + ' .row-' + ind).append("<div class='start-time-infos'>" + start + "</div><div class='start-time-uhr'> - </div>");
                    $('#' + courtSelector + ' .row-' + ind).append("<div class='end-time-infos'>" + end + "</div><div class='end-time-uhr'> Uhr </div>");
                    $('#' + courtSelector + ' .row-' + ind).append("<div class='player0-infos'>" + game.Player0Name + "</div>");
                    $('#' + courtSelector + ' .row-' + ind).append("<div class='player1-infos'>" + game.Player1Name + "</div>");
                    if (game.Player2Name != null) {
                        $('#' + courtSelector + ' .row-' + ind).append("<div class='player2-infos'>" + game.Player2Name + "</div>");
                    } if (game.Player3Name != null) {
                        $('#' + courtSelector + ' .row-' + ind).append("<div class='player3-infos'>" + game.Player3Name + "</div>");
                    }
                    setTypeOfGame(game, ind, gameType, "#" + courtSelector);
                    $('#' + courtSelector + ' .row-' + ind).css("margin-top", prevPos + 50 + "px");
                });
            });
        },
        //Schreibt die booking Informationen für jedes vorhandene booked li in der Leiste, in ein Infoblatt.
        applyLiInfos = function (courtId, bookings) {
            $('#' + courtId + ' li.booked').each(function (index, val) {
                var start = utils.toHhMmString(utils.aspNetJsonToDate(bookings[index].StartTime));
                var end = utils.toHhMmString(utils.aspNetJsonToDate(bookings[index].EndTime));
                var player0 = bookings[index].Player0Name;
                var player1 = bookings[index].Player1Name;
                var player2 = bookings[index].Player2Name;
                var player3 = bookings[index].Player3Name;
                var bookingTypeString = "";
                var selectorBase = '#' + courtId + ' .show-booking-infos';
                var pos = $(this).position();

                start = start.substr(0, 5);
                end = end.substr(0, 5);

                bookingTypeString = determineBookingType(bookings[index].TypeOfGame);
                $('#' + courtId + ' .row-' + index).remove();
                $(selectorBase).append("<div class='row-" + index + " row-infos'></div>");
                $(selectorBase + ' .row-' + index).append("<div class='start-time-infos'>" + start + "</div><div class='start-time-uhr'> - </div>");
                $(selectorBase + ' .row-' + index).append("<div class='end-time-infos'>" + end + "</div><div class='end-time-uhr'> Uhr </div>");
                $(selectorBase + ' .row-' + index).append("<div class='player0-infos'>" + player0 + "</div>");
                $(selectorBase + ' .row-' + index).append("<div class='player1-infos'>" + player1 + "</div>");
                $(selectorBase + ' .row-' + index).append("<div class='player2-infos'>" + player2 + "</div>");
                $(selectorBase + ' .row-' + index).append("<div class='player3-infos'>" + player3 + "</div>");
                setTypeOfGame(bookings[index], index, bookingTypeString, selectorBase);
                $('#' + courtId + ' .row-' + index).css("margin-top", (pos.top + 50) + "px");
                $('#' + courtId + ' .present-button > button').hide();
                $('#' + courtId + ' .past-button > button').show();
            });
        },

      applyBookingInfo = function (courtIndex, bookingInfo) {
          function checkName(name) {
              if (name == "") {
                  name = null;
              }
              return name;
          };

          var courtId = courtIds[courtIndex],
            selector,
            nameselector = '#' + courtId + ' .tennis-court .player-name',
            lastEnd = new Date(),
            currentStart,
            currentEnd,
            bookingCount,
            bookingIndex,
            bookings,
            booking = null;

          $(nameselector).text('');

          if (bookingInfo === null) {
              resetCourt(courtIndex);
          } else {
              bookingIndex = 0;
              bookingCount = bookingInfo.Bookings.length;

              var now = $.now();

              if (courtIsEmpty(bookingInfo, bookingCount, now)) {
                  resetCourt(courtIndex);
              }

              if (bookingInfo.Bookings[0].BookingType == 4) {
                  if (bookingInfo.Game1Player0Name == "Gast Spieler") {
                      bookingInfo.Game1Player0Name = "NULL";
                  }

                  bookingInfo.Game1Player0Name = checkName(bookingInfo.Game1Player0Name);
                  bookingInfo.Game1Player1Name = checkName(bookingInfo.Game1Player1Name);
                  bookingInfo.Game1Player2Name = checkName(bookingInfo.Game1Player2Name);
                  bookingInfo.Game1Player3Name = checkName(bookingInfo.Game1Player3Name);

                  $(nameselector + '.player-0').text(bookingInfo.Game1Player0Name);
                  $(nameselector + '.player-1').text(bookingInfo.Game1Player2Name);
                  $(nameselector + '.player-2').text(bookingInfo.Game1Player1Name);
                  $(nameselector + '.player-3').text(bookingInfo.Game1Player3Name);

                  $(nameselector).hide();
              }
              else if (bookingInfo.Game1Player0Name != null && bookingInfo.Game1Player1Name != null &&
                  bookingInfo.Game1Player2Name == null && bookingInfo.Game1Player3Name == null && !courtIsEmpty(bookingInfo, bookingCount, now)) {
                  $(nameselector + '.player-0').text(bookingInfo.Game1Player0Name);
                  $(nameselector + '.player-1').text(bookingInfo.Game1Player1Name);
              }
              else if (bookingInfo.Game1Player2Name != null && bookingInfo.Game1Player3Name != null && !courtIsEmpty(bookingInfo, bookingCount, now)) {
                  $(nameselector + '.player-0').text(bookingInfo.Game1Player0Name);
                  $(nameselector + '.player-1').text(bookingInfo.Game1Player2Name);
                  $(nameselector + '.player-2').text(bookingInfo.Game1Player1Name);
                  $(nameselector + '.player-3').text(bookingInfo.Game1Player3Name);
              }

              if (bookingCount > 0) {
                  booking = bookingInfo.Bookings[bookingIndex];
                  currentStart = utils.aspNetJsonToDate(booking.StartTime);
                  currentEnd = utils.aspNetJsonToDate(booking.EndTime);

                  if (bookingInfo.EndLastGame.Seconds > 0) {
                      setAvilableTime(courtId, bookingInfo.EndLastGame);
                  } else {
                      setAvilableTime(courtId, utils.aspNetJsonToDate(bookingInfo.NextStartTime));
                  }
              }

              selector = '#' + courtId + ' ul.booking-bar li';
              $(selector).each(function (liIndex) {
                  var liHeight,
                      tDiff;
                  while ((bookingIndex < bookingCount) && (lastEnd >= currentEnd)) {
                      ++bookingIndex;
                      if (bookingIndex < bookingCount) {
                          booking = bookingInfo.Bookings[bookingIndex];
                          currentStart = utils.aspNetJsonToDate(booking.StartTime);
                          currentEnd = utils.aspNetJsonToDate(booking.EndTime);
                      }
                  }

                  if (bookingIndex < bookingCount) {
                      tDiff = (currentStart - lastEnd) / 1000 / 60;
                      if (((liIndex === 0) && (tDiff < 5)) || (tDiff < DEFAULT_BLOCK_TIME)) {
                          if (liIndex === 0) {
                              booking = bookingInfo.Bookings[bookingIndex];
                              var court = '#' + courtId;
                              var bb = $(court + ' .booking-bar li:first-child');
                              switch (booking.BookingType) {
                                  case 0:
                                      $(court + ' .tennis-court').removeClass('school tournament tournamentFun locked');
                                      $(court + ' img.ongoing-game').show();
                                      $(court + ' .tennis-court .player-name').show();
                                      break;
                                  case 1:
                                      $(court + ' .tennis-court').removeClass('school tournament tournamentFun');
                                      $(court + ' .tennis-court').addClass('locked');
                                      $(court + ' .tennis-court').append('<div class="icon"></div>');
                                      $(court + ' .caption').text("GESPERRT");
                                      $(court + ' img.ongoing-game').hide();
                                      $(court + ' .booking-bar').hide();
                                      $(court + ' .tennis-court .player-name').hide();
                                      $(court + ' .available-time').hide();
                                      break;
                                  case 2:
                                      $(court + ' .tennis-court').removeClass('tournament tournamentFun locked');
                                      $(court + ' .tennis-court').addClass('school');
                                      $(bb).addClass('school');
                                      if (booking.CourtComment == null) {
                                          $(court + ' .caption').text("TENNISSCHULE");
                                      } else {
                                          $(court + ' .caption').text(booking.CourtComment);
                                      }
                                      $(court + ' img.ongoing-game').hide();
                                      $(court + ' .tennis-court .player-name').hide();
                                      $(court + ' .tennis-court').append('<div class="icon-school-1"></div>');
                                      $(court + ' .tennis-court').append('<div class="icon-school-2"></div>');
                                      break;
                                  case 3:
                                      $(court + ' .tennis-court').removeClass('school tournamentFun locked');
                                      $(court + ' .tennis-court').addClass('tournament');
                                      $(bb).addClass('tournament');
                                      if (booking.CourtComment == null) {
                                          $(court + ' .caption').text("INTERCLUB");
                                      } else {
                                          $(court + ' .caption').text(booking.CourtComment);
                                      }
                                      $(court + ' img.ongoing-game').hide();
                                      $(court + ' .tennis-court .player-name').hide();
                                      break;
                                  case 4:
                                      $(court + ' .tennis-court').removeClass('school tournament locked');
                                      $(court + ' .tennis-court').addClass('tournamentFun');
                                      $(bb).addClass('tournament');
                                      if (booking.CourtComment == null) {
                                          $(court + ' .caption').text("PLAUSCHTURNIER");
                                      } else {
                                          $(court + ' .caption').text(booking.CourtComment);
                                      }
                                      $(court + ' img.ongoing-game').hide();
                                      $(court + ' .tennis-court .player-name').show();
                                      //IPA Relevant Begin
                                      //switch (booking.TournamentMode) {
                                      //  case 0:
                                      //    booking.Player3 = "";
                                      //    booking.Player4 = "";
                                      //    break;
                                      //  case 1:
                                      //    if (booking.Player3 == '') {
                                      //      $(court + ' .tennis-court .player-name').hide()
                                      //    }
                                      //    break;
                                      //}                    
                                      //IPA Relevant End
                                      $(court + ' .tennis-court').append('<div class="icon"></div>');
                                      break;
                                  default:
                                      $(court + ' .tennis-court .player-name').hide();
                                      break;
                              }
                              //SASACOM

                              liHeight = Math.max((currentEnd - lastEnd) / 1000 / 60 * PIXEL_PER_MINUTE, 2);
                          } else {
                              liHeight = booking.Duration * PIXEL_PER_MINUTE;
                          }

                          switch (booking.BookingType) {
                              case 0:
                                  $(this).removeClass("free school tournament tournamentFun locked").addClass("booked").css("height", liHeight);
                                  break;
                              case 2:
                                  $(this).removeClass("free school tournament tournamentFun locked").addClass("booked school").css("height", liHeight);
                                  break;
                              case 3:
                                  $(this).removeClass("free school tournament tournamentFun locked").addClass("booked tournament").css("height", liHeight);
                                  break;
                              case 4:
                                  $(this).removeClass("free school tournament tournamentFun locked").addClass("booked tournament").css("height", liHeight);
                                  break;
                          }

                          $('#' + courtId + ' .booking-bar').click(function () {
                              $('#close-click-helper').remove();
                              $('#' + courtId + ' .show-booking-infos').show();
                              $('.page').append("<div id='close-click-helper'></div>");
                              setTimeout(function () { $('#' + courtId + ' .show-booking-infos').hide(); $('#close-click-helper').remove(); }, 1000000);
                              $('#close-click-helper').click(function () {
                                  $('#' + courtId + ' .show-booking-infos').hide();
                                  $('#close-click-helper').remove();
                                  refreshBookings();
                                  $('.past-button button').each(function () {
                                      $(this).data('count', 0);
                                  });
                              });
                              //setTimeout(function () {
                              //  $('#' + courtId + ' .show-booking-infos').hide();
                              //}, 5000);
                          });

                          lastEnd = currentEnd;
                      } else {
                          tDiff = (currentStart - lastEnd) / 1000 / 60;

                          if (tDiff > DEFAULT_BLOCK_TIME) {
                              bookingLiReset($(this));
                              tDiff = DEFAULT_BLOCK_TIME;
                          } else {
                              liHeight = tDiff * PIXEL_PER_MINUTE;
                              bookingLiReset($(this), liHeight);
                              tDiff += 2;
                          }

                          if (liIndex === 0) {
                              $('#' + courtId + ' img.ongoing-game').hide();
                          }

                          lastEnd.setMinutes(lastEnd.getMinutes() + tDiff);
                      }
                  } else {
                      bookingLiReset($(this));

                      if (liIndex === 0) {
                          $('#' + courtId + ' img.ongoing-game').hide();
                      }
                  }
              });
          }
          $.ajaxSetup({
              cache: false
          });
          $.getJSON(TennisChallenge.Constants.getNextSeven, { courtIdBook: courtIndex }, function () {
          }).success(function (data) {
              bookings = data;
              applyLiInfos("court_" + courtIndex, bookings);
          });
      };

    function init() {
        model.setStateChangedCallback(modelStateChanged);
        model.setRfidCall(selectPlayerById);

        $('.court-container').each(function () {
            var id = this.id;
            var index = Number(id.substr(6));
            courtIds[index] = id;
        });

        $('#ranked-men').click(function () {
            showRankingView(2);
        });

        $('#ranked-women').click(function () {
            showRankingView(1);
        });

        $('#close-ranked').click(function () {
            $('#ranking-view').hide();
        });

        $('#next').click(function () {
            nextRankedGame();
        });

        $('#previous').click(function () {
            previousRankedGame();
        });

        $('.past-button > button').click(function () {
            var courtId = $(this).attr('data-courtid'),
             courtSelector = $(this).parent().parent().parent().attr('id'),
             counter = $(this).data('count');
            showPastGames(courtId, courtSelector, counter);
            $(this).data().count++;
        });

        $('.present-button > button').click(function () {
            var pastButton = $(this).parent().parent().find('.past-button > button');

            var courtId = pastButton.attr('data-courtid'),
             courtSelector = $(this).parent().parent().parent().attr('id'),
             counter = pastButton.data('count');
            if (counter == 1) {
                refreshBookings();
            } else {
                showPastGames(courtId, courtSelector, counter - 2);
            }
            pastButton.data().count--;
        });

        $('.court-container .tennis-court').click(function () {
            var courtId = $(this).parent('.court-container').attr('id'),
                courtNb = Number(courtId.substr(6));

            //5. court is made available, but just commented out, in case it gets used later. 

            //if (courtId === 'court_4') {
            //demo();
            //} else {
            model.courtClicked(courtNb);
            //}
        });

        $('.challenge-player').click(function () {
            model.challengePlayerClick();
        });

        //IPA Relevant Begin
        //the data-id's are the tournamentId
        $('.admin-button').click(function () {
            model.verifyAdminClick($(this).data('data-id'));
        });
        $('.add-to-game').click(function () {
            model.addToGameClick($(this).data('data-id'));
        });

        $('#cancel-tournament').click(function () {
            model.removeFromGameClick();
        });
        //IPA Relevant End

        $('.ranked-games').click(function () {
            hideAllRankedInfo();
            model.viewRankedGamesClick();
        });

        $('#club_site_map #mitspielen').click(function () {
            model.courtClicked(1, true);
        });

        $('#login-view #login-dialog button[name="cancel"]').click(model.cancel);
        $('#back-ranked').click(model.cancel);

        $("#cancel-guest").click(function () {
            $("#guest-cost").hide();
        });

        $("#ok-guest").click(function () {
            var listItem = $("#guest-in-list");
            selectPlayer(listItem);
            $("#guest-cost").hide();
            //$.post(TennisChallenge.Constants.AddToBank, { memberId: $("#guest-cost").data("member-id") });
            //alert($("#guest-cost").data("member-id"))
        });

        //IPA Relevant begin
        $('#cancel-tournament-view').click(model.cancel);

        //sets the loginClick and checks which type of login it is, and executes the respective function
        $('#login-button.normal').click(function () {
            var userName = $('#login-view #login-dialog input[name="email-address"]').val(),
                password = $('#login-view #login-dialog input[name="password"]').val(),
                isRanked = $('#login-button').hasClass('ranked'),
                isVerifyAdmin = $('#login-button').hasClass('verify-admin'),
                isTournament = $('#login-button').hasClass('add-to-tourn');

            if (!isRanked && !isVerifyAdmin && !isTournament) {
                model.login(userName, password);
            }
        });
        //sets the loginClick and checks which type of login it is, and executes the respective function
        $('#login-button.ranked').click(function () {
            var userName = $('#login-view #login-dialog input[name="email-address"]').val(),
                password = $('#login-view #login-dialog input[name="password"]').val(),
                isNormal = $('#login-button').hasClass('normal'),
                isVerifyAdmin = $('#login-button').hasClass('verify-admin'),
                isTournament = $('#login-button').hasClass('add-to-tourn');

            if (!isNormal && !isVerifyAdmin && !isTournament) {
                model.loginRanked(userName, password);
            }
        });
        //sets the loginClick and checks which type of login it is, and executes the respective function
        $('#login-button.verify-admin').click(function () {
            var userName = $('#login-view #login-dialog input[name="email-address"]').val(),
                password = $('#login-view #login-dialog input[name="password"]').val(),
                isNormal = $('#login-button').hasClass('normal'),
                isRanked = $('#login-button').hasClass('ranked'),
                isTournament = $('#login-button').hasClass('add-to-tourn');

            if (!isNormal && !isRanked && !isTournament) {
                model.loginAdmin(userName, password);
            }
        });
        //sets the loginClick and checks which type of login it is, and executes the respective function
        $('#login-button.add-to-tourn').click(function () {
            var userName = $('#login-view #login-dialog input[name="email-address"]').val(),
                password = $('#login-view #login-dialog input[name="password"]').val(),
                tournId = $('#login-button').data('data-id'),
                isNormal = $('#login-button').hasClass('normal'),
                isRanked = $('#login-button').hasClass('ranked'),
                isVerifyAdmin = $('#login-button').hasClass('verify-admin');

            $('#cancel-tournament').data('username', userName);
            $('#cancel-tournament').data('tourId', tournId);

            if (!isNormal && !isRanked && !isVerifyAdmin) {
                model.loginTournament(userName, password, tournId);
            }
        });
        //IPA Relevant End

        $('#assign-rfid-dialog button[name="cancel"]').click(model.cancel);

        $('#assign-rfid-dialog button[name="login"]').click(function () {
            var userName = $('#assign-rfid-dialog input[name="rfid-email-address"]').val(),
                password = $('#assign-rfid-dialog input[name="rfid-password"]').val();

            model.assignRfid(userName, password);
        });

        $('#already-booked-dialog button[name="cancel"]').click(model.cancel);
        $('#already-booked-dialog button[name="cancelBooking"]').click(model.cancelBooking);

        $('#booking-dialog .single-double').click(singleDoubleClicked);
        $('#booking-dialog #player-2, #booking-dialog #player-3, #booking-dialog #player-4').click(removePlayer);

        $('#booking-view #footer button[name="cancel"]').click(model.cancel);

        $('#booking-view #footer button[name="book"]').click(bookTennisCourt);

        intervallID = setInterval(refreshBookings, 60000);
        refreshBookings();

        $.keyboard.keyaction.enter = function (base) {

            if (base.preview.name === "password") {
                var userName = $('#login-view #login-dialog input[name="email-address"]').val(),
                    password = $('#login-view #login-dialog input[name="password"]').val(),
                    tournId = $('#login-button').data('data-id');

                if (($('#login-button').hasClass('ranked'))) {
                    model.loginRanked(userName, password);
                } else if (($('#login-button').hasClass('normal'))) {
                    model.login(userName, password);
                    //IPA Relevant Begin
                    //checks if enter was pressed in the verifyAdmin login
                } else if (($('#login-button').hasClass('verify-admin'))) {
                    model.loginAdmin(userName, password);
                    //checks if enter was pressed in the add-to-tourn login
                } else if (($('#login-button').hasClass('add-to-tourn'))) {
                    $('#cancel-tournament').data('username', userName);
                    $('#cancel-tournament').data('tourId', tournId);
                    model.loginTournament(userName, password, tournId);
                    //IPA relevant End
                }
            }
            if (base.preview.name === "rfid-password") {
                var userName = $('#assign-rfid-dialog input[name="rfid-email-address"]').val(),
                  password = $('#assign-rfid-dialog input[name="rfid-password"]').val();

                model.assignRfid(userName, password);
            }
        };

        //If there is a cached booking, submit the booking
        var bookingRequest = model.getCachedBookingRequest();
        if (bookingRequest != null) {
            model.changeState(model.STATES.BOOKING);
            model.retryBookingRequest();
        }
    }

    function hideAllRankedInfo() {
        $('#ranking-list-wrap').hide();
        $('#ranked-games-view').hide();
    }

    function demo() {
        var court = $('#court_4 .tennis-court'),
          bb = $('#court_4 .booking-bar li:first-child');

        demoIndex = (demoIndex + 1) % 5;

        court.removeClass('closed school tournament');
        bb.removeClass('free booked school tournament');

        switch (demoIndex) {
            case 0:
                court.find('.caption').text('Tennisplatz #5');
                bb.addClass('free');
                break;
            case 1:
                court.addClass("closed");
                court.find('.caption').text('Gesperrt');
                bb.addClass('free');
                break;
            case 2:
                court.addClass("school");
                court.find('.caption').text('Tennisschule');
                bb.addClass('school');
                break;
            case 3:
                court.addClass("tournament");
                court.find('.caption').text('Interclub');
                bb.addClass('tournament');
                break;
            case 4:
                court.addClass("tournament");
                court.find('.caption').text('Plauschturnier');
                bb.addClass('tournament');
                break;
            default:
        }
    }

    function modelStateChanged(oldState, newState, stateData) {
        var messageText, openBooking;

        if (stateData.bookingMember == null) {
            stateData.bookingMember = model.getCachedBookingMember();
        }

        switch (newState) {
            case model.STATES.DEFAULT:
                hideAllViews();

                if (oldState === model.STATES.BOOKING || oldState === model.STATES.ALREADY_BOOKED)
                    refreshBookings();
                break;
            case model.STATES.LOGIN:
                hideAllViews();
                showLoginView();
                break;
            case model.STATES.LOGIN_ERROR:
            case model.STATES.OPEN_BOOKING_ERROR:
            case model.STATES.BOOKING_ERROR:
            case model.STATES.ASSIGN_ERROR:
                error.showErrorDialog(stateData.errorMessage, function () {
                    model.next();
                });
                break;
            case model.STATES.CHALLENGE_PLAYER:
                hideAllViews();
                showChallengePlayer();
                break;
                //IPA Relevant Begin
                //displays 
            case model.STATES.TOURNAMENT_ADMIN:
                hideAllViews();
                showTournamentAdmin();
                break;
            case model.STATES.HIDE_LOGIN:
                hideAllViews();
                break;
            case model.STATES.ALREADY_TOURNAMENT:
                hideAllViews();
                showAlreadyTournament();
                break;
                //IPA Relevent End
            case model.STATES.RANKED_VIEW:
                hideAllViews();
                showRankingView;
                break;
            case model.STATES.BOOKING:
                hideAllViews();

                bookingRequestInfo = stateData.bookingInfo;

                matchConfigurationReset();
                fillPlayerbox('player-1', stateData.bookingMember.Id, stateData.bookingMember.FullName,
                  stateData.bookingMember.Classification, stateData.bookingMember.Club, stateData.bookingMember.PictureUrl);
                setBookingInfo();

                $("#guest-cost").data("member-id", stateData.bookingMember.Id);
                showBookingView(stateData.bookingMember.Roles);
                break;
            case model.STATES.ASSIGN_RFID:
                if (loginViewVisible)
                    hideLoginView();
                showAssignRfidView();
                break;
            case model.STATES.ALREADY_BOOKED:
                hideAllViews();

                openBooking = stateData.bookingMember.openBooking;
                messageText = "Du bist bereits auf " + openBooking.CourtName + ' für ein Spiel von ' +
                  utils.toHhMmString(utils.aspNetJsonToDate(openBooking.StartTime)) + ' Uhr bis ' +
                  utils.toHhMmString(utils.aspNetJsonToDate(openBooking.EndTime)) + ' Uhr registriert.';
                showAlreadyBookedView(messageText);
                break;
            default:
        }
    }

    function showIframe(targetUrl) {
        $('#booking-dialog').append('<div id="iframe"><iframe id="frame" scrolling="no" src="' + targetUrl + '"></iframe></div>');
        $('#rightsList ul').css('visibility', 'hidden');
        $('#rightsList ul').removeClass('toggle');
        $('#frame').load(function () {
            $('#frame').contents().find('#back-button').remove();
            $('#frame').contents().find('input[name=open-create-booking-series]').remove();
            $('#frame').contents().find('#booking-series-links').remove();
            $('#frame').contents().find('#add-to-ranked').remove();
            //attachFrameKeyboard($('#frame').contents().find('.editor-field #Title'));
            $('#iframe').append('<input class="closeIframe" type="button" value="Schliessen" />');
            //$('#iframe').append('<input class="execute" type="button" value="Abschliessen" />');
            $('.execute').click(model.cancel);
            $('.closeIframe').on('click', function () {
                $('#iframe').remove();
            });
        });
    }

    function showAdminButton(roles) {
        if (roles.length != 0) {
            $('#match-configuration').append('<div id="adminButton"><input id="admin" type="button" name="adminButton"><div id="rightsList"><ul></ul></div></div>');
            if (($.inArray("tennisteacher", roles)) != -1) {
                $('#adminButton ul').append('<li id="platzReservieren"><button type="button">Tennislehrer</button></li>');
            }
            if (($.inArray("janitor", roles)) != -1) {
                $('#adminButton ul').append('<li id="platzSperren"><button type="button">Platz sperren</button></li>');
            }
            if (($.inArray("intercluborganizer", roles)) != -1) {
                $('#adminButton ul').append('<li id="interclub"><button type="button">Interclub</button></li>');
            }
            if (($.inArray("casualtournamentorganizer", roles)) != -1) {
                $('#adminButton ul').append('<li id="turnier"><button type="button">Turnier</button></li>');
            }

            $('#adminButton #admin').click(function () {
                if ($('#rightsList ul').hasClass('toggle')) {
                    $('#rightsList ul').css('visibility', 'hidden');
                    $('#rightsList ul').removeClass('toggle');
                }
                else {
                    $('#rightsList ul').css('visibility', 'visible');
                    $('#rightsList ul').addClass('toggle');
                }
            });

            $('#rightsList li').each(function (index) {
                $(this).click(function () {
                    var adminUrl = TennisChallenge.Constants.adminUrl;
                    switch (this.id) {
                        case 'platzReservieren':
                            showIframe(adminUrl + "/CreateTeacherBooking");
                            break;
                        case 'platzSperren':
                            showIframe(adminUrl + "/EditCourtLockings");
                            break;
                        case 'interclub':
                            showIframe(adminUrl + "/CreateInterclubBooking");
                            break;
                        case 'turnier':
                            showIframe(adminUrl + "/CreateTournamentBooking");
                    }
                });
            });
        }
    }

    function resetMemberList() {
        var list = $('#booking-view #member-list ul');
        list.empty();
        list.append('<li id="guest-in-list" data-user-id="' + TennisChallenge.Constants.guestGuid +
          '" data-user-info="null#Kein Club#null">Gast Spieler</li>');
        return list;
    }

    //Set the cached booking request info (in case a retry is needed)
    function setCachedMemberList(memberList) {
        localStorage["memberList"] = JSON.stringify(memberList);
    }

    function getCachedMemberList() {
        var memberList = localStorage.getItem("memberList");
        if (memberList != null) {
            memberList = JSON.parse(memberList, JSON.dateParser);
            return memberList;
        }
        return null;
    }

    function loadMemberList(filter) {
        if (filter != null)
            filter = { filter: filter };

        $.getJSON(TennisChallenge.Constants.memberListUrl, filter, function (data) {
            setCachedMemberList(data);
            presentMemberList(data);
        })
        .error(function () {
            var data = getCachedMemberList();
            if (data != null) {
                presentMemberList(data);
            }
        });
    }

    function presentMemberList(data) {
        var list = resetMemberList();

        $.each(data, function (index, item) {
            var classes = "";

            if (item.HasOpenBooking) {
                classes = ' class="selected" ';
            }
            list.append('<li data-user-id="' + item.Id + '" data-user-info="' +
              item.Classification + '#TC Thalwil#' + item.PictureUrl + '"' +
              classes + '>' + item.FullName + '</li>');
        });

        $('#booking-view #member-list li').click(function () {
            if ($(this).attr("data-user-id") == "94e13f9a-823d-4217-932d-c57465cbacb4") {
                $("div#guest-cost").show();
            } else {
                selectPlayer($(this));
            }
        });

        markSelectedInMemberList();
    }

    function markSelectedInMemberList() {
        var selectedIds = [],
            userId;

        $('#booking-dialog .player-box').each(function () {
            userId = $(this).attr(dataUserId);
            if (userId != undefined && userId !== TennisChallenge.Constants.guestGuid) {
                selectedIds.push(userId);
            }
        });

        $('#booking-view #member-list li').each(function () {
            var $this = $(this),
                selected = false;

            userId = $this.attr(dataUserId);

            for (var i = 0; i < selectedIds.length; i++) {
                if (selectedIds[i] === userId) {
                    selected = true;
                    break;
                }
            }

            if (selected) {
                $this.addClass("selected");
            }
        });
    }

    function filterMemberList() {
        loadMemberList($('#booking-view #member-list input').val());
    }

    function matchConfigurationReset() {
        playerBoxClear('player-1');
        playerBoxClear('player-2');
        playerBoxClear('player-3');
        playerBoxClear('player-4');

        updateSingleDouble();

        highlightNextSelectable();
    }

    function fillPlayerbox(id, userId, name, classification, club, picture) {

        var divSelector = "#booking-dialog #" + id,
        playerDiv = $(divSelector);

        playerDiv.removeClass("highlight");
        playerDiv.removeClass("empty");

        if (userId === null) {
            playerDiv.removeAttr(dataUserId);
        } else {
            playerDiv.attr(dataUserId, userId);
        }
        $(divSelector + ' h4').text(name);

        if (!(picture === "" || picture === null || picture === "null")) {

            switch (id) {
                case "player-1":
                    $("#bild1").attr("src", TennisChallenge.Constants.uploadImagePath + picture);
                    break;
                case "player-2":
                    $("#bild2").attr("src", TennisChallenge.Constants.uploadImagePath + picture);
                    break;
                case "player-3":
                    $("#bild3").attr("src", TennisChallenge.Constants.uploadImagePath + picture);
                    break;
                case "player-4":
                    $("#bild4").attr("src", TennisChallenge.Constants.uploadImagePath + picture);
                    break;
            }
        }

        if (picture == "" || picture === null || picture === "null") {

            switch (id) {
                case "player-1":
                    $("#bild1").attr("src", TennisChallenge.Constants.defaultPlayerImage);
                    break;
                case "player-2":
                    $("#bild2").attr("src", TennisChallenge.Constants.defaultPlayerImage);
                    break;
                case "player-3":
                    $("#bild3").attr("src", TennisChallenge.Constants.defaultPlayerImage);
                    break;
                case "player-4":
                    $("#bild4").attr("src", TennisChallenge.Constants.defaultPlayerImage);
                    break;
            }
        }

        if (classification === null || classification === 'null')
            classification = '';

        playerDiv.find('span.classification').text(classification);
        playerDiv.find('span.club').text(club);

        return playerDiv;
    }

    function playerBoxClear(id) {
        var userId = $('#booking-dialog #' + id).attr(dataUserId),
            playerDiv = fillPlayerbox(id, null, '', '', '', '');

        if (userId != undefined) {
            $('#booking-dialog #member-list li[data-user-id="' + userId + '"]').removeClass('selected');
        }

        playerDiv.addClass('empty');
    }

    function updateSingleDouble() {
        if (bookingRequestInfo == null) {
            bookingRequestInfo = getCachedBookingRequestInfo();
        }

        if (bookingRequestInfo.isSingle) {
            $('#booking-dialog .single-double').addClass('double');
            $('#booking-dialog #player-3').hide();
            $('#booking-dialog #player-4').hide();
            $('#booking-dialog .number.two').hide();
            playerBoxClear('player-3');
            playerBoxClear('player-4');

            if ($('#booking-dialog #player-2').hasClass('empty')) {
                $('#booking-dialog .single-double').hide();
            } else {
                $('#booking-dialog .single-double').show();
            }
        } else {
            $('#booking-dialog .single-double').removeClass('double');
            $('#booking-dialog #player-3').show();
            $('#booking-dialog #player-4').show();
            $('#booking-dialog .number.two').show();
        }
    }

    function getNextSelectable() {
        if ($('#booking-dialog #player-2').hasClass('empty')) {
            return 'player-2';
        }

        if (bookingRequestInfo.isSingle) {
            return null;
        }

        if ($('#booking-dialog #player-3').hasClass('empty')) {
            return 'player-3';
        }
        if ($('#booking-dialog #player-4').hasClass('empty')) {
            return 'player-4';
        }
        return null;
    }

    function highlightNextSelectable() {
        $('#booking-dialog .player-box').removeClass('highlight');

        var nextSelectable = getNextSelectable();

        if (nextSelectable !== null) {
            $('#booking-dialog #' + nextSelectable).addClass('highlight');
        }
    }

    function checkTime(hours, minutes) {
        var minutes = 5 * Math.round(minutes / 5);

        if (minutes == 60) {
            minutes = 0;
            hours++;
        }

        if (minutes < 10)
            minutes = "0" + minutes;
        if (hours < 10)
            hours = "0" + hours;
        var time = hours + ":" + minutes + " Uhr";

        return time;
    }

    function setBookingInfo() {
        if (!bookingRequestInfo.singleOffer) {
            var duration = 0;
            var startTime = "GESPERRT";
            var endTime = "GESPERRT";
        } else {

            try {
                var duration = bookingRequestInfo.isSingle ? bookingRequestInfo.singleOffer.duration : bookingRequestInfo.doubleOffer.duration,
                startTimeCom = bookingRequestInfo.isSingle ? bookingRequestInfo.singleOffer.startTime : bookingRequestInfo.doubleOffer.startTime,
                endTimeCom = bookingRequestInfo.isSingle ? bookingRequestInfo.singleOffer.endTime : bookingRequestInfo.doubleOffer.endTime,
                startTimeH = startTimeCom.getHours(),
                startTimeM = startTimeCom.getMinutes(),
                endTimeH = endTimeCom.getHours(),
                endTimeM = endTimeCom.getMinutes();

                var endTime = checkTime(endTimeH, endTimeM);
                var startTime = checkTime(startTimeH, startTimeM);
                duration = 5 * Math.round(duration / 5);

                var infoText = 'Ab: ' + startTime + '<br />' +
                  'Ende: ' + endTime + '<br />' +
                  'Dauer: ' + duration + ' Minuten';

                $('#booking-dialog #booking-info span').html(infoText);
            } catch (error) {
                //Error handling here
            }
        }
    }

    //Set the cached booking request info (in case a retry is needed)
    function setCachedBookingRequestInfo(bookingRequestInfo) {
        localStorage["bookingRequestInfo"] = JSON.stringify(bookingRequestInfo);
    }

    function getCachedBookingRequestInfo() {
        var cachedBookingRequestInfo = localStorage.getItem("bookingRequestInfo");
        if (cachedBookingRequestInfo != null) {
            cachedBookingRequestInfo = JSON.parse(cachedBookingRequestInfo, JSON.dateParser);
            return cachedBookingRequestInfo;
        }
        return null;
    }

    function bookTennisCourt() {
        var member0 = $('#booking-dialog div#player-1').attr('data-user-id'),
            member1 = $('#booking-dialog div#player-2').attr('data-user-id'),
            member2 = null, member3 = null;

        if (member0 == undefined || member1 == undefined)
            return;

        if (!bookingRequestInfo.isSingle) {
            member2 = $('#booking-dialog div#player-3').attr('data-user-id');
            member3 = $('#booking-dialog div#player-4').attr('data-user-id');

            if (member2 == undefined || member3 == undefined)
                return;
        }

        setCachedBookingRequestInfo(bookingRequestInfo);
        model.sendBookingRequest({
            StartTime: bookingRequestInfo.isSingle ? bookingRequestInfo.singleOffer.startTime.toISOString() : bookingRequestInfo.doubleOffer.startTime.toISOString(),
            EndTime: bookingRequestInfo.isSingle ? bookingRequestInfo.singleOffer.endTime.toISOString() : bookingRequestInfo.doubleOffer.endTime.toISOString(),
            CourtId: bookingRequestInfo.courtId,
            Member0Fk: member0,
            Member1Fk: member1,
            Member2Fk: member2,
            Member3Fk: member3,
            BookingType: 0
        });

        $('.player-box[data-user-id="94e13f9a-823d-4217-932d-c57465cbacb4"]').each(function data() {
            $.post(TennisChallenge.Constants.AddToBank, { memberId: $("#guest-cost").data("member-id") });
        });
    }

    function selectPlayerById(id) {
        var listItem = $('#booking-dialog #member-list li[data-user-id="' + id.userId + '"]');
        selectPlayer(listItem);
    }

    function selectPlayer(listItem) {
        var $this = listItem,
        targetId = getNextSelectable(),
        userId, name, infos;

        if (!$this.hasClass('selected') && targetId !== null) {
            userId = $this.attr(dataUserId);
            name = $this.text();
            infos = $this.attr('data-user-info').split('#');

            if (userId !== TennisChallenge.Constants.guestGuid)
                $this.addClass('selected');

            fillPlayerbox(targetId, userId, name, infos[0], infos[1], infos[2]);

            updateSingleDouble();
            highlightNextSelectable();
        }
    }

    function removePlayer() {
        var $this = $(this),
            userid = $this.attr(dataUserId);

        if (userid != undefined) {
            playerBoxClear(this.id);
            updateSingleDouble();
            highlightNextSelectable();
        }
    }

    function singleDoubleClicked() {
        var duration, endTime;

        if (bookingRequestInfo == null) {
            return;
        }

        if (bookingRequestInfo.isSingle) {
            bookingRequestInfo.isSingle = false;
            duration = bookingRequestInfo.doubleOffer.duration;
        } else {
            bookingRequestInfo.isSingle = true;
            duration = bookingRequestInfo.singleOffer.duration;
        }

        endTime = new Date(bookingRequestInfo.isSingle ? bookingRequestInfo.singleOffer.startTime : bookingRequestInfo.doubleOffer.startTime);
        var startTime = bookingRequestInfo.isSingle ? bookingRequestInfo.singleOffer.startTime : bookingRequestInfo.doubleOffer.startTime;
        endTime.setMinutes(startTime.getMinutes() + duration);
        if (bookingRequestInfo.isSingle) {
            bookingRequestInfo.singleOffer.endTime = endTime;
        }
        else {
            bookingRequestInfo.doubleOffer.endTime = endTime;
        }

        updateSingleDouble();
        markSelectedInMemberList();
        setBookingInfo();
        highlightNextSelectable();
    }

    function refreshBookings() {
        var now = new Date();
        tournamentSetup();
        clubtournSetup();
        $.getJSON(TennisChallenge.Constants.getCurrentBookingsUrl, function (data) {
            var nbOfCourts = getNumberOfCourts(),
                nbOfBookingInfos = data.length,
                bookingInfo;

            for (var i = 0; i < nbOfCourts; i++) {    // -1 nur für Demo -> unterbindet das Update von Court 5
                bookingInfo = null;

                // Is there information for this court?
                for (var j = 0; j < nbOfBookingInfos; j++) {
                    if (data[j].CourtId === i) {
                        bookingInfo = data[j];
                        break;
                    }
                }
                applyBookingInfo(i, bookingInfo);
            }
        });
    }

    $(function () {
        init();
    });

    return {
        VIEW_STATES: viewStates,
        getNumberOfCourts: getNumberOfCourts,
        showBookingView: showBookingView,
    };
}(jQuery));