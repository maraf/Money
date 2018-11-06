var isStarted = false;
var isLoaded = false;

window.Bootstrap = {
    Modal: {
        Register: function (id) {
            var target = $("#" + id);
            target.on('shown.bs.modal', function (e) {
                $(e.currentTarget).find('[data-autofocus]').select().focus();
            });
            target.on('hidden.bs.modal', function (e) {
                DotNet.invokeMethodAsync("Money.UI.Blazor", "Bootstrap_ModalHidden", e.currentTarget.id);
            });

            return true;
        },
        Toggle: function (id, isVisible) {
            var target = $("#" + id);
            target.modal(isVisible ? 'show' : 'hide');

            return true;
        }
    }
};

window.Money = {
    ApplicationStarted: function () {
        isLoaded = true;
    },
    NavigateTo: function (href) {
        window.location.href = href;
        return true;
    }
};

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/api")
    .build();;

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