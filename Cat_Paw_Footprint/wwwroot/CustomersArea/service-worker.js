// 🔧 開發用 service-worker：完全不快取，全部直接抓最新
self.addEventListener("install", event => {
    console.log("✅ Service Worker 安裝完成");
    self.skipWaiting();
});

self.addEventListener("activate", event => {
    console.log("✅ Service Worker 啟用完成");
    // 清除舊快取（保險措施）
    event.waitUntil(
        caches.keys().then(keys => Promise.all(keys.map(k => caches.delete(k))))
    );
});

// ⚡ 所有請求直接走網路
self.addEventListener("fetch", event => {
    event.respondWith(fetch(event.request));
});

// 🔔 推播事件仍保留
self.addEventListener('push', e => {
    const data = e.data ? e.data.json() : {};
    const title = data.title || '貓爪足跡通知';
    const message = data.message || '您有新的通知';
    const icon = '/images/Logo.png';

    e.waitUntil(
        self.registration.showNotification(title, {
            body: message,
            icon: icon
        })
    );
});