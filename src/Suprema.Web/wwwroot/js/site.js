/* Suprema South Africa — site.js */

(function () {
    'use strict';

    // ── Scroll-driven header (transparent → white) ───────────────────────────
    var header = document.getElementById('site-header');
    if (header) {
        var SCROLL_THRESHOLD = 80;
        function updateHeader() {
            if (window.scrollY > SCROLL_THRESHOLD) {
                header.classList.remove('header-transparent');
                header.classList.add('header-white');
            } else {
                header.classList.remove('header-white');
                header.classList.add('header-transparent');
            }
        }
        updateHeader();
        window.addEventListener('scroll', updateHeader, { passive: true });
    }

    // ── Active nav link ──────────────────────────────────────────────────────
    var path = window.location.pathname.toLowerCase();
    document.querySelectorAll('.navbar-nav .nav-link').forEach(function (link) {
        var href = (link.getAttribute('href') || '').toLowerCase();
        if (!href || href === '#') return;
        if (path === href || (href !== '/' && path.startsWith(href))) {
            link.classList.add('active');
            link.setAttribute('aria-current', 'page');
            var dropdown = link.closest('.dropdown');
            if (dropdown) {
                var toggle = dropdown.querySelector('.dropdown-toggle');
                if (toggle) toggle.classList.add('active');
            }
        }
    });

    // ── Scroll reveal (IntersectionObserver) ────────────────────────────────
    if ('IntersectionObserver' in window) {
        var revealObserver = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add('visible');
                    revealObserver.unobserve(entry.target);
                }
            });
        }, { threshold: 0.12, rootMargin: '0px 0px -48px 0px' });

        document.querySelectorAll('.reveal').forEach(function (el) {
            revealObserver.observe(el);
        });
    } else {
        // Fallback: show all immediately
        document.querySelectorAll('.reveal').forEach(function (el) {
            el.classList.add('visible');
        });
    }

    // ── Swiper — featured products carousel ─────────────────────────────────
    if (typeof Swiper !== 'undefined' && document.querySelector('.products-swiper')) {
        new Swiper('.products-swiper', {
            slidesPerView: 1,
            spaceBetween: 24,
            loop: true,
            pagination: {
                el: '.products-swiper .swiper-pagination',
                clickable: true
            },
            navigation: {
                nextEl: '.products-swiper .swiper-button-next',
                prevEl: '.products-swiper .swiper-button-prev'
            },
            breakpoints: {
                576:  { slidesPerView: 2 },
                992:  { slidesPerView: 3 },
                1200: { slidesPerView: 4 }
            }
        });
    }

    // ── Scroll-to-top button ─────────────────────────────────────────────────
    var scrollBtn = document.createElement('button');
    scrollBtn.id = 'scroll-top';
    scrollBtn.setAttribute('aria-label', 'Back to top');
    scrollBtn.innerHTML = '&#8679;';
    scrollBtn.style.cssText = [
        'position:fixed',
        'bottom:90px',
        'right:28px',
        'width:48px',
        'height:48px',
        'border-radius:50%',
        'border:none',
        'background:#a12944',
        'color:#fff',
        'font-size:1.5rem',
        'line-height:1',
        'cursor:pointer',
        'opacity:0',
        'transition:opacity .3s',
        'z-index:1000',
        'box-shadow:5px 5px 30px rgba(0,0,0,.35)',
        'display:flex',
        'align-items:center',
        'justify-content:center'
    ].join(';');
    document.body.appendChild(scrollBtn);

    window.addEventListener('scroll', function () {
        scrollBtn.style.opacity = window.scrollY > 400 ? '1' : '0';
    }, { passive: true });

    scrollBtn.addEventListener('click', function () {
        window.scrollTo({ top: 0, behavior: 'smooth' });
    });

})();
