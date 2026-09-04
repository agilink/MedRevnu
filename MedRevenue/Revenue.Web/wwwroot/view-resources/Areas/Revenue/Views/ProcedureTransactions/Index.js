(function () {
    $(function () {
        var _$transactionsTable = $('#ProcedureTransactionsTable');
        var _transactionsService = abp.services.app.procedureTransactions;
        var CASE_STATUS_LABELS = {
            1: 'Open', 2: 'Scheduled', 3: 'Completed', 4: 'Billed', 5: 'Paid', 6: 'Closed'
        };


        var _permissions = {
            create: abp.auth.hasPermission('Pages.Revenue.ProcedureTransactions.Create'),
            edit: abp.auth.hasPermission('Pages.Revenue.ProcedureTransactions.Edit'),
            delete: abp.auth.hasPermission('Pages.Revenue.ProcedureTransactions.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Revenue/ProcedureTransactions/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Revenue/Views/ProcedureTransactions/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditProcedureTransactionModal',
            // The case form carries a device grid and a dozen fields, so it takes the
            // whole page rather than ModalManager's default modal-lg.
            modalSize: 'modal-fullscreen'
        });

        var dataTable = _$transactionsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _transactionsService.getAll,
                inputFilter: function () {
                    var filter = $('#ProcedureTransactionsTableFilter').val();
                    var year = $('#YearFilter').val();
                    var month = $('#MonthFilter').val();
                    var hospitalId = $('#HospitalFilter').val();
                    var physicianId = $('#PhysicianFilter').val();

                    return {
                        filter: filter || null,
                        yearFilter: year ? parseInt(year) : null,
                        monthFilter: month ? parseInt(month) : null,
                        hospitalIdFilter: hospitalId ? parseInt(hospitalId) : null,
                        physicianIdFilter: physicianId ? parseInt(physicianId) : null,
                        implantTypeFilter: null
                    };
                }
            },
            columnDefs: [
                {
                    targets: 0,
                    data: 'caseNumber',
                    name: 'caseNumber'
                },
                {
                    targets: 1,
                    data: 'hospitalName',
                    name: 'hospitalName',
                    render: function (hospitalName) {
                        return hospitalName || '-';
                    }
                },
                {
                    targets: 2,
                    data: 'physicianName',
                    name: 'physicianName'
                },
                {
                    targets: 3,
                    data: 'procedureDate',
                    name: 'procedureDate',
                    render: function (procedureDate) {
                        return procedureDate ? moment(procedureDate).format('L') : '';
                    }
                },
                {
                    targets: 4,
                    data: 'implantType',
                    name: 'implantType',
                    render: function (implantType) {
                        // ImplantType enum: 1 = DeNovo, 2 = GenChange
                        if (implantType === 1) {
                            return '<span class="badge bg-success">NEW (De Novo)</span>';
                        } else if (implantType === 2) {
                            return '<span class="badge bg-info">GEN CHANGE</span>';
                        }
                        return '-';
                    }
                },
                {
                    targets: 5,
                    data: 'status',
                    name: 'status',
                    render: function (status) {
                        // Recorded only: reports still count every case regardless of status.
                        var label = CASE_STATUS_LABELS[status] || '-';
                        var css = status === 5 || status === 6 ? 'badge-light-success' : 'badge-light-primary';
                        return '<span class="badge ' + css + '">' + label + '</span>';
                    }
                },
                {
                    // A case can use several devices, so the grid summarises them.
                    targets: 6,
                    data: 'productSummary',
                    name: 'productSummary',
                    orderable: false,
                    render: function (productSummary, type, row) {
                        if (!row.deviceCount) {
                            return '<span class="text-danger">' + app.localize('NoDevicesYet') + '</span>';
                        }
                        var units = row.totalUnits && row.totalUnits !== row.deviceCount
                            ? ' (' + row.totalUnits + ' units)'
                            : '';
                        return $('<span></span>').text((productSummary || '') + units).html();
                    }
                },
                {
                    targets: 7,
                    data: 'totalAmount',
                    name: 'totalAmount',
                    className: 'text-end',
                    render: function (totalAmount) {
                        return '$' + (totalAmount || 0).toLocaleString('en-US', {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        });
                    }
                },
                {
                    // Icon buttons rather than a dropdown, and last rather than first: the actions
                    // belong beside the row they act on, and one click instead of two.
                    targets: 8,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    className: 'text-end ati-row-actions',
                    render: function (unused, type, row) {
                        var html = '';

                        if ((function (data) { return _permissions.edit; })({ record: row })) {
                            html += '<button type="button" class="btn btn-sm btn-icon btn-light-primary ms-1 ati-act-edit" '
                                + 'title="' + app.localize('Edit') + '"><i class="fa fa-pen"></i></button>';
                        }

                        if ((function (data) { return _permissions.delete; })({ record: row })) {
                            html += '<button type="button" class="btn btn-sm btn-icon btn-light-danger ms-1 ati-act-delete" '
                                + 'title="' + app.localize('Delete') + '"><i class="fa fa-trash"></i></button>';
                        }

                        return html;
                    }
                }
            ]
        });


        // Delegated: the grid redraws its rows, so per-row binding would not
        // survive a page change or a sort.

        _$transactionsTable.on('click', '.ati-act-edit', function () {
            var data = { record: dataTable.row($(this).closest('tr')).data() };
            _createOrEditModal.open({ id: data.record.id });
        });

        _$transactionsTable.on('click', '.ati-act-delete', function () {
            var data = { record: dataTable.row($(this).closest('tr')).data() };
            deleteTransaction(data.record);
        });

        function getTransactions() {
            dataTable.ajax.reload();
        }

        function deleteTransaction(transaction) {
            abp.message.confirm(
                app.localize('ProcedureTransactionDeleteWarningMessage'),
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _transactionsService
                            .delete({
                                id: transaction.id
                            })
                            .done(function () {
                                getTransactions();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                    }
                }
            );
        }

        $('#GetProcedureTransactionsButton').click(function (e) {
            e.preventDefault();
            getTransactions();
        });

        $('#ProcedureTransactionsTableFilter').on('keydown', function (e) {
            if (e.keyCode === 13) {
                e.preventDefault();
                getTransactions();
            }
        });

        // Advanced filters toggle
        $('#ShowAdvancedFiltersSpan').click(function () {
            $('#ShowAdvancedFiltersSpan').hide();
            $('#HideAdvancedFiltersSpan').show();
            $('#AdvancedAuditFiltersArea').slideDown();
        });

        $('#HideAdvancedFiltersSpan').click(function () {
            $('#HideAdvancedFiltersSpan').hide();
            $('#ShowAdvancedFiltersSpan').show();
            $('#AdvancedAuditFiltersArea').slideUp();
        });

        // Filter changes in advanced filters
        $('#YearFilter, #MonthFilter, #HospitalFilter, #PhysicianFilter').change(function () {
            getTransactions();
        });

        $('#CreateNewTransactionButton').click(function () {
            _createOrEditModal.open();
        });

        $('#RefreshTransactionsButton').click(function (e) {
            e.preventDefault();
            getTransactions();
        });

        abp.event.on('app.createOrEditProcedureTransactionModalSaved', function () {
            getTransactions();
        });
    });
})();
