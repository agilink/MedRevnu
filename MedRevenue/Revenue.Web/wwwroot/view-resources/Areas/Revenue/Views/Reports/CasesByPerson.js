(function () {
    $(function () {
        var _$table = $('#CasesByPersonTable');

        $('#GenerateReportButton').click(function (e) {
            e.preventDefault();
            var year = parseInt($('#YearFilter').val());
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
                    hospitalId: hospitalId ? parseInt(hospitalId) : null,
                    physicianId: physicianId ? parseInt(physicianId) : null
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
    });
})();
