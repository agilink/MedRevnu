(function () {
    $(function () {
        var _$patientsTable = $('#PatientsTable');
        var _patientsService = abp.services.app.patients;
        var _selectedPermissionNames = [];
        var $selectedDate = {
            startDate: null,
            endDate: null,
        };

        //$('.date-picker').on('apply.daterangepicker', function (ev, picker) {
        //    $(this).val(picker.startDate.format('MM/DD/YYYY'));
        //});

        //$('.startDate')
        //    .daterangepicker()
        //    .on('apply.daterangepicker', (ev, picker) => {
        //        $selectedDate.startDate = picker.startDate;
        //        getPatients();
        //    })
        //    .on('cancel.daterangepicker', function (ev, picker) {
        //        $(this).val('');
        //        $selectedDate.startDate = null;
        //        getPatients();
        //    });

        //$('.endDate')
        //    .daterangepicker()
        //    .on('apply.daterangepicker', (ev, picker) => {
        //        $selectedDate.endDate = picker.startDate;
        //        getPatients();
        //    })
        //    .on('cancel.daterangepicker', function (ev, picker) {
        //        $(this).val('');
        //        $selectedDate.endDate = null;
        //        getPatients();
        //    });


        var _permissions = {
            create: abp.auth.hasPermission('Pages.Patients.Create'),
            edit: abp.auth.hasPermission('Pages.Patients.Edit'),
            delete: abp.auth.hasPermission('Pages.Patients.Delete'),
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Pharmacy/Patients/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Pharmacy/Views/Patients/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditPatientModal',
            modalSize: 'modal-fullscreen'
        });

        var _viewPatientModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Pharmacy/Patients/ViewpatientModal',
            modalClass: 'ViewPatientModal',
        });

        var getDateFilter = function (element) {
            if ($selectedDate.startDate == null) {
                return null;
            }
            return $selectedDate.startDate.format('YYYY-MM-DDT00:00:00Z');
        };

        var getMaxDateFilter = function (element) {
            if ($selectedDate.endDate == null) {
                return null;
            }
            return $selectedDate.endDate.format('YYYY-MM-DDT23:59:59Z');
        };

        var _excelColumnSelectionModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Pharmacy/Patients/ExcelColumnSelectionModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Pharmacy/Views/Patients/_ExcelColumnSelectionModal.js',
            modalClass: 'PatientExcelColumnSelectionModal',
        });


        var columns = [
            {
                className: 'control responsive',
                orderable: false,
                render: function () {
                    return '';
                },
                targets: 0,
            },

            {
                targets: 1,
                data: 'patient.name',
                name: 'User.name',
                className: 'dt-left'
            },
            {
                targets: 2,
                data: 'patient.surname',
                name: 'User.surname',
                className: 'dt-left'
            },
            {
                targets: 3,
                data: 'patient.dateOfBirth',
                name: 'User.dateOfBirth',
                className: 'dt-left',
                render: function (data, type, row) {
                    if (type === 'display' && data) {
                        // Format the date using JavaScript
                        const date = new Date(data);
                        return `${(date.getMonth() + 1).toString().padStart(2, '0')}/${date.getDate().toString().padStart(2, '0')}/${date.getFullYear()}`;
                    }
                    return data;
                }
            },
            {
                targets: 4,
                data: 'patient.phoneNumber',
                name: 'User.phoneNumber',
                className: 'dt-left',
                render: function (data, type, row) {
                    // Format phone number (e.g., (123) 456-7890)
                    return formatPhoneNumber(data);
                }
            },
            {
                targets: 5,
                data: 'patient.genderName',
                name: 'genderName',
                className: 'dt-left'
            }
        ];

        if (_permissions.edit) {
            columns.push({
                data: null,
                orderable: false,
                defaultContent: '', visible: function () {
                    return _permissions.edit;
                },
                render: function (data) {
                    // Using the action function to create a button
                    return "<button class='btn btn-primary edit-patient' data-id='" + data.patient.id + "'><i class='fa fa-edit'></i></button>";
                },
                targets: 6
            });
        }

        var dataTable = _$patientsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _patientsService.getAll,
                inputFilter: function () {
                    return {
                        filter: $('#PatientsTableFilter').val() || '',
                       
                    };
                },
            },
            columnDefs: columns
        });

        function formatPhoneNumber(phoneNumber) {
            // Remove non-numeric characters
            const cleaned = ('' + phoneNumber).replace(/\D/g, '');
            const match = cleaned.match(/^(\d{3})(\d{3})(\d{4})$/);
            if (match) {
                return '(' + match[1] + ') ' + match[2] + '-' + match[3];
            }
            return phoneNumber; // Return as is if not valid
        }

        function getPatients() {
            dataTable.ajax.reload();
        }

        function deletePatient(patient) {
            abp.message.confirm('', app.localize('AreYouSure'), function (isConfirmed) {
                if (isConfirmed) {
                    _patientsService
                        .delete({
                            id: patient.id,
                        })
                        .done(function () {
                            getPatients(true);
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                }
            });
        }
        // Initialize click event listeners for action buttons
        _$patientsTable.on('draw.dt', function () {
            // Re-initialize button click actions after data has been loaded/redrawn           
            $('.edit-patient').on('click', function () {
                var id = $(this).attr("data-id");
                _createOrEditModal.open({ id: id });

            });

            $('.view-patient').on('click', function () {
                var id = $(this).attr("data-id");
                _viewDoctorModal.open({ id: id });
            });
        });
        $('#ShowAdvancedFiltersSpan').click(function () {
            $('#ShowAdvancedFiltersSpan').hide();
            $('#HideAdvancedFiltersSpan').show();
            $('#AdvacedAuditFiltersArea').slideDown();
        });

        $('#HideAdvancedFiltersSpan').click(function () {
            $('#HideAdvancedFiltersSpan').hide();
            $('#ShowAdvancedFiltersSpan').show();
            $('#AdvacedAuditFiltersArea').slideUp();
        });

        $('#CreateNewPatientButton').click(function () {
            _createOrEditModal.open();
        });
       //$('#ExportToExcelButton').click(function (e) {
       //    _excelColumnSelectionModal.open({
       //         filter: $('#PatientsTableFilter').val(),
       //         //permissions: _selectedPermissionNames,
       //         //role: $('#RoleSelectionCombo').val(),
       //         //onlyLockedUsers: $('#UsersTable_OnlyLockedUsers').is(':checked'),
       //         sorting: getSortingFromDatatable(),
       //     });
       // });

        $('#ExportToExcelButton').click(function () {
            _excelColumnSelectionModal.open({
                filter: $('#PatientsTableFilter').val() || '',
                defaultFacilityId:0,
            });
        });


        abp.event.on('app.createOrEditPatientModalSaved', function () {
            getPatients();
        });
        abp.event.on('app.patientDataDeleted', function () {
            getPatients();
        });

        $('#GetPatientsButton').click(function (e) {
            e.preventDefault();
            getPatients();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getPatients();
            }
        });

        $('.reload-on-change').change(function (e) {
            getPatients();
        });
            //$('#FacilityList').change(function (e) {
            //    getPatients();
            //});

        $('.reload-on-keyup').keyup(function (e) {
            getPatients();
        });

        $('#btn-reset-filters').click(function (e) {
            $('.reload-on-change,.reload-on-keyup,#MyEntsTableFilter').val('');
            getPatients();
        });

        var getSortingFromDatatable = function () {
            if (dataTable.ajax.params().order.length > 0) {
                var columnIndex = dataTable.ajax.params().order[0].column;
                var dir = dataTable.ajax.params().order[0].dir;
                var columnName = dataTable.ajax.params().columns[columnIndex].data;

                return columnName + ' ' + dir;
            } else {
                return '';
            }
        };

        var url = abp.appPath + 'Pharmacy/Patients/ImportFromExcel';
        const id = '#ImportPatientsFromExcelButton';
        let dropZone;

        dropZone = new Dropzone(id, {
            url: url,
            method: 'post',
            acceptedFiles: '.xlsx, .xls, .csv',
            paramName: 'file',
            maxFilesize: 1048576 * 100,
            maxFiles: 1,
            clickable: id + " .dropzone-select"
        });

        dropZone.on("sending", function (file, xhr, formData) {
            var token = abp.security.antiForgery.getToken();
            formData.append('__RequestVerificationToken', token);
        });

        dropZone.on("success", function () {
            abp.notify.info(app.localize('ImportPatientsProcessStart'));

            $(id).prop('disabled', !$.support.fileInput)
                .parent()
                .addClass($.support.fileInput ? undefined : 'disabled');
        });

        dropZone.on("error", function () {
            abp.notify.warn(app.localize('ImportPatientsProcessFailed'));
        });

        dropZone.on("complete", function (file) {
            dropZone.removeFile(file);
        });
    });
})();
