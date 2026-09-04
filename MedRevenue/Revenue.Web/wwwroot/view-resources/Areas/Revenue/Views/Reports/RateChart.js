(function () {
    $(function () {
        // Blank filters must post as null, not NaN: every report filter is optional.
        function optionalInt(selector) {
            var raw = $(selector).val();
            if (raw === null || raw === undefined || String(raw).trim() === '') {
                return null;
            }
            var parsed = parseInt(raw, 10);
            return isNaN(parsed) ? null : parsed;
        }

        var _$table = $('#RateChartTable');

        function money(value) {
            return '$' + (value || 0).toLocaleString('en-US', {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            });
        }

        $('#GenerateReportButton').click(function (e) {
            e.preventDefault();
            var hospitalId = optionalInt('#HospitalFilter');

            abp.ui.setBusy();
            $.ajax({
                url: abp.appPath + 'Revenue/Reports/GetRateChartData',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    hospitalId: hospitalId,
                    productCode: $('#ProductCodeFilter').val() || null
                }),
                // ABP wraps a JsonResult from an ABP controller in an AjaxResponse
                // envelope ({result, success, error, __abp}). Unwrap it: read raw,
                // the payload sat one level down and every field was undefined, so
                // the report rendered empty over good data.
                success: function (response) {
                    var result = (response && response.__abp) ? response.result : response;
                    if (result.success) {
                        renderTable(result.data);
                        $('#reportContainer').show();
                    } else {
                        abp.notify.error('Failed to load report data');
                    }
                },
                error: function () {
                    abp.notify.error('Failed to load report data');
                },
                complete: function () {
                    abp.ui.clearBusy();
                }
            });
        });

        function renderTable(data) {
            var tbody = _$table.find('tbody');
            tbody.empty();

            if (data && data.length > 0) {
                data.forEach(function (item) {
                    var row = $('<tr></tr>');
                    row.append($('<td></td>').text(item.hospitalName || ''));
                    row.append($('<td></td>').text(item.productCategoryName || ''));
                    row.append($('<td></td>').text(item.productCode || ''));
                    row.append($('<td></td>').text(item.productName || ''));
                    row.append('<td>' + (item.isSystem ? '<span class="badge bg-success">System</span>' : '<span class="badge bg-info">Generator</span>') + '</td>');
                    row.append($('<td class="text-end"></td>').text(money(item.basePrice)));

                    // Blank rather than a misleading 0.00 where this hospital has no
                    // contracted price, or where no hospital was chosen.
                    if (item.contractedPrice === null || item.contractedPrice === undefined) {
                        row.append('<td class="text-end text-muted">-</td>');
                    } else {
                        row.append($('<td class="text-end"></td>').text(money(item.contractedPrice)));
                    }

                    row.append($('<td class="text-end fw-bold"></td>').text(money(item.effectivePrice)));

                    // Where the effective price came from. A hospital that has never been
                    // priced for a product silently inherits the base price, and without
                    // this the row looks like a negotiated rate.
                    if (item.contractedPrice === null || item.contractedPrice === undefined) {
                        row.append('<td><span class="badge badge-light-warning">'
                            + app.localize('PriceBaseFallback') + '</span></td>');
                    } else {
                        row.append('<td><span class="badge badge-light-primary">'
                            + app.localize('PriceContracted') + '</span></td>');
                    }
                    row.append($('<td class="text-center"></td>').text(item.unitsSoldThisYear || 0));
                    row.append($('<td class="text-center"></td>').text(item.casesThisYear || 0));
                    tbody.append(row);
                });
            } else {
                tbody.append('<tr><td colspan="9" class="text-center">No data found</td></tr>');
            }
        }

        // Export the report as it is currently filtered. The file comes back through the
        // application's standard temp-file download.
        $('#ExportToExcelButton').click(function (e) {
            e.preventDefault();

            abp.ui.setBusy();
            $.ajax({
                url: abp.appPath + 'Revenue/Reports/ExportRateChart',
                type: 'POST',
                data: JSON.stringify({
                    hospitalId: optionalInt('#HospitalFilter'),
                    productCode: $('#ProductCodeFilter').val() || null
                }),
                contentType: 'application/json',
                success: function (response) {
                    var file = (response && response.__abp) ? response.result : response;
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

        // Run once on arrival with the defaults, so the page opens with the
        // current period already loaded instead of an empty table. Triggering the
        // button reuses its validation, busy indicator and error handling.
        $('#GenerateReportButton').click();

    });
})();
