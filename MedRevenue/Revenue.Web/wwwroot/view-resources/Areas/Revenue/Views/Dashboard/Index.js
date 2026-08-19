(function () {
    $(function () {

        function money(value) {
            return '$' + (value || 0).toLocaleString('en-US', {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            });
        }

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
                $('#TotalRevenue').text(money(data.totalRevenue));
                $('#TotalCases').text(data.totalCases || 0);

                this.renderCategoryCards(data.revenueByCategory || []);
                this.renderCategoryBreakdown(data.revenueByCategory || []);
            },

            // Cards are built from whatever categories actually have revenue, so adding
            // or renaming a product category does not need a change here.
            renderCategoryCards: function (categories) {
                var container = $('#CategoryCards');
                container.empty();

                if (!categories.length) {
                    container.html('<div class="text-muted">No category revenue on this date.</div>');
                    return;
                }

                var palette = ['bg-warning', 'bg-danger', 'bg-info', 'bg-secondary'];
                var row = $('<div class="row"></div>');

                categories.slice(0, 4).forEach(function (cat, i) {
                    var card = ''
                        + '<div class="col-6 mb-2">'
                        + '  <div class="info-box">'
                        + '    <span class="info-box-icon ' + palette[i % palette.length] + '"><i class="fas fa-heartbeat"></i></span>'
                        + '    <div class="info-box-content">'
                        + '      <span class="info-box-text"></span>'
                        + '      <span class="info-box-number"></span>'
                        + '    </div>'
                        + '  </div>'
                        + '</div>';

                    var $card = $(card);
                    $card.find('.info-box-text').text(cat.productCategoryName);
                    $card.find('.info-box-number').text(money(cat.revenue));
                    row.append($card);
                });

                container.append(row);
            },

            renderCategoryBreakdown: function (categories) {
                var table = $('<table class="table table-sm"></table>');
                table.append('<thead><tr><th>Category</th><th>Revenue</th><th>Cases</th></tr></thead>');
                var tbody = $('<tbody></tbody>');

                if (!categories.length) {
                    tbody.append('<tr><td colspan="3" class="text-center">No revenue recorded on this date</td></tr>');
                }

                categories.forEach(function (cat) {
                    var tr = $('<tr></tr>');
                    tr.append($('<td></td>').text(cat.productCategoryName));
                    tr.append($('<td></td>').text(money(cat.revenue)));
                    tr.append($('<td></td>').text(cat.caseCount || 0));
                    tbody.append(tr);
                });

                table.append(tbody);
                $('#CategoryBreakdown').empty().append(table);
            },

            loadMonthlyQuota: function () {
                var now = new Date();

                $.ajax({
                    url: '/Revenue/Dashboard/GetRevenueVsQuota',
                    type: 'POST',
                    data: JSON.stringify({ month: now.getMonth() + 1, year: now.getFullYear() }),
                    contentType: 'application/json',
                    success: function (data) {
                        dashboardService.updateQuotaTable(data);
                    },
                    error: function () {
                        $('#QuotaTableBody').html('<tr><td colspan="8" class="text-center text-danger">Failed to load quota data</td></tr>');
                    }
                });
            },

            updateQuotaTable: function (data) {
                var tbody = $('#QuotaTableBody');
                tbody.empty();

                if (!data || data.length === 0) {
                    tbody.html('<tr><td colspan="8" class="text-center">No quota or revenue data for this month</td></tr>');
                    return;
                }

                data.forEach(function (item) {
                    var percentClass;
                    if (!item.hasQuota) {
                        percentClass = 'text-muted';
                    } else if (item.percentageAchieved >= 100) {
                        percentClass = 'text-success';
                    } else if (item.percentageAchieved >= 80) {
                        percentClass = 'text-warning';
                    } else {
                        percentClass = 'text-danger';
                    }

                    var tr = $('<tr></tr>');
                    tr.append($('<td></td>').text(item.hospitalName || '-'));
                    tr.append($('<td></td>').text(item.productCategoryName || '-'));
                    tr.append($('<td></td>').text(item.productName || 'All devices'));

                    // Revenue with no quota behind it is listed so it cannot go
                    // unnoticed, but it has no target to compare against.
                    if (item.hasQuota) {
                        tr.append($('<td></td>').text(money(item.targetAmount)));
                        tr.append($('<td></td>').text(money(item.actualRevenue)));
                        tr.append($('<td></td>')
                            .addClass(item.variance >= 0 ? 'text-success' : 'text-danger')
                            .text(money(item.variance)));
                        tr.append($('<td></td>')
                            .addClass(percentClass)
                            .text(item.percentageAchieved.toFixed(1) + '%'));
                    } else {
                        tr.append($('<td class="text-muted">No target set</td>'));
                        tr.append($('<td></td>').text(money(item.actualRevenue)));
                        tr.append($('<td class="text-muted">-</td>'));
                        tr.append($('<td class="text-muted">-</td>'));
                    }

                    tr.append($('<td></td>').text(item.actualUnits || 0));
                    tbody.append(tr);
                });
            }
        };

        dashboardService.init();
    });
})();
