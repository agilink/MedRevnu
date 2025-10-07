(function () {
    $(function () {
        var _doctorsService = abp.services.app.doctors;
        var _$doctorInformationForm = $('form[name=DoctorInformationsForm]');
        _$doctorInformationForm.validate();

        $('.date-picker').daterangepicker({
            singleDatePicker: true,
            locale: abp.localization.currentLanguage.name,
            format: 'L'
        });

        function save(successCallback) {
            if (!_$doctorInformationForm.valid()) {
                return;
            }

            var doctor = _$doctorInformationForm.serializeFormToObject();

			 abp.ui.setBusy();
			 _doctorsService.createOrEdit(
				doctor
			 ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               abp.event.trigger('app.createOrEditDoctorModalSaved');
               
               if(typeof(successCallback)==='function'){
                    successCallback();
               }
			 }).always(function () {
			    abp.ui.clearBusy();
			});
        };
        
        function clearForm(){
            _$doctorInformationForm[0].reset();
        }
        
        $('#saveBtn').click(function(){
            save(function(){
                window.location="/Pharmacy/Doctors";
            });
        });
        
        $('#saveAndNewBtn').click(function(){
            save(function(){
                if (!$('input[name=id]').val()) {//if it is create page
                   clearForm();
                }
            });
        });
        
        
    });
})();