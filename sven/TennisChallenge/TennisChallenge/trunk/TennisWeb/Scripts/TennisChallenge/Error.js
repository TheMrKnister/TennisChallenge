TennisChallenge.InfoBoard.Error = (function ($) {
    var initialized = false,
        barIntervallId = null,
        barEndDate = null,
        errorbarSelector = '#club_site_map #error-bar',
        dialogCallback = null,
        clearBarMessage = function () {
            if (barIntervallId != null) {
                clearInterval(barIntervallId);
                barIntervallId = null;
            }

            barEndDate = null;

            $(errorbarSelector).text('');
        },
        hideDialog = function () {
            $('#error-window .error-text').text('');
            $('#error-window').hide();

            if (dialogCallback != null) {
                dialogCallback();
                dialogCallback = null;
            }
        },
        addBarMessage = function (message, displaytime) {
            if (typeof displaytime === "undefined")
                displaytime = 10;

            if (barIntervallId != null) {
                clearInterval(barIntervallId);
                barIntervallId = null;
            }

            var now = new Date();
            var endDate = new Date();
            endDate.setSeconds(now.getSeconds() + displaytime);

            if (barEndDate != null && barEndDate > endDate) {
                displaytime = (barEndDate.getTime() - now.getTime()) / 1000;
            } else {
                barEndDate = endDate;
            }

            var text = document.createTextNode(message);
            $(errorbarSelector).append(text);

            barIntervallId = setInterval(clearBarMessage, displaytime * 1000);
        },
        showErrorDialog = function (message, callback, tournamentDemo) {
            if (!initialized) {
                $('#error-window button[name="ok"]').click(hideDialog);
                initialized = true;
            }

            hideDialog();

            if (typeof callback === 'function') {
                dialogCallback = callback;
            }

            $('#error-window .error-text').text(message);
            if (tournamentDemo) {
                $('#error-window .title-bar').text('Erfolg');
            } else {
                $('#error-window .title-bar').text('Meldung');
            }

            $('#error-window').show();
        }
    ;

    return {
        addBarMessage: addBarMessage,
        showErrorDialog: showErrorDialog
    };
}(jQuery));