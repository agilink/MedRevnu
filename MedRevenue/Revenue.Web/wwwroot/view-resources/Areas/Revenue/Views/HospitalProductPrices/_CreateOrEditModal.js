(function ($) {
    app.modals.CreateOrEditHospitalProductPriceModal = function () {
        var _pricesService = abp.services.app.hospitalProductPrices;
        var _modalManager;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$form = _modalManager.getModal().find('form[name=HospitalProductPriceCreateOrEditForm]');
            _$form.validate();

            // No .save-button binding here on purpose. ModalManager already binds that
            // button to this script's save(), so binding it again made one click submit
            // twice - the first call saved and the second came back "already in use".
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
