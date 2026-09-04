(function () {
    $(function () {
        var _$hospitalsTable = $('#HospitalsTable');
        var _hospitalsService = abp.services.app.hospitals;

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Revenue.Hospitals.Create'),
            edit: abp.auth.hasPermission('Pages.Revenue.Hospitals.Edit'),
            delete: abp.auth.hasPermission('Pages.Revenue.Hospitals.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Revenue/Hospitals/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Revenue/Views/Hospitals/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditHospitalModal'
        });

        var dataTable = _$hospitalsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _hospitalsService.getAll,
                inputFilter: function () {
                    return {
                        filter: $('#HospitalsTableFilter').val() || null
                    };
                }
            },
            columnDefs: [
                { targets: 0, data: 'hospitalName', name: 'FacilityName' },
                {
                    targets: 1,
                    data: 'physicianCount',
                    name: 'physicianCount',
                    orderable: false,
                    className: 'text-center'
                },
                {
                    targets: 2,
                    data: 'productPriceCount',
                    name: 'productPriceCount',
                    orderable: false,
                    className: 'text-center'
                },
                {
                    // Icon buttons rather than a dropdown, and last rather than first: the actions
                    // belong beside the row they act on, and one click instead of two.
                    targets: 3,
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


        // Delegated: the grid redraws its rows, so per-row binding would not survive
        // a page change or a sort.

        _$hospitalsTable.on('click', '.ati-act-edit', function () {
            var data = { record: dataTable.row($(this).closest('tr')).data() };
            _createOrEditModal.open({ id: data.record.id });
        });

        _$hospitalsTable.on('click', '.ati-act-delete', function () {
            var data = { record: dataTable.row($(this).closest('tr')).data() };
            deleteHospital(data.record);
        });

        function reload() {
            dataTable.ajax.reload();
        }

        function deleteHospital(hospital) {
            abp.message.confirm(
                app.localize('HospitalDeleteWarningMessage'),
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        // The service refuses while transactions, quotas, prices or
                        // physicians still reference it, and says which.
                        _hospitalsService.delete({ id: hospital.id }).done(function () {
                            reload();
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                    }
                }
            );
        }

        $('#GetHospitalsButton').click(function (e) {
            e.preventDefault();
            reload();
        });

        $('#HospitalsTableFilter').on('keydown', function (e) {
            if (e.keyCode === 13) {
                e.preventDefault();
                reload();
            }
        });

        $('#CreateNewHospitalButton').click(function () {
            _createOrEditModal.open();
        });

        abp.event.on('app.createOrEditHospitalModalSaved', function () {
            reload();
        });
    });
})();
