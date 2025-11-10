(function ($) {
    app.modals.CreateOrEditProductModal = function () {

        var _modalManager;
        var _productsService = abp.services.app.products;
        var _$productInformationForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;

            var modal = _modalManager.getModal();
            
            _$productInformationForm = modal.find('form[name=ProductCreateOrEditForm]');
            _$productInformationForm.validate();
        };

        this.save = function () {
            if (!_$productInformationForm.valid()) {
                return;
            }

            var productData = _$productInformationForm.serializeFormToObject();

            _modalManager.setBusy(true);

            if (productData.Id) {
                _productsService.update(productData).done(function () {
                    abp.notify.info(app.localize('SavedSuccessfully'));
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditProductModalSaved');
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            } else {
                _productsService.create(productData).done(function () {
                    abp.notify.info(app.localize('SavedSuccessfully'));
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditProductModalSaved');
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            }
        };
    };
})(jQuery);