(function ($) {
    debugger
    app.modals.CreateOrEditUserModal = function () {
        var _userService = abp.services.app.userExtended; // Replace with your actual service name

        var _modalManager;
        var _$userInformationForm = null;
        var _$deleteButton = null;
        var _$comapnyListDiv = $('.company-list');
        var _$comapnyNewDiv = $('.company-new');
        var _$newCompanyBtn = $('#create-company');
        var _$selectCompanyBtn = $('#select-company');
        this.init = function (modalManager) {
            debugger
            _modalManager = modalManager;

            var modal = _modalManager.getModal();

            // Initialize Date Picker
            //modal.find('.date-picker').daterangepicker(
            //    {
            //        locale: {
            //            format: 'MM-DD-YYYY' // Change the format here
            //        },
            //        singleDatePicker: true
            //    }
            //);
            //modal.find('.date-picker').inputmask('99-99-9999');

            // Initialize Select2 for Role Dropdown
            modal.find('#User_Role').select2({
                theme: 'bootstrap5',
                dropdownParent: modal,
                selectionCssClass: 'form-select',
                language: abp.localization.currentCulture.name,
                width: '100%',
                dropdownCssClass: "long-select2",
                placeholder: 'Select Role'
            });
            modal.find('#companyddl').select2({
                theme: 'bootstrap5',
                placeholder: "Select Location",
                dropdownParent: '.modal',
                selectionCssClass: 'form-select',
                language: abp.localization.currentCulture.name,
                width: '100%',
                dropdownCssClass: "long-select2",
            });
            _$newCompanyBtn.click(function () {
                _$comapnyListDiv.addClass("hide");
                _$comapnyNewDiv.removeClass("hide");
                $('#companyddl').val(0);
            });

            _$selectCompanyBtn.click(function () {
                _$comapnyListDiv.removeClass("hide");
                _$comapnyNewDiv.addClass("hide");
                $('#Employee_Company').val('');
            });
            var defaultPharmacyId = [5];
            if ($("#companyddl").select2("val") != null && $("#companyddl").select2("val").includes(5))
                changeCompany(defaultPharmacyId);
            else {
                $('#roleId option').each(function () {
                    $(this).show();
                });
            }

            $("#companyddl").on("change", function () {
                debugger
                var thisCompany = $(this);
                changeCompany(thisCompany.select2("val"));
            });

            // Form Initialization
            _$userInformationForm = _modalManager.getModal().find('form[name=UserInformationForm]');
            _$userInformationForm.validate({
                rules: {
                    userName: {
                        required: true,
                        minlength: 3,
                        maxlength: 50
                    },
                    name: {
                        required: true,
                        minlength: 3,
                        maxlength: 50
                    },
                    surname: {
                        required: true,
                        minlength: 3,
                        maxlength: 50
                    },
                    dateOfBirth: {
                        required: true,
                        date: true
                    },
                    emailAddress: {
                        email: true
                    }
                }
            });
        };

        changeCompany = function (companyVal) {
            debugger
            if (companyVal != null && companyVal.includes(5)) {
                $('#roleId option').each(function () {
                    var role = $(this).text();

                    // Show or hide options based on the selected category
                    if (role === "" || role.includes("Pharmacy") || role.includes("Select")) {
                        $(this).show();  // Show option
                    } else {
                        $(this).hide();  // Hide option
                    }
                });
            }
            else {
                $('#roleId option').each(function () {

                    var role = $(this).text();

                    // Show or hide options based on the selected category
                    if (role === "" || role.includes("Prescriber") || role.includes("Select")) {
                        $(this).show();  // Show option
                    } else {
                        $(this).hide();  // Hide option
                    }
                });
            }

        }
        this.save = function () {
            debugger
            if (!_$userInformationForm.valid()) {
                return;
            }
            var user = _$userInformationForm.serializeFormToObject(); // Serializes the form into a JavaScript object

            // Get selected roles from Select2 dropdown
            user.RoleIds = _modalManager.getModal().find('#User_Role').select2('val');
            user.IsActive = ($('#userIsActive') == null || $('#userIsActive') == undefined) ? true : $('#userIsActive').prop("checked");
            user.facilityIds = $("#companyddl").select2("val");
            _modalManager.setBusy(true);
            _userService
                .createOrEdit(user)
                .done(function () {
                    abp.notify.success(app.localize('SavedSuccessfully'));
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditUserModalSaved');
                })
                .always(function () {
                    _modalManager.setBusy(false);
                });
        };
    };
})(jQuery);
