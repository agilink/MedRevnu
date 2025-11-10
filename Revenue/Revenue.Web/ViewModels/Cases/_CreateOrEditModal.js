(function ($) {
    app.modals.CreateOrEditCaseModal = function () {

        var _modalManager;
        var _casesService = abp.services.app.cases;
        var _$caseInformationForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;

            var modal = _modalManager.getModal();
            
            _$caseInformationForm = modal.find('form[name=CaseCreateOrEditForm]');
            _$caseInformationForm.validate();
        };

        this.save = function () {
            if (!_$caseInformationForm.valid()) {
                return;
            }

            var caseData = _$caseInformationForm.serializeFormToObject();

            _modalManager.setBusy(true);

            if (caseData.Id) {
                _casesService.update(caseData).done(function () {
                    abp.notify.info(app.localize('SavedSuccessfully'));
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditCaseModalSaved');
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            } else {
                _casesService.create(caseData).done(function () {
                    abp.notify.info(app.localize('SavedSuccessfully'));
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditCaseModalSaved');
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            }
        };
    };
})(jQuery);