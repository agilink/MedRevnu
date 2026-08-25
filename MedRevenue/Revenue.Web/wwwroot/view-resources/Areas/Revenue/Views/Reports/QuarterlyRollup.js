(function () {
    $(function () {
        var _$body = $('#QuarterlyRollupTableBody');

        function money(value) {
            return '$' + (value || 0).toLocaleString('en-US', {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            });
        }

        function percentClass(row) {
            if (!row.hasPlan) {
                return 'text-muted';
            }
            if (row.percentToPlan >= 100) {
                return 'text-success';
            }
            if (row.percentToPlan >= 80) {
                return 'text-warning';
            }
            return 'text-danger';
        }

        function render(rows) {
            _$body.empty();

            if (!rows || !rows.length) {
                _$body.html('<tr><td colspan="10" class="text-center">No targets or revenue for this period</td></tr>');
                return;
            }

            var currentQuarter = null;

            rows.forEach(function (row) {
                // Subtotal each quarter, which is what the workbook's Quarters tab shows.
                if (currentQuarter !== null && row.quarter !== currentQuarter) {
                    appendSubtotal(rows, currentQuarter);
                }
                currentQuarter = row.quarter;

                var tr = $('<tr></tr>');
                tr.append($('<td></td>').text(row.quarterName));
                tr.append($('<td></td>').text(row.hospitalName || '-'));
                tr.append($('<td></td>').text(row.productCategoryName));

                if (row.hasPlan) {
                    tr.append($('<td class="text-end"></td>').text(money(row.totalPlan)));
                } else {
                    tr.append($('<td class="text-end text-muted">No plan</td>'));
                }

                tr.append($('<td class="text-end"></td>').text(money(row.totalSold)));
                tr.append($('<td class="text-end"></td>')
                    .addClass(row.variance >= 0 ? 'text-success' : 'text-danger')
                    .text(money(row.variance)));

                if (row.hasPlan) {
                    tr.append($('<td class="text-end"></td>')
                        .addClass(percentClass(row))
                        .text(row.percentToPlan.toFixed(1) + '%'));
                } else {
                    tr.append($('<td class="text-end text-muted">-</td>'));
                }

                tr.append($('<td class="text-center"></td>').text(row.totalCases || 0));
                tr.append($('<td class="text-center"></td>').text(row.deNovoCases || 0));
                tr.append($('<td class="text-center"></td>').text(row.genChangeCases || 0));

                _$body.append(tr);
            });

            if (currentQuarter !== null) {
                appendSubtotal(rows, currentQuarter);
            }
        }

        function appendSubtotal(rows, quarter) {
            var quarterRows = rows.filter(function (r) { return r.quarter === quarter; });

            var plan = quarterRows.reduce(function (sum, r) { return sum + r.totalPlan; }, 0);
            var sold = quarterRows.reduce(function (sum, r) { return sum + r.totalSold; }, 0);
            var cases = quarterRows.reduce(function (sum, r) { return sum + r.totalCases; }, 0);
            var deNovo = quarterRows.reduce(function (sum, r) { return sum + r.deNovoCases; }, 0);
            var genChange = quarterRows.reduce(function (sum, r) { return sum + r.genChangeCases; }, 0);
            var pct = plan > 0 ? (sold / plan) * 100 : 0;

            var tr = $('<tr class="fw-bold table-light"></tr>');
            tr.append($('<td colspan="3"></td>').text('Q' + quarter + ' total'));
            tr.append($('<td class="text-end"></td>').text(money(plan)));
            tr.append($('<td class="text-end"></td>').text(money(sold)));
            tr.append($('<td class="text-end"></td>')
                .addClass(sold - plan >= 0 ? 'text-success' : 'text-danger')
                .text(money(sold - plan)));
            tr.append($('<td class="text-end"></td>').text(plan > 0 ? pct.toFixed(1) + '%' : '-'));
            tr.append($('<td class="text-center"></td>').text(cases));
            tr.append($('<td class="text-center"></td>').text(deNovo));
            tr.append($('<td class="text-center"></td>').text(genChange));

            _$body.append(tr);
        }

        function run() {
            var quarter = $('#QuarterFilter').val();
            var hospitalId = $('#HospitalFilter').val();

            _$body.html('<tr><td colspan="10" class="text-center">Loading...</td></tr>');

            $.ajax({
                url: abp.appPath + 'Revenue/Reports/GetQuarterlyRollupData',
                type: 'POST',
                data: JSON.stringify({
                    year: parseInt($('#YearFilter').val(), 10),
                    quarter: quarter ? parseInt(quarter, 10) : null,
                    hospitalId: hospitalId ? parseInt(hospitalId, 10) : null
                }),
                contentType: 'application/json',
                success: function (result) {
                    if (result && result.success) {
                        render(result.data);
                    } else {
                        _$body.html('<tr><td colspan="10" class="text-center text-danger">Failed to load report</td></tr>');
                    }
                },
                error: function () {
                    _$body.html('<tr><td colspan="10" class="text-center text-danger">Failed to load report</td></tr>');
                }
            });
        }

        $('#ExportToExcelButton').click(function (e) {
            e.preventDefault();

            var quarter = $('#QuarterFilter').val();
            var hospitalId = $('#HospitalFilter').val();

            abp.ui.setBusy();
            $.ajax({
                url: abp.appPath + 'Revenue/Reports/ExportQuarterlyRollup',
                type: 'POST',
                data: JSON.stringify({
                    year: parseInt($('#YearFilter').val(), 10),
                    quarter: quarter ? parseInt(quarter, 10) : null,
                    hospitalId: hospitalId ? parseInt(hospitalId, 10) : null
                }),
                contentType: 'application/json',
                success: function (file) {
                    app.downloadTempFile(file);
                },
                error: function () {
                    abp.notify.error(app.localize('ExportFailed'));
                },
                complete: function () {
                    abp.ui.clearBusy();
                }
            });
        });

        $('#RunReportButton').click(function (e) {
            e.preventDefault();
            run();
        });

        $('#YearFilter, #QuarterFilter, #HospitalFilter').change(function () {
            run();
        });

        run();
    });
})();
