// Main JavaScript for Task Management API Documentation

document.addEventListener('DOMContentLoaded', function() {
    initTheme();
    initializeNavigation();
    initializeSearch();
    initializeSidebar();
    initializeCodeCopy();
    initializeScrollAnimations();
    initializeMobileMenu();
    initCopyrightYear();
    highlightCode();
});

// ========== THEME TOGGLE LOGIC ==========
function initTheme() {
    const theme = localStorage.getItem('theme') || (window.matchMedia('(prefers-color-scheme: light)').matches ? 'light' : 'dark');
    document.documentElement.setAttribute('data-theme', theme);
    createThemeToggle(theme);
}

function createThemeToggle(currentTheme) {
    const headerContainer = document.querySelector('.header-container');
    if (!headerContainer) return;

    // Ensure header-actions wrapper exists
    let headerActions = document.querySelector('.header-actions');
    if (!headerActions) {
        headerActions = document.createElement('div');
        headerActions.className = 'header-actions';
        headerContainer.appendChild(headerActions);
    }

    const toggleContainer = document.createElement('div');
    toggleContainer.className = 'theme-toggle-container';
    
    const toggleBtn = document.createElement('button');
    toggleBtn.className = 'theme-toggle-btn';
    toggleBtn.setAttribute('aria-label', 'Toggle theme');
    toggleBtn.innerHTML = currentTheme === 'light' ? '<i class="fas fa-moon"></i>' : '<i class="fas fa-sun"></i>';
    
    toggleBtn.addEventListener('click', () => {
        const newTheme = document.documentElement.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
        document.documentElement.setAttribute('data-theme', newTheme);
        localStorage.setItem('theme', newTheme);
        toggleBtn.innerHTML = newTheme === 'light' ? '<i class="fas fa-moon"></i>' : '<i class="fas fa-sun"></i>';
    });

    toggleContainer.appendChild(toggleBtn);
    headerActions.prepend(toggleContainer); // Prepend to keep it before hamburger
}

// ========== COPYRIGHT YEAR ==========
function initCopyrightYear() {
    const yearElements = document.querySelectorAll('.current-year');
    const currentYear = new Date().getFullYear();
    yearElements.forEach(el => {
        el.textContent = currentYear;
    });
}

// ========== NAVIGATION & SIDEBAR ==========
function initializeNavigation() {
    const currentPage = window.location.pathname.split('/').pop() || 'index.html';
    const navLinks = document.querySelectorAll('.navbar a');
    
    navLinks.forEach(link => {
        const href = link.getAttribute('href');
        if (href === currentPage || (currentPage === '' && href === 'index.html')) {
            link.classList.add('active');
        } else {
            link.classList.remove('active');
        }
    });
}

function initializeSearch() {
    const searchInput = document.getElementById('searchInput');
    if (!searchInput) return;

    searchInput.addEventListener('input', function(e) {
        const searchTerm = e.target.value.toLowerCase();
        const sidebarLinks = document.querySelectorAll('.sidebar-link');
        
        sidebarLinks.forEach(link => {
            const text = link.textContent.toLowerCase();
            if (text.includes(searchTerm) || searchTerm === '') {
                link.style.display = 'flex';
            } else {
                link.style.display = 'none';
            }
        });
    });
}

function initializeSidebar() {
    const sidebarLinks = document.querySelectorAll('.sidebar-link');
    
    sidebarLinks.forEach(link => {
        link.addEventListener('click', function() {
            sidebarLinks.forEach(l => l.classList.remove('active'));
            this.classList.add('active');
        });
    });

    window.addEventListener('scroll', updateActiveSidebarLink);
}

function updateActiveSidebarLink() {
    const sections = document.querySelectorAll('section');
    const sidebarLinks = document.querySelectorAll('.sidebar-link');
    
    let current = '';
    sections.forEach(section => {
        const sectionTop = section.offsetTop;
        if (window.pageYOffset >= sectionTop - 150) {
            current = section.getAttribute('id');
        }
    });

    sidebarLinks.forEach(link => {
        link.classList.remove('active');
        if (link.getAttribute('href') === '#' + current) {
            link.classList.add('active');
        }
    });
}

// ========== CODE COPY FUNCTIONALITY ==========
function initializeCodeCopy() {
    const docBoxes = document.querySelectorAll('.docbox');
    
    docBoxes.forEach(box => {
        const codeElement = box.querySelector('code');
        if (!codeElement) return;

        const copyButton = document.createElement('button');
        copyButton.className = 'copy-btn';
        copyButton.innerHTML = '<i class="far fa-copy"></i>';
        copyButton.setAttribute('aria-label', 'Copy code');
        
        box.appendChild(copyButton);
        
        copyButton.addEventListener('click', () => {
            const text = codeElement.innerText;
            navigator.clipboard.writeText(text).then(() => {
                copyButton.innerHTML = '<i class="fas fa-check"></i>';
                copyButton.classList.add('copied');
                
                setTimeout(() => {
                    copyButton.innerHTML = '<i class="far fa-copy"></i>';
                    copyButton.classList.remove('copied');
                }, 2000);
            });
        });
    });
}

// ========== UI ENHANCEMENTS ==========
function initializeScrollAnimations() {
    const observerOptions = { threshold: 0.1, rootMargin: '0px 0px -50px 0px' };

    const observer = new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('revealed');
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);

    document.querySelectorAll('.feature-card, .getting-started-card, .endpoint-card, section').forEach(el => {
        el.classList.add('reveal-on-scroll');
        observer.observe(el);
    });
}

function initializeMobileMenu() {
    const navbar = document.querySelector('.navbar');
    if (!navbar) return;

    // Ensure header-actions wrapper exists
    let headerActions = document.querySelector('.header-actions');
    if (!headerActions) {
        headerActions = document.createElement('div');
        headerActions.className = 'header-actions';
        headerContainer.appendChild(headerActions);
    }

    const toggleBtn = document.createElement('button');
    toggleBtn.className = 'mobile-menu-toggle';
    toggleBtn.innerHTML = '<span></span><span></span><span></span>';
    headerActions.appendChild(toggleBtn);

    const backdrop = document.createElement('div');
    backdrop.className = 'nav-backdrop';
    document.body.appendChild(backdrop);

    const toggleMenu = (open) => {
        navbar.classList.toggle('active', open);
        toggleBtn.classList.toggle('active', open);
        backdrop.classList.toggle('active', open);
        document.body.style.overflow = open ? 'hidden' : '';
    };

    toggleBtn.addEventListener('click', () => toggleMenu(!navbar.classList.contains('active')));
    backdrop.addEventListener('click', () => toggleMenu(false));
    navbar.querySelectorAll('a').forEach(link => link.addEventListener('click', () => toggleMenu(false)));
}

function highlightCode() {
    const codeBlocks = document.querySelectorAll('.docbox code');
    codeBlocks.forEach(block => {
        // Use textContent to get clean text without existing HTML
        let text = block.textContent;
        
        // Escape special characters to prevent XSS
        let html = text
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;");
        
        // Highlight JSON Keys ("key":)
        html = html.replace(/"([^"]+)":/g, '<span class="hl-key">"$1"</span>:');
        
        // Highlight JSON Strings after colon (: "value")
        html = html.replace(/: &quot;(.*?)&quot;/g, ': <span class="hl-string">&quot;$1&quot;</span>');
        
        // Highlight HTTP Methods
        html = html.replace(/\b(GET|POST|PUT|DELETE|PATCH)\b/g, '<span class="hl-method">$1</span>');
        
        // Highlight Numbers
        html = html.replace(/\b(\d+)\b/g, '<span class="hl-number">$1</span>');
        
        // Highlight Authorization tokens
        html = html.replace(/(Authorization: Bearer )([^\s]+)/g, '$1<span class="hl-token">$2</span>');

        block.innerHTML = html;
    });
}

document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function(e) {
        const href = this.getAttribute('href');
        if (href !== '#') {
            e.preventDefault();
            const target = document.querySelector(href);
            if (target) {
                target.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        }
    });
});

console.log('Task Management API Documentation V2 loaded');
