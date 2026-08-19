(function () {
    $(function () {
        var _$transactionsTable = $('#ProcedureTransactionsTable');
        var _transactionsService = abp.services.app.procedureTransactions;

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Revenue.ProcedureTransactions.Create'),
            edit: abp.auth.hasPermission('Pages.Revenue.ProcedureTransactions.Edit'),
            delete: abp.auth.hasPermission('Pages.Revenue.ProcedureTransactions.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Revenue/ProcedureTransactions/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Revenue/Views/ProcedureTransactions/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditProcedureTransactionModal'
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
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        cssClass: 'btn btn-sm btn-light btn-active-light-primary',
                        text: '<i class="fa fa-cog"></i> ' + app.localize('Actions') + ' <span class="caret"></span>',
                        items: [
                            {
                                text: app.localize('Edit'),
                                visible: function () {
                                    return _permissions.edit;
                                },
                                action: function (data) {
                                    _createOrEditModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: app.localize('Delete'),
                                visible: function () {
                                    return _permissions.delete;
                                },
                                action: function (data) {
                                    deleteTransaction(data.record);
                                }
                            }
                        ]
                    }
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
                        if (procedureDate) {
                            return moment(procedureDate).format('L');
                        }
                        return '';
                    }
                },
                {
                    targets: 4,
                    data: 'productName',
                    name: 'productName'
                },
                {
                    targets: 5,
                    data: 'productCode',
                    name: 'productCode',
                    render: function (productCode) {
                        return productCode || '-';
                    }
                },
                {
                    targets: 6,
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
                    targets: 7,
                    data: 'quantity',
                    name: 'quantity',
                    className: 'text-center'
                },
                {
                    targets: 8,
                    data: 'unitPrice',
                    name: 'unitPrice',
                    render: function (unitPrice) {
                        if (unitPrice) {
                            return '$' + unitPrice.toFixed(2);
                        }
                        return '$0.00';
                    }
                },
                {
                    targets: 9,
                    data: 'totalAmount',
                    name: 'totalAmount',
                    render: function (totalAmount) {
                        if (totalAmount) {
                            return '$' + totalAmount.toFixed(2);
                        }
                        return '$0.00';
                    }
                }
            ]
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
