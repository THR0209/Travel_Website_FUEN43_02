// ===============================
// 回頂部按鈕
// ===============================
(() => {
    $(window).scroll(function () {
        if ($(this).scrollTop() > 200) {
            $('#backToTop').fadeIn();
        } else {
            $('#backToTop').fadeOut();
        }
    });
    $('#backToTop').click(function () {
        $('html, body').animate({ scrollTop: 0 }, 300);
        return false;
    });
})();


// ===============================
// 
// ===============================