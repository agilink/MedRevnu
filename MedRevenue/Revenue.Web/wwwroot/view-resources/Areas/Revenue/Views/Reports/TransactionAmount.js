(function () {
    $(function () {
        var _$table = $('#TransactionAmountTable');

        $('#GenerateReportButton').click(function (e) {
            e.preventDefault();
            var year = parseInt($('#YearFilter').val());
            var physicianId = $('#PhysicianFilter').val();
            var productCategoryId = $('#ProductCategoryFilter').val();

            if (!year) {
                abp.notify.error('Please enter a year');
                return;
            }

            abp.ui.setBusy();
            $.ajax({
                url: abp.appPath + 'Revenue/Reports/GetTransactionAmountData',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    year: year,
                    physicianId: physicianId ? parseInt(physicianId) : null,
                    productCategoryId: productCategoryId ? parseInt(productCategoryId) : null
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
                    row.append('<td>' + (item.productCategoryName || '') + '</td>');

                    // ImplantType enum: 1 = DeNovo, 2 = GenChange
                    var implantTypeBadge = '';
                    if (item.implantType === 1) {
                        implantTypeBadge = '<span class="badge bg-success">NEW (De Novo)</span>';
                    } else if (item.implantType === 2) {
                        implantTypeBadge = '<span class="badge bg-info">GEN CHANGE</span>';
                    }
                    row.append('<td>' + implantTypeBadge + '</td>');

                    row.append('<td class="text-center">' + (item.totalCases || 0) + '</td>');
                    row.append('<td>$' + (item.totalAmount ? item.totalAmount.toFixed(2) : '0.00') + '</td>');
                    row.append('<td>$' + (item.averageAmount ? item.averageAmount.toFixed(2) : '0.00') + '</td>');
                    tbody.append(row);
                });
            } else {
                tbody.append('<tr><td colspan="6" class="text-center">No data found</td></tr>');
            }
        }
    });
})();
