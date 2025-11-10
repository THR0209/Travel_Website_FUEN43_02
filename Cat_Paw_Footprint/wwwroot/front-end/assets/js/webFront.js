    // ===================================================
    // 🐾 Cat Paw Footprint - Web Front 全域前端腳本
    // 
    // ---------------------------------------------------
    // ✨ 功能總覽：
    // 00. Service Worker 註冊（PWA）
    // 01. 購物車數量更新（登入後自動載入）
    // 02. 首頁 Trip Link 智慧導向（首頁切換 Tab + 平滑滾動）
    // 03. SweetAlert2 收藏提示 Toast（showFavResult）
    // 04. 公用函式（onIdle、loadScriptOnce、qs/qsa 等）
    // 05. SweetAlert2 通用 alert / confirm 彈窗
    // 06. 通知系統（未讀數、清單、事件委派）
    // 07. 桌面推播（Desktop Notification）
    // 08. 首階段初始化（Header Shrink、平滑錨點）
    // 09. 第二階段初始化（SignalR、Swiper、FAQ、MarkAllRead、通知懶載）
    // 10. 收藏模組（全域收藏按鈕互動、未登入彈窗）
    // ===================================================

    /* ---------------------------------------------------
     * 0. PWA：Service Worker 註冊
     * --------------------------------------------------- */
    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.register('/CustomersArea/service-worker.js')
            .then(reg => console.log('✅ Service Worker 註冊成功:', reg))
            .catch(err => console.error('❌ Service Worker 註冊失敗:', err));
    }

    /* ---------------------------------------------------
     * 1. 購物車數量更新（僅登入會員）
     * --------------------------------------------------- */
    function updateCartCount(count) {
        const cartCount = document.getElementById('cart-count');
        if (cartCount) cartCount.innerText = count;
    }

    document.addEventListener('DOMContentLoaded', function () {
        // Razor 注入：isCustomerLoggedIn
        if (typeof isCustomerLoggedIn === 'boolean' && !isCustomerLoggedIn) return;

        fetch('/CustomersArea/Cart/count')
            .then(response => {
                if (!response.ok) throw new Error(`HTTP ${response.status}`);
                return response.json();
            })
            .then(data => {
                if (data.count > 0) updateCartCount(data.count);
            })
            .catch(error => console.error('Error fetching cart count:', error));
    });

/* ---------------------------------------------------
 * 2. 首頁 Trip Link / Promotion Link 行為 (有異常)
 * --------------------------------------------------- */
document.addEventListener("DOMContentLoaded", function () {
    const path = window.location.pathname.toLowerCase();

    // ==============================
    // 🐾 跟團行程 navLink 行為
    // ==============================
    const tripLink = document.getElementById("navTripLink");
    if (tripLink) {
        tripLink.addEventListener("click", function (e) {
            e.preventDefault();
            if (!window.isHomePage) {
                window.location.href = "/CustomersArea/Products/Browse?type=trip";
            } else {
                document.querySelector('.search-tab[data-type="trip"]')?.click();
                const wrapper = document.querySelector(".search-wrapper");
                if (wrapper) {
                    window.scrollTo({
                        top: wrapper.offsetTop - 80,
                        behavior: "smooth"
                    });
                }
            }
        });
    }

    // ==============================
    // 🐾 優惠活動 navLink 行為
    // ==============================
    const promoLink = document.querySelector('.nav-link[href="#promotions"], .nav-link[data-type="promotion"]');
    const promoNav = promoLink?.closest('li');

    if (promoLink) {
        promoLink.addEventListener("click", function (e) {
            e.preventDefault();

            if (window.isHomePage) {
                const promoTarget =
                    document.querySelector("#promotions") ||
                    document.querySelector("#promotionSection") ||
                    document.querySelector("#promotion") ||
                    document.querySelector("#hotPromotions") ||
                    document.querySelector(".promotion-wrapper");

                if (promoTarget) {
                    window.scrollTo({ top: promoTarget.offsetTop - 60, behavior: 'smooth' });
                } else {
                    console.warn('⚠️ 找不到優惠活動區塊 (#promotions)');
                }
            } else {
                window.location.href = "/CustomersArea/Promotions/Index";
            }
        });
    }

    // ✅ 當前頁為優惠活動頁時加上 active
    if (path.includes('/customersarea/promotions/index')) {
        document.querySelectorAll('.navbar-nav .nav-link').forEach(link => link.classList.remove('active'));
        if (promoLink) {
            promoLink.classList.add('active');
            if (promoNav) promoNav.classList.add('active');
        }
    }
});



    /* ---------------------------------------------------
     * 3. SweetAlert2 收藏提示 Toast（showFavResult）
     * --------------------------------------------------- */
    const favToast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 1500,
        timerProgressBar: true
    });

    window.showFavResult = function (isFav, count) {
        favToast.fire({
            icon: isFav ? 'success' : 'info',
            title: isFav ? '已加入收藏' : '已取消收藏',
            html: (typeof count === 'number')
                ? `<span style="font-size:.9rem;">目前此商品被收藏 <b>${count}</b> 次</span>`
                : ''
        });
    };

    /* ---------------------------------------------------
     * 4. 公用：idle 與選取器
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
     * 5. SweetAlert2：showAlert / showConfirm
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
            title: `🐾 ${title} `,  
            html: `${text} 🐾`,
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
     * 6. 通知系統：未讀數 / 下拉清單（事件委派）
     * --------------------------------------------------- */

    // 6.1 未讀數：小且重要 → 允許在第一階段執行
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

    // 6.2 下拉清單（字串先格式化，避免迴圈重工）
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

    // 6.3 事件委派（一次綁在 document）
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
     * 7. 桌面推播（按需）
     * --------------------------------------------------- */
    window.showDesktopNotification = async function (title, message) {
        if (!("Notification" in window)) return;
        const permission = await Notification.requestPermission();
        if (permission === "granted") {
            new Notification(title, { body: message, icon: "/images/logo.png" });
        }
    };

    /* ---------------------------------------------------
     * 8. 第一階段初始化（最小化）
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
                window.requestAnimationFrame(() => {
                    updateOnScroll(lastY);
                    ticking = false;
                });
                ticking = true;
            }
        }
        window.addEventListener('scroll', onScroll, { passive: true });
        updateOnScroll(window.scrollY || window.pageYOffset);

        if (toTop) {
            toTop.addEventListener('click', (e) => {
                e.preventDefault();
                window.scrollTo({ top: 0, behavior: 'smooth' });
            });
        }
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
     * 9. 第二階段初始化（idle/延後）
     * --------------------------------------------------- */

    // 9.1 SignalR（穩定連線 + 回補）
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

    // 9.2 Swiper：視區附近才載入腳本並初始化
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

    // 9.3 FAQ 熱門（idle 載入，使用 jQuery 以保留相容）
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

    // 9.4 導覽列「常見問題」智慧行為 + active 樣式
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

    // 9.5 「全部已讀」按鈕（navbar / page）
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

    // 9.6 延後載入通知清單策略（初始只載入未讀數）
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
     * 10. 啟動流程
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


    // 以全站通用方式處理收藏按鈕（Search/Browse/首頁卡片都會套用）

    (function () {
        // 避免同檔或多頁重複綁定
        if (window.__favBound) return;
        window.__favBound = true;

        const LOGIN_URL = '/CustomersArea/CusLogReg/Login';

        const FAV = {
            myIds: new Set(),
            inited: false,

            async init() {
                if (this.inited) return;
                this.inited = true;

                // 讀取「我已收藏」列表（未登入可能 401，忽略即可）
                try {
                    const res = await fetch('/CustomersArea/Favorites/MyIds', { credentials: 'same-origin' });
                    if (res.ok && (res.headers.get('content-type') || '').includes('application/json')) {
                        (await res.json()).forEach(id => this.myIds.add(+id));
                    }
                } catch (_) { /* ignore */ }

                this.paintAll();

                // 事件委派
                document.addEventListener('click', (ev) => this.onClick(ev));

                // DOM 變化（例如 Swiper 注入）時重刷 UI
                const mo = new MutationObserver(() => this.paintAll());
                mo.observe(document.body, { childList: true, subtree: true });
            },

            async onClick(ev) {
                const btn = ev.target.closest('.fav-btn, .fav-float');
                if (!btn) return;

                // 避免被判定拖拽 & 連點重複送出
                ev.preventDefault();
                ev.stopPropagation();
                if (btn.dataset.busy === '1') return;
                btn.dataset.busy = '1';

                const id = +btn.dataset.productId;
                if (!id) { delete btn.dataset.busy; return; }

                try {
                    const form = new URLSearchParams();
                    form.set('productId', String(id));

                    const res = await fetch('/CustomersArea/Favorites/Toggle', {
                        method: 'POST',
                        credentials: 'same-origin',
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded',
                            'X-Requested-With': 'XMLHttpRequest',   // 讓伺服器辨識為 Ajax（避免 302）
                            'Accept': 'application/json',
                            'RequestVerificationToken': (window.afToken || '')
                        },
                        body: form.toString()
                    });

                    // 未登入：可能被 302 追到 login 或直接回 401
                    if (res.status === 401 || res.redirected || (res.url && /\/login/i.test(res.url))) {
                        await this.askLogin(); return;
                    }

                    // 安全解析 JSON；不是 JSON 就當錯誤處理
                    let json;
                    if ((res.headers.get('content-type') || '').includes('application/json')) {
                        json = await res.json();
                    } else {
                        const text = await res.text();
                        throw new Error(text?.slice(0, 200) || `${res.status} ${res.statusText}`);
                    }

                    if (!json || json.ok !== true) {
                        // 伺服器主動回覆需登入
                        if (json?.redirect) { await this.askLogin(json.redirect); return; }
                        throw new Error(json?.message || '收藏操作失敗');
                    }

                    // 更新本地狀態 + 畫面
                    if (json.isFav) this.myIds.add(id); else this.myIds.delete(id);
                    this.paintById(id, json.count);

                    // SweetAlert（大彈窗）
                    Swal.fire({
                        icon: json.isFav ? 'success' : 'warning',
                        title: json.isFav ? '已加入收藏！' : '已取消收藏！',
                        text: (typeof json.count === 'number') ? `目前共有 ${json.count} 人收藏此商品` : '',
                        confirmButtonText: '確定',
                        confirmButtonColor: '#22B3C1',
                        width: '32rem'
                    });

                } catch (err) {
                    console.error(err);
                    Swal.fire({
                        icon: 'error',
                        title: '操作失敗',
                        text: (err && err.message) ? err.message : '請稍後再試',
                        confirmButtonColor: '#22B3C1'
                    });
                } finally {
                    delete btn.dataset.busy;
                }
            },

            async askLogin(redirectFromServer) {
                const result = await Swal.fire({
                    icon: 'info',
                    title: '請先登入',
                    text: '登入後即可收藏喜歡的商品。',
                    showCancelButton: true,
                    confirmButtonText: '立即登入',
                    cancelButtonText: '再等等',
                    confirmButtonColor: '#22B3C1'
                });

                // 使用者按「再等等」→ 不跳轉
                if (!result.isConfirmed) return;

                // 使用者按「立即登入」→ 導到登入頁，並帶回原頁
                const base = redirectFromServer || window.customerLoginUrl || '/CustomersArea/CusLogReg/Login';
                const returnUrl = encodeURIComponent(location.pathname + location.search);
                location.href = base.includes('returnUrl=') ? base : `${base}?returnUrl=${returnUrl}`;
            },

            paintAll() {
                document.querySelectorAll('.fav-btn, .fav-float').forEach(btn => {
                    const id = +btn.dataset.productId;
                    const isFav = this.myIds.has(id);
                    btn.classList.toggle('is-fav', isFav);
                    btn.setAttribute('aria-pressed', isFav ? 'true' : 'false');

                    const countEl = btn.querySelector('.fav-count');
                    if (countEl && countEl.dataset.loaded !== '1') {
                        fetch('/CustomersArea/Favorites/Count?productId=' + id, { credentials: 'same-origin' })
                            .then(r => r.ok ? r.json() : null)
                            .then(j => {
                                if (j && typeof j.count === 'number') {
                                    countEl.textContent = `(${j.count})`;
                                    countEl.dataset.loaded = '1';
                                }
                            }).catch(() => { });
                    }
                });
            },

            paintById(id, count) {
                document
                    .querySelectorAll(`.fav-btn[data-product-id="${id}"], .fav-float[data-product-id="${id}"]`)
                    .forEach(btn => {
                        const isFav = this.myIds.has(id);
                        btn.classList.toggle('is-fav', isFav);
                        btn.setAttribute('aria-pressed', isFav ? 'true' : 'false');
                        const countEl = btn.querySelector('.fav-count');
                        if (countEl && typeof count === 'number') {
                            countEl.textContent = `(${count})`;
                            countEl.dataset.loaded = '1';
                        }
                    });
            }
        };

        document.addEventListener('DOMContentLoaded', () => FAV.init());
    })();