/**
 * main.js - 修正版
 */

'use strict';

document.addEventListener("DOMContentLoaded", function () {
    let menu, animate;

    // ---------------------
    // Initialize menu
    // ---------------------
    let layoutMenuEls = document.querySelectorAll('#layout-menu');
    layoutMenuEls.forEach(function (element) {
        if (element) {
            try {
                menu = new Menu(element, {
                    orientation: 'vertical',
                    closeChildren: false
                });
                window.Helpers.scrollToActive((animate = false));
                window.Helpers.mainMenu = menu;
            } catch (e) {
                console.warn("Menu 初始化失敗:", e);
            }
        }
    });

    // ---------------------
    // Menu toggler click
    // ---------------------
    let menuTogglers = document.querySelectorAll('.layout-menu-toggle');
    menuTogglers.forEach(item => {
        item.addEventListener('click', event => {
            event.preventDefault();
            window.Helpers.toggleCollapsed();
        });
    });

    // ---------------------
    // Menu hover delay
    // ---------------------
    let delayHover = function (elem, callback) {
        let timeout = null;
        elem.onmouseenter = function () {
            if (!Helpers.isSmallScreen()) {
                timeout = setTimeout(callback, 300);
            } else {
                timeout = setTimeout(callback, 0);
            }
        };
        elem.onmouseleave = function () {
            document.querySelector('.layout-menu-toggle')?.classList.remove('d-block');
            clearTimeout(timeout);
        };
    };
    let layoutMenu = document.getElementById('layout-menu');
    if (layoutMenu) {
        delayHover(layoutMenu, function () {
            if (!Helpers.isSmallScreen()) {
                document.querySelector('.layout-menu-toggle')?.classList.add('d-block');
            }
        });
    }

    // ---------------------
    // Menu scroll shadow
    // ---------------------
    let menuInnerContainer = document.getElementsByClassName('menu-inner'),
        menuInnerShadow = document.getElementsByClassName('menu-inner-shadow')[0];
    if (menuInnerContainer.length > 0 && menuInnerShadow) {
        menuInnerContainer[0].addEventListener('ps-scroll-y', function () {
            if (this.querySelector('.ps__thumb-y')?.offsetTop) {
                menuInnerShadow.style.display = 'block';
            } else {
                menuInnerShadow.style.display = 'none';
            }
        });
    }

    // ---------------------
    // Bootstrap Tooltip
    // ---------------------
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.forEach(function (el) {
        new bootstrap.Tooltip(el);
    });

    // ---------------------
    // Accordion active class
    // ---------------------
    const accordionActiveFunction = function (e) {
        if (e.type === 'show.bs.collapse') {
            e.target.closest('.accordion-item')?.classList.add('active');
        } else {
            e.target.closest('.accordion-item')?.classList.remove('active');
        }
    };
    const accordionList = [].slice.call(document.querySelectorAll('.accordion'));
    accordionList.forEach(function (accordionEl) {
        accordionEl.addEventListener('show.bs.collapse', accordionActiveFunction);
        accordionEl.addEventListener('hide.bs.collapse', accordionActiveFunction);
    });

    // ---------------------
    // Layout helpers
    // ---------------------
    window.Helpers.setAutoUpdate(true);
    window.Helpers.initPasswordToggle();
    window.Helpers.initSpeechToText();

    // ---------------------
    // Menu collapsed for large screens
    // ---------------------
    if (!window.Helpers.isSmallScreen()) {
        window.Helpers.setCollapsed(true, false);
    }
});
