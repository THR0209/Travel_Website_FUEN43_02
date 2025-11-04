// ===================================================
// 🐾 Cat Paw Footprint - Web Front 全域前端腳本
// 功能：導覽列互動、通知系統、彈窗提示、動畫初始化
// ===================================================

// ----------- 統一彈窗函式（全域通用） -----------
window.showAlert = function (type, title, text, timer = 1000) {
    Swal.fire({
        icon: type,
        title: `🐾 ${title}`,
        text,
        timer,
        showConfirmButton: false,
        timerProgressBar: true,
        toast: false,
        position: "center",
        background: "#fff",
        customClass: { popup: 'shadow-sm rounded-3' }
    });
};

// ----------- 全域函式：更新未讀通知數 -----------
window.updateUnread = async function () {
    try {
        const res = await axios.get("/CustomersArea/Notifications/GetUnreadCount");
        const count = res.data.count;
        const badge = document.getElementById("notifBadge");

        if (!badge) return;

        if (count > 0) {
            badge.style.display = "inline-block";
            badge.innerText = count > 99 ? "99+" : count;
        } else {
            badge.style.display = "none";
        }
    } catch (e) {
        console.error("❌ 更新未讀通知數失敗", e);
    }
};

// ----------- 全域函式：更新通知下拉列表 -----------
window.updateList = async function () {
    try {
        const res = await axios.get("/CustomersArea/Notifications/GetLatestNotifications");
        const data = res.data;
        const list = document.getElementById("notifList");

        if (!list) return;

        if (!data || data.length === 0) {
            list.innerHTML = `<div class="text-center text-muted py-3">目前沒有通知 💤</div>`;
            return;
        }

        // === 通知清單動態生成 ===
        list.innerHTML = data.map(n => `
            <div class="notif-item border px-3 py-2 ${n.isRead ? 'opacity-50' : ''}" 
                 data-id="${n.notificationID}" style="cursor:pointer;">
                <div class="fw-bold text-truncate">${n.title}</div>
                <div class="small text-muted text-truncate">${n.message}</div>
                <div class="text-end small text-secondary">${dayjs(n.createdAt).format('MM/DD HH:mm')}</div>
            </div>
        `).join('');

        // === 🔹 綁定通知點擊事件（客服 + 訂單 + 優惠活動導向） ===
        document.querySelectorAll('.notif-item').forEach(item => {
            item.addEventListener('click', async function () {
                const id = this.dataset.id;
                const title = this.querySelector('.fw-bold')?.textContent || "";
                const msg = this.querySelector('.small.text-muted')?.textContent || "";

                try {
                    // ✅ 標記已讀
                    await axios.post('/CustomersArea/Notifications/MarkAsRead', { id });
                    this.classList.add('opacity-50');
                    await window.updateUnread();

                    // ✅ 客服通知導向
                    if (
                        title.includes('客服服務已完成') ||
                        title.includes('客服評價提醒') ||
                        title.includes('客服回覆') ||
                        title.includes('客服訊息')
                    ) {
                        const match = msg.match(/#\s*(\d+)/);
                        if (match && match[1]) {
                            window.location.href = `/CustomersArea/CustomerService/Index?ticketId=${match[1]}`;
                            return;
                        }
                    }

                    // ✅ 訂單通知導向
                    if (
                        title.includes('訂單成立通知') ||
                        title.includes('付款成功通知') ||
                        title.includes('訂單狀態') ||
                        title.includes('系統公告')
                    ) {
                        const match = msg.match(/#\s*(\d+)/);
                        if (match && match[1]) {
                            window.location.href = `/CustomersArea/Orders?orderId=${match[1]}`;
                        } else {
                            window.location.href = `/CustomersArea/Orders`;
                        }
                        return;
                    }

                    // ✅ 其他通知導向通知中心
                    if (title.includes('優惠') || title.includes('公告') || title.includes('提醒')) {
                        window.location.href = `/CustomersArea/Notifications/Index`;
                        return;
                    }

                } catch (err) {
                    console.error("❌ 標記通知為已讀失敗", err);
                    window.showAlert('error', '錯誤', '無法標記通知為已讀');
                }
            });
        });

    } catch (e) {
        console.error("載入通知清單失敗", e);
        window.showAlert('warning', '載入失敗', '通知清單載入失敗');
    }
};



// ----------- 全域函式：桌面推播通知 -----------
window.showDesktopNotification = async function (title, message) {
    if (!("Notification" in window)) return;
    const permission = await Notification.requestPermission();
    if (permission === "granted") {
        new Notification(title, { body: message, icon: "/images/logo.png" });
    }
};

// ----------- 主程式初始化 -----------
document.addEventListener('DOMContentLoaded', function () {

    // ----------- 登入強制跳轉（Razor注入於頁面） -----------
    if (typeof isCustomerLoggedIn !== "undefined" && isCustomerLoggedIn !== "true") {
        window.location.href = "/CustomersArea/CusLogReg/Login";
        return;
    }

    // ----------- Header縮放、回到頂部、Hero動畫 -----------
    const header = document.querySelector('.site-header');
    const toTop = document.getElementById('backToTop');
    const heroMedia = document.getElementById('heroMedia');

    window.addEventListener('scroll', () => {
        const y = window.scrollY || window.pageYOffset;
        if (header) header.classList.toggle('shrink', y > 8);
        if (toTop) toTop.classList.toggle('show', y > 480);
        if (heroMedia) {
            const clamp = Math.min(Math.max(y, 0), 280);
            heroMedia.style.transform = `scale(${1.1 + clamp / 2800}) translateY(${clamp * 0.06}px)`;
        }
    });

    if (toTop) toTop.addEventListener('click', () => window.scrollTo({ top: 0, behavior: 'smooth' }));

    // ----------- Swiper熱門輪播 -----------
    if (typeof Swiper !== 'undefined') {
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
    }

    // ----------- 全部已讀按鈕事件 -----------
    const btn = document.getElementById('markAllReadBtn');
    if (btn) {
        btn.addEventListener('click', async () => {
            try {
                const res = await axios.post('/CustomersArea/Notifications/MarkAllAsRead');
                if (res.data.success) {
                    window.showAlert('success', '通知中心', '全部通知已標記為已讀 🐾');
                    await window.updateUnread();
                    await window.updateList();
                } else {
                    window.showAlert('warning', '操作失敗', res.data.message || '請稍後再試');
                }
            } catch (err) {
                console.error('❌ 全部已讀錯誤:', err);
                window.showAlert('error', '錯誤', '伺服器連線失敗');
            }
        });
    }


    // ----------- 熱門FAQ Accordion載入（首頁） -----------
    $.getJSON('/CustomersArea/FrontFAQs/api/hot', function (faqs) {
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
        $('#homeHotFaqAccordion').html(html);
    });

    // ----------- 導覽列「常見問題」智慧行為 + active 樣式 -----------
    const path = window.location.pathname.toLowerCase();
    const faqLink = document.querySelector('.faq-nav-link');
    const faqNav = faqLink?.closest('li');

    if (faqLink) {
        faqLink.addEventListener('click', function (e) {
            e.preventDefault();

            // 1️ 如果目前在首頁 → 平滑滾動到 FAQ 區
            if (path.includes('/customersarea/home') || path === '/customersarea' || path === '/customersarea/') {
                const faqTarget = document.querySelector('#faqSection');
                if (faqTarget) {
                    window.scrollTo({ top: faqTarget.offsetTop - 60, behavior: 'smooth' });
                } else {
                    console.warn('⚠️ 找不到 #faqSection 元素');
                }
            }
            // 2️ 不在首頁 → 先導向首頁並自動滾動
            else {
                window.location.href = '/CustomersArea/FrontFAQs/Index';
            }
        });
    }

    // 加上 active 樣式判斷（仍保留原行為）
    if (path.includes('/customersarea/frontfaqs/index')) {
        document.querySelectorAll('.navbar-nav .nav-link').forEach(link => link.classList.remove('active'));
        if (faqLink) {
            faqLink.classList.add('active');
            if (faqNav) faqNav.classList.add('active');
        }
    }


    // ----------- 支援一般錨點平滑滾動 -----------
    $('.nav-link[href^="#"]').on('click', function (e) {
        const target = $(this).attr('href');

        // 🚫 忽略 href="#" 或空值，避免 jQuery 選擇器錯誤
        if (!target || target === "#") return;

        const $target = $(target);
        if ($target.length) {
            e.preventDefault();
            $('html, body').animate({ scrollTop: $target.offset().top - 60 }, 500);
        }
    });

    // ----------- 即時通知系統（SignalR） -----------
    const badge = document.getElementById("notifBadge");
    const list = document.getElementById("notifList");

    // ✅ 新增自動重連設定
    window.connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub", {
            withCredentials: true
         })
        .withAutomaticReconnect([0, 2000, 5000, 10000])
        .build();

    // ✅ 設定接收事件
    connection.on("ReceiveNotification", (title, message, type) => {
        console.log("📨 收到通知:", { title, message, type });

        const path = window.location.pathname.toLowerCase();

        // 🚫 若目前在客服中心或通知中心，略過彈窗與桌面通知
        const isInSilentPage =
            path.includes("/customersarea/customerservice") ||
            path.includes("/customersarea/notifications");

        if (isInSilentPage) {
            console.log("🚫 使用者目前在客服中心或通知中心，略過通知提示。");
            // ✅ 仍保持未讀同步（但不顯示彈窗）
            window.updateUnread();
            return;
        }

        window.showAlert('info', title, message, 4000);
        window.updateUnread();
        window.updateList();
        window.showDesktopNotification(title, message);
    });

    // ✅ 連線關閉時自動刷新 cookie 並重連
    connection.onclose(async () => {
        console.warn("🔴 SignalR 已斷線，嘗試刷新 cookie 後重連...");
        try {
            // 🔹 用 axios 發一個 request，確保 cookie 有附上
            await axios.get("/CustomersArea/Notifications/GetUnreadCount");
        } catch { }
        // 🔹 延遲 1.5 秒後重新啟動連線
        setTimeout(startConnection, 1500);
    });

    // ✅ 自動重連機制
    connection.onreconnected(() => {
        console.log("🔁 SignalR 已重新連線");
        window.updateUnread();
        window.updateList();
    });

    // ✅ 啟動連線（含重試機制）
    async function startConnection() {
        try {
            await connection.start();
            console.log("✅ SignalR 已連線");
            await window.updateUnread();
            await window.updateList();
        } catch (err) {
            console.error("SignalR 連線失敗，5秒後重試:", err);
            setTimeout(startConnection, 5000);
        }
    }

    // 🔹 延遲執行：確保 Cookie / Service Worker / Vue 都已初始化
    window.addEventListener('load', () => {
        setTimeout(startConnection, 800); // 延遲 0.8 秒啟動 SignalR
    });



    // ----------- 登出時中斷 SignalR -----------
    const logoutForm = document.querySelector('form[action*="CusLogReg/Logout"]');
    if (logoutForm) {
        logoutForm.addEventListener('submit', function () {
            try {
                if (connection && connection.stop) connection.stop();
            } catch (e) { console.warn("SignalR 停止失敗", e); }
            if (badge) badge.style.display = "none";
            if (list) list.innerHTML = `<div class="text-center text-muted py-3">請重新登入後查看通知</div>`;
        });
    }

    // ----------- 初始化通知系統 -----------
    window.updateUnread();
    window.updateList();
});

