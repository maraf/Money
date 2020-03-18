window.Pwa = {
    install: function () {
        if (window.PwaInstallPrompt) {
            window.PwaInstallPrompt.prompt();
            window.PwaInstallPrompt.userChoice.then(function () {
                window.PwaInstallPrompt = null;
            });
        }
    },
    notify: function () {
        DotNet.invokeMethodAsync('Money.UI.Blazor', 'Pwa.Installable')
            .then(function () { }, function () { setTimeout(Pwa.notify, 1000); });
    }
};

window.addEventListener('beforeinstallprompt', function (e) {
    window.PwaInstallPrompt = e;
    Pwa.notify();
});

if ("serviceWorker" in navigator) {
    navigator.serviceWorker.register('service-worker.js');
}