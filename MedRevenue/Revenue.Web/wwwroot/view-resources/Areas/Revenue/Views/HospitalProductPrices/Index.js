(function () {
    $(function () {
        var _$pricesTable = $('#HospitalProductPricesTable');
        var _pricesService = abp.services.app.hospitalProductPrices;

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Revenue.HospitalProductPrices.Create'),
            edit: abp.auth.hasPermission('Pages.Revenue.HospitalProductPrices.Edit'),
            delete: abp.auth.hasPermission('Pages.Revenue.HospitalProductPrices.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Revenue/HospitalProductPrices/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Revenue/Views/HospitalProductPrices/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditHospitalProductPriceModal'
        });

        var dataTable = _$pricesTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _pricesService.getAll,
                inputFilter: function () {
                    return {
                        filter: $('#HospitalProductPricesTableFilter').val() || null,
                        hospitalIdFilter: $('#HospitalFilter').val() ? parseInt($('#HospitalFilter').val()) : null,
                        isActiveFilter: $('#IsActiveFilter').val() !== '' ? ($('#IsActiveFilter').val() === 'true') : null
                    };
                }
            },
            columnDefs: [
                {
                    targets: 0,
                    data: 'hospitalName',
                    name: 'hospitalName'
                },
                {
                    targets: 1,
                    data: 'productName',
                    name: 'productName'
                },
                {
                    targets: 2,
                    data: 'productCode',
                    name: 'productCode',
                    render: function (productCode) {
                        return productCode || '-';
                    }
                },
                {
                    targets: 3,
                    data: 'unitPrice',
                    name: 'unitPrice',
                    className: 'text-end',
                    render: function (unitPrice) {
                        return '$' + parseFloat(unitPrice).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    }
                },
                {
                    targets: 4,
                    data: 'effectiveDate',
                    name: 'effectiveDate',
                    render: function (effectiveDate) {
                        if (effectiveDate) {
                            return moment(effectiveDate).format('MM/DD/YYYY');
                        }
                        return '';
                    }
                },
                {
                    targets: 5,
                    data: 'isActive',
                    name: 'isActive',
                    className: 'text-center',
                    render: function (isActive) {
                        if (isActive) {
                            return '<span class="badge badge-light-success">' + app.localize('Active') + '</span>';
                        }
                        return '<span class="badge badge-light-danger">' + app.localize('Inactive') + '</span>';
                    }
                },
                {
                    targets: 6,
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
                                    deletePrice(data.record);
                                }
                            }
                        ]
                    }
                }
            ]
        });

        function getPrices() {
            dataTable.ajax.reload();
        }

        function deletePrice(price) {
            abp.message.confirm(
                app.localize('HospitalProductPriceDeleteWarningMessage'),
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _pricesService
                            .delete({ id: price.id })
                            .done(function () {
                                getPrices();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                    }
                }
            );
        }

        $('#GetHospitalProductPricesButton').click(function (e) {
            e.preventDefault();
            getPrices();
        });

        $('#HospitalProductPricesTableFilter').on('keydown', function (e) {
            if (e.keyCode === 13) {
                e.preventDefault();
                getPrices();
            }
        });

        $('#ShowAdvancedFiltersSpan').click(function () {
            $('#ShowAdvancedFiltersSpan').hide();
            $('#HideAdvancedFiltersSpan').show();
            $('#AdvancedFiltersArea').slideDown();
        });

        $('#HideAdvancedFiltersSpan').click(function () {
            $('#HideAdvancedFiltersSpan').hide();
            $('#ShowAdvancedFiltersSpan').show();
            $('#AdvancedFiltersArea').slideUp();
        });

        $('#HospitalFilter, #IsActiveFilter').change(function () {
            getPrices();
        });

        $('#CreateNewHospitalProductPriceButton').click(function () {
            if (_permissions.create) {
                _createOrEditModal.open();
            }
        });

        $('#RefreshHospitalProductPricesButton').click(function (e) {
            e.preventDefault();
            getPrices();
        });

        abp.event.on('app.createOrEditHospitalProductPriceModalSaved', function () {
            getPrices();
        });
    });
})();
