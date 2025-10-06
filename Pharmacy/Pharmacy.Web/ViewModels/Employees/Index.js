(function () {
    $(function () {
        var _$usersTable = $('#UsersTable');
        var _userService = abp.services.app.userExtended;

        var _selectedPermissionNames = [];

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Employee.Create'),
            edit: abp.auth.hasPermission('Pages.Employee.Edit'),
            delete: abp.auth.hasPermission('Pages.Employee.Delete'),
        };

        //var _createOrEditModal = new app.ModalManager({
        //    viewUrl: abp.appPath + 'Core/Users/CreateOrEditModal',
        //    scriptUrl: abp.appPath + 'view-resources/Areas/Core/Views/Users/_CreateOrEditModal.js',
        //    modalClass: 'CreateOrEditUserModal',
        //});
        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Pharmacy/Employees/CreateOrEdit',
            scriptUrl: abp.appPath + 'view-resources/Areas/Pharmacy/Views/Employees/_PharmacyUserDetails.js',
            modalClass: 'CreateOrEditUserModal',
            modalSize: 'modal-fullscreen'
        });

        debugger
        var dataTable = _$usersTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _userService.getUsers,
                inputFilter: function () {
                    return {
                        filter: $('#UsersTableFilter').val(),
                        facility: $('#CompanyList').val(),
                        role: $('#RoleList').val()
                    };
                },
            },
            drawCallback: function () {
                $('[data-bs-toggle="tooltip"]').tooltip();
            },
            columnDefs: [
                {
                    className: 'dtr-control responsive',
                    orderable: false,
                    render: function () {
                        return '';
                    },
                    targets: 0,
                },
                {
                    targets: 1,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        text:
                            '<i class="fa fa-cog"></i> <span class="d-none d-md-inline-block d-lg-inline-block d-xl-inline-block">' +
                            app.localize('Actions') +
                            '</span> <span class="caret"></span>',
                        items: [

                            {
                                text: app.localize('Edit'),
                                visible: function () {
                                    return _permissions.edit;
                                },
                                action: function (data) {
                                    _createOrEditModal.open({ id: data.record.id });
                                },
                            },

                            {
                                text: app.localize('Delete'),
                                visible: function () {
                                    return _permissions.delete;
                                },
                                action: function (data) {
                                    deleteUser(data.record);
                                },
                            },
                        ],
                    },
                },
                {
                    targets: 2,
                    data: 'userName',
                },
                {
                    targets: 3,
                    data: 'name',
                },
                {
                    targets: 4,
                    data: 'surname',
                },
                {
                    targets: 5,
                    data: 'roles',
                    orderable: false,
                    render: function (roles) {
                        var roleNames = '';
                        for (var j = 0; j < roles.length; j++) {
                            if (roleNames.length) {
                                roleNames = roleNames + ', ';
                            }

                            roleNames = roleNames + roles[j].roleName;
                        }

                        return roleNames;
                    },
                },
                {
                    targets: 6,
                    data: 'company',
                    orderable: false
                },
                {
                    targets: 7,
                    data: 'emailAddress',
                },
                {
                    targets: 8,
                    data: 'isEmailConfirmed',
                    render: function (isEmailConfirmed) {
                        var $span = $('<span/>').addClass('label');
                        if (isEmailConfirmed) {
                            $span.addClass('badge badge-success').text(app.localize('Yes'));
                        } else {
                            $span.addClass('badge badge-dark').text(app.localize('No'));
                        }
                        return $span[0].outerHTML;
                    },
                },
                {
                    targets: 9,
                    data: 'isActive',
                    render: function (isActive) {
                        var $span = $('<span/>').addClass('label');
                        if (isActive) {
                            $span.addClass('badge badge-success').text(app.localize('Yes'));
                        } else {
                            $span.addClass('badge badge-dark').text(app.localize('No'));
                        }
                        return $span[0].outerHTML;
                    },
                },
                {
                    targets: 10,
                    data: 'creationTime',
                    render: function (creationTime) {
                        return moment(creationTime).format('L');
                    },
                },
            ],
        });

        function getUsers() {
            dataTable.ajax.reload();
        }

        function deleteUser(user) {
            if (user.userName === app.consts.userManagement.defaultAdminUserName) {
                abp.message.warn(app.localize('{0}UserCannotBeDeleted', app.consts.userManagement.defaultAdminUserName));
                return;
            }

            abp.message.confirm(
                app.localize('UserDeleteWarningMessage', user.userName),
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _userService
                            .deleteUser({
                                id: user.id,
                            })
                            .done(function () {
                                getUsers(true);
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                    }
                }
            );
        }



        $('#CreateNewUserButton').click(function () {
            _createOrEditModal.open();
        });

        $('#GetUsersButton').click(function (e) {
            e.preventDefault();
            getUsers();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getUsers();
            }
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


        $('#UsersTableFilter').on('keydown', function (e) {
            if (e.keyCode !== 13) {
                return;
            }

            e.preventDefault();
            getUsers();
        });

        abp.event.on('app.createOrEditUserModalSaved', function () {
            getUsers();
        });

        $('#UsersTableFilter').focus();

    });
})();
