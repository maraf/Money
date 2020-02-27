const baseURL = '/';
const indexURL = '/index.html';
const networkFetchEvent = 'fetch';
const swInstallEvent = 'install';
const swInstalledEvent = 'installed';
const swActivateEvent = 'activate';
const staticCachePrefix = 'blazor-cache-v';
const staticCacheName = 'blazor-cache-v0.0.637181529069255726';
const requiredFiles = [
"/_framework/blazor.boot.json",
"/_framework/blazor.webassembly.js",
"/_framework/wasm/dotnet.js",
"/_framework/wasm/dotnet.wasm",
"/_framework/_bin/Microsoft.AspNetCore.Blazor.dll",
"/_framework/_bin/Microsoft.AspNetCore.Blazor.HttpClient.dll",
"/_framework/_bin/Microsoft.AspNetCore.Components.dll",
"/_framework/_bin/Microsoft.AspNetCore.Components.Forms.dll",
"/_framework/_bin/Microsoft.AspNetCore.Components.Web.dll",
"/_framework/_bin/Microsoft.Bcl.AsyncInterfaces.dll",
"/_framework/_bin/Microsoft.Extensions.Configuration.Abstractions.dll",
"/_framework/_bin/Microsoft.Extensions.Configuration.dll",
"/_framework/_bin/Microsoft.Extensions.DependencyInjection.Abstractions.dll",
"/_framework/_bin/Microsoft.Extensions.DependencyInjection.dll",
"/_framework/_bin/Microsoft.Extensions.Logging.Abstractions.dll",
"/_framework/_bin/Microsoft.Extensions.Options.dll",
"/_framework/_bin/Microsoft.Extensions.Primitives.dll",
"/_framework/_bin/Microsoft.JSInterop.dll",
"/_framework/_bin/Money.Api.Shared.dll",
"/_framework/_bin/Money.Api.Shared.pdb",
"/_framework/_bin/Money.dll",
"/_framework/_bin/Money.Models.dll",
"/_framework/_bin/Money.Models.pdb",
"/_framework/_bin/Money.pdb",
"/_framework/_bin/Money.UI.Blazor.dll",
"/_framework/_bin/Money.UI.Blazor.pdb",
"/_framework/_bin/Mono.Security.dll",
"/_framework/_bin/Mono.WebAssembly.Interop.dll",
"/_framework/_bin/mscorlib.dll",
"/_framework/_bin/Neptuo.dll",
"/_framework/_bin/Neptuo.pdb",
"/_framework/_bin/netstandard.dll",
"/_framework/_bin/System.Buffers.dll",
"/_framework/_bin/System.ComponentModel.Annotations.dll",
"/_framework/_bin/System.ComponentModel.Composition.dll",
"/_framework/_bin/System.ComponentModel.DataAnnotations.dll",
"/_framework/_bin/System.Core.dll",
"/_framework/_bin/System.Data.DataSetExtensions.dll",
"/_framework/_bin/System.Data.dll",
"/_framework/_bin/System.dll",
"/_framework/_bin/System.Drawing.Common.dll",
"/_framework/_bin/System.IO.Compression.dll",
"/_framework/_bin/System.IO.Compression.FileSystem.dll",
"/_framework/_bin/System.Memory.dll",
"/_framework/_bin/System.Net.Http.dll",
"/_framework/_bin/System.Numerics.dll",
"/_framework/_bin/System.Numerics.Vectors.dll",
"/_framework/_bin/System.Runtime.CompilerServices.Unsafe.dll",
"/_framework/_bin/System.Runtime.Serialization.dll",
"/_framework/_bin/System.ServiceModel.Internals.dll",
"/_framework/_bin/System.Text.Encodings.Web.dll",
"/_framework/_bin/System.Text.Json.dll",
"/_framework/_bin/System.Threading.Tasks.Extensions.dll",
"/_framework/_bin/System.Transactions.dll",
"/_framework/_bin/System.Xml.dll",
"/_framework/_bin/System.Xml.Linq.dll",
"/_framework/_bin/WebAssembly.Bindings.dll",
"/_framework/_bin/WebAssembly.Net.Http.dll",
"/_framework/_bin/WebAssembly.Net.WebSockets.dll",
"/css/site.min.css",
"/favicon.ico",
"/images/city.png",
"/images/icon-w-192x192.png",
"/images/icon-w-40x40.png",
"/images/icon-w-512x512.png",
"/images/icon-w-64x64.png",
"/index.html",
"/js/site.js",
"/js/site.min.js",
"/lib/bootstrap/dist/css/bootstrap-grid.css",
"/lib/bootstrap/dist/css/bootstrap-grid.css.map",
"/lib/bootstrap/dist/css/bootstrap-grid.min.css",
"/lib/bootstrap/dist/css/bootstrap-grid.min.css.map",
"/lib/bootstrap/dist/css/bootstrap-reboot.css",
"/lib/bootstrap/dist/css/bootstrap-reboot.css.map",
"/lib/bootstrap/dist/css/bootstrap-reboot.min.css",
"/lib/bootstrap/dist/css/bootstrap-reboot.min.css.map",
"/lib/bootstrap/dist/css/bootstrap.css",
"/lib/bootstrap/dist/css/bootstrap.css.map",
"/lib/bootstrap/dist/css/bootstrap.min.css",
"/lib/bootstrap/dist/css/bootstrap.min.css.map",
"/lib/bootstrap/dist/js/bootstrap.bundle.js",
"/lib/bootstrap/dist/js/bootstrap.bundle.js.map",
"/lib/bootstrap/dist/js/bootstrap.bundle.min.js",
"/lib/bootstrap/dist/js/bootstrap.bundle.min.js.map",
"/lib/bootstrap/dist/js/bootstrap.js",
"/lib/bootstrap/dist/js/bootstrap.js.map",
"/lib/bootstrap/dist/js/bootstrap.min.js",
"/lib/bootstrap/dist/js/bootstrap.min.js.map",
"/lib/bootstrap/dist/js/popper.min.js",
"/lib/fontawesome/css/all.css",
"/lib/fontawesome/css/all.min.css",
"/lib/fontawesome/css/brands.css",
"/lib/fontawesome/css/brands.min.css",
"/lib/fontawesome/css/fontawesome.css",
"/lib/fontawesome/css/fontawesome.min.css",
"/lib/fontawesome/css/regular.css",
"/lib/fontawesome/css/regular.min.css",
"/lib/fontawesome/css/solid.css",
"/lib/fontawesome/css/solid.min.css",
"/lib/fontawesome/css/svg-with-js.css",
"/lib/fontawesome/css/svg-with-js.min.css",
"/lib/fontawesome/css/v4-shims.css",
"/lib/fontawesome/css/v4-shims.min.css",
"/lib/fontawesome/js/all.js",
"/lib/fontawesome/js/all.min.js",
"/lib/fontawesome/js/brands.js",
"/lib/fontawesome/js/brands.min.js",
"/lib/fontawesome/js/conflict-detection.js",
"/lib/fontawesome/js/conflict-detection.min.js",
"/lib/fontawesome/js/fontawesome.js",
"/lib/fontawesome/js/fontawesome.min.js",
"/lib/fontawesome/js/regular.js",
"/lib/fontawesome/js/regular.min.js",
"/lib/fontawesome/js/solid.js",
"/lib/fontawesome/js/solid.min.js",
"/lib/fontawesome/js/v4-shims.js",
"/lib/fontawesome/js/v4-shims.min.js",
"/lib/fontawesome/webfonts/fa-brands-400.eot",
"/lib/fontawesome/webfonts/fa-brands-400.ttf",
"/lib/fontawesome/webfonts/fa-brands-400.woff",
"/lib/fontawesome/webfonts/fa-brands-400.woff2",
"/lib/fontawesome/webfonts/fa-regular-400.eot",
"/lib/fontawesome/webfonts/fa-regular-400.ttf",
"/lib/fontawesome/webfonts/fa-regular-400.woff",
"/lib/fontawesome/webfonts/fa-regular-400.woff2",
"/lib/fontawesome/webfonts/fa-solid-900.eot",
"/lib/fontawesome/webfonts/fa-solid-900.ttf",
"/lib/fontawesome/webfonts/fa-solid-900.woff",
"/lib/fontawesome/webfonts/fa-solid-900.woff2",
"/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js",
"/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js",
"/lib/jquery-validation/dist/additional-methods.js",
"/lib/jquery-validation/dist/additional-methods.min.js",
"/lib/jquery-validation/dist/jquery.validate.js",
"/lib/jquery-validation/dist/jquery.validate.min.js",
"/lib/jquery/dist/jquery.js",
"/lib/jquery/dist/jquery.min.js",
"/lib/jquery/dist/jquery.min.map",
"/lib/signalr/signalr.js",
"/lib/signalr/signalr.js.map",
"/lib/signalr/signalr.min.js",
"/lib/signalr/signalr.min.js.map",
"/service-worker-register.js",
"/manifest.json"
];
// * listen for the install event and pre-cache anything in filesToCache * //
self.addEventListener(swInstallEvent, event => {
    self.skipWaiting();
    event.waitUntil(
        caches.open(staticCacheName)
            .then(cache => {
                return cache.addAll(requiredFiles);
            })
    );
});
self.addEventListener(swActivateEvent, function (event) {
    event.waitUntil(
        caches.keys().then(function (cacheNames) {
            return Promise.all(
                cacheNames.map(function (cacheName) {
                    if (staticCacheName !== cacheName && cacheName.startsWith(staticCachePrefix)) {
                        return caches.delete(cacheName);
                    }
                })
            );
        })
    );
});
self.addEventListener(networkFetchEvent, event => {
    const requestUrl = new URL(event.request.url);
    if (requestUrl.origin === location.origin) {
        if (requestUrl.pathname === baseURL) {
            event.respondWith(caches.match(indexURL));
            return;
        }
    }
    event.respondWith(
        caches.match(event.request)
            .then(response => {
                if (response) {
                    return response;
                }
                return fetch(event.request)
                    .then(response => {
                        if (response.ok) {
                            if (requestUrl.origin === location.origin) {
                                caches.open(staticCacheName).then(cache => {
                                    cache.put(event.request.url, response);
                                });
                            }
                        }
                        return response.clone();
                    });
            }).catch(error => {
                console.error(error);
            })
    );
});
