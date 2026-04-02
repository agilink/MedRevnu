(function () {
    $(function () {
        var dashboardService = {
            init: function () {
                this.loadDailySummary();
                this.loadMonthlyQuota();
                this.bindEvents();
            },

            bindEvents: function () {
                var self = this;
                $('#DashboardDate').on('change', function () {
                    self.loadDailySummary();
                });
            },

            loadDailySummary: function () {
                var date = $('#DashboardDate').val();
                var self = this;

                $.ajax({
                    url: '/Revenue/Dashboard/GetDailyRevenueSummary',
                    type: 'POST',
                    data: JSON.stringify({ date: date }),
                    contentType: 'application/json',
                    success: function (data) {
                        self.updateDailySummary(data);
                    },
                    error: function () {
                        abp.message.error('Failed to load daily summary');
                    }
                });
            },

            updateDailySummary: function (data) {
                $('#TotalRevenue').text('$' + data.totalRevenue.toLocaleString('en-US', { minimumFractionDigits: 2 }));
                $('#TotalCases').text(data.totalCases);

                // Update category cards
                var categories = data.revenueByCategory || [];
                var pacemakerRevenue = 0;
                var defibrillatorRevenue = 0;

                categories.forEach(function (cat) {
                    if (cat.categoryName === 'Pacemaker') {
                        pacemakerRevenue = cat.revenue;
                    } else if (cat.categoryName === 'Defibrillator') {
                        defibrillatorRevenue = cat.revenue;
                    }
                });

                $('#PacemakerRevenue').text('$' + pacemakerRevenue.toLocaleString('en-US', { minimumFractionDigits: 2 }));
                $('#DefibrillatorRevenue').text('$' + defibrillatorRevenue.toLocaleString('en-US', { minimumFractionDigits: 2 }));

                // Update category breakdown
                this.renderCategoryBreakdown(categories);
            },

            renderCategoryBreakdown: function (categories) {
                var html = '<table class="table table-sm">';
                html += '<thead><tr><th>Category</th><th>Revenue</th><th>Cases</th></tr></thead><tbody>';

                categories.forEach(function (cat) {
                    html += '<tr>';
                    html += '<td>' + cat.categoryName + '</td>';
                    html += '<td>$' + cat.revenue.toLocaleString('en-US', { minimumFractionDigits: 2 }) + '</td>';
                    html += '<td>' + cat.caseCount + '</td>';
                    html += '</tr>';
                });

                html += '</tbody></table>';
                $('#CategoryBreakdown').html(html);
            },

            loadMonthlyQuota: function () {
                var now = new Date();
                var month = now.getMonth() + 1;
                var year = now.getFullYear();

                $.ajax({
                    url: '/Revenue/Dashboard/GetRevenueVsQuota',
                    type: 'POST',
                    data: JSON.stringify({ month: month, year: year }),
                    contentType: 'application/json',
                    success: function (data) {
                        dashboardService.updateQuotaTable(data);
                    },
                    error: function () {
                        $('#QuotaTableBody').html('<tr><td colspan="7" class="text-center text-danger">Failed to load quota data</td></tr>');
                    }
                });
            },

            updateQuotaTable: function (data) {
                var tbody = $('#QuotaTableBody');
                tbody.empty();

                if (!data || data.length === 0) {
                    tbody.html('<tr><td colspan="7" class="text-center">No quota data available</td></tr>');
                    return;
                }

                data.forEach(function (item) {
                    var percentClass = '';
                    if (item.percentageAchieved >= 100) {
                        percentClass = 'text-success';
                    } else if (item.percentageAchieved >= 80) {
                        percentClass = 'text-warning';
                    } else {
                        percentClass = 'text-danger';
                    }

                    var varianceClass = item.variance >= 0 ? 'text-success' : 'text-danger';

                    var row = '<tr>';
                    row += '<td>' + item.procedureTypeName + '</td>';
                    row += '<td>' + item.categoryGroupName + '</td>';
                    row += '<td>$' + item.quotaValue.toLocaleString('en-US', { minimumFractionDigits: 2 }) + '</td>';
                    row += '<td>$' + item.actualRevenue.toLocaleString('en-US', { minimumFractionDigits: 2 }) + '</td>';
                    row += '<td class="' + varianceClass + '">$' + item.variance.toLocaleString('en-US', { minimumFractionDigits: 2 }) + '</td>';
                    row += '<td class="' + percentClass + '">' + item.percentageAchieved.toFixed(1) + '%</td>';
                    row += '<td>' + item.caseCount + '</td>';
                    row += '</tr>';

                    tbody.append(row);
                });
            }
        };

        dashboardService.init();
    });
})();
