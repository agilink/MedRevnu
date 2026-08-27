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

        var _$table = $('#CasesByPersonTable');

        $('#GenerateReportButton').click(function (e) {
            e.preventDefault();
            var year = optionalInt('#YearFilter');
            var hospitalId = $('#HospitalFilter').val();
            var physicianId = $('#PhysicianFilter').val();

            if (!year) {
                abp.notify.error('Please enter a year');
                return;
            }

            abp.ui.setBusy();
            $.ajax({
                url: abp.appPath + 'Revenue/Reports/GetCasesByPersonData',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    year: year,
                    hospitalId: optionalInt('#HospitalFilter'),
                    physicianId: optionalInt('#PhysicianFilter')
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
                    row.append('<td>' + (item.physicianName || '') + '</td>');
                    row.append('<td>' + (item.hospitalName || '') + '</td>');
                    row.append('<td class="text-center">' + (item.totalCases || 0) + '</td>');
                    row.append('<td class="text-center">' + (item.deNovoCases || 0) + '</td>');
                    row.append('<td class="text-center">' + (item.genChangeCases || 0) + '</td>');
                    row.append('<td>$' + (item.totalRevenue ? item.totalRevenue.toFixed(2) : '0.00') + '</td>');
                    tbody.append(row);
                });
            } else {
                tbody.append('<tr><td colspan="6" class="text-center">No data found</td></tr>');
            }
        }

        // Export the report as it is currently filtered. The file comes back through the
        // application's standard temp-file download.
        $('#ExportToExcelButton').click(function (e) {
            e.preventDefault();

            abp.ui.setBusy();
            $.ajax({
                url: abp.appPath + 'Revenue/Reports/ExportCasesByPerson',
                type: 'POST',
                data: JSON.stringify({ year: optionalInt('#YearFilter'), hospitalId: optionalInt('#HospitalFilter'), physicianId: optionalInt('#PhysicianFilter') }),
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
