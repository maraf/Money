var isStarted = false;
var isLoaded = false;

window.Bootstrap = {
    Modal: {
        Show: function (container) {
            $(container).modal({
                "show": true,
                "focus": true
            }).on('shown.bs.modal', function () {
                $(container).find('[data-autofocus]').first().trigger('focus');
            });
        },
        Hide: function (container) {
            $(container).modal('hide');
        }
    }
};

var connection = null;
function StartSignalR(url, token) {
    StopSignalR();

    var connection = new signalR.HubConnectionBuilder()
        .withUrl(url, { accessTokenFactory: function () { return token; } })
        .build();

    connection.on("RaiseEvent", function (e) {
        console.log("JS: Event: " + e);

        DotNet.invokeMethodAsync("Money.UI.Blazor", "RaiseEvent", e);
    });

    connection.on("RaiseException", function (e) {
        console.log("JS: Exception: " + e);

        DotNet.invokeMethodAsync("Money.UI.Blazor", "RaiseException", e);
    });

    connection.onclose(function () {
        if (window.location.hostname != "localhost") {
            alert('Underlaying connection to the server has closed. Reloading the page...');
        }

        setTimeout(function () {
            window.location.reload();
        }, 2000);
    });
    connection.start().then(function () {
        isStarted = true;
    });
}

function StopSignalR() {
    if (connection != null) {
        connection.stop();
        connection = null;
    }
}

window.Money = {
    ApplicationStarted: function () {
        isLoaded = true;
    },
    NavigateTo: function (href) {
        window.location.href = href;
        return true;
    },
    StartSignalR: StartSignalR,
    StopSignalR: StopSignalR,
    SaveToken: function (token) {
        if ("localStorage" in window) {
            if (token == null)
                window.localStorage.removeItem("token");
            else
                window.localStorage.setItem("token", token);
        }
    },
    LoadToken: function () {
        if ("localStorage" in window) {
            return window.localStorage.getItem("token");
        }

        return null;
    }
};