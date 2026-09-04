(function () {
    $(function () {
        var _$quotasTable = $('#ProductQuotasTable');
        var _quotasService = abp.services.app.productQuotas;

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Revenue.ProductQuotas.Create'),
            edit: abp.auth.hasPermission('Pages.Revenue.ProductQuotas.Edit'),
            delete: abp.auth.hasPermission('Pages.Revenue.ProductQuotas.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Revenue/ProductQuotas/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Revenue/Views/ProductQuotas/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditProductQuotaModal'
        });

        var dataTable = _$quotasTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _quotasService.getAll,
                inputFilter: function () {
                    var filter = $('#ProductQuotasTableFilter').val();
                    var year = $('#YearFilter').val();
                    var month = $('#MonthFilter').val();
                    var hospitalId = $('#HospitalFilter').val();
                    var categoryId = $('#ProductCategoryFilter').val();

                    return {
                        filter: filter || null,
                        yearFilter: year ? parseInt(year) : null,
                        monthFilter: month ? parseInt(month) : null,
                        hospitalIdFilter: hospitalId ? parseInt(hospitalId) : null,
                        productCategoryIdFilter: categoryId ? parseInt(categoryId) : null
                    };
                }
            },
            columnDefs: [
                {
                    targets: 0,
                    data: 'periodYear',
                    name: 'periodYear',
                    className: 'text-center'
                },
                {
                    targets: 1,
                    data: 'periodMonth',
                    name: 'periodMonth',
                    className: 'text-center',
                    render: function (periodMonth) {
                        var monthNames = ['January', 'February', 'March', 'April', 'May', 'June',
                            'July', 'August', 'September', 'October', 'November', 'December'];
                        return monthNames[periodMonth - 1] || periodMonth;
                    }
                },
                {
                    targets: 2,
                    data: 'productCategoryName',
                    name: 'productCategoryName'
                },
                {
                    targets: 3,
                    data: 'targetAmount',
                    name: 'targetAmount',
                    render: function (targetAmount) {
                        if (targetAmount) {
                            return '$' + targetAmount.toFixed(2);
                        }
                        return '$0.00';
                    }
                },
                {
                    targets: 4,
                    data: 'hospitalName',
                    name: 'hospitalName'
                },
                {
                    targets: 5,
                    data: 'targetUnits',
                    name: 'targetUnits',
                    className: 'text-center',
                    render: function (targetUnits) {
                        return targetUnits || '-';
                    }
                },
                {
                    // Icon buttons rather than a dropdown, and last rather than first: the actions
                    // belong beside the row they act on, and one click instead of two.
                    targets: 6,
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

        _$quotasTable.on('click', '.ati-act-edit', function () {
            var data = { record: dataTable.row($(this).closest('tr')).data() };
            _createOrEditModal.open({ id: data.record.id });
        });

        _$quotasTable.on('click', '.ati-act-delete', function () {
            var data = { record: dataTable.row($(this).closest('tr')).data() };
            deleteQuota(data.record);
        });

        function getQuotas() {
            dataTable.ajax.reload();
        }

        function deleteQuota(quota) {
            abp.message.confirm(
                app.localize('ProductQuotaDeleteWarningMessage'),
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _quotasService
                            .delete({
                                id: quota.id
                            })
                            .done(function () {
                                getQuotas();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                    }
                }
            );
        }

        // Main search button
        $('#GetProductQuotasButton').click(function (e) {
            e.preventDefault();
            getQuotas();
        });

        // Search on Enter key in main filter
        $('#ProductQuotasTableFilter').on('keydown', function (e) {
            if (e.keyCode === 13) {
                e.preventDefault();
                getQuotas();
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
        $('#YearFilter, #MonthFilter, #HospitalFilter, #ProductCategoryFilter').change(function () {
            getQuotas();
        });

        $('#CreateNewQuotaButton').click(function () {
            _createOrEditModal.open();
        });

        $('#RefreshQuotasButton').click(function (e) {
            e.preventDefault();
            getQuotas();
        });

        abp.event.on('app.createOrEditProductQuotaModalSaved', function () {
            getQuotas();
        });
    });
})();
