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
        },
        Dispose: function (container) {
            $(container).modal('dispose');
        }
    }
};

window.Network = {
    Initialize: function (interop) {
        function handler() {
            interop.invokeMethodAsync("Network.StatusChanged", navigator.onLine);
        }

        window.addEventListener('online', handler);
        window.addEventListener('offline', handler);

        if (!navigator.onLine) {
            handler(navigator.onLine);
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
    },
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
    },
    AnimateSplash: function () {
        setTimeout(function () { $(".splash").addClass("animate"); }, 300);
    }
};