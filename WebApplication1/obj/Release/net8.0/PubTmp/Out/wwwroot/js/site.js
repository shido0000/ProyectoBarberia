// Barbershop Pro - Site JavaScript

// Auto-dismiss alerts after 5 seconds
document.addEventListener('DOMContentLoaded', function() {
    var alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(function(alert) {
        setTimeout(function() {
            var bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });
});

// Confirm before delete actions
document.addEventListener('click', function(e) {
    if (e.target.closest('[data-confirm]')) {
        return confirm(e.target.closest('[data-confirm]').dataset.confirm);
    }
});
