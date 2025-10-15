
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
if (typeof Swiper !== 'undefined') {
    const swiper = new Swiper('.swiper', {
        slidesPerView: 1.2,
        spaceBetween: 12,
        navigation: { nextEl: '.swiper-next', prevEl: '.swiper-prev' },
        breakpoints: { 576: { slidesPerView: 2, spaceBetween: 16 }, 992: { slidesPerView: 3, spaceBetween: 18 }, 1200: { slidesPerView: 4, spaceBetween: 18 } },
        keyboard: { enabled: true },
        a11y: { enabled: true }
    });
}


$(function () {
    // 熱門FAQ Accordion Only
    $.getJSON('/CustomersArea/FrontFAQs/api/hot', function (faqs) {
        let html = `<div class="accordion" id="homeHotFaqAccordionInner">`;
        faqs.forEach((faq, idx) => {
            // 將答案的 <p> 標籤轉成 <div>
            let answer = faq.answer ? faq.answer.replace(/<\s*p(\s+[^>]*)?>/gi, '<div$1>').replace(/<\s*\/\s*p\s*>/gi, '</div>') : '';
            // 如果答案內容空，顯示「暫無答案」
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
            </div>
            `;
        });
        html += `</div>`;
        $('#homeHotFaqAccordion').html(html);
    });

    // 平滑捲動
    $('.nav-link[href="#faqSection"]').on('click', function (e) {
        e.preventDefault();
        $('html, body').animate({
            scrollTop: $('#faqSection').offset().top - 60 // header高度
        }, 500);
    });
});