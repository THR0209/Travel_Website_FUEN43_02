// ===================================================
// 🐾 Cat Paw Footprint - Web Front 全域前端腳本（jQuery 相容＋性能最佳化最終版）
// 功能：導覽列互動、通知系統、彈窗提示、FAQ、SignalR、Swiper
// 亮點：多階段初始化、事件委派、rAF + passive scroll、真正 lazy Swiper、idle FAQ、
//       requestIdleCallback fallback、避免重複綁定、減少首屏 API 與主緒阻塞
// 相容：不覆蓋 jQuery 的 $、不觸發 browserLink 的 $.noConflict 錯誤
// ===================================================

/* ---------------------------------------------------
 * 0. 公用：idle 與選取器
 * --------------------------------------------------- */

// jQuery 別名：不動用全域 $
const $q = window.jQuery || window.$;
if (!$q) console.warn('⚠️ 未偵測到 jQuery，與 jQuery 相關功能將被略過');

// 安全的 requestIdleCallback（所有延後工作統一走這個）
function onIdle(fn, { timeout = 3000 } = {}) {
    if ('requestIdleCallback' in window) {
        return window.requestIdleCallback(fn, { timeout });
    }
    return setTimeout(fn, timeout);
}

// 原生選取器（避免覆蓋 $）：單一 / 多個
const qs = (sel, root = document) => root.querySelector(sel);
const qsa = (sel, root = document) => [...root.querySelectorAll(sel)];

// 動態載入外部腳本（如需真正 lazy 載入 Swiper）
function loadScriptOnce(src) {
    return new Promise((resolve, reject) => {
        if (document.querySelector(`script[data-dyn="${src}"]`) || [...document.scripts].some(s => s.src.includes(src))) {
            resolve();
            return;
        }
        const s = document.createElement('script');
        s.src = src;
        s.async = true;
        s.defer = true;
        s.dataset.dyn = src;
        s.onload = resolve;
        s.onerror = reject;
        document.head.appendChild(s);
    });
}

/* ---------------------------------------------------
 * 1. SweetAlert2：showAlert / showConfirm
 * --------------------------------------------------- */
window.showAlert = function (type, title, text, timer = 1000) {
    Swal.fire({
        icon: type,                 // success / error / warning / info / question
        title: `🐾 ${title}`,      // 標題前加上貓爪符號
        text,         // 顯示內容文字
        timer,                    // 自動關閉時間（毫秒）
        showConfirmButton: false,    // 不顯示確認鍵
        timerProgressBar: true,     // 進度條
        toast: false,   // 置中模式
        position: "center",  // 彈窗置中
        background: "#fff", // 白色背景
        customClass: { popup: 'shadow-sm rounded-3' }   // 圓角+陰影
    });
};

window.showConfirm = async function (options = {}) {
    const {
        imageUrl = '/images/Logo.png',                       // 預設圖示
        title = `確定要執行此操作嗎？`,        // 與 showAlert 一樣加上貓爪符號
        text = '此操作無法復原，是否繼續？',       // 提示文字
        confirmText = '確定',                     // 確認按鈕文字
        cancelText = '取消',                      // 取消按鈕文字
        confirmColor = '#d33',                    // 確認按鈕顏色（紅色）
        cancelColor = '#3085d6'                   // 取消按鈕顏色（藍色）        
    } = options;   

    return await Swal.fire({
        imageUrl,                // ✅ 顯示自訂圖片
        imageWidth: 80,          // ✅ 可以調整圖片大小
        imageHeight: 80,
        title: `🐾 ${title} 🐾`,  
        text,
        showCancelButton: true,
        confirmButtonText: confirmText,
        cancelButtonText: cancelText,
        confirmButtonColor: confirmColor,
        cancelButtonColor: cancelColor,
        background: "#fff",                      // ✅ 與 showAlert 一樣的白底
        position: "center",                      // ✅ 同樣置中顯示
        timerProgressBar: true,
        customClass: { popup: 'shadow-sm rounded-3' } // ✅ 加上陰影與圓角
    });
};

/* ---------------------------------------------------
 * 2. 通知系統：未讀數 / 下拉清單（事件委派）
 * --------------------------------------------------- */

// 2.1 未讀數：小且重要 → 允許在第一階段執行
window.updateUnread = async function () {
    try {
        const res = await axios.get("/CustomersArea/Notifications/GetUnreadCount");
        const count = res.data.count;
        const badge = qs("#notifBadge");
        if (!badge) return;

        if (count > 0) {
            badge.style.display = "inline-block";
            badge.innerText = count > 99 ? "99+" : String(count);
        } else {
            badge.style.display = "none";
        }
    } catch (e) {
        console.error("❌ 更新未讀通知數失敗", e);
    }
};

// 2.2 下拉清單（字串先格式化，避免迴圈重工）
function fmt(ts) { return dayjs(ts).format('MM/DD HH:mm'); }

window.updateList = async function () {
    try {
        const res = await axios.get("/CustomersArea/Notifications/GetLatestNotifications");
        const data = res.data;
        const list = qs("#notifList");
        if (!list) return;

        if (!data || data.length === 0) {
            list.innerHTML = `<div class="text-center text-muted py-3">目前沒有通知 💤</div>`;
            return;
        }

        list.innerHTML = data.map(n => {
            const timeStr = fmt(n.createdAt);
            return `
        <div class="notif-item border px-3 py-2 ${n.isRead ? 'opacity-50' : ''}"
             data-id="${n.notificationID}"
             data-type="${n.type || ''}"
             data-title="${(n.title || '').replace(/"/g, '&quot;')}"
             data-message="${(n.message || '').replace(/"/g, '&quot;')}"
             style="cursor:pointer;">
          <div class="fw-bold text-truncate">${n.title || '無標題'}</div>
          <div class="small text-muted text-truncate">${n.message || ''}</div>
          <div class="text-end small text-secondary">${timeStr}</div>
        </div>`;
        }).join('');

    } catch (e) {
        console.error("載入通知清單失敗", e);
        window.showAlert('warning', '載入失敗', '通知清單載入失敗');
    }
};

// 2.3 事件委派（一次綁在 document）
document.addEventListener('click', async (evt) => {
    const item = evt.target.closest('.notif-item');
    if (!item) return;

    const id = item.dataset.id;
    const title = item.dataset.title || item.querySelector('.fw-bold')?.textContent || "無標題";
    const msg = item.dataset.message || item.querySelector('.small.text-muted')?.textContent || "";
    const type = item.dataset.type || "";

    try {
        // 標記已讀（失敗不阻塞導向）
        axios.post('/CustomersArea/Notifications/MarkAsRead', { id })
            .then(() => { item.classList.add('opacity-50'); window.updateUnread(); })
            .catch(err => console.warn('標記已讀失敗，但不阻斷導向', err));

        // 導向規則
        const ticketMatch = msg.match(/#\s*(\d+)/);

        if (/客服服務已完成|客服評價提醒|客服回覆|客服訊息|訂單取消通知/.test(title) || /取消申請/.test(msg)) {
            if (ticketMatch && ticketMatch[1]) {
                window.location.href = `/CustomersArea/CustomerService/Index?ticketId=${ticketMatch[1]}`;
                return;
            }
        }

        if (type === "優惠活動" || /優惠活動/.test(title) || /優惠券/.test(msg)) {
            window.location.href = "/CustomersArea/Coupons/Index";
            return;
        }

        if (/訂單成立通知|付款成功通知|訂單狀態|系統公告/.test(title)) {
            const orderMatch = msg.match(/#\s*(\d+)/);
            window.location.href = orderMatch?.[1]
                ? `/CustomersArea/Orders?orderId=${orderMatch[1]}`
                : `/CustomersArea/Orders`;
            return;
        }

        if (/優惠|公告|提醒/.test(title)) {
            window.location.href = `/CustomersArea/Notifications/Index`;
            return;
        }

    } catch (err) {
        console.error("❌ 通知點擊處理錯誤", err);
        window.showAlert('error', '錯誤', '無法處理通知點擊');
    }
});

/* ---------------------------------------------------
 * 3. 桌面推播（按需）
 * --------------------------------------------------- */
window.showDesktopNotification = async function (title, message) {
    if (!("Notification" in window)) return;
    const permission = await Notification.requestPermission();
    if (permission === "granted") {
        new Notification(title, { body: message, icon: "/images/logo.png" });
    }
};

/* ---------------------------------------------------
 * 4. 第一階段初始化（最小化）
 * --------------------------------------------------- */

function setupScrollHandlers() {
    const header = qs('.site-header');
    const toTop = qs('#backToTop');
    const heroMedia = qs('#heroMedia');

    let lastY = 0, ticking = false;

    // 建議 CSS：#heroMedia { will-change: transform; backface-visibility: hidden; }
    function updateOnScroll(y) {
        if (header) header.classList.toggle('shrink', y > 8);
        if (toTop) toTop.classList.toggle('show', y > 480);
        if (heroMedia) {
            const clamp = Math.min(Math.max(y, 0), 280);
            heroMedia.style.transform = `translateZ(0) scale(${1.1 + clamp / 2800}) translateY(${clamp * 0.06}px)`;
        }
    }

    function onScroll() {
        lastY = window.scrollY || window.pageYOffset;
        if (!ticking) {
            window.requestAnimationFrame(() => { updateOnScroll(lastY); ticking = false; });
            ticking = true;
        }
    }

    window.addEventListener('scroll', onScroll, { passive: true });
    updateOnScroll(window.scrollY || window.pageYOffset);
}

// 平滑錨點（忽略 href="#"）
function setupSmoothAnchors() {
    document.addEventListener('click', (e) => {
        const link = e.target.closest('.nav-link[href^="#"]');
        if (!link) return;
        const targetSel = link.getAttribute('href');
        if (!targetSel || targetSel === '#') return;
        const target = document.querySelector(targetSel);
        if (!target) return;

        e.preventDefault();
        window.scrollTo({ top: target.offsetTop - 60, behavior: 'smooth' });
    });
}

/* ---------------------------------------------------
 * 5. 第二階段初始化（idle/延後）
 * --------------------------------------------------- */

// 5.1 SignalR（穩定連線 + 回補）
function setupSignalR() {
    if (!window.signalR) { console.warn('⚠️ 未載入 SignalR，略過 setupSignalR'); return; }

    const listEl = qs("#notifList");

    window.connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub", { withCredentials: true })
        .configureLogging(signalR.LogLevel.Information)
        .withAutomaticReconnect([0, 2000, 5000, 10000, 20000])
        .build();

    window.connection.serverTimeoutInMilliseconds = 60000;

    connection.on("ReceiveNotification", (title, message, type) => {
        console.log("📨 收到通知:", { title, message, type });

        const path = window.location.pathname.toLowerCase();
        const isInSilentPage =
            path.includes("/customersarea/customerservice") ||
            path.includes("/customersarea/notifications");

        if (isInSilentPage) {
            console.log("🚫 使用者目前在客服中心或通知中心，略過提示。");
            window.updateUnread();
            return;
        }

        window.showAlert('info', title, message, 4000);
        window.updateUnread();
        if (listEl && listEl.dataset.loaded === "true") window.updateList();
        window.showDesktopNotification(title, message);
    });

    connection.onreconnecting(err => console.warn("🔁 SignalR 正在重新連線...", err));

    connection.onreconnected(async id => {
        console.log("🔁 SignalR 已重新連線 id:", id);
        await window.updateUnread();
        if (listEl && listEl.dataset.loaded === "true") await window.updateList();
    });

    connection.onclose(async (error) => {
        console.warn("🔴 SignalR 已斷線:", error);
        try { await axios.get("/CustomersArea/Notifications/GetUnreadCount"); } catch (e) { console.error("❌ 無法刷新通知計數", e); }
        if (!navigator.onLine) {
            console.log("📡 裝置離線，恢復網路後重試…");
            window.addEventListener('online', () => attemptRestartWithBackoff(), { once: true });
            return;
        }
        attemptRestartWithBackoff();
    });

    async function attemptRestartWithBackoff() {
        const maxAttempts = 6;
        const baseDelay = 1500;
        for (let attempt = 1; attempt <= maxAttempts; attempt++) {
            try {
                if (window.connection.state === signalR.HubConnectionState.Connected) { console.log("✅ 已連線"); return; }
                if ([signalR.HubConnectionState.Connecting, signalR.HubConnectionState.Reconnecting].includes(window.connection.state)) {
                    console.log("⏳ 連線/重連中，稍候檢查…");
                    await new Promise(r => setTimeout(r, 1000));
                    if (window.connection.state === signalR.HubConnectionState.Connected) return;
                    continue;
                }
                console.log(`🔄 嘗試重連 (${attempt}/${maxAttempts})...`);
                await window.connection.start();
                console.log("✅ 手動重連成功");
                await window.updateUnread();
                if (listEl && listEl.dataset.loaded === "true") await window.updateList();
                return;
            } catch (err) {
                const delay = baseDelay * Math.pow(2, attempt - 1);
                console.error(`重連失敗（${attempt}），${delay}ms 後再試：`, err);
                await new Promise(r => setTimeout(r, delay));
            }
        }
        console.error("❌ 超過最大重試次數，暫停重連。請檢查伺服器或網路。");
    }

    async function initialStart() {
        try {
            const state = window.connection.state;
            if (state === signalR.HubConnectionState.Connected) {
                console.log("✅ SignalR 已連線（initialStart）");
                await window.updateUnread();
                if (listEl && listEl.dataset.loaded === "true") await window.updateList();
                return;
            }
            if ([signalR.HubConnectionState.Connecting, signalR.HubConnectionState.Reconnecting].includes(state)) {
                console.log("⏳ 連線/重連中，initialStart 不呼叫 start()");
                return;
            }
            await window.connection.start();
            console.log("✅ SignalR 初始連線成功");
            await window.updateUnread();
            if (listEl && listEl.dataset.loaded === "true") await window.updateList();
        } catch (err) {
            console.error("SignalR 初始連線失敗，5 秒後重試:", err);
            setTimeout(initialStart, 5000);
        }
    }

    setTimeout(initialStart, 800);

    // 登出：中斷 Hub + 更新 UI
    const logoutForm = document.querySelector('form[action*="CusLogReg/Logout"]');
    if (logoutForm) {
        logoutForm.addEventListener('submit', function () {
            try { if (connection && connection.stop) connection.stop(); } catch (e) { console.warn("SignalR 停止失敗", e); }
            const badge = qs("#notifBadge");
            const list = qs("#notifList");
            if (badge) badge.style.display = "none";
            if (list) list.innerHTML = `<div class="text-center text-muted py-3">請重新登入後查看通知</div>`;
        });
    }

    // 暴露給開發測試
    window.signalRAttemptRestart = attemptRestartWithBackoff;
}

// 5.2 Swiper：視區附近才載入腳本並初始化
function lazyInitSwiper() {
    const swiperEl = qs('.swiper');
    if (!swiperEl) return;

    const init = () => {
        if (window.Swiper) {
            new Swiper('.swiper', {
                slidesPerView: 1.2,
                spaceBetween: 12,
                navigation: { nextEl: '.swiper-next', prevEl: '.swiper-prev' },
                breakpoints: {
                    576: { slidesPerView: 2, spaceBetween: 16 },
                    992: { slidesPerView: 3, spaceBetween: 18 },
                    1200: { slidesPerView: 4, spaceBetween: 18 }
                },
                keyboard: { enabled: true },
                a11y: { enabled: true }
            });
        } else {
            loadScriptOnce('/lib/swiper-bundle.min.js')
                .then(() => new Swiper('.swiper', {
                    slidesPerView: 1.2, spaceBetween: 12,
                    navigation: { nextEl: '.swiper-next', prevEl: '.swiper-prev' },
                    breakpoints: {
                        576: { slidesPerView: 2, spaceBetween: 16 },
                        992: { slidesPerView: 3, spaceBetween: 18 },
                        1200: { slidesPerView: 4, spaceBetween: 18 }
                    },
                    keyboard: { enabled: true }, a11y: { enabled: true }
                }))
                .catch(err => console.error('載入 Swiper 失敗', err));
        }
    };

    if ('IntersectionObserver' in window) {
        const obs = new IntersectionObserver((entries, o) => {
            if (entries.some(e => e.isIntersecting)) { init(); o.disconnect(); }
        }, { rootMargin: '400px' });
        obs.observe(swiperEl);
    } else {
        onIdle(init, { timeout: 2000 });
    }
}

// 5.3 FAQ 熱門（idle 載入，使用 jQuery 以保留相容）
function loadHotFAQ() {
    if (!$q) return;
    $q.getJSON('/CustomersArea/FrontFAQs/api/hot', function (faqs) {
        let html = `<div class="accordion" id="homeHotFaqAccordionInner">`;
        faqs.forEach((faq, idx) => {
            const answer = faq.answer
                ? faq.answer.replace(/<\s*p(\s+[^>]*)?>/gi, '<div$1>').replace(/<\s*\/\s*p\s*>/gi, '</div>')
                : '<div class="text-muted">暫無答案</div>';
            html += `
        <div class="accordion-item">
          <h2 class="accordion-header" id="homeHotHeading${idx}">
            <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse"
              data-bs-target="#homeHotCollapse${idx}" aria-expanded="false"
              aria-controls="homeHotCollapse${idx}">
              ${faq.question}
            </button>
          </h2>
          <div id="homeHotCollapse${idx}" class="accordion-collapse collapse"
            aria-labelledby="homeHotHeading${idx}" data-bs-parent="#homeHotFaqAccordionInner">
            <div class="accordion-body">${answer}</div>
          </div>
        </div>`;
        });
        html += `</div>`;
        const host = qs('#homeHotFaqAccordion');
        if (host) host.innerHTML = html;
    });
}

// 5.4 導覽列「常見問題」智慧行為 + active 樣式
function setupFaqNavSmart() {
    const path = window.location.pathname.toLowerCase();
    const faqLink = qs('.faq-nav-link');
    const faqNav = faqLink?.closest('li');

    if (faqLink) {
        faqLink.addEventListener('click', function (e) {
            e.preventDefault();
            if (path.includes('/customersarea/home') || path === '/customersarea' || path === '/customersarea/') {
                const faqTarget = qs('#faqSection');
                if (faqTarget) {
                    window.scrollTo({ top: faqTarget.offsetTop - 60, behavior: 'smooth' });
                } else {
                    console.warn('⚠️ 找不到 #faqSection 元素');
                }
            } else {
                window.location.href = '/CustomersArea/FrontFAQs/Index';
            }
        });
    }

    if (path.includes('/customersarea/frontfaqs/index')) {
        qsa('.navbar-nav .nav-link').forEach(link => link.classList.remove('active'));
        if (faqLink) {
            faqLink.classList.add('active');
            if (faqNav) faqNav.classList.add('active');
        }
    }
}

// 5.5 「全部已讀」按鈕（navbar / page）
function setupMarkAllReadButtons() {
    qsa('#markAllReadBtnNavbar, #markAllReadBtnPage').forEach(btn => {
        btn.addEventListener('click', async () => {
            try {
                const res = await axios.post('/CustomersArea/Notifications/MarkAllAsRead');
                if (res.data.success) {
                    window.showAlert('success', '通知中心', '全部通知已標記為已讀 🐾');
                    await window.updateUnread();

                    const notifListEl = qs('#notifList');
                    if (notifListEl && notifListEl.dataset.loaded === "true") await window.updateList();

                    qsa('.notif-card').forEach(c => c.classList.add('is-read'));
                } else {
                    window.showAlert('warning', '操作失敗', res.data.message || '請稍後再試');
                }
            } catch (err) {
                console.error('❌ 全部已讀錯誤:', err);
                window.showAlert('error', '錯誤', '伺服器連線失敗');
            }
        });
    });
}

// 5.6 延後載入通知清單策略（初始只載入未讀數）
function setupDeferredNotificationListLoad() {
    const notifListEl = qs('#notifList');
    if (notifListEl) notifListEl.dataset.loaded = "false";

    const notifToggle =
        qs('#notifToggle') ||
        document.querySelector('[data-notif-toggle]') ||
        qs('#notifDropdownToggle') ||
        qs('#notifBadge');

    let listLoaded = false;
    function loadListNow() {
        if (listLoaded) return;
        listLoaded = true;
        if (notifListEl) notifListEl.dataset.loaded = "true";
        window.updateList();
    }

    if (notifToggle) {
        notifToggle.addEventListener('click', loadListNow, { once: true, passive: true });
        notifToggle.addEventListener('mouseenter', loadListNow, { once: true, passive: true });
    }

    onIdle(() => { if (!listLoaded) loadListNow(); }, { timeout: 2500 });
}

/* ---------------------------------------------------
 * 6. 啟動流程
 * --------------------------------------------------- */
document.addEventListener('DOMContentLoaded', function () {
    // 登入保護（Razor 注入 isCustomerLoggedIn）
    if (typeof isCustomerLoggedIn !== "undefined" && isCustomerLoggedIn !== "true") {
        window.location.href = "/CustomersArea/CusLogReg/Login";
        return;
    }

    // 第一階段：可視與互動的必要功能
    setupScrollHandlers();
    setupSmoothAnchors();
    window.updateUnread();

    // 第二階段：重型任務延後（SignalR / FAQ / Swiper / 全部已讀 / 通知清單懶載）
    onIdle(() => {
        setupSignalR();
        setupMarkAllReadButtons();
        setupDeferredNotificationListLoad();
        lazyInitSwiper();
        loadHotFAQ();
        setupFaqNavSmart();
    }, { timeout: 3000 });

    // ✅ 開發測試：
    // window.updateList();              // 強制載入下拉清單
    // window.signalRAttemptRestart?.(); // 手動重連 SignalR
});
