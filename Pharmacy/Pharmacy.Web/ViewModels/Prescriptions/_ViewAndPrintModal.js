(function ($) {
    app.modals.ViewPrescriberModal = function () {
        var _prescriptionsService = abp.services.app.prescriptions;
        var completeButton = $(".complete-button");

        complete = function () {

            var prescriptionId = $("#hdnPrescriptionId").val();

            if (prescriptionId == null || prescriptionId == "" || prescriptionId == 0)
                abp.notify.warn("Please save before complete!");

            _modalManager.setBusy(true);
            _prescriptionsService
                .complete({ id: prescriptionId })
                .done(function (res) {
                    abp.notify.success("Completed Successfully!");
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditPrescriptionModalSaved');
                }).error(function () {
                    abp.notify.error("Some error occured while completing.");
                })
                .always(function () {
                    _modalManager.setBusy(false);
                });
        }


        this.init = function (modalManager) {
            _modalManager = modalManager;
            var modal = _modalManager.getModal();

            if (abp.auth.hasPermission('Pages.Prescriptions.Complete') && ($("#hdnStatusId").val() * 1 == 2)) {
                completeButton.show();
            }
            else {
                completeButton.hide();
            }

            completeButton.click(function () {

                abp.message.confirm('', app.localize('AreYouSure'), function (isConfirmed) {
                    if (isConfirmed) {
                        complete();

                    }
                });
            });
        };
    };
})(jQuery);
