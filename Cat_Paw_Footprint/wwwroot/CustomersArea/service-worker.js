self.addEventListener("install", (event) => {
    console.log("Service Worker: 安裝完成");
    self.skipWaiting(); // 讓 SW 立即生效
});

self.addEventListener("activate", (event) => {
    console.log("Service Worker: 啟用完成");
});

self.addEventListener("fetch", (event) => {
    // 最小化版本：直接放行所有請求
});

self.addEventListener('install', e => {
    console.log('Service Worker 已安裝');
});

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
