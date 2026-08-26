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
                                visible: function () { return _permissions.edit; },
                                action: function (data) {
                                    _createOrEditModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: app.localize('Delete'),
                                visible: function () { return _permissions.delete; },
                                action: function (data) {
                                    deleteHospital(data.record);
                                }
                            }
                        ]
                    }
                },
                { targets: 1, data: 'hospitalName', name: 'FacilityName' },
                {
                    targets: 2,
                    data: 'physicianCount',
                    name: 'physicianCount',
                    orderable: false,
                    className: 'text-center'
                },
                {
                    targets: 3,
                    data: 'productPriceCount',
                    name: 'productPriceCount',
                    orderable: false,
                    className: 'text-center'
                }
            ]
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
