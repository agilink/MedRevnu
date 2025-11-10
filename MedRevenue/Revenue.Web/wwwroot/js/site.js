// Revenue Module JavaScript

// Initialize DataTables on document ready
$(document).ready(function() {
    // Check if DataTable exists on the page
    if ($.fn.DataTable && $('.data-table').length > 0) {
        $('.data-table').DataTable({
            responsive: true,
            pageLength: 10,
            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
            language: {
                search: "Search:",
                lengthMenu: "Show _MENU_ entries",
                info: "Showing _START_ to _END_ of _TOTAL_ entries",
                paginate: {
                    first: "First",
                    last: "Last",
                    next: "Next",
                    previous: "Previous"
                }
            }
        });
    }

    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    });

    // Confirmation dialogs
    $('.delete-confirm').on('click', function(e) {
        if (!confirm('Are you sure you want to delete this item?')) {
            e.preventDefault();
        }
    });
});

// Revenue Module specific functions
var RevenueModule = {
    
    // Show loading spinner
    showLoading: function() {
        $('#loadingSpinner').show();
    },
    
    // Hide loading spinner
    hideLoading: function() {
        $('#loadingSpinner').hide();
    },
    
    // Show success message
    showSuccess: function(message) {
        var alertHtml = '<div class="alert alert-success alert-dismissible fade show" role="alert">' +
                       message +
                       '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
                       '</div>';
        $('#messageArea').html(alertHtml);
    },
    
    // Show error message
    showError: function(message) {
        var alertHtml = '<div class="alert alert-danger alert-dismissible fade show" role="alert">' +
                       message +
                       '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
                       '</div>';
        $('#messageArea').html(alertHtml);
    },
    
    // Load cases data
    loadCases: function() {
        $.ajax({
            url: '/Revenue/Cases/GetAll',
            type: 'GET',
            success: function(data) {
                // Process cases data
                console.log('Cases loaded:', data);
            },
            error: function(xhr, status, error) {
                RevenueModule.showError('Failed to load cases');
            }
        });
    },
    
    // Load products data
    loadProducts: function() {
        $.ajax({
            url: '/Revenue/Products/GetAllActive',
            type: 'GET',
            success: function(data) {
                // Process products data
                console.log('Products loaded:', data);
            },
            error: function(xhr, status, error) {
                RevenueModule.showError('Failed to load products');
            }
        });
    }
};