// ===================================================
// 🐾 Cat Paw Footprint - Web Front 全域前端腳本
// 功能：導覽列互動、通知系統、彈窗提示、動畫初始化
// ===================================================

// ----------- 統一彈窗函式（全域通用） -----------
window.showAlert = function (type, title, text, timer = 2000) {
    Swal.fire({
        icon: type,
        title,
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

        list.innerHTML = data.map(n => `
            <div class="border-bottom py-2 px-2 ${n.isRead ? 'opacity-50' : ''}">
                <div class="fw-bold">${n.title}</div>
                <div class="small text-muted">${n.message}</div>
                <div class="text-end small text-secondary">${dayjs(n.createdAt).format('MM/DD HH:mm')}</div>
            </div>
        `).join('');
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
    const toTop = document.getElementById('toTop');
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

    const isHome =
        path.includes('/customersarea/home') ||
        path === '/customersarea' ||
        path === '/customersarea/';

    if (faqLink) {
        faqLink.addEventListener('click', function (e) {
            e.preventDefault();
            if (isHome) {
                const faqTarget = document.querySelector('#faqSection');
                if (faqTarget) window.scrollTo({ top: faqTarget.offsetTop - 60, behavior: 'smooth' });
                else console.warn('⚠️ 找不到 #faqSection 元素');
            } else {
                window.location.href = '/CustomersArea/FrontFAQs/Index';
            }
        });
    }

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
        if ($(target).length) {
            e.preventDefault();
            $('html, body').animate({ scrollTop: $(target).offset().top - 60 }, 500);
        }
    });

    // ----------- 即時通知系統（SignalR） -----------
    const badge = document.getElementById("notifBadge");
    const list = document.getElementById("notifList");

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .build();

    connection.on("ReceiveNotification", (title, message, type) => {
        window.showAlert('info', title, message, 4000);
        window.updateUnread();
        window.updateList();
        window.showDesktopNotification(title, message);
    });

    connection.start()
        .then(() => console.log("✅ SignalR 已連線"))
        .catch(err => {
            console.error("SignalR 錯誤：", err);
            window.showAlert('error', '通知系統錯誤', '無法連線至伺服器');
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
