(function ($) {
    app.modals.CreateOrEditHospitalProductPriceModal = function () {
        var _pricesService = abp.services.app.hospitalProductPrices;
        var _modalManager;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$form = _modalManager.getModal().find('form[name=HospitalProductPriceCreateOrEditForm]');
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

            var hospitalProductPrice = _$form.serializeFormToObject();

            // Ensure boolean is handled correctly
            hospitalProductPrice.isActive = _$form.find('#IsActive').is(':checked');

            _modalManager.setBusy(true);
            _pricesService
                .createOrEdit(hospitalProductPrice)
                .done(function () {
                    abp.notify.info(app.localize('SavedSuccessfully'));
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditHospitalProductPriceModalSaved');
                })
                .always(function () {
                    _modalManager.setBusy(false);
                });
        }

        this.save = save;
    };
})(jQuery);
