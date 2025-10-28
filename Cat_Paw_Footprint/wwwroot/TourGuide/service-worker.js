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