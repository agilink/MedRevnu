(function () {
    $(function () {
        var _$casesTable = $('#CasesTable');
        var _casesService = abp.services.app.cases;

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Revenue.Cases.Create'),
            edit: abp.auth.hasPermission('Pages.Revenue.Cases.Edit'),
            delete: abp.auth.hasPermission('Pages.Revenue.Cases.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Revenue/Cases/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Revenue/Views/Cases/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditCaseModal'
        });

        var dataTable = _$casesTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _casesService.getAllFiltered,
                inputFilter: function () {
                    return {
                        caseNumberFilter: $('#CaseNumberFilter').val(),
                        clientNameFilter: $('#ClientNameFilter').val(),
                        statusFilter: $('#StatusFilter').val()
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
                    data: 'clientName',
                    name: 'clientName'
                },
                {
                    targets: 2,
                    data: 'caseDate',
                    name: 'caseDate',
                    render: function (caseDate) {
                        if (caseDate) {
                            return moment(caseDate).format('L');
                        }
                        return '';
                    }
                },
                {
                    targets: 3,
                    data: 'totalAmount',
                    name: 'totalAmount',
                    render: function (totalAmount) {
                        if (totalAmount) {
                            return '$' + totalAmount.toFixed(2);
                        }
                        return '$0.00';
                    }
                },
                {
                    targets: 4,
                    data: 'status',
                    name: 'status',
                    render: function (status) {
                        var badge = 'secondary';
                        if (status === 'Open') badge = 'success';
                        else if (status === 'In Progress') badge = 'warning';
                        else if (status === 'Closed') badge = 'danger';
                        return '<span class="badge bg-' + badge + '">' + status + '</span>';
                    }
                },
                {
                    targets: 5,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        cssClass: 'btn btn-brand dropdown-toggle',
                        text: '<i class="fa fa-cog"></i> ' + app.localize('Actions') + ' <span class="caret"></span>',
                        items: [
                            {
                                text: app.localize('View'),
                                action: function (data) {
                                    window.location.href = abp.appPath + 'Revenue/Cases/Details?id=' + data.record.id;
                                }
                            },
                            {
                                text: app.localize('Edit'),
                                //visible: function () {
                                //    return _permissions.edit;
                                //},
                                action: function (data) {
                                    _createOrEditModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: app.localize('Delete'),
                                //visible: function () {
                                //    return _permissions.delete;
                                //},
                                action: function (data) {
                                    deleteCase(data.record);
                                }
                            }
                        ]
                    }
                }
            ]
        });

        function getCases() {
            dataTable.ajax.reload();
        }

        function deleteCase(caseItem) {
            abp.message.confirm(
                app.localize('CaseDeleteWarningMessage', caseItem.caseNumber),
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _casesService
                            .delete({
                                id: caseItem.id
                            })
                            .done(function () {
                                getCases();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                    }
                }
            );
        }

        $('#FilterButton').click(function (e) {
            e.preventDefault();
            getCases();
        });

        $('#ClearFilterButton').click(function (e) {
            e.preventDefault();
            $('#CaseNumberFilter').val('');
            $('#ClientNameFilter').val('');
            $('#StatusFilter').val('');
            getCases();
        });

        $('#CreateNewCaseButton').click(function () {
            _createOrEditModal.open();
        });

        $('#RefreshCasesButton').click(function (e) {
            e.preventDefault();
            getCases();
        });

        abp.event.on('app.createOrEditCaseModalSaved', function () {
            getCases();
        });
    });
})();
