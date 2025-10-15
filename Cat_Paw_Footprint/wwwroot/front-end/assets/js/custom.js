(function ($) {
    "use strict";

    $(window).on('load', function () {
        $('#js-preloader').addClass('loaded');

        if ($('.cover').length) {
            $('.cover').parallax({
                imageSrc: $('.cover').data('image'),
                zIndex: '1'
            });
        }

        $("#preloader").animate({ 'opacity': '0' }, 600, function () {
            setTimeout(function () {
                $("#preloader").css("visibility", "hidden").fadeOut();
            }, 300);
        });

        // WOW JS
        if ($(".wow").length) {
            new WOW({ boxClass: 'wow', animateClass: 'animated', offset: 20, mobile: true, live: true }).init();
        }
    });

    $(window).on('scroll', function () {
        // 設定淡出起始/結束滾動高度
        var fadeStart = 0;       // 開始淡出 (例如 header-row 底部)
        var fadeEnd = 150;       // 滾到 150px 時完全消失
        var scroll = $(window).scrollTop();
        var $secondNav = $('.second-nav-area');

        // 計算淡出透明度
        if (scroll <= fadeStart) {
            $secondNav.css('opacity', 1);
        } else if (scroll >= fadeEnd) {
            $secondNav.css('opacity', 0);
        } else {
            var opacity = 1 - ((scroll - fadeStart) / (fadeEnd - fadeStart));
            $secondNav.css('opacity', opacity);
        }
    });

    $(function () {
        function bannerSwitcher() {
            let next = $('.sec-1-input').filter(':checked').next('.sec-1-input');
            if (next.length) next.prop('checked', true);
            else $('.sec-1-input').first().prop('checked', true);
        }

        var bannerTimer = setInterval(bannerSwitcher, 5000);

        $('nav .controls label').on('click', function () {
            clearInterval(bannerTimer);
            bannerTimer = setInterval(bannerSwitcher, 5000);
        });
    });

    // Header background on scroll
    $(window).scroll(function () {
        var scroll = $(window).scrollTop();
        var box = $('.header-text').height();
        var header = $('header').height();
        if (scroll >= box - header) $("header").addClass("background-header");
        else $("header").removeClass("background-header");
    });

    $(document).ready(function () {
        // 初始化 Isotope
        var $grid = $(".grid").isotope({
            itemSelector: ".all",
            percentPosition: true,
            masonry: { columnWidth: ".all" }
        });

        // 篩選按鈕
        $('.filters ul li').click(function () {
            $('.filters ul li').removeClass('active');
            $(this).addClass('active');

            var filterValue = $(this).data('filter');

            // 防止非法 selector
            if (/^(\*|[.#][\w-]+)$/.test(filterValue)) {
                $grid.isotope({ filter: filterValue });
            } else { }
            $grid.isotope({ filter: filterValue });
        });

        $(document).ready(function () {
            $('.package-carousel').owlCarousel({
                loop: true,
                margin: 20,
                nav: true,
                dots: true,
                responsive: {
                    0: { items: 1 },
                    576: { items: 2 },
                    768: { items: 3 },
                    992: { items: 4 },
                    1200: { items: 5 }
                }
            });
            $('.spot-carousel').owlCarousel({
                loop: true,
                margin: 20,
                nav: true,
                dots: true,
                responsive: {
                    0: { items: 1 },
                    576: { items: 2 },
                    768: { items: 3 },
                    992: { items: 4 },
                    1200: { items: 5 }
                }
            });
        });


        // Naccs menu tabs
        $(document).on("click", ".naccs .menu div", function () {
            var index = $(this).index();
            if (!$(this).hasClass("active")) {
                $(".naccs .menu div, .naccs ul li").removeClass("active");
                $(this).addClass("active");
                $(".naccs ul li:eq(" + index + ")").addClass("active");
                $(".naccs ul").height($(".naccs ul li:eq(" + index + ")").innerHeight());
            }
        });

        // Owl Carousel
        $('.owl-cites-town').owlCarousel({ items: 4, loop: true, dots: false, nav: true, autoplay: true, margin: 30, responsive: { 0: { items: 1 }, 800: { items: 2 }, 1000: { items: 4 } } });
        $('.owl-weekly-offers').owlCarousel({ items: 3, loop: true, dots: false, nav: true, autoplay: true, margin: 15, responsive: { 0: { items: 1 }, 800: { items: 2 }, 1000: { items: 3 } } });
        $('.owl-banner').owlCarousel({ items: 1, loop: true, dots: false, nav: true, autoplay: true, margin: 30, responsive: { 0: { items: 1 }, 600: { items: 1 }, 1000: { items: 1 } } });

        // Menu Toggle
        $(".menu-trigger").on('click', function () {
            $(this).toggleClass('active');
            $('.header-area .nav').slideToggle(200);
        });

        // Smooth scroll
        $('.second-nav-area a[href^="#"]').on('click', function (e) {
            e.preventDefault();
            $(document).off("scroll");
            $('.scroll-to-section a').removeClass('active');
            $(this).addClass('active');

            var target = $(this.hash);
            if (target.length) {
                var headerHeight = $('header').outerHeight() || 0;
                var secondNavHeight = $('.second-nav-area').outerHeight() || 0;
                var totalOffset = headerHeight + secondNavHeight; 
                $('html, body').stop().animate({ scrollTop: target.offset().top - totalOffset }, 500, 'swing', function () {
                    window.location.hash = target.selector;
                    $(document).on("scroll", onScroll);
                });
            }
        }); 

        function onScroll() {
            var scrollPos = $(document).scrollTop();
            $('.nav a').each(function () {
                var currLink = $(this);
                var refElement = $(currLink.attr("href"));
                // 加判斷 refElement 是否存在
                if (refElement.length > 0) {
                    if (refElement.position().top <= scrollPos && refElement.position().top + refElement.height() > scrollPos) {
                        $('.nav ul li a').removeClass("active");
                        currLink.addClass("active");
                    } else {
                        currLink.removeClass("active");
                    }
                }
            });
        }

        $(document).on("scroll", onScroll);
    });

})(window.jQuery);