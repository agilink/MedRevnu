(function ($) {
    app.modals.CreateOrEditPhysicianModal = function () {
        var _physiciansService = abp.services.app.physicians;
        var _modalManager;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$form = _modalManager.getModal().find('form[name=PhysicianCreateOrEditForm]');
            _$form.validate();

            // No .save-button binding here on purpose. ModalManager already binds that
            // button to this script's save(), so binding it again made one click submit
            // twice - the first call saved and the second came back "already in use".
        };

        function save() {
            if (!_$form.valid()) {
                return;
            }

            var physician = _$form.serializeFormToObject();

            // Empty selects post as '' but the DTO takes nullable ints, so blank them out
            // rather than sending a value the model binder rejects.
            physician.hospitalId = physician.HospitalId ? parseInt(physician.HospitalId, 10) : null;
            physician.employeeStatus = physician.EmployeeStatus ? parseInt(physician.EmployeeStatus, 10) : null;
            delete physician.HospitalId;
            delete physician.EmployeeStatus;

            _modalManager.setBusy(true);
            _physiciansService
                .createOrEdit(physician)
                .done(function () {
                    abp.notify.info(app.localize('SavedSuccessfully'));
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditPhysicianModalSaved');
                })
                .always(function () {
                    _modalManager.setBusy(false);
                });
        }

        this.save = save;
    };
})(jQuery);
