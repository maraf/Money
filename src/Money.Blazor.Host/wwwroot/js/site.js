var isStarted = false;
var isLoaded = false;

window.Bootstrap = {
    Modal: {
        Show: function (container) {
            var $container = $(container);
            if (!$container.data("modal-initialized")) {
                var modal = new bootstrap.Modal(container, {});
                $container.data("modal", modal);

                $container.data("modal-initialized", true).on('shown.bs.modal', function () {
                    var $select = $container.find("[data-select]");
                    if ($select.length > 0) {
                        $select[0].setSelectionRange(0, $select[0].value.length)
                    }

                    const autofocus = $container.find('[data-autofocus]');
                    if (autofocus.length > 0) {
                        autofocus.first().trigger('focus');
                    }
                });
            }

            $container.data("modal").show();
        },
        Hide: function (container) {
            $(container).data("modal").hide();
        },
        IsOpen: function (container) {
            return $(container).hasClass("show");
        },
        Dispose: function (container) {
            $(container).data("modal").dispose();
        }
    },
    Offcanvas: {
        Initialize: function (interop, container) {
            let offcanvas = bootstrap.Offcanvas.getInstance(container);
            if (!offcanvas) {
                offcanvas = new bootstrap.Offcanvas(container);
                container.addEventListener("show.bs.offcanvas", () => {
                    interop.invokeMethodAsync("Offcanvas.VisibilityChanged", true);
                });
                container.addEventListener("hide.bs.offcanvas", () => {
                    interop.invokeMethodAsync("Offcanvas.VisibilityChanged", false);
                });
            }
        },
        Show: function (container) {
            bootstrap.Offcanvas.getInstance(container).show()
        },
        Hide: function (container) {
            bootstrap.Offcanvas.getInstance(container).hide();
        },
        Dispose: function (container) {
            bootstrap.Offcanvas.getInstance(container).dispose();
        }
    },
    Theme: {
        Apply: function (theme) {
            document.documentElement.setAttribute("data-bs-theme", theme);
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

window.AutoloadNext = {
    Initialize: function (element, interop) {
        if ("IntersectionObserver" in window) {
            const observer = new IntersectionObserver(
                (entries) => {
                    entries.forEach(entry => {
                        if (entry.isIntersecting) {
                            if (window.AutoloadNext._lastInteropPromise) {
                                return;
                            }

                            window.AutoloadNext._lastInteropPromise = interop.invokeMethodAsync("AutoloadNext.Intersected");
                            window.AutoloadNext._lastInteropPromise
                                .then(() => {
                                    window.AutoloadNext._lastInteropPromise = null;
                                })
                                .catch(() => {
                                    window.AutoloadNext._lastInteropPromise = null;
                                });
                        }
                    });
                },
                {
                    root: null
                }
            );
    
            observer.observe(element);
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
            const $element = $(element);
            const $modal = $element.parents(".modal");
            if ($modal.length > 0 && (!$modal.data("modal") || $modal.data("modal")._isTransitioning)) {
                const modal = $modal[0];
                const eventHandler = e => {
                    element.focus();
                    modal.removeEventListener("shown.bs.modal", eventHandler);
                }
                modal.addEventListener("shown.bs.modal", eventHandler);
            } else {
                element.focus();
            }

            if ($(element).is("[data-select]")) {
                element.setSelectionRange(0, element.value.length)
            }
        }
    },
    ShowTips: () => {
        var el = document.querySelector(".app-title");
        if (el) {
            const tips = [
                "Create recurring templates and use monthly checklist to see what's the status",
                "Paying abroad? Create a new currency, set exchange rate and use it in your expenses",
                "Use categories to group your expenses",
                "Looking for similar expenses? Use context menu to open search with similar",
                "Balances compares your income and expenses",
                "Balances can include expected expenses",
                "Bottom menu items on mobile can be configured in settings",
                "Dark or light theme can be configured in settings",
                "Trends shows how your expenses in selected category change over time",
                "Find expenses for expense template using context menu",
                "Expense templates can be sorted and filtered",
                "Category can have icon and color",
                "Go to settings to adapt the app to your needs"
            ];

            const updateTip = () => {
                if (el.parentElement && el.parentElement.classList.contains("animate")) {
                    clearInterval(intervalId);
                    return;
                }
                const randomTip = tips[Math.floor(Math.random() * tips.length)];
                el.innerHTML = randomTip;
            };

            updateTip();
            const intervalId = setInterval(updateTip, 3000);
        }
    },
    WaitForDotNet: () => window.Money._DotNetPromise,
    DotNetReady: () => window.Money._DotNetPromiseResolve()
};
window.Money._DotNetPromise = new Promise(resolve => window.Money._DotNetPromiseResolve = resolve);

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
            const tresholdX = 100;
            const tresholdY = 100;
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
                    if (_startX < (tresholdX / 2)) {
                        _isActive = 1;
                        return;
                    }

                    if (window.innerWidth - _startX < (tresholdX / 2)) {
                        _isActive = 2;
                        return;
                    }
                }
                _isActive = false;
            };

            const move = e => {
                _lastDeltaX = Math.floor(Math.floor(e.touches[0].pageX) - _startX);
                _lastDeltaY = Math.floor(Math.floor(e.touches[0].pageY) - _startY);

                if (Math.abs(_lastDeltaY) > tresholdY) {
                    _isActive = false;
                    swapLeftIcon(false);
                    swapRightIcon(false);
                    $leftUi.css("margin-left", 0);
                    $rightUi.css("margin-right", 0);
                    return;
                }

                if (Math.abs(_lastDeltaX) < 20) {
                    return;
                }

                if (_isActive === 1) {
                    $leftUi.css("margin-left", Math.min(_lastDeltaX, tresholdX * 2));
                } else if (_isActive === 2) {
                    _lastDeltaX *= -1;
                    $rightUi.css("margin-right", Math.min(_lastDeltaX, tresholdX * 2));
                }

                if (_isActive && _lastDeltaX > tresholdX && _lastDeltaY < tresholdY && prerequisities()) {
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

                if (_isActive && _lastDeltaX > tresholdX && _lastDeltaY < tresholdY && prerequisities()) {
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

Money.ShowTips();
Blazor.start({
    configureRuntime: dotnet => dotnet.withEnvironmentVariable(
        "ALLOWED_LOG_SCOPE_PREFIXES", 
        window.localStorage.getItem("allowedLogScopePrefixes") ?? ""
    )
});