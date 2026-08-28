(function () {
    $(function () {
        var _$physiciansTable = $('#PhysiciansTable');
        var _physiciansService = abp.services.app.physicians;

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Revenue.Physicians.Create'),
            edit: abp.auth.hasPermission('Pages.Revenue.Physicians.Edit'),
            delete: abp.auth.hasPermission('Pages.Revenue.Physicians.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Revenue/Physicians/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Revenue/Views/Physicians/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditPhysicianModal'
        });

        var STATUS_LABELS = {
            1: 'Active',
            2: 'On Leave',
            3: 'Terminated',
            4: 'Retired',
            5: 'Suspended',
            6: 'Inactive'
        };

        var dataTable = _$physiciansTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _physiciansService.getAll,
                inputFilter: function () {
                    return {
                        filter: $('#PhysiciansTableFilter').val() || null,
                        // 'none' finds physicians with no hospital at all; blank means no filter.
                        hospitalIdFilter: $('#HospitalFilter').val() && $('#HospitalFilter').val() !== 'none'
                            ? parseInt($('#HospitalFilter').val(), 10)
                            : null,
                        unassignedHospitalOnly: $('#HospitalFilter').val() === 'none',
                        employeeStatusFilter: $('#EmployeeStatusFilter').val()
                            ? parseInt($('#EmployeeStatusFilter').val())
                            : null
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
                                text: app.localize('CreateUser'),
                                // Hidden once they have one - a physician gets one login,
                                // and the flag column shows which already do.
                                visible: function (data) {
                                    return _permissions.edit && !data.record.hasUser;
                                },
                                action: function (data) {
                                    createUserForPhysician(data.record);
                                }
                            },
                            {
                                text: app.localize('Delete'),
                                visible: function () { return _permissions.delete; },
                                action: function (data) {
                                    deletePhysician(data.record);
                                }
                            }
                        ]
                    }
                },
                { targets: 1, data: 'lastName', name: 'LAST_NAME' },
                { targets: 2, data: 'firstName', name: 'FIRST_NAME' },
                {
                    targets: 3,
                    data: 'hospitalName',
                    name: 'hospitalName',
                    orderable: false,
                    render: function (hospitalName) {
                        // A physician with no hospital leaves their transactions unassigned,
                        // so it is called out rather than shown as an empty cell.
                        return hospitalName
                            ? hospitalName
                            : '<span class="badge badge-light-warning">' + app.localize('NotSet') + '</span>';
                    }
                },
                {
                    targets: 4,
                    data: 'employeeId',
                    name: 'EMPLOYEE_ID',
                    render: function (employeeId) { return employeeId || '-'; }
                },
                {
                    targets: 5,
                    data: 'emailWork',
                    name: 'EMAIL_WORK',
                    render: function (emailWork) { return emailWork || '-'; }
                },
                {
                    targets: 6,
                    data: 'mobileNumber',
                    name: 'NUMBER_MOBILE',
                    render: function (mobileNumber) { return mobileNumber || '-'; }
                },
                {
                    targets: 7,
                    data: 'employeeStatus',
                    name: 'EmployeeStatusID',
                    render: function (employeeStatus) {
                        if (!employeeStatus) {
                            return '<span class="text-muted">' + app.localize('NotSet') + '</span>';
                        }

                        var css = employeeStatus === 1 ? 'badge-light-success' : 'badge-light-secondary';
                        return '<span class="badge ' + css + '">' + (STATUS_LABELS[employeeStatus] || employeeStatus) + '</span>';
                    }
                },
                {
                    targets: 8,
                    data: 'userName',
                    name: 'UserId',
                    orderable: false,
                    render: function (userName, type, row) {
                        // The username is the flag: it says both that a login exists and
                        // what it is, which is what anyone looking at this column wants.
                        // The wrapper carries the physician id so creating a login can put
                        // a spinner in this exact cell without depending on row internals.
                        var content = row.hasUser
                            ? '<span class="badge badge-light-success">' + $('<div>').text(userName || '').html() + '</span>'
                            : '<span class="text-muted">-</span>';

                        return '<span class="login-cell" data-physician-id="' + row.id + '">' + content + '</span>';
                    }
                }
            ]
        });

        function reload() {
            dataTable.ajax.reload();
        }

        function deletePhysician(physician) {
            abp.message.confirm(
                app.localize('PhysicianDeleteWarningMessage'),
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _physiciansService.delete({ id: physician.id }).done(function () {
                            reload();
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                    }
                }
            );
        }

        // Physicians whose login is being created right now. Creating one takes a
        // noticeable moment - it writes a user, assigns a role and updates the physician -
        // so the row has to say something is happening, and a second click on the same
        // physician has to be ignored while it does.
        var _creatingUserFor = {};

        function $loginCell(physicianId) {
            return _$physiciansTable.find('.login-cell[data-physician-id="' + physicianId + '"]');
        }

        function showCreatingUser(physicianId) {
            $loginCell(physicianId).html(
                '<span class="spinner-border spinner-border-sm align-middle text-primary" role="status"></span>'
                + '<span class="text-muted fs-8 ms-2">' + app.localize('CreatingLogin') + '</span>');
        }

        function createUserForPhysician(physician) {
            if (_creatingUserFor[physician.id]) {
                return;
            }

            abp.message.confirm(
                abp.utils.formatString(
                    app.localize('CreateUserForPhysicianConfirmation'),
                    physician.fullName,
                    buildUserName(physician)),
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (!isConfirmed) {
                        return;
                    }

                    _creatingUserFor[physician.id] = true;
                    showCreatingUser(physician.id);

                    _physiciansService.createUserForPhysician({ id: physician.id })
                        .done(function (result) {
                            // The reload replaces the spinner with the new username.
                            reload();

                            abp.notify.success(abp.utils.formatString(
                                app.localize('PhysicianUserCreated'),
                                result.userName,
                                result.password));

                            // A generated address cannot receive password-reset mail, so
                            // say so now rather than let it be discovered later.
                            if (result.usedFallbackEmail) {
                                abp.message.warn(abp.utils.formatString(
                                    app.localize('PhysicianUserFallbackEmailWarning'),
                                    result.emailAddress));
                            }
                        })
                        .fail(function () {
                            // Put the cell back; leaving a spinner spinning forever would
                            // read as still working.
                            $loginCell(physician.id).html('<span class="text-muted">-</span>');
                        })
                        .always(function () {
                            delete _creatingUserFor[physician.id];
                        });
                }
            );
        }

        // Mirrors the server so the confirmation can show the username before it is
        // created. The server remains the authority, and appends a number on a clash.
        function buildUserName(physician) {
            var initial = (physician.firstName || '').replace(/[^a-z0-9]/gi, '').charAt(0);
            var surname = (physician.lastName || '').replace(/[^a-z0-9]/gi, '');

            return (initial + surname).toLowerCase();
        }

        $('#GetPhysiciansButton').click(function (e) {
            e.preventDefault();
            reload();
        });

        $('#PhysiciansTableFilter').on('keydown', function (e) {
            if (e.keyCode === 13) {
                e.preventDefault();
                reload();
            }
        });

        $('#HospitalFilter, #EmployeeStatusFilter').change(function () {
            reload();
        });

        $('#CreateNewPhysicianButton').click(function () {
            _createOrEditModal.open();
        });

        abp.event.on('app.createOrEditPhysicianModalSaved', function () {
            reload();
        });
    });
})();
