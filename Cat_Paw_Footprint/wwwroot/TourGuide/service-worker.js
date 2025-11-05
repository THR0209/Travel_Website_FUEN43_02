const CACHE_NAME = "tourguide-cache-v2";
const ASSETS = [
    "/TourGuide/login.html",
    "/TourGuide/group-list.html",
    "/TourGuide/chat.html",
    "/TourGuide/icon-192.png",
    "/TourGuide/icon-512.png",
    "/TourGuide/manifest.json"
];

// 安裝階段：預快取主要頁面與資源
self.addEventListener("install", (event) => {
    console.log("Service Worker: 安裝中...");
    event.waitUntil(
        caches.open(CACHE_NAME).then((cache) => cache.addAll(ASSETS))
    );
    self.skipWaiting();
});

// 啟用階段：清除舊版快取
self.addEventListener("activate", (event) => {
    console.log("Service Worker: 啟用完成");
    event.waitUntil(
        caches.keys().then((keys) =>
            Promise.all(keys.map((key) => {
                if (key !== CACHE_NAME) return caches.delete(key);
            }))
        )
    );
    self.clients.claim();
});

// 請求攔截：先取快取，失敗再抓網路
self.addEventListener("fetch", (event) => {
    event.respondWith(
        caches.match(event.request).then((res) => {
            return (
                res ||
                fetch(event.request).catch(() =>
                    caches.match("/TourGuide/login.html")
                )
            );
        })
    );
});

self.addEventListener('push', event => {
    const data = event.data.json();
    const title = data.title || "新訊息通知";
    const options = {
        body: data.body || "有人在群組發話",
        icon: "/TourGuide/icon-192.png",
        badge: "/TourGuide/icon-192.png"
    };
    event.waitUntil(self.registration.showNotification(title, options));
});