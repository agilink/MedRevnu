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

        $('#GenerateReportButton').click(function (e) {
            e.preventDefault();
            var hospitalId = optionalInt('#HospitalFilter');

            abp.ui.setBusy();
            $.ajax({
                url: abp.appPath + 'Revenue/Reports/GetRateChartData',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ hospitalId: hospitalId }),
                success: function (result) {
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
                    row.append('<td>' + (item.productCategoryName || '') + '</td>');
                    row.append('<td>' + (item.productCode || '') + '</td>');
                    row.append('<td>' + (item.productName || '') + '</td>');
                    row.append('<td>' + (item.isSystem ? '<span class="badge bg-success">System</span>' : '<span class="badge bg-info">Generator</span>') + '</td>');
                    row.append('<td>$' + (item.basePrice ? item.basePrice.toFixed(2) : '0.00') + '</td>');
                    tbody.append(row);
                });
            } else {
                tbody.append('<tr><td colspan="5" class="text-center">No data found</td></tr>');
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
                data: JSON.stringify({ hospitalId: optionalInt('#HospitalFilter') }),
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

    });
})();
