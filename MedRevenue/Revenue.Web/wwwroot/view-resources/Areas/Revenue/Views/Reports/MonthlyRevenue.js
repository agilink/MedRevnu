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

        var _$table = $('#MonthlyRevenueTable');

        $('#GenerateReportButton').click(function (e) {
            e.preventDefault();
            var year = optionalInt('#YearFilter');
            var month = optionalInt('#MonthFilter');
            var hospitalId = $('#HospitalFilter').val();

            if (!year || !month) {
                abp.notify.error('Please select year and month');
                return;
            }

            abp.ui.setBusy();
            $.ajax({
                url: abp.appPath + 'Revenue/Reports/GetMonthlyRevenueData',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    year: year,
                    month: month,
                    hospitalId: optionalInt('#HospitalFilter')
                }),
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
                    row.append('<td>' + moment(item.procedureDate).format('L') + '</td>');
                    row.append('<td>' + (item.productCategoryName || '') + '</td>');
                    row.append('<td class="text-center">' + (item.transactionCount || 0) + '</td>');
                    row.append('<td>$' + (item.dailyRevenue ? item.dailyRevenue.toFixed(2) : '0.00') + '</td>');
                    tbody.append(row);
                });
            } else {
                tbody.append('<tr><td colspan="4" class="text-center">No data found</td></tr>');
            }
        }

        // Export the report as it is currently filtered. The file comes back through the
        // application's standard temp-file download.
        $('#ExportToExcelButton').click(function (e) {
            e.preventDefault();

            abp.ui.setBusy();
            $.ajax({
                url: abp.appPath + 'Revenue/Reports/ExportMonthlyRevenue',
                type: 'POST',
                data: JSON.stringify({ year: optionalInt('#YearFilter'), month: optionalInt('#MonthFilter'), hospitalId: optionalInt('#HospitalFilter') }),
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
