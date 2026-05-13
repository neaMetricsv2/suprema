/* Suprema South Africa — site.js */

(function () {
    'use strict';

    // ── Active nav link ─────────────────────────────────────────────────────
    var path = window.location.pathname.toLowerCase();
    document.querySelectorAll('.navbar-nav .nav-link').forEach(function (link) {
        var href = (link.getAttribute('href') || '').toLowerCase();
        if (!href || href === '#') return;
        if (path === href || (href !== '/' && path.startsWith(href))) {
            link.classList.add('active');
            link.setAttribute('aria-current', 'page');
            // Also mark parent dropdown toggle active
            var dropdown = link.closest('.dropdown');
            if (dropdown) {
                var toggle = dropdown.querySelector('.dropdown-toggle');
                if (toggle) toggle.classList.add('active');
            }
        }
    });

    // ── Scroll-to-top button ─────────────────────────────────────────────────
    var scrollBtn = document.createElement('button');
    scrollBtn.id = 'scroll-top';
    scrollBtn.setAttribute('aria-label', 'Back to top');
    scrollBtn.innerHTML = '&#8679;';
    scrollBtn.style.cssText = [
        'position:fixed',
        'bottom:80px',
        'right:24px',
        'width:44px',
        'height:44px',
        'border-radius:50%',
        'border:none',
        'background:#1B3557',
        'color:#fff',
        'font-size:1.4rem',
        'line-height:1',
        'cursor:pointer',
        'opacity:0',
        'transition:opacity .3s',
        'z-index:1000',
        'box-shadow:0 2px 8px rgba(0,0,0,.25)'
    ].join(';');
    document.body.appendChild(scrollBtn);

    window.addEventListener('scroll', function () {
        scrollBtn.style.opacity = window.scrollY > 400 ? '1' : '0';
    }, { passive: true });

    scrollBtn.addEventListener('click', function () {
        window.scrollTo({ top: 0, behavior: 'smooth' });
    });

    // ── Cookie consent helper exposed on window (also defined inline in _CookieConsent.cshtml)
    // Nothing extra needed here — the inline script handles it.

})();
