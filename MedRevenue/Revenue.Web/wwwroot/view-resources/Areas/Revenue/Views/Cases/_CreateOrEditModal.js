(function () {
    app.modals.CreateOrEditCaseModal = function () {
        var _modalManager;
        var _casesService = abp.services.app.cases;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$form = _modalManager.getModal().find('form[name=CaseCreateOrEditForm]');
            _$form.validate();
        };

        this.save = function () {
            if (!_$form.valid()) {
                return;
            }

            var caseId = _$form.find('input[name=Id]').val();

            var caseData = {
                caseNumber: _$form.find('input[name=CaseNumber]').val(),
                clientName: _$form.find('input[name=ClientName]').val(),
                description: _$form.find('textarea[name=Description]').val(),
                caseDate: _$form.find('input[name=CaseDate]').val(),
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
