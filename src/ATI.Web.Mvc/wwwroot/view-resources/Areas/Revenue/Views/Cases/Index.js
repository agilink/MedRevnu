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

        // Load procedure types for filter
        function loadProcedureTypes() {
            $.ajax({
                url: '/Revenue/Cases/GetProcedureTypes',
                type: 'POST',
                contentType: 'application/json',
                success: function (result) {
                    var $procedureTypeFilter = $('#ProcedureTypeFilter');
                    $procedureTypeFilter.empty();
                    $procedureTypeFilter.append('<option value="">All Procedure Types</option>');

                    if (result && result.length > 0) {
                        result.forEach(function (pt) {
                            $procedureTypeFilter.append(
                                $('<option></option>')
                                    .attr('value', pt.id)
                                    .text(pt.name)
                            );
                        });
                    }
                },
                error: function () {
                    console.log('Failed to load procedure types');
                }
            });
        }

        // Load facilities for filter
        function loadFacilities() {
            $.ajax({
                url: '/Admin/Facilities/GetAll',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ maxResultCount: 1000, skipCount: 0 }),
                success: function (result) {
                    var $facilityFilter = $('#FacilityFilter');
                    $facilityFilter.empty();
                    $facilityFilter.append('<option value="">All Facilities</option>');

                    if (result && result.items && result.items.length > 0) {
                        result.items.forEach(function (facility) {
                            $facilityFilter.append(
                                $('<option></option>')
                                    .attr('value', facility.id)
                                    .text(facility.name)
                            );
                        });
                    }
                },
                error: function () {
                    console.log('Facilities endpoint not available yet');
                }
            });
        }

        // Initialize filters
        loadProcedureTypes();
        loadFacilities();

        var dataTable = _$casesTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _casesService.getAllFiltered,
                inputFilter: function () {
                    var filter = $('#CasesTableFilter').val();
                    var procedureTypeId = $('#ProcedureTypeFilter').val();
                    var facilityId = $('#FacilityFilter').val();
                    var procedureDate = $('#ProcedureDateFilter').val();

                    return {
                        filter: filter,
                        caseNumberFilter: $('#CaseNumberFilter').val(),
                        clientNameFilter: $('#ClientNameFilter').val(),
                        statusFilter: $('#StatusFilter').val(),
                        procedureTypeIdFilter: procedureTypeId ? parseInt(procedureTypeId) : null,
                        facilityIdFilter: facilityId ? parseInt(facilityId) : null,
                        surgeonNameFilter: $('#SurgeonNameFilter').val(),
                        minProcedureDateFilter: procedureDate || null,
                        maxProcedureDateFilter: procedureDate || null
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
                    data: 'procedureTypeName',
                    name: 'procedureTypeName',
                    render: function (procedureTypeName) {
                        return procedureTypeName || '-';
                    }
                },
                {
                    targets: 3,
                    data: 'surgeonName',
                    name: 'surgeonName',
                    render: function (surgeonName) {
                        return surgeonName || '-';
                    }
                },
                {
                    targets: 4,
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
                    targets: 5,
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
                    targets: 6,
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
                    targets: 7,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        cssClass: 'btn btn-sm btn-light btn-active-light-primary',
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

        // Main search button
        $('#GetCasesButton').click(function (e) {
            e.preventDefault();
            getCases();
        });

        // Search on Enter key in main filter
        $('#CasesTableFilter').on('keydown', function (e) {
            if (e.keyCode === 13) {
                e.preventDefault();
                getCases();
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
        $('#CaseNumberFilter, #ClientNameFilter, #StatusFilter, #ProcedureTypeFilter, #FacilityFilter, #SurgeonNameFilter, #ProcedureDateFilter').on('change', function () {
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
