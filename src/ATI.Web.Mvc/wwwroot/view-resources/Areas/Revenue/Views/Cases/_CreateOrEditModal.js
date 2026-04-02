(function () {
    app.modals.CreateOrEditCaseModal = function () {
        var _modalManager;
        var _casesService = abp.services.app.cases;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$form = _modalManager.getModal().find('form[name=CaseCreateOrEditForm]');
            _$form.validate();

            // Load procedure types
            loadProcedureTypes();

            // Load facilities
            loadFacilities();
        };

        function loadProcedureTypes() {
            $.ajax({
                url: '/Revenue/Cases/GetProcedureTypes',
                type: 'POST',
                contentType: 'application/json',
                success: function (result) {
                    var $procedureTypeSelect = _$form.find('#ProcedureTypeId');
                    $procedureTypeSelect.empty();
                    $procedureTypeSelect.append('<option value="">-- Select Procedure Type --</option>');

                    if (result && result.length > 0) {
                        result.forEach(function (pt) {
                            $procedureTypeSelect.append(
                                $('<option></option>')
                                    .attr('value', pt.id)
                                    .text(pt.name)
                            );
                        });
                    }

                    // Set selected value if editing
                    var selectedProcedureTypeId = _$form.find('input[name=ProcedureTypeId]').val();
                    if (selectedProcedureTypeId) {
                        $procedureTypeSelect.val(selectedProcedureTypeId);
                    }
                },
                error: function () {
                    abp.message.error('Failed to load procedure types');
                }
            });
        }

        function loadFacilities() {
            // TODO: This will need to call the Admin module's facilities service
            // For now, we'll add a placeholder
            $.ajax({
                url: '/Admin/Facilities/GetAll',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ maxResultCount: 1000, skipCount: 0 }),
                success: function (result) {
                    var $facilitySelect = _$form.find('#FacilityId');
                    $facilitySelect.empty();
                    $facilitySelect.append('<option value="">-- Select Facility --</option>');

                    if (result && result.items && result.items.length > 0) {
                        result.items.forEach(function (facility) {
                            $facilitySelect.append(
                                $('<option></option>')
                                    .attr('value', facility.id)
                                    .text(facility.name)
                            );
                        });
                    }

                    // Set selected value if editing
                    var selectedFacilityId = _$form.find('input[name=FacilityId]').val();
                    if (selectedFacilityId) {
                        $facilitySelect.val(selectedFacilityId);
                    }
                },
                error: function (xhr) {
                    // If facilities endpoint doesn't exist yet, just log it
                    console.log('Facilities endpoint not available yet');
                }
            });
        }

        this.save = function () {
            if (!_$form.valid()) {
                return;
            }

            var caseId = _$form.find('input[name=Id]').val();

            var procedureTypeId = _$form.find('select[name=ProcedureTypeId]').val();
            var facilityId = _$form.find('select[name=FacilityId]').val();
            var procedureDate = _$form.find('input[name=ProcedureDate]').val();

            var caseData = {
                caseNumber: _$form.find('input[name=CaseNumber]').val(),
                clientName: _$form.find('input[name=ClientName]').val(),
                description: _$form.find('textarea[name=Description]').val(),
                caseDate: _$form.find('input[name=CaseDate]').val(),
                procedureTypeId: procedureTypeId ? parseInt(procedureTypeId) : null,
                procedureDate: procedureDate || null,
                facilityId: facilityId ? parseInt(facilityId) : null,
                surgeonName: _$form.find('input[name=SurgeonName]').val(),
                totalAmount: parseFloat(_$form.find('input[name=TotalAmount]').val()) || 0,
                status: _$form.find('select[name=Status]').val(),
                notes: _$form.find('textarea[name=Notes]').val(),
                caseProducts: [] // Initialize empty products array
            };

            _modalManager.setBusy(true);

            if (caseId && caseId !== '0') {
                // Update existing case
                caseData.id = parseInt(caseId);
                _casesService
                    .update(caseData)
                    .done(function () {
                        abp.notify.info(app.localize('SavedSuccessfully'));
                        _modalManager.close();
                        abp.event.trigger('app.createOrEditCaseModalSaved');
                    })
                    .fail(function (error) {
                        console.error('Update failed:', error);
                    })
                    .always(function () {
                        _modalManager.setBusy(false);
                    });
            } else {
                // Create new case
                _casesService
                    .create(caseData)
                    .done(function () {
                        abp.notify.info(app.localize('SavedSuccessfully'));
                        _modalManager.close();
                        abp.event.trigger('app.createOrEditCaseModalSaved');
                    })
                    .fail(function (error) {
                        console.error('Create failed:', error);
                    })
                    .always(function () {
                        _modalManager.setBusy(false);
                    });
            }
        };
    };
})();
