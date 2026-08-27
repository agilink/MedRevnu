(function () {
    $(function () {

        // Deliberately dependency-free. Chart.js is listed in package.json but is not
        // deployed to wwwroot, and Metronic's ApexCharts is not loaded on this page, so a
        // chart library here would leave the page blank. Bars are plain CSS, which cannot
        // fail to render.

        function money(value) {
            return '$' + (Number(value) || 0).toLocaleString('en-US', {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            });
        }

        function count(value) {
            return (Number(value) || 0).toLocaleString('en-US');
        }

        function percent(value) {
            return (Number(value) || 0).toFixed(1) + '%';
        }

        function shortDate(iso) {
            if (!iso) {
                return '-';
            }
            // Dates arrive as local dates with no offset; take the date part so no time
            // zone shift can move a case to the previous day.
            return moment(String(iso).substring(0, 10), 'YYYY-MM-DD').format('ddd, DD MMM YYYY');
        }

        function toInputDate(date) {
            return moment(date).format('YYYY-MM-DD');
        }

        // Green once the target is met, amber when close, red when not - and grey where
        // there is no target, because 0% would read as failure rather than "not measured".
        function achievementClass(percentAchieved, hasPlan) {
            if (!hasPlan) {
                return 'text-muted';
            }
            if (percentAchieved >= 100) {
                return 'text-success';
            }
            if (percentAchieved >= 80) {
                return 'text-warning';
            }
            return 'text-danger';
        }

        function bar(widthPercent, cssClass) {
            var width = Math.max(0, Math.min(100, Number(widthPercent) || 0));
            return $('<div class="progress h-6px bg-light"></div>')
                .append($('<div class="progress-bar ' + cssClass + '"></div>')
                    .css('width', width + '%'));
        }

        function emptyRow(columns) {
            return $('<tr></tr>').append(
                $('<td class="text-center text-muted"></td>')
                    .attr('colspan', columns)
                    .text(app.localize('NoDataForPeriod')));
        }

        var dashboard = {

            init: function () {
                this.bindEvents();
                this.load();
            },

            bindEvents: function () {
                var self = this;

                $('#ApplyFilters').on('click', function () {
                    self.load();
                });

                $('#HospitalFilter').on('change', function () {
                    self.load();
                });

                $('.period-preset').on('click', function () {
                    self.applyPreset($(this).data('preset'));
                    self.load();
                });

                $('#FromDate, #ToDate').on('change', function () {
                    self.load();
                });
            },

            applyPreset: function (preset) {
                var from;
                var to;

                switch (preset) {
                    case 'lastMonth':
                        from = moment().subtract(1, 'month').startOf('month');
                        to = moment().subtract(1, 'month').endOf('month');
                        break;
                    case 'thisQuarter':
                        from = moment().startOf('quarter');
                        to = moment();
                        break;
                    case 'thisYear':
                        from = moment().startOf('year');
                        to = moment();
                        break;
                    default:
                        from = moment().startOf('month');
                        to = moment();
                        break;
                }

                $('#FromDate').val(toInputDate(from));
                $('#ToDate').val(toInputDate(to));
            },

            load: function () {
                var self = this;
                var hospitalId = $('#HospitalFilter').val();

                // Every filter is optional. An empty box means "no restriction", not a
                // validation error, so the page always has something to show.
                var input = {
                    fromDate: $('#FromDate').val() || null,
                    toDate: $('#ToDate').val() || null,
                    hospitalId: hospitalId ? parseInt(hospitalId, 10) : null
                };

                abp.ui.setBusy($('#kt_app_content, .app-container'));

                // abp.ajax, not $.ajax. The controller derives from an ABP controller, so
                // ABP wraps every JsonResult in an AjaxResponse envelope
                // ({result, success, error, __abp}). Read with $.ajax the payload sits one
                // level down at data.result, so every field came back undefined and the
                // page rendered zeros and empty tables over perfectly good data.
                // abp.ajax unwraps the envelope, adds the anti-forgery header and reports
                // ABP errors itself.
                abp.ajax({
                    url: abp.appPath + 'Revenue/Dashboard/GetDashboard',
                    type: 'POST',
                    data: JSON.stringify(input)
                }).done(function (data) {
                    self.render(data || {});
                }).always(function () {
                    abp.ui.clearBusy($('#kt_app_content, .app-container'));
                });
            },

            render: function (data) {
                this.renderKpis(data);
                this.renderDeviceTypes(data.deviceTypes || []);
                this.renderCollection($('#TopPhysiciansBody'), data.topPhysicians || [], true);
                this.renderCollection($('#TopHospitalsBody'), data.topHospitals || [], false);
                this.renderDailyRevenue(data.dailyRevenue || []);
            },

            renderKpis: function (data) {
                $('#KpiTotalCases').text(count(data.totalCases));
                $('#KpiTotalUnits').text(app.localize('TotalUnits') + ': ' + count(data.totalUnits));
                $('#KpiTotalSold').text(money(data.totalSold));
                $('#KpiTotalPlanned').text(money(data.totalPlanned));

                $('#KpiPeriodLabel').text(
                    moment(String(data.fromDate).substring(0, 10)).format('DD MMM') +
                    ' - ' +
                    moment(String(data.toDate).substring(0, 10)).format('DD MMM YYYY'));

                // Say which months the target came from: a range that starts mid-month
                // still counts that month's whole target, and the caption is the only
                // place that admits it.
                var months = data.plannedMonths || [];
                $('#KpiPlannedMonths').text(
                    months.length
                        ? abp.utils.formatString(app.localize('PlannedTakenFromMonths'), months.join(', '))
                        : '');

                var percentClass = achievementClass(data.percentAchieved, data.hasPlan);

                $('#KpiPercentAchieved')
                    .removeClass('text-success text-warning text-danger text-muted text-dark')
                    .addClass(percentClass)
                    .text(data.hasPlan ? percent(data.percentAchieved) : '-');

                $('#KpiVariance')
                    .removeClass('text-success text-danger text-muted')
                    .addClass(data.hasPlan ? (data.variance >= 0 ? 'text-success' : 'text-danger') : 'text-muted')
                    .text(data.hasPlan
                        ? app.localize('Variance') + ': ' + money(data.variance)
                        : app.localize('NoTargetsForPeriod'));
            },

            renderDeviceTypes: function (rows) {
                var tbody = $('#DeviceTypeBody').empty();

                if (!rows.length) {
                    tbody.append(emptyRow(8));
                    return;
                }

                rows.forEach(function (row) {
                    var tr = $('<tr></tr>');

                    tr.append($('<td class="fw-semibold"></td>').text(row.deviceType || '-'));

                    // Two stacked bars rather than a number alone: the gap between sold
                    // and planned is the point of this table.
                    var barCell = $('<td></td>');
                    var soldWidth = row.planned > 0
                        ? (row.sold / row.planned) * 100
                        : (row.sold > 0 ? 100 : 0);

                    barCell.append(bar(soldWidth, row.percentAchieved >= 100 || !row.hasPlan ? 'bg-success' : 'bg-primary'));
                    barCell.append(bar(row.planned > 0 ? 100 : 0, 'bg-secondary').addClass('mt-1'));
                    tr.append(barCell);

                    tr.append($('<td class="text-end"></td>').text(money(row.sold)));

                    if (row.hasPlan) {
                        tr.append($('<td class="text-end"></td>').text(money(row.planned)));
                        tr.append($('<td class="text-end"></td>')
                            .addClass(row.variance >= 0 ? 'text-success' : 'text-danger')
                            .text(money(row.variance)));
                        tr.append($('<td class="text-end"></td>')
                            .addClass(achievementClass(row.percentAchieved, true))
                            .text(percent(row.percentAchieved)));
                    } else {
                        // Revenue with no target behind it is still listed, so it cannot
                        // go unnoticed - but it has nothing to be measured against.
                        tr.append($('<td class="text-end text-muted"></td>').text(app.localize('NoTargetSet')));
                        tr.append($('<td class="text-end text-muted"></td>').text('-'));
                        tr.append($('<td class="text-end text-muted"></td>').text('-'));
                    }

                    tr.append($('<td class="text-end"></td>').text(count(row.cases)));
                    tr.append($('<td class="text-end"></td>').text(count(row.units)));

                    tbody.append(tr);
                });
            },

            renderCollection: function (tbody, rows, showSecondary) {
                tbody.empty();

                if (!rows.length) {
                    tbody.append(emptyRow(4));
                    return;
                }

                rows.forEach(function (row) {
                    var tr = $('<tr></tr>');

                    var nameCell = $('<td></td>');
                    nameCell.append($('<div class="fw-semibold"></div>').text(row.name || '-'));

                    if (showSecondary && row.secondaryName) {
                        nameCell.append($('<div class="text-muted fs-8"></div>').text(row.secondaryName));
                    }

                    tr.append(nameCell);
                    tr.append($('<td class="text-end"></td>').text(count(row.cases)));
                    tr.append($('<td class="text-end fw-semibold"></td>').text(money(row.collection)));

                    var shareCell = $('<td></td>');
                    shareCell.append(bar(row.sharePercent, 'bg-primary'));
                    shareCell.append($('<div class="text-muted fs-8"></div>').text(percent(row.sharePercent)));
                    tr.append(shareCell);

                    tbody.append(tr);
                });
            },

            renderDailyRevenue: function (rows) {
                var tbody = $('#DailyRevenueBody').empty();

                if (!rows.length) {
                    tbody.append(emptyRow(5));
                    return;
                }

                rows.forEach(function (row) {
                    var tr = $('<tr></tr>');
                    tr.append($('<td></td>').text(shortDate(row.date)));
                    tr.append($('<td class="text-end"></td>').text(count(row.cases)));
                    tr.append($('<td class="text-end"></td>').text(count(row.units)));
                    tr.append($('<td class="text-end fw-semibold"></td>').text(money(row.revenue)));
                    tr.append($('<td></td>').append(bar(row.sharePercent, 'bg-primary')));
                    tbody.append(tr);
                });
            }
        };

        dashboard.init();
    });
})();
