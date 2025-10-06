(function ($) {
    app.modals.CreateOrEditPatientModal = function () {
        var _patientsService = abp.services.app.patients;

        var _modalManager;
        var _$patientInformationForm = null;
        var _$deletButton = null;
        var _$patientEle = null;
        var _permissions = {
            create: abp.auth.hasPermission('Pages.Patients.Create'),
            edit: abp.auth.hasPermission('Pages.Patients.Edit'),
            delete: abp.auth.hasPermission('Pages.Patients.Delete'),
        };

        this.init = function (modalManager) {
            _modalManager = modalManager;


            var modal = _modalManager.getModal();


            modal.find('.date-picker').daterangepicker(
                {
                    locale: {
                        format: 'MM-DD-YYYY' // Change the format here
                    },
                    singleDatePicker: true,
                    autoUpdateInput: false
                }, function (start, end, label) {
                    // Manually update the input field with the selected date
                    $('.date-picker').val(start.format('MM-DD-YYYY'));
                }
            );
            debugger
            modal.find('.date-picker').inputmask('99-99-9999')
            modal.find('#DoctorId').select2({
                theme: 'bootstrap5',
                placeholder: "Select Prescriber",
                dropdownParent: '.modal',
                selectionCssClass: 'form-select',
                language: abp.localization.currentCulture.name,
                width: '100%',
                dropdownCssClass: "long-select2",
            });
            modal.find('#StateId').select2({
                theme: 'bootstrap5',
                placeholder: "Select State",
                dropdownParent: '.modal',
                selectionCssClass: 'form-select',
                language: abp.localization.currentCulture.name,
                width: '100%',
                dropdownCssClass: "long-select2",
            });
            modal.find('#AllergyId').select2({
                theme: 'bootstrap5',
                dropdownParent: '.modal',
                selectionCssClass: 'form-select',
                language: abp.localization.currentCulture.name,
                width: '100%',
                multiple: true,
                dropdownCssClass: "long-select2",
            });
            modal.find('#Patient_EmergencyContactPhone').inputmask('(999) 999-9999');
            modal.find('#Patient_PhoneNumber').inputmask('(999) 999-9999');
            modal.find('#FaxNo').inputmask('(999) 999-9999');

            _$patientEle = modal.find("#hdnPatientId");
            _$deletButton = modal.find('.delete-button');

            if (_$patientEle.length > 0) {
                if ((typeof (hideDelete) != "undefined" && typeof (hideDelete) != undefined && hideDelete == 1) || !_permissions.delete) {
                    _$deletButton.hide();
                } else {
                    _$deletButton.show();
                }
            }
            else {
                _$deletButton.hide();
            }

            //Delete Doctor
            _$deletButton.click(function () {
                var patient = _$patientInformationForm.serializeFormToObject();
                abp.message.confirm('', app.localize('AreYouSure'), function (isConfirmed) {
                    if (isConfirmed) {
                        _modalManager.setBusy(true);
                        _patientsService
                            .delete({
                                id: patient.id,
                            })
                            .done(function () {
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                                _modalManager.close();
                                abp.event.trigger('app.patientDataDeleted');
                            })
                            .always(function () {
                                _modalManager.setBusy(false);
                            });

                    }
                });
            });

            _$patientInformationForm = _modalManager.getModal().find('form[name=PatientInformationsForm]');
            _$patientInformationForm.validate();
        };

        this.save = function () {
            if (!_$patientInformationForm.valid()) {
                return;
            }

            var patient = _$patientInformationForm.serializeFormToObject();
            patient.AllergyIds = _modalManager.getModal().find('#AllergyId').select2('val').toString();
            _modalManager.setBusy(true);
            _patientsService
                .createOrEdit(patient)
                .done(function () {
                    abp.notify.success(app.localize('SavedSuccessfully'));
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditPatientModalSaved');
                })
                .always(function () {
                    _modalManager.setBusy(false);
                });
        };
    };
})(jQuery);
