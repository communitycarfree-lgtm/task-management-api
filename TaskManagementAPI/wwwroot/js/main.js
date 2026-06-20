/* ============================================================
   Task Management API — main.js
   ============================================================ */

(function () {
  'use strict';

  /* ----------------------------------------------------------
     PAGE LOADER — shimmer effect from logo
  ---------------------------------------------------------- */
  function initLoader() {
    const loader = document.getElementById('page-loader');
    if (!loader) return;

    // Allow CSS fonts / images a moment to settle, then dismiss
    const dismiss = () => {
      loader.classList.add('hidden');
      loader.addEventListener('transitionend', () => loader.remove(), { once: true });
    };

    if (document.readyState === 'complete') {
      setTimeout(dismiss, 600);
    } else {
      window.addEventListener('load', () => setTimeout(dismiss, 600), { once: true });
    }
  }

  /* ----------------------------------------------------------
     THEME — persist with localStorage, sync icon
  ---------------------------------------------------------- */
  function initTheme() {
    const saved = localStorage.getItem('tm-theme');
    // Use saved preference; otherwise keep the HTML-defined default (dark) — don't override with system pref.
    const theme = saved || document.documentElement.getAttribute('data-theme') || 'dark';
    applyTheme(theme);

    document.querySelectorAll('[data-theme-toggle]').forEach(btn => {
      btn.addEventListener('click', () => {
        const next = document.documentElement.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
        applyTheme(next);
        localStorage.setItem('tm-theme', next);
      });
    });
  }

  function applyTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme);
    document.querySelectorAll('[data-theme-toggle]').forEach(btn => {
      btn.innerHTML = theme === 'dark'
        ? '<i class="fas fa-sun" aria-hidden="true"></i>'
        : '<i class="fas fa-moon" aria-hidden="true"></i>';
      btn.setAttribute('aria-label', theme === 'dark' ? 'Switch to light mode' : 'Switch to dark mode');
    });
  }

  /* ----------------------------------------------------------
     MOBILE NAV — hamburger toggle
  ---------------------------------------------------------- */
  function initMobileNav() {
    const nav = document.querySelector('.site-nav');
    const burger = document.querySelector('.hamburger');
    const backdrop = document.querySelector('.nav-backdrop');
    if (!nav || !burger) return;

    const open  = () => { nav.classList.add('open'); burger.classList.add('open'); if (backdrop) backdrop.classList.add('open'); document.body.style.overflow = 'hidden'; };
    const close = () => { nav.classList.remove('open'); burger.classList.remove('open'); if (backdrop) backdrop.classList.remove('open'); document.body.style.overflow = ''; };

    burger.addEventListener('click', () => nav.classList.contains('open') ? close() : open());
    if (backdrop) backdrop.addEventListener('click', close);
    nav.querySelectorAll('a').forEach(a => a.addEventListener('click', close));
  }

  /* ----------------------------------------------------------
     ACTIVE NAV LINK — highlight current page
  ---------------------------------------------------------- */
  function initActiveNav() {
    const page = location.pathname.split('/').pop() || 'index.html';
    document.querySelectorAll('.site-nav a, .sidebar-link').forEach(a => {
      const href = a.getAttribute('href');
      if (!href) return;
      // Nav links: match by filename
      if (!href.startsWith('#') && (href === page || (page === '' && href === 'index.html'))) {
        a.classList.add('active');
      }
    });
  }

  /* ----------------------------------------------------------
     SIDEBAR — scroll-spy + search filter
  ---------------------------------------------------------- */
  function initSidebar() {
    // Search filter
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
      searchInput.addEventListener('input', () => {
        const q = searchInput.value.toLowerCase();
        document.querySelectorAll('.sidebar-link').forEach(link => {
          link.style.display = (!q || link.textContent.toLowerCase().includes(q)) ? '' : 'none';
        });
      });
    }

    // Scroll-spy
    const sections = Array.from(document.querySelectorAll('section[id]'));
    if (!sections.length) return;

    const sidebarLinks = document.querySelectorAll('.sidebar-link[href^="#"]');
    const HEADER_H = parseInt(getComputedStyle(document.documentElement).getPropertyValue('--header-h')) || 68;

    const spy = () => {
      const scrollY = window.scrollY + HEADER_H + 32;
      let current = sections[0].id;
      sections.forEach(s => { if (s.offsetTop <= scrollY) current = s.id; });
      sidebarLinks.forEach(l => l.classList.toggle('active', l.getAttribute('href') === '#' + current));
    };

    window.addEventListener('scroll', spy, { passive: true });
    spy();
  }

  /* ----------------------------------------------------------
     CODE — copy to clipboard
  ---------------------------------------------------------- */
  function initCodeCopy() {
    document.querySelectorAll('.code-block').forEach(block => {
      const code = block.querySelector('code');
      const btn  = block.querySelector('.copy-btn');
      if (!code || !btn) return;

      btn.addEventListener('click', async () => {
        try {
          await navigator.clipboard.writeText(code.innerText);
          btn.innerHTML = '<i class="fas fa-check"></i> Copied!';
          btn.classList.add('copied');
          setTimeout(() => {
            btn.innerHTML = '<i class="far fa-copy"></i> Copy';
            btn.classList.remove('copied');
          }, 2200);
        } catch {
          btn.textContent = 'Error';
        }
      });
    });
  }

  /* ----------------------------------------------------------
     SYNTAX HIGHLIGHTING — lightweight, safe
  ---------------------------------------------------------- */
  function initHighlight() {
    document.querySelectorAll('.code-block code, .code-snippet').forEach(el => {
      let html = el.textContent
        .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');

      // JSON keys
      html = html.replace(/"([^"]+)"\s*:/g, '<span class="hl-key">"$1"</span>:');
      // JSON string values
      html = html.replace(/:\s*"([^"]*)"/g, ': <span class="hl-string">"$1"</span>');
      // JSON booleans/null
      html = html.replace(/\b(true|false)\b/g, '<span class="hl-bool">$1</span>');
      html = html.replace(/\bnull\b/g, '<span class="hl-null">null</span>');
      // HTTP methods
      html = html.replace(/\b(GET|POST|PUT|DELETE|PATCH)\b/g, '<span class="hl-method">$1</span>');
      // Numbers (standalone)
      html = html.replace(/(?<!["\w])(\d+)(?!["\w])/g, '<span class="hl-number">$1</span>');
      // Bearer token
      html = html.replace(/(Bearer\s)(\S+)/g, '$1<span class="hl-token">$2</span>');

      el.innerHTML = html;
    });
  }

  /* ----------------------------------------------------------
     SCROLL REVEAL — IntersectionObserver
  ---------------------------------------------------------- */
  function initReveal() {
    const targets = document.querySelectorAll('.reveal');
    if (!targets.length) return;

    const io = new IntersectionObserver(entries => {
      entries.forEach(e => {
        if (e.isIntersecting) {
          e.target.classList.add('in-view');
          io.unobserve(e.target);
        }
      });
    }, { threshold: 0.08, rootMargin: '0px 0px -40px 0px' });

    targets.forEach(t => io.observe(t));
  }

  /* ----------------------------------------------------------
     COPYRIGHT YEAR
  ---------------------------------------------------------- */
  function initYear() {
    document.querySelectorAll('.current-year').forEach(el => {
      el.textContent = new Date().getFullYear();
    });
  }

  /* ----------------------------------------------------------
     SMOOTH ANCHOR SCROLL
  ---------------------------------------------------------- */
  function initSmoothScroll() {
    document.querySelectorAll('a[href^="#"]').forEach(a => {
      a.addEventListener('click', e => {
        const id = a.getAttribute('href').slice(1);
        const target = document.getElementById(id);
        if (target) {
          e.preventDefault();
          target.scrollIntoView({ behavior: 'smooth' });
        }
      });
    });
  }

  /* ----------------------------------------------------------
     BOOT
  ---------------------------------------------------------- */
  document.addEventListener('DOMContentLoaded', () => {
    initLoader();
    initTheme();
    initMobileNav();
    initActiveNav();
    initSidebar();
    initCodeCopy();
    initHighlight();
    initReveal();
    initYear();
    initSmoothScroll();
  });

})();
