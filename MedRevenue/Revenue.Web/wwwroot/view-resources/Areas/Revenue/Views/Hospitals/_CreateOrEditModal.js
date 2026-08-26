(function ($) {
    app.modals.CreateOrEditHospitalModal = function () {
        var _hospitalsService = abp.services.app.hospitals;
        var _modalManager;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$form = _modalManager.getModal().find('form[name=HospitalCreateOrEditForm]');
            _$form.validate();

            _modalManager.getModal().find('.save-button').click(function (e) {
                e.preventDefault();
                save();
            });
        };

        function save() {
            if (!_$form.valid()) {
                return;
            }

            var hospital = _$form.serializeFormToObject();

            // An unchosen company posts as '' which will not bind to int?.
            hospital.companyId = hospital.CompanyId ? parseInt(hospital.CompanyId, 10) : null;
            delete hospital.CompanyId;

            _modalManager.setBusy(true);
            _hospitalsService
                .createOrEdit(hospital)
                .done(function () {
                    abp.notify.info(app.localize('SavedSuccessfully'));
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditHospitalModalSaved');
                })
                .always(function () {
                    _modalManager.setBusy(false);
                });
        }

        this.save = save;
    };
})(jQuery);
