// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).

self.addEventListener('fetch', () => { });
self.addEventListener('message', onMessage);
self.addEventListener('push', event => event.waitUntil(onPush(event)));
self.addEventListener('notificationclick', event => event.waitUntil(onNotificationClick(event)));

function onMessage(event) {
    if (event.data.action === 'skipWaiting') {
        self.skipWaiting();
    }
}

async function onPush(event) {
    const data = event.data ? event.data.json() : {};
    const title = data.title || 'Money';
    const options = {
        body: data.body || 'You have scheduled expenses due today.',
        icon: 'images/icon-192x192.png',
        tag: data.tag || 'money-expense-template',
        renotify: true,
        data: { url: data.url || '/expense-templates' }
    };
    await self.registration.showNotification(title, options);
}

async function onNotificationClick(event) {
    event.notification.close();
    const url = event.notification.data && event.notification.data.url ? event.notification.data.url : '/';
    const clients = await self.clients.matchAll({ type: 'window', includeUncontrolled: true });
    for (const client of clients) {
        if (client.url.includes(url) && 'focus' in client) {
            return client.focus();
        }
    }
    if (self.clients.openWindow) {
        return self.clients.openWindow(url);
    }
}