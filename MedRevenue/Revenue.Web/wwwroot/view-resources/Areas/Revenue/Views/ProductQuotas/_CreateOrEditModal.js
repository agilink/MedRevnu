(function ($) {
    app.modals.CreateOrEditProductQuotaModal = function () {
        var _quotasService = abp.services.app.productQuotas;
        var _modalManager;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$form = _modalManager.getModal().find('form[name=ProductQuotaCreateOrEditForm]');
            _$form.validate();

            // Bind save button click event
            // No .save-button binding here on purpose. ModalManager already binds that
            // button to this script's save(), so binding it again made one click submit
            // twice - the first call saved and the second came back "already in use".
        };

        function save() {
            if (!_$form.valid()) {
                return;
            }

            var productQuota = _$form.serializeFormToObject();

            _modalManager.setBusy(true);
            _quotasService
                .createOrEdit(productQuota)
                .done(function () {
                    abp.notify.info(app.localize('SavedSuccessfully'));
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditProductQuotaModalSaved');
                })
                .always(function () {
                    _modalManager.setBusy(false);
                });
        }

        this.save = save;
    };
})(jQuery);
