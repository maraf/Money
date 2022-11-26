var isStarted = false;
var isLoaded = false;

window.Bootstrap = {
    Modal: {
        Show: function (container) {
            var $container = $(container);
            if (!$container.data("modal-initialized")) {
                $container.data("modal-initialized", true).on('shown.bs.modal', function () {
                    var $select = $container.find("[data-select]");
                    if ($select.length > 0) {
                        $select[0].setSelectionRange(0, $select[0].value.length)
                    }

                    $container.find('[data-autofocus]').first().trigger('focus');
                });
            }

            $container.modal({
                "show": true,
                "focus": true
            });
        },
        Hide: function (container) {
            $(container).modal('hide');
        },
        IsOpen: function (container) {
            return $(container).hasClass("show");
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
        setTimeout(function () { $(".splash").addClass("animate"); }, 1000);
    },
    BlurActiveElement: function () {
        setTimeout(() => document.activeElement.blur(), 1);
    },
    FocusElementById: function (id) {
        const element = document.getElementById(id);
        if (element) {
            element.focus();
        }
    }
};

window.PullToRefresh = {
    Initialize: function (interop) {
        const container = document.body;
        const listenerOptions = { passive: true };
        
        const prerequisities = () => !document.querySelector(".modal.fade.show") && document.querySelector("nav.navbar-dark .navbar-collapse.collapse");
        
        refreshClosure = (() => {
            const treshold = 200;
            const $ui = $(".refresher");
            
            let _isActive = false;
            let _startX;
            let _startY;
            let _lastDeltaX = 0;
            let _lastDeltaY = 0;

            const start = e => {
                _startX = 0;
                _startY = 0;
                _lastDeltaX = 0;
                _lastDeltaY = 0;
    
                if (document.scrollingElement.scrollTop === 0 && prerequisities()) {
                    _startX = Math.floor(e.touches[0].pageX);
                    _startY = Math.floor(e.touches[0].pageY);
                    _isActive = true;
                } else {
                    _isActive = false;
                }
            };

            const move = e => {
                _lastDeltaX = Math.floor(Math.floor(e.touches[0].pageX) - _startX);
                _lastDeltaY = Math.floor(Math.floor(e.touches[0].pageY) - _startY);
                if (_isActive && _lastDeltaY > treshold && _lastDeltaX < (treshold / 2) && prerequisities()) {
                    $ui.addClass("visible");
                } else {
                    $ui.removeClass("visible");
                }
            };

            const end = () => {
                if (_isActive && _lastDeltaY > treshold && _lastDeltaX < (treshold / 2) && prerequisities()) {
                    interop.invokeMethodAsync("PullToRefresh.Pulled");
                }
    
                $ui.removeClass("visible");
            };

            return { start, move, end };
        })();
        
        swipeClosure = (() => {
            const treshold = 100;
            const $leftUi = $(".swipe-left");
            const $rightUi = $(".swipe-right");
            
            let _isActive = false;
            let _startX;
            let _startY;
            let _lastDeltaX = 0;
            let _lastDeltaY = 0;

            const swapLeftIcon = (done) => {
                if (done) {
                    $leftUi.find("i").removeClass("fa-arrow-left").addClass("fa-arrow-circle-left");
                } else {
                    $leftUi.find("i").removeClass("fa-arrow-circle-left").addClass("fa-arrow-left");
                }
            }

            const swapRightIcon = (done) => {
                if (done) {
                    $rightUi.find("i").removeClass("fa-arrow-right").addClass("fa-arrow-circle-right");
                } else {
                    $rightUi.find("i").removeClass("fa-arrow-circle-right").addClass("fa-arrow-right");
                }
            }

            const start = e => {
                _startX = 0;
                _startY = 0;
                _lastDeltaX = 0;
                _lastDeltaY = 0;
    
                if (prerequisities()) {
                    _startX = Math.floor(e.touches[0].pageX);
                    _startY = Math.floor(e.touches[0].pageY);
                    if (_startX < (treshold / 2)) {
                        _isActive = 1;
                        return;
                    }
                    
                    if (window.innerWidth - _startX < (treshold / 2)) {
                        _isActive = 2;
                        return;
                    }
                }
                _isActive = false;
            };

            const move = e => {
                _lastDeltaX = Math.floor(Math.floor(e.touches[0].pageX) - _startX);
                _lastDeltaY = Math.floor(Math.floor(e.touches[0].pageY) - _startY);

                if (_isActive === 1) {
                    $leftUi.css("margin-left", Math.min(_lastDeltaX, treshold * 2));
                } else if (_isActive === 2) {
                    _lastDeltaX *= -1;
                    $rightUi.css("margin-right", Math.min(_lastDeltaX, treshold * 2));
                }

                if (_isActive && _lastDeltaX > treshold && _lastDeltaY < (treshold * 2) && prerequisities()) {
                    if (_isActive === 1) {
                        swapLeftIcon(true);
                    } else if (_isActive === 2) {
                        swapRightIcon(true);
                    }

                    return;
                }

                swapLeftIcon(false);
                swapRightIcon(false);
            };
            
            const end = () => {
                $leftUi.css("margin-left", 0);
                $rightUi.css("margin-right", 0);

                if (_isActive && _lastDeltaX > treshold && _lastDeltaY < (treshold * 2) && prerequisities()) {
                    if (_isActive === 1) {
                        interop.invokeMethodAsync("Swiped.Left");
                    }
                    if (_isActive === 2) {
                        interop.invokeMethodAsync("Swiped.Right");
                    }
                }

                swapLeftIcon(false);
                swapRightIcon(false);
            };

            return { start, move, end };
        })();

        const closures = [
            refreshClosure,
            swipeClosure
        ];

        container.addEventListener('touchstart', e => closures.forEach(c => c.start(e)), listenerOptions);
        container.addEventListener('touchmove', e => closures.forEach(c => c.move(e)), listenerOptions);
        container.addEventListener("touchend", () => closures.forEach(c => c.end()), listenerOptions);
    }
}
