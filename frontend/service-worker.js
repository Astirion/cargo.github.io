const CACHE_NAME = 'pwa-courier-v1';
const urlsToCache = ['/frontend/', '/frontend/index.html', '/frontend/style.css', '/frontend/app.js'];

self.addEventListener('install', (event) => {
  event.waitUntil(
    caches.open(CACHE_NAME).then((cache) => cache.addAll(urlsToCache))
  );
});

self.addEventListener('fetch', (event) => {
  event.respondWith(
    caches.match(event.request).then((response) => response || fetch(event.request))
  );
});

