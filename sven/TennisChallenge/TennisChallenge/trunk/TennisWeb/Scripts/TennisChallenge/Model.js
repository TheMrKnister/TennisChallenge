TennisChallenge.InfoBoard.Model = (function ($) {
    var utils = TennisChallenge.Utils,
        error = TennisChallenge.InfoBoard.Error,
        state = 0,
        stateData = null,
        stateChangedCallback = null, // stateChangedCallback(oldState, newState, stateData);
        rfidCall = null,
        tournamentDemo = false,

        getState = function () {
            return state;
        },
        getStateData = function () {
            return stateData;
        },
        setCachedStateData = function (stateData) {
            localStorage["stateData"] = JSON.stringify(stateData);
        },
        setCachedUserInfo = function (userInfo) {
            localStorage["userInfo"] = JSON.stringify(userInfo);
        },
        getCachedUserInfo = function () {
            var userInfo = localStorage.getItem("userInfo");
            if (userInfo != null) {
                userInfo = JSON.parse(userInfo, JSON.dateParser);
                return userInfo;
            }
            return null;
        },
        getCachedStateData = function () {
            var stateData = localStorage.getItem("stateData");
            if (stateData != null) {
                stateData = JSON.parse(stateData, JSON.dateParser);
                return stateData;
            }
            return null;
        },
        setStateChangedCallback = function (callback) {
            if (typeof callback === "function") {
                stateChangedCallback = callback;
            }
        },
        setRfidCall = function (callback) {
            if (typeof callback === "function") {
                rfidCall = callback;
            }
        },
        reset = function () {
            stateData = {
                selectedCourtNb: null,
                bookingMember: null,
                errorMessage: null,
                rfidToAssign: null,
                bookingInfo: null
            };
        },
        changeState = function (newState) {
            var oldState = state;
            state = newState;

            if (newState === utils.utils.states.DEFAULT) {
                reset();
            }

            if (stateChangedCallback != null) {
                stateChangedCallback(oldState, state, stateData);
            }
        },
        onVerfiyError = function () {
            stateData.errorMessage = "Die Anmeldung war leider nicht erfolgreich. Bitte versuche nochmals.";
            changeState(utils.states.LOGIN_ERROR);
        },
        cancel = function () {
            if (state === utils.utils.states.LOGIN) {
                changeState(utils.states.DEFAULT);
                $.post(TennisChallenge.Constants.swipeTeams);
            } else if (state === utils.states.ALREADY_BOOKED ||
                //IPA Relevant Beginn added utils.states ALREADY_TOURNAMENT an TOURNAMENT_ADMIN to ifs
                state === utils.states.ASSIGN_RFID ||
                state === utils.states.ALREADY_TOURNAMENT) {
                changeState(utils.states.DEFAULT);
            } else if (state === utils.states.BOOKING ||
                state === utils.states.CHALLENGE_PLAYER ||
                state === utils.states.TOURNAMENT_ADMIN) {
                //IPA Relevant End
                changeState(utils.states.DEFAULT);
                //Commented out code to log off when cancelling a booking
                //var url = TennisChallenge.Constants.InfoBoardLogOff;
                //window.location.href = url;
            }

        },
        next = function () {
            if (state === utils.states.LOGIN_ERROR) {
                changeState(utils.states.LOGIN);
            } else if (state === utils.states.ASSIGN_ERROR) {
                changeState(utils.states.ASSIGN_RFID);
            } else if (state === utils.states.BOOKING_ERROR || state === utils.states.OPEN_BOOKING_ERROR) {
                changeState(utils.states.DEFAULT);
            }
        },
        courtClicked = function (courtNumber, settournamentDemo) {
            if (state !== utils.states.DEFAULT)
                return;

            tournamentDemo = settournamentDemo;

            stateData.selectedCourtNb = courtNumber;
            $("#login-button").removeClass("ranked verify-admin add-to-tourn");
            $("#login-button").addClass("normal");
            changeState(utils.states.LOGIN);
        },
        challengePlayerClick = function () {
            $("#login-button").removeClass("normal verify-admin add-to-tourn");
            $("#login-button").addClass("ranked");
            changeState(utils.states.LOGIN);
        },
        //IPA relevant Begin
        //sets the rights classes and data, which is the tournamentId admin verify
        verifyAdminClick = function (tournId) {
            $("#login-button").removeClass("normal ranked add-to-tourn");
            $("#login-button").addClass("verify-admin");
            $("#tourn-admin-buttons button").data("data-id", tournId);
            changeState(utils.states.LOGIN);
        },
        //sets the rights classes and data, which is the tournament id for admin verify
        addToGameClick = function (tournId) {
            $("#login-button").removeClass("normal ranked verify-admin");
            $("#login-button").addClass("add-to-tourn");
            $("#login-button").data("data-id", tournId);
            changeState(utils.states.LOGIN);
        },
                //takes the userInfo username and sends it to the function removeFromTournament in the MemberController
        removeFromTournament = function (userInfo) {
            $.post(TennisChallenge.Constants.removeFromTournament, userInfo);
        },
        //takes the userInfo username, password, rfid and tounamentId and sends it to the function addToTourn in the MemberController
        addToTournament = function (userInfo) {
            $.getJSON(TennisChallenge.Constants.addToTournament,
                    userInfo,
                    function (data) {
                        if (data != null) {
                            if (data.status === "added") {
                                stateData.errorMessage = "Sie wurden dem Turnier hinzugefügt";
                                changeState(utils.states.BOOKING_ERROR);
                            } else if (data.status === "tournClosed") {
                                stateData.errorMessage = "Die Anmeldung am Turnier ist geschlossen.";
                                changeState(utils.states.BOOKING_ERROR);
                            } else if (data.status === "already") {
                                changeState(utils.states.ALREADY_TOURNAMENT);
                            } else if (data.status === "unassigned") {
                                stateData.errorMessage = "Sie müssen die Karte zuerst mit einem Konto verbinden";
                                changeState(utils.states.BOOKING_ERROR);
                            } else if ((data.status === "team")) {
                                stateData
                                    .errorMessage =
                                    "Spieler 1 erfolgreich angemeldet, nun bitte Spieler 2 anmelden. ACHTUNG: Falls kein zweites Teammitglied angemeldet wird, wird auch Spieler 1 wieder abgemeldet.";
                                changeState(utils.states.BOOKING_ERROR);
                                changeState(utils.states.LOGIN);
                            } else {
                                onVerfiyError();
                            }
                        } else {
                            onVerfiyError();
                        }
                    })
                .error(function () {
                    onVerfiyError();
                });
        },
        //sets the rights classes and data, which is rthe username, for removal from Tournament
        removeFromGameClick = function () {
            var username = $("#cancel-tournament").data("username");
            var rfid = $("#cancel-tournament").data("rfid");
            var tourId = $("#cancel-tournament").data("tourId");
            removeFromTournament({ username: username, rfid: rfid, tournId: tourId });
            changeState(utils.states.DEFAULT);
        },
        //IPA Relevant End
        viewRankedGamesClick = function () {
            $("#ranked-games-view").show();
        },
        setSessionOpenedCallback = function (callback) {
            if (typeof callback === "function") {
                sessionOpenedCallback = callback;
            }
        },
        onOpenBookingError = function () {
            //Original code
            //stateData.errorMessage = "Es ist ein Fehler aufgetreten. Bitte versuche es später nochmals.";
            //changeState(utils.states.OPEN_BOOKING_ERROR);

            //New code
            //Use existing stateData if there is a connecion error
            var data = getCachedStateData();
            if (data != null) {
                stateData = data;
                changeState(utils.states.BOOKING);
            }
        },
        openBookingRequest = function () {
            var url = TennisChallenge.Constants.getNextAvailableTime;

            $.getJSON(url,
                    { courtId: stateData.selectedCourtNb },
                    function (data) {
                        if (!data.Success) {
                            stateData.bookingInfo = { bookingNotPossible: true };
                            changeState(utils.states.BOOKING);
                            return;
                        }

                        var startTimeSingle = utils.aspNetJsonToDate(data.Single.StartTime),
                            endTimeSingle = utils.aspNetJsonToDate(data.Single.EndTime),
                            startTimeDouble = utils.aspNetJsonToDate(data.Double.StartTime),
                            endTimeDouble = utils.aspNetJsonToDate(data.Double.EndTime);

                        stateData.bookingInfo = {
                            isSingle: true,
                            courtId: stateData.selectedCourtNb
                        };

                        stateData.bookingInfo.singleOffer = {
                            startTime: startTimeSingle,
                            endTime: endTimeSingle,
                            duration: data.Single.Duration.TotalMinutes
                        };

                        stateData.bookingInfo.doubleOffer = {
                            startTime: startTimeDouble,
                            endTime: endTimeDouble,
                            duration: data.Double.Duration.TotalMinutes
                        };

                        setCachedStateData(stateData);

                        changeState(utils.states.BOOKING);
                    })
                .error(onOpenBookingError);
        },
        checkBooking = function () {
            if (stateData.bookingMember.HasOpenBooking) {
                changeState(utils.states.ALREADY_BOOKED);
            } else {
                openBookingRequest();
            }
        },
        verifyUserCallback = function (data) {
            if (data != null) {
                if (data.status === "valid") {
                    stateData.bookingMember = data.member;

                    if (data.member.HasOpenBooking) {
                        stateData.bookingMember.openBooking = data.bookingInfo;
                    }

                    setCachedStateData(stateData);
                    if (tournamentDemo) {
                        error.showErrorDialog("Sie wurden erfolgreich dem Turnier hinzugefügt.",
                                function () {
                                    tournamentDemo = false;
                                    changeState(utils.states.DEFAULT);
                                },
                                true
                            );
                    } else {
                        checkBooking();
                    }
                } else if (data.status === "unassigned") {
                    stateData.rfidToAssign = getCachedUserInfo().rfid;
                    changeState(utils.states.ASSIGN_RFID);
                } else {
                    onVerfiyError();
                }
            } else {
                onVerfiyError();
            }
        },
        verfiyUser = function (userInfo, callback) {
            setCachedUserInfo(userInfo);
            $.getJSON(TennisChallenge.Constants.verifyMember,
                    userInfo,
                    function (data) {
                        if (callback != null) {
                            callback(data);
                        } else {
                            verifyUserCallback(data);
                        }
                    })
                .error(function () {
                    //Use existing stateData if there is a connecion error
                    var data = getCachedStateData();
                    if (data != null) {
                        stateData = data;
                        checkBooking();
                    }
                    //onVerfiyError();
                });
        },
        fillChallPlayerList = function (members) {
            $.each(members,
                function (_, member) {
                    $("#challenge-list")
                        .append(
                            $("<li></li>")
                            .text(member.Rank + ". " + member.FullName),
                            $("<li></li>")
                            .text(member.Email),
                            $("<li></li>")
                            .text(member.TelNr)
                        );
                });
        },
        //IPA RelevantBegin
        //takes the user Infos username, password, and rfid and sends it to the verify Admin Function in the Member Controller
        verifyAdmin = function (userInfo) {
            $.getJSON(TennisChallenge.Constants.verifyAdmin,
                    userInfo,
                    function (data) {
                        if (data != null) {
                            if (data.status === "valid") {
                                if ($("#tournament-members-view").is(":visible")) {
                                    changeState(utils.states.HIDE_LOGIN);
                                    //function in infoboard-gameplan.js
                                    displayMembersAdmin();
                                } else {
                                    changeState(utils.states.TOURNAMENT_ADMIN);
                                }
                            } else if (data.status === "unassigned") {
                                stateData.errorMessage = "Sie müssen die Karte zuerst mit einem Konto verbinden";
                                changeState(utils.states.BOOKING_ERROR);
                            } else if (data.status === "notAllowed") {
                                stateData.errorMessage = "Sie haben nicht die nötigen Berechtigungen";
                                changeState(utils.states.BOOKING_ERROR);
                            } else {
                                onVerfiyError();
                            }
                        } else {
                            onVerfiyError();
                        }
                    })
                .error(function () {
                    onVerfiyError();
                });
        },
        //IPA Relevant End
        verifyUserRanked = function (userInfo) {
            $.getJSON(TennisChallenge.Constants.verifyMemberRanked,
                    userInfo,
                    function (data) {
                        if (data != null) {
                            if (data.status === "valid") {
                                changeState(utils.states.CHALLENGE_PLAYER);
                                fillChallPlayerList(data.memberModel.Data);
                            } else if (data.status === "unassigned") {
                                stateData.errorMessage = "Sie müssen die Karte zuerst mit einem Konto verbinden";
                                changeState(utils.states.BOOKING_ERROR);
                            } else {
                                onVerfiyError();
                            }
                        } else {
                            onVerfiyError();
                        }
                    })
                .error(function () {
                    onVerfiyError();
                });
        },
        onAssignError = function () {
            stateData.errorMessage = "Die Verknüpfung der Karte mit deinem Konto ist fehlgeschlagen.";
            changeState(utils.states.ASSIGN_ERROR);
        },
        assignRfid = function (username, password) {
            var parameters = {
                username: username,
                password: password,
                rfid: stateData.rfidToAssign
            };

            $.post(TennisChallenge.Constants.assignRfid,
                    parameters,
                    function (data) {
                        if (data != null && data.status === true) {
                            stateData.bookingMember = data.member;

                            if (data.member.HasOpenBooking) {
                                stateData.bookingMember.openBooking = data.bookingInfo;
                            }

                            checkBooking();
                        } else {
                            onAssignError();
                        }
                    })
                .error(function () {
                    onAssignError();
                });
        },
        onRfidRead = function (rfid) {
            if (state === utils.states.LOGIN || state === utils.states.ASSIGN_RFID || state === utils.states.ALREADY_BOOKED) {
                if ($("#login-button").hasClass("normal")) {
                    verfiyUser({ rfid: rfid });
                } else if ($("#login-button").hasClass("ranked")) {
                    verifyUserRanked({ rfid: rfid });
                    //IPA Relevant Begin
                } else if ($("#login-button").hasClass("verify-admin")) {
                    verifyAdmin({ rfid: rfid });
                } else if ($("#login-button").hasClass("add-to-tourn")) {
                    $("#cancel-tournament").data("rfid", rfid);
                    var tournId = $("#login-button").data("data-id");
                    $("#cancel-tournament").data("tourId", tournId);
                    addToTournament({ rfid: rfid, tourId: tournId });
                    //IPA Relevant End
                }
            } else if (state === utils.states.BOOKING) {
                $.getJSON(TennisChallenge.Constants.FindByRfId,
                    { rfid: rfid },
                    function (data) {
                        if (data != null) {
                            rfidCall(data);
                        }
                    });
            }
        },
        login = function (username, password) {
            verfiyUser({ userName: username, password: password });
        },
        loginRanked = function (username, password) {
            verifyUserRanked({ userName: username, password: password });
        },
        //IPA Relevant Beginn
        loginAdmin = function (username, password) {
            verifyAdmin({ userName: username, password: password });
        },
        loginTournament = function (username, password, tournId) {
            addToTournament({ userName: username, password: password, tourId: tournId });
        },
        //IPA Relevant End
        retryBookingRequest = function () {
            //When a booking error occurs, retry the booking every 30 seconds
            var timer = setTimeout(function () {
                clearTimeout(timer);
                var bookingRequest = getCachedBookingRequest();
                if (bookingRequest != null) {
                    var userInfo = getCachedUserInfo();
                    verfiyUser(userInfo,
                        function () {
                            changeState(utils.states.BOOKING);
                            sendBookingRequest(bookingRequest);
                        });
                }
            },
                30000);
        },
        onBookingError = function () {
            stateData.errorMessage = "Die Reservation ist fehlgeschlagen.";
            changeState(utils.states.BOOKING_ERROR);
            retryBookingRequest();
        },
        //Set a cached booking request so that it can be sent later
        setCachedBookingRequest = function (bookingRequest) {
            localStorage["bookingRequest"] = JSON.stringify(bookingRequest);
        },
        getCachedBookingRequest = function () {
            var bookingRequest = localStorage.getItem("bookingRequest");
            if (bookingRequest != null) {
                bookingRequest = JSON.parse(bookingRequest);
                return bookingRequest;
            }
            return null;
        },
        //Set a cached booking member (used as part of sending a booking request)
        setCachedBookingMember = function (bookingMember) {
            localStorage["bookingMember"] = JSON.stringify(bookingMember);
        },
        getCachedBookingMember = function () {
            var bookingMember = localStorage.getItem("bookingMember");
            if (bookingMember != null) {
                bookingMember = JSON.parse(bookingMember, JSON.dateParser);
                return bookingMember;
            }
            return null;
        },
        //Clear the cached data
        clearCachedData = function () {
            localStorage.clear();
        },
        sendBookingRequest = function (bookingRequest) {
            if (state !== utils.states.BOOKING)
                return;

            //Cache the booking request
            setCachedBookingRequest(bookingRequest);
            setCachedBookingMember(stateData.bookingMember);

            $.post(TennisChallenge.Constants.bookTennisCourt,
                    bookingRequest,
                    function (data) {
                        if (data.success) {
                            //If the booking is successful, clear the cached data
                            clearCachedData();
                            changeState(utils.states.DEFAULT);
                            var url = TennisChallenge.Constants.InfoBoardLogOff;
                            window.location.href = url;
                        } else {
                            onBookingError();
                        }
                    })
                .error(onBookingError);
        },
        cancelBooking = function () {
            if (state !== utils.states.ALREADY_BOOKED)
                return;

            $.post(TennisChallenge.Constants.cancelBooking,
                    { id: stateData.bookingMember.openBooking.BookingKey },
                    function () {
                        changeState(utils.states.DEFAULT);
                    })
                .error(function () {
                    changeState(utils.states.DEFAULT);
                });
        };

    changeState(utils.states.DEFAULT);

    return {
    states: utils.states,
    getState: getState,
    getStateData: getStateData,
    setStateChangedCallback: setStateChangedCallback,
    setRfidCall: setRfidCall,
    cancel: cancel,
    next: next,
    courtClicked: courtClicked,
    challengePlayerClick: challengePlayerClick,
    //IPA Relevant Begin
    verifyAdminClick: verifyAdminClick,
    addToGameClick: addToGameClick,
    removeFromGameClick: removeFromGameClick,
    //IPA Relevant End
    viewRankedGamesClick: viewRankedGamesClick,
    onRfidRead: onRfidRead,
    login: login,
    loginRanked: loginRanked,
    //IPA relevant Begin
    loginAdmin: loginAdmin,
    loginTournament: loginTournament,
    //IPA relevant End
    assignRfid: assignRfid,
    setSessionOpenedCallback: setSessionOpenedCallback,
    sendBookingRequest: sendBookingRequest,
    cancelBooking: cancelBooking,
    getCachedBookingRequest: getCachedBookingRequest,
    getCachedBookingMember: getCachedBookingMember,
    changeState: changeState,
    retryBookingRequest: retryBookingRequest
};
}(jQuery));