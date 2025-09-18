// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(() => {
    // ===============================
    // 顯示/隱藏按鈕
    // ===============================
    $(window).scroll(function () {
        if ($(this).scrollTop() > 300) { $('#backToTop').fadeIn(); }
        else { $('#backToTop').fadeOut(); }
    });

    // ===============================
    // 點擊回到頂部
    // ===============================
    $('#backToTop').click(function () {
        $('html, body').animate({ scrollTop: 0 }, 200);
    });
})();
