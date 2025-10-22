// ----------- 登入強制跳轉（Razor注入於頁面） -----------
if (typeof isCustomerLoggedIn !== "undefined") {
    document.addEventListener('DOMContentLoaded', function () {
        if (isCustomerLoggedIn !== "true") {
            window.location.href = "/CustomersArea/CusLogReg/Login";
        }
    });
}

// ----------- Header縮放、回到頂部、Hero動畫 -----------
document.addEventListener('DOMContentLoaded', function () {
    const header = document.querySelector('.site-header');
    const toTop = document.getElementById('toTop');
    const heroMedia = document.getElementById('heroMedia');

    // Header縮放
    window.addEventListener('scroll', () => {
        const y = window.scrollY || window.pageYOffset;
        if (header) header.classList.toggle('shrink', y > 8);
        if (toTop) toTop.classList.toggle('show', y > 480);
        if (heroMedia) {
            const clamp = Math.min(Math.max(y, 0), 280);
            heroMedia.style.transform = `scale(${1.1 + clamp / 2800}) translateY(${clamp * 0.06}px)`;
        }
    });

    // 回到頂部按鈕
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
            let answer = faq.answer ? faq.answer.replace(/<\s*p(\s+[^>]*)?>/gi, '<div$1>').replace(/<\s*\/\s*p\s*>/gi, '</div>') : '';
            let answerText = answer.replace(/<[^>]+>/g, '').replace(/&nbsp;/g, '').trim();
            let answerHtml = answerText ? answer : '<div class="text-muted">暫無答案</div>';
            html += `
            <div class="accordion-item">
                <h2 class="accordion-header" id="homeHotHeading${idx}">
                    <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse"
                        data-bs-target="#homeHotCollapse${idx}" aria-expanded="false" aria-controls="homeHotCollapse${idx}">
                        ${faq.question}
                    </button>
                </h2>
                <div id="homeHotCollapse${idx}" class="accordion-collapse collapse"
                    aria-labelledby="homeHotHeading${idx}" data-bs-parent="#homeHotFaqAccordionInner">
                    <div class="accordion-body">${answerHtml}</div>
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

    // ✅ 修正版：更嚴謹判斷首頁
    const isHome =
        path === '/customersarea/home/index' ||
        path === '/customersarea/home' ||
        path === '/customersarea' ||
        path === '/customersarea/' ||
        path === '/customersarea/home/index/';

    if (faqLink) {
        faqLink.addEventListener('click', function (e) {
            e.preventDefault(); // ✅ 先阻止預設行為

            if (isHome) {
                // ✅ 在首頁 → 平滑滾動到 FAQ 區塊
                const faqTarget = document.querySelector('#faqSection');
                if (faqTarget) {
                    window.scrollTo({
                        top: faqTarget.offsetTop - 60,
                        behavior: 'smooth'
                    });
                } else {
                    console.warn('⚠️ 找不到 #faqSection 元素');
                }
            } else {
                // ✅ 不在首頁 → 導向 FAQ 專頁
                window.location.href = '/CustomersArea/FrontFAQs/Index';
            }
        });
    }





    // ----------- 自動標示 active（在 FAQ 頁面時） -----------
    if (path.includes('/customersarea/frontfaqs/index')) {
        document.querySelectorAll('.navbar-nav .nav-link').forEach(link => link.classList.remove('active'));
        if (faqLink) {
            faqLink.classList.add('active');
            if (faqNav) faqNav.classList.add('active');
        }
    }

    // ----------- 支援一般錨點平滑滾動（首頁其他區） -----------
    $('.nav-link[href^="#"]').on('click', function (e) {
        const target = $(this).attr('href');
        if ($(target).length) {
            e.preventDefault();
            $('html, body').animate({
                scrollTop: $(target).offset().top - 60
            }, 500);
        }
    });
});
