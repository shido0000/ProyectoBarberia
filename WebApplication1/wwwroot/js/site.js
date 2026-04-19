// Barbershop Pro - Site JavaScript

// ===== THEME TOGGLE - DARK/LIGHT MODE =====
(function() {
    const themeToggle = document.getElementById('theme-toggle');
    const themeIcon = document.getElementById('theme-icon');
    
    if (!themeToggle) return;
    
    // Iconos para cada tema
    const icons = {
        light: 'bi-moon-stars',
        dark: 'bi-sun-fill'
    };
    
    // Función para establecer el tema
    function setTheme(theme) {
        const html = document.documentElement;
        
        if (theme === 'dark') {
            html.setAttribute('data-theme', 'dark');
            localStorage.setItem('theme', 'dark');
            if (themeIcon) {
                themeIcon.className = `bi ${icons.dark}`;
            }
        } else {
            html.removeAttribute('data-theme');
            localStorage.setItem('theme', 'light');
            if (themeIcon) {
                themeIcon.className = `bi ${icons.light}`;
            }
        }
        
        // Actualizar atributo aria-label para accesibilidad
        themeToggle.setAttribute('aria-label', `Cambiar a tema ${theme === 'dark' ? 'claro' : 'oscuro'}`);
    }
    
    // Detectar tema inicial
    function getInitialTheme() {
        const savedTheme = localStorage.getItem('theme');
        
        // Si hay un tema guardado, usarlo
        if (savedTheme === 'dark' || savedTheme === 'light') {
            return savedTheme;
        }
        
        // Si no, detectar preferencia del sistema
        if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
            return 'dark';
        }
        
        // Por defecto, tema claro
        return 'light';
    }
    
    // Establecer tema inicial
    const initialTheme = getInitialTheme();
    setTheme(initialTheme);
    
    // Escuchar cambios en la preferencia del sistema
    if (window.matchMedia) {
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
            // Solo cambiar si el usuario no ha establecido una preferencia manual
            if (!localStorage.getItem('theme')) {
                setTheme(e.matches ? 'dark' : 'light');
            }
        });
    }
    
    // Event listener para el botón de toggle
    themeToggle.addEventListener('click', () => {
        const isDark = document.documentElement.getAttribute('data-theme') === 'dark';
        setTheme(isDark ? 'light' : 'dark');
        
        // Animación adicional al hacer click
        themeToggle.style.transform = 'rotate(360deg)';
        setTimeout(() => {
            themeToggle.style.transform = '';
        }, 300);
    });
})();

// ===== AUTO-DISMISS ALERTS =====
document.addEventListener('DOMContentLoaded', function() {
    // Auto-dismiss alerts after 5 seconds
    var alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(function(alert) {
        setTimeout(function() {
            var bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });
    
    // Initialize tooltips and popovers
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
    
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });
});

// ===== CONFIRM BEFORE DELETE =====
document.addEventListener('click', function(e) {
    if (e.target.closest('[data-confirm]')) {
        const element = e.target.closest('[data-confirm]');
        const confirmed = confirm(element.dataset.confirm);
        if (!confirmed) {
            e.preventDefault();
            e.stopPropagation();
        }
        return confirmed;
    }
});

// ===== SMOOTH SCROLL FOR ANCHOR LINKS =====
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        const href = this.getAttribute('href');
        if (href !== '#' && href.length > 1) {
            const target = document.querySelector(href);
            if (target) {
                e.preventDefault();
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        }
    });
});

// ===== LAZY LOADING IMAGES =====
if ('loading' in HTMLImageElement.prototype) {
    const images = document.querySelectorAll('img[loading="lazy"]');
    images.forEach(img => {
        img.src = img.dataset.src;
    });
} else {
    // Fallback for browsers that don't support lazy loading
    const script = document.createElement('script');
    script.src = 'https://cdnjs.cloudflare.com/ajax/libs/lazysizes/5.3.2/lazysizes.min.js';
    document.body.appendChild(script);
}

// ===== FORM VALIDATION ENHANCEMENTS =====
(function() {
    'use strict';
    
    // Fetch all forms to apply custom Bootstrap validation styles
    const forms = document.querySelectorAll('.needs-validation');
    
    Array.from(forms).forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            
            form.classList.add('was-validated');
        }, false);
    });
})();

// ===== COUNTER ANIMATION FOR STATS =====
function animateCounter(element) {
    const target = parseInt(element.getAttribute('data-count'));
    const duration = 2000; // 2 seconds
    const step = target / (duration / 16); // 60fps
    let current = 0;
    
    const timer = setInterval(() => {
        current += step;
        if (current >= target) {
            element.textContent = target.toLocaleString();
            clearInterval(timer);
        } else {
            element.textContent = Math.floor(current).toLocaleString();
        }
    }, 16);
}

// Intersection Observer for stats animation
const observerOptions = {
    threshold: 0.5,
    rootMargin: '0px'
};

const statsObserver = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            const statNumber = entry.target.querySelector('.stat-number');
            if (statNumber && !statNumber.classList.contains('animated')) {
                statNumber.classList.add('animated');
                animateCounter(statNumber);
            }
            statsObserver.unobserve(entry.target);
        }
    });
}, observerOptions);

document.querySelectorAll('.stat-card').forEach(card => {
    statsObserver.observe(card);
});

// ===== NAVBAR SCROLL EFFECT =====
let lastScroll = 0;
const navbar = document.querySelector('.navbar');

if (navbar) {
    window.addEventListener('scroll', () => {
        const currentScroll = window.pageYOffset;
        
        if (currentScroll > 100) {
            navbar.style.boxShadow = 'var(--shadow-md)';
            navbar.style.background = 'var(--bg-navbar)';
        } else {
            navbar.style.boxShadow = 'none';
        }
        
        lastScroll = currentScroll;
    });
}
// ===== NOTIFICATIONS SYSTEM =====
(function() {
    // Only initialize if user is authenticated (notification bell exists)
    const notificationBell = document.getElementById('notification-bell');
    if (!notificationBell) return;
    
    const notificationBadge = document.getElementById('notification-badge');
    const notificationsList = document.getElementById('notifications-list');
    const notificationsLoading = document.getElementById('notifications-loading');
    const notificationsEmpty = document.getElementById('notifications-empty');
    
    let notificationPollInterval;
    
    // Load notifications from server
    async function loadNotifications() {
        try {
            const response = await fetch('/Notifications/GetNotifications?count=5');
            if (!response.ok) throw new Error('Failed to fetch notifications');
            
            const data = await response.json();
            renderNotifications(data.notifications, data.unreadCount);
        } catch (error) {
            console.error('Error loading notifications:', error);
        }
    }
    
    // Render notifications in dropdown
    function renderNotifications(notifications, unreadCount) {
        // Update badge
        if (unreadCount > 0) {
            notificationBadge.textContent = unreadCount > 9 ? '9+' : unreadCount;
            notificationBadge.classList.remove('d-none');
        } else {
            notificationBadge.classList.add('d-none');
        }
        
        // Hide loading
        notificationsLoading.classList.add('d-none');
        
        // Show empty state or notifications
        if (notifications.length === 0) {
            notificationsList.classList.add('d-none');
            notificationsEmpty.classList.remove('d-none');
        } else {
            notificationsEmpty.classList.add('d-none');
            notificationsList.classList.remove('d-none');
            
            // Build notification items
            let html = '';
            notifications.forEach(n => {
                const iconClass = getNotificationIcon(n.type);
                const colorClass = getNotificationColor(n.type);
                const fontWeight = n.isRead ? '' : 'fw-semibold';
                
                html += `
                    <li>
                        <a href="${n.relatedUrl || '#'}" class="dropdown-item py-2 ${!n.isRead ? 'bg-primary bg-opacity-10' : ''}" onclick="markAsRead(${n.id}); event.stopPropagation();">
                            <div class="d-flex align-items-start gap-2">
                                <i class="bi ${iconClass} ${colorClass} mt-1"></i>
                                <div class="flex-grow-1">
                                    <p class="mb-0 small ${fontWeight}">${escapeHtml(n.message)}</p>
                                    <small class="text-muted">${n.createdAt}</small>
                                </div>
                            </div>
                        </a>
                    </li>
                `;
            });
            
            notificationsList.innerHTML = html;
        }
    }
    
    // Get icon class based on notification type
    function getNotificationIcon(type) {
        const icons = {
            'NewMembershipRequest': 'bi-person-plus',
            'RequestAccepted': 'bi-check-circle',
            'RequestRejected': 'bi-x-circle',
            'BarberLeft': 'bi-box-arrow-right',
            'AppointmentReminder': 'bi-calendar-event',
            'Info': 'bi-info-circle',
            'System': 'bi-gear'
        };
        return icons[type] || 'bi-bell';
    }
    
    // Get color class based on notification type
    function getNotificationColor(type) {
        const colors = {
            'NewMembershipRequest': 'text-primary',
            'RequestAccepted': 'text-success',
            'RequestRejected': 'text-danger',
            'BarberLeft': 'text-warning',
            'AppointmentReminder': 'text-info',
            'Info': 'text-secondary',
            'System': 'text-muted'
        };
        return colors[type] || 'text-secondary';
    }
    
    // Escape HTML to prevent XSS
    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
    
    // Mark single notification as read
    window.markAsRead = async function(id) {
        try {
            await fetch(`/Notifications/MarkAsRead?id=${id}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            
            // Reload notifications after marking as read
            setTimeout(() => loadNotifications(), 500);
        } catch (error) {
            console.error('Error marking notification as read:', error);
        }
    };
    
    // Mark all notifications as read
    window.markAllAsRead = async function() {
        try {
            await fetch('/Notifications/MarkAllAsRead', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            
            // Reload notifications
            loadNotifications();
        } catch (error) {
            console.error('Error marking all notifications as read:', error);
        }
    };
    
    // Load notifications when dropdown is opened
    notificationBell.addEventListener('click', function() {
        loadNotifications();
    });
    
    // Initial load
    loadNotifications();
    
    // Poll for new notifications every 30 seconds
    notificationPollInterval = setInterval(loadNotifications, 30000);
    
    // Clean up interval when page unloads
    window.addEventListener('beforeunload', () => {
        if (notificationPollInterval) {
            clearInterval(notificationPollInterval);
        }
    });
})();
