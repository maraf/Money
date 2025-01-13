window.Pwa = {
    Install: function () {
        if (window.PwaInstallPrompt) {
            window.PwaInstallPrompt.prompt();
            window.PwaInstallPrompt.userChoice.then(function () {
                window.PwaInstallPrompt = null;
            });
        }
    },
    Update: function () {
        navigator.serviceWorker.ready.then(function (registration) {
            var newWorker = null;
            if (registration.waiting != null) {
                newWorker = registration.waiting;
            }

            if (newWorker != null) {
                newWorker.postMessage({ action: 'skipWaiting' });
            }
        });
    },
    installable: async () => {
        await Money.WaitForDotNet();
        DotNet.invokeMethodAsync('Money.Blazor.Host', 'Pwa.Installable');
    },
    updateable: async () => {
        await Money.WaitForDotNet();
        DotNet.invokeMethodAsync('Money.Blazor.Host', 'Pwa.Updateable');
    }
};

window.addEventListener('beforeinstallprompt', function (e) {
    window.PwaInstallPrompt = e;
    Pwa.installable();
});

if ("serviceWorker" in navigator) {
    navigator.serviceWorker.register('service-worker.js').then(function (registration) {
        if (registration.waiting !== null) {
            if (navigator.serviceWorker.controller) {
                Pwa.updateable();
            }
        } else {
            registration.addEventListener("updatefound", function () {
                var installing = registration.installing;
                if (installing !== null) {
                    installing.addEventListener("statechange", function () {
                        switch (installing.state) {
                            case 'installed':
                                if (navigator.serviceWorker.controller) {
                                    Pwa.updateable();
                                }

                                break;
                        }
                    });
                } else if (registration.waiting !== null) {
                    if (navigator.serviceWorker.controller) {
                        Pwa.updateable();
                    }
                }
            });
        }
    });

    var isRefreshing = false;
    navigator.serviceWorker.addEventListener('controllerchange', function () {
        if (isRefreshing) {
            return;
        }

        window.location.reload();
        isRefreshing = true;
    })
}