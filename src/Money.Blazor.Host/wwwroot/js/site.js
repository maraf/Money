var isStarted = false;
var isLoaded = false;

window.Bootstrap = {
    Modal: {
        Show: function (container) {
            $(container).modal({
                "show": true,
                "focus": true
            }).on('shown.bs.modal', function () {
                var $container = $(container);
                var $select = $container.find("[data-select]");
                if ($select.length > 0) {
                    $select[0].setSelectionRange(0, $select[0].value.length)
                }

                $container.find('[data-autofocus]').first().trigger('focus');
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
            handler();
        }
    }
};

window.Visibility = {
    Initialize: function (interop) {
        let propertyName, eventName;
        if (typeof document.hidden !== "undefined") {
            propertyName = "hidden";
            eventName = "visibilitychange";
        } else if (typeof document.msHidden !== "undefined") {
            propertyName = "msHidden";
            eventName = "msvisibilitychange";
        } else if (typeof document.webkitHidden !== "undefined") {
            propertyName = "webkitHidden";
            eventName = "webkitvisibilitychange";
        }

        function handler() {
            interop.invokeMethodAsync("Visibility.StatusChanged", !document[propertyName]);
        }

        if (typeof document.addEventListener === "undefined" || propertyName === undefined) {
            console.log("This demo requires a browser, such as Google Chrome or Firefox, that supports the Page Visibility API.");
        } else {
            document.addEventListener(eventName, handler, false);

            if (document[propertyName]) {
                handler();
            }
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
    AnimateSplash: function () {
        setTimeout(function () { $(".splash").addClass("animate"); }, 300);
    }
};

window.PullToRefresh = {
    Initialize: function (interop) {
        window.PullToRefresh._interop = interop;

        const refreshTreshold = 200;
        let _isActive = false;
        let _startY;
        let _maxY = 0;
        let container = document.body;
        const listenerOptions = { passive: true };

        let $refreshUi = $(".refresher");

        container.addEventListener('touchstart', e => {
            _startY = 0;
            _maxY = 0;

            if (document.scrollingElement.scrollTop === 0) {
                _startY = Math.floor(e.touches[0].pageY);
                _isActive = true;
            } else {
                _isActive = false;
            }
        }, listenerOptions);

        container.addEventListener('touchmove', e => {
            const y = e.touches[0].pageY;
            _maxY = Math.floor(y);

            const delta = Math.floor(_maxY - _startY);
            if (_isActive && delta > refreshTreshold) {
                $refreshUi.addClass("visible");
            } else {
                $refreshUi.removeClass("visible");
            }
        }, listenerOptions);

        container.addEventListener("touchend", () => {
            const delta = Math.floor(_maxY - _startY);
            if (_isActive && delta > refreshTreshold) {
                window.PullToRefresh.Raise();
            }

            $refreshUi.removeClass("visible");
        }, listenerOptions);
    },
    Raise: function() {
        window.PullToRefresh._interop.invokeMethodAsync("PullToRefresh.Pulled");
    }
}

