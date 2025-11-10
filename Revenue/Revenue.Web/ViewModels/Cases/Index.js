(function ($) {
    $(function () {
        var _$casesTable = $('#CasesTable');
        var _casesService = abp.services.app.cases;

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Cases.Create'),
            edit: abp.auth.hasPermission('Pages.Cases.Edit'),
            'delete': abp.auth.hasPermission('Pages.Cases.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Revenue/Cases/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Revenue/Views/Cases/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditCaseModal'
        });

        var _viewCaseModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Revenue/Cases/ViewCaseModal',
            modalClass: 'ViewCaseModal'
        });

        var dataTable = _$casesTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _casesService.getAllFiltered,
                inputFilter: function () {
                    return {
                        filter: $('#CasesTableFilter').val()
                    };
                }
            },
            buttons: [
                {
                    name: 'refresh',
                    text: '<i class="fas fa-redo-alt"></i>',
                    action: function () {
                        dataTable.draw(false);
                    }
                }
            ],
            responsive: {
                details: {
                    type: 'column'
                }
            },
            columnDefs: [
                {
                    targets: 0,
                    className: 'control',
                    defaultContent: '',
                },
                {
                    targets: 1,
                    data: 'caseNumber',
                    render: function (caseNumber) {
                        return '<span>' + caseNumber + '</span>';
                    }
                },
                {
                    targets: 2,
                    data: 'clientName'
                },
                {
                    targets: 3,
                    data: 'caseDate',
                    render: function (caseDate) {
                        return moment(caseDate).format('L');
                    }
                },
                {
                    targets: 4,
                    data: 'totalAmount',
                    render: function (totalAmount) {
                        return ' + totalAmount.toFixed(2);
                    }
                },
                {
                    targets: 5,
                    data: 'status',
                    render: function (status) {
                        var badgeClass = 'secondary';
                        if (status === 'Open') badgeClass = 'success';
                        else if (status === 'In Progress') badgeClass = 'warning';
                        return '<span class="badge bg-' + badgeClass + '">' + status + '</span>';
                    }
                },
                {
                    targets: 6,
                    data: null,
                    orderable: false,
                    render: function (data) {
                        var actions = [
                            '<button class="btn btn-sm btn-info view-case" data-case-id="' + data.id + '" data-bs-toggle="tooltip" title="' + app.localize('View') + '">',
                            '   <i class="fa fa-eye"></i>',
                            '</button>'
                        ];

                        if (_permissions.edit) {
                            actions.push(
                                '<button class="btn btn-sm btn-primary edit-case ms-1" data-case-id="' + data.id + '" data-bs-toggle="tooltip" title="' + app.localize('Edit') + '">',
                                '   <i class="fa fa-edit"></i>',
                                '</button>'
                            );
                        }

                        if (_permissions.delete) {
                            actions.push(
                                '<button class="btn btn-sm btn-danger delete-case ms-1" data-case-id="' + data.id + '" data-case-name="' + data.caseNumber + '" data-bs-toggle="tooltip" title="' + app.localize('Delete') + '">',
                                '   <i class="fa fa-trash"></i>',
                                '</button>'
                            );
                        }

                        return actions.join('');
                    }
                }
            ]
        });

        function getCases() {
            dataTable.ajax.reload();
        }

        function deleteCase(caseId, caseName) {
            abp.message.confirm(
                abp.utils.formatString(app.localize('AreYouSureWantToDelete'), caseName),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _casesService.delete({
                            id: caseId
                        }).done(function () {
                            getCases();
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                    }
                }
            );
        }

        $('#CreateNewCaseButton').click(function () {
            _createOrEditModal.open();
        });

        $('#GetCasesButton').click(function (e) {
            e.preventDefault();
            getCases();
        });

        $(document).on('click', '.edit-case', function () {
            var caseId = $(this).data('case-id');
            _createOrEditModal.open({ id: caseId });
        });

        $(document).on('click', '.delete-case', function () {
            var caseId = $(this).data('case-id');
            var caseName = $(this).data('case-name');
            deleteCase(caseId, caseName);
        });

        $(document).on('click', '.view-case', function () {
            var caseId = $(this).data('case-id');
            _viewCaseModal.open({ id: caseId });
        });

        abp.event.on('app.createOrEditCaseModalSaved', function () {
            getCases();
        });
    });
})(jQuery);