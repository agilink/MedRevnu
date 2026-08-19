(function () {
    app.modals.CreateOrEditCaseModal = function () {
        var _modalManager;
        var _casesService = abp.services.app.cases;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$form = _modalManager.getModal().find('form[name=CaseCreateOrEditForm]');
            _$form.validate();

            var currentStatus = _$form.find('#Status').data('current-status');
            if (currentStatus) {
                _$form.find('#Status').val(currentStatus);
            }

            loadProcedureTypes();
            loadFacilities();
            loadPersonnel();
        };

        function loadProcedureTypes() {
            abp.ajax({
                url: abp.appPath + 'Revenue/Cases/GetProcedureTypes',
                type: 'GET',
                abpHandleError: false
            }).done(function (result) {
                var $select = _$form.find('#ProcedureTypeId');
                $select.empty().append('<option value="">-- Select Procedure Type --</option>');
                if (result && result.length > 0) {
                    $.each(result, function (i, pt) {
                        $select.append($('<option>').val(pt.id).text(pt.name));
                    });
                }
                var currentId = $select.data('current-id');
                if (currentId) {
                    $select.val(currentId);
                }
            }).fail(function () {
                abp.message.error('Failed to load procedure types');
            });
        }

        function loadFacilities() {
            abp.ajax({
                url: abp.appPath + 'Revenue/Cases/GetFacilities',
                type: 'GET',
                abpHandleError: false
            }).done(function (result) {
                var $select = _$form.find('#FacilityId');
                $select.empty().append('<option value="">-- Select Hospital --</option>');
                if (result && result.length > 0) {
                    $.each(result, function (i, f) {
                        $select.append($('<option>').val(f.id).text(f.name));
                    });
                }
                var currentId = $select.data('current-id');
                if (currentId) {
                    $select.val(currentId);
                }
            }).fail(function () {
                abp.message.error('Failed to load hospitals');
            });
        }

        function loadPersonnel() {
            abp.ajax({
                url: abp.appPath + 'Revenue/Cases/GetPersonnel',
                type: 'GET',
                abpHandleError: false
            }).done(function (result) {
                var $select = _$form.find('#SurgeonName');
                $select.empty().append('<option value="">-- Select Surgeon --</option>');
                if (result && result.length > 0) {
                    $.each(result, function (i, p) {
                        $select.append($('<option>').val(p.name).text(p.name));
                    });
                }
                var currentName = $select.data('current-name');
                if (currentName) {
                    $select.val(currentName);
                }
            }).fail(function () {
                abp.message.error('Failed to load personnel');
            });
        }

        this.save = function () {
            if (!_$form.valid()) {
                return;
            }

            var caseId = _$form.find('input[name=Id]').val();
            var procedureTypeId = _$form.find('#ProcedureTypeId').val();
            var facilityId = _$form.find('#FacilityId').val();
            var procedureDate = _$form.find('input[name=ProcedureDate]').val();

            var caseData = {
                caseNumber: _$form.find('input[name=CaseNumber]').val(),
                clientName: _$form.find('input[name=ClientName]').val(),
                description: _$form.find('textarea[name=Description]').val() || null,
                caseDate: _$form.find('input[name=CaseDate]').val(),
                procedureTypeId: procedureTypeId ? parseInt(procedureTypeId) : null,
                procedureDate: procedureDate || null,
                facilityId: facilityId ? parseInt(facilityId) : null,
                surgeonName: _$form.find('#SurgeonName').val() || null,
                totalAmount: parseFloat(_$form.find('input[name=TotalAmount]').val()) || 0,
                status: _$form.find('#Status').val(),
                notes: _$form.find('textarea[name=Notes]').val() || null
            };

            _modalManager.setBusy(true);

            var isCreate = !caseId || caseId === '0';
            if (!isCreate) {
                caseData.id = parseInt(caseId);
            }

            abp.ajax({
                url: abp.appPath + 'api/services/app/Cases/' + (isCreate ? 'Create' : 'Update'),
                type: isCreate ? 'POST' : 'PUT',
                data: JSON.stringify(caseData),
                contentType: 'application/json'
            })
            .done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
                abp.event.trigger('app.createOrEditCaseModalSaved');
            })
            .fail(function (err) {
                // ABP already shows the error dialog via abp.ajax.showError — no duplicate needed
            })
            .always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})();
