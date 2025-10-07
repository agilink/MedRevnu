(function ($) {
    app.modals.CreateOrEditPrescriptionModal = function () {
        var _prescriptionsService = abp.services.app.prescriptions;
        var _patientsService = abp.services.app.patients;
        var _doctorsService = abp.services.app.doctors;
        var _modalManager;
        var _$prescriptionInformationForm = null;
        var _$prescriptionEle = null;
        var _$isLoading = false;
        var _$isDefaultFacilityChanged = false;


        var progressListItems = $("#progressbar li");
        var progressBar = $(".progress-bar");
        var currentStep = 0;

        var mainObj = [];
        var dosageRouteId = 0;

        var nextStepButtons = $(".next-step");
        var prevStepButtons = $(".previous-step");
        var submitButton = $(".submit-button");
        var _$deleteButton = $(".delete-button");
        var _$saveSubmitLaterButton = $(".save-submit-later-button");

        var savePatientButton = $("#save-patient-button");

        validateDiv = function (selector) {
            // Clear previous validation states
            $(selector).find('.form-control').removeClass('is-valid is-invalid');
            $(selector).find('.invalid-feedback').hide();

            var isValid = true;

            // Check each input inside the div
            $(selector).find('.form-control').each(function () {
                var field = $(this);

                // Check if field is valid or not
                if (!field[0].validity.valid) {
                    field.addClass('is-invalid');
                    field.next('.invalid-feedback').show();
                    isValid = false;
                } else {
                    field.addClass('is-valid');
                }
            });

            return isValid;
        }
        validatePrescriber = function (selector) {
            var isValid = true;
            if ($('input[name="DeliveryTypeId"]') != undefined) {
                if ($('input[name="DeliveryTypeId"]:checked').length > 0)
                    isValid = true;
                else
                    isValid = false;

                if (!isValid)
                    abp.notify.warn("Please select a delivery type!")
            }

            if (isValid && $('input[name="BillingTo"]') != undefined) {
                if ($('input[name="BillingTo"]:checked').length > 0)
                    isValid = true;
                else
                    isValid = false;

                if (!isValid)
                    abp.notify.warn("Please select a billing type!")
            }

            return isValid;
        }

        updateProgress = function () {
            var percent =
                (currentStep / (progressListItems.length - 1)) * 100;
            progressBar.css("width", percent + "%");

            progressListItems.each(function (index) {
                if (index === currentStep) {
                    $(this).addClass("active");
                } else {
                    $(this).removeClass("active");
                }
            });
        }

        showStep = function (stepIndex) {
            var steps =
                $(".step-container fieldset");
            steps.each(function (index) {
                if (index === stepIndex) {
                    if (index == 0) {
                        $(".previous-step").css("display", "none");
                        $(".next-step").css("display", "block");
                        $(".save-button").css("display", "none");
                        $(".submit-button").css("display", "none");
                        $(".save-submit-later-button").css("display", "none");
                    }
                    else if (index == 1) {
                        $(".previous-step").css("display", "block");
                        $(".next-step").css("display", "block");
                        $(".save-button").css("display", "none");
                        $(".submit-button").css("display", "none");
                        $(".save-submit-later-button").css("display", "none");
                    }
                    else if (index == 2) {
                        $(".previous-step").css("display", "block");

                        $(".save-button").css("display", "block");
                        $(".next-step").css("display", "none");

                        $(".submit-button").css("display", "none");
                        $(".save-submit-later-button").css("display", "none");
                    }
                    else {
                        $(".previous-step").css("display", "block");
                        $(".next-step").css("display", "none");
                        $(".save-button").css("display", "none");
                        if (abp.auth.hasPermission('Pages.Prescriptions.Submit'))
                            $(".submit-button").css("display", "block");

                        $(".save-submit-later-button").css("display", "block");
                    }

                    if ($("#hdnPrescriptionId").val() * 1 == 0) {
                        // $(".cancel-button").css("display", "block");
                        $(".delete-button").css("display", "none");
                    }
                    else {
                        //$(".cancel-button").css("display", "none");
                        $(".delete-button").css("display", "block");
                    }

                    $(this).css("display", "block");
                } else {
                    $(this).css("display", "none");
                }
            });
        }

        nextStep = function () {
            if (currentStep == 0 && ($("#hdnPatientID").val() == 0 || $("#hdnPatientID").val() == null)) {
                abp.notify.warn("Please save the patient or select an existing patient to proceed")
                return false;
            }
            else if (currentStep == 1) {
                var img = $("#signature-img")[0];
                if (($("#hdnDoctorId").val() == 0 || $("#hdnDoctorId").val() == "" || $("#hdnDoctorId").val() == null)) {
                    abp.notify.warn("Please select a Prescriber first!");
                    return false;
                }
                else if (_$isLoading) {
                    abp.notify.warn("Please wait while prescriber data loads!");
                    return false;
                }
                else if ($("#hdnSignatureId").val() == "" || $("#hdnSignatureId").val() == null) {
                    abp.notify.error("Doctor has no signature!");
                    return false;
                }
            }
            if (currentStep < progressListItems.length - 1) {
                currentStep++;
                showStep(currentStep);
                updateProgress();
            }
        }
        loadSubmit = function () {
            var prescriptionId = $("#hdnPrescriptionId").val();
            $.ajax({
                url: '/Pharmacy/Prescriptions/ViewPrescription/' + prescriptionId,  // Path to the action in the controller
                type: 'GET',  // You can use 'POST' if required
                success: function (data) {
                    $("#submit-view").empty();
                    $("#submit-view").append(data);
                },
                error: function (xhr, status, error) {
                    // Handle error
                    console.log("Error calling child action:", error);
                }
            });
        }
        submit = function () {

            var prescriptionId = $("#hdnPrescriptionId").val();

            if (prescriptionId == null || prescriptionId == "" || prescriptionId == 0)
                abp.notify.warn("Please save before submit!");

            _modalManager.setBusy(true);
            _prescriptionsService
                .submit({ id: prescriptionId })
                .done(function (res) {
                    if (res > 0) {
                        abp.notify.success("Submitted Successfully!");
                    }
                    else {
                        abp.notify.error("Some error occured while submitting.");
                    }
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditPrescriptionModalSaved');
                })
                .always(function () {
                    _modalManager.setBusy(false);
                });
        }

        prevStep = function () {
            if (currentStep > 0) {
                currentStep--;
                showStep(currentStep);
                updateProgress();
            }
        }
        togglePatientDetails = function () {
            if ($(".patient-details").hasClass('hide')) {
                $(".patient-details").removeClass('hide');
                $(".icon").removeClass('fa-plus');
                $(".icon").addClass('fa-minus');
                $("#select2Patient").val("");
                reloadPatient(0);
            }
            else {
                $(".patient-details").addClass('hide');
                $(".icon").addClass('fa-plus');
                $(".icon").removeClass('fa-minus');
            }
        }
        cleanPatient = function () {
            $("#hdnPatientID").val("");
            $("#select2Patient").val("");
            reloadPatient(0);
            $(".patient-details").addClass('hide')
        }
        reloadPatient = function (id) {
            debugger
            $.ajax({
                url: '/Pharmacy/Patients/CreateOrEditPatient/' + id,  // Path to the action in the controller
                type: 'GET',  // You can use 'POST' if required
                success: function (data) {
                    // Handle success
                    // Do something with the result, e.g., display it in the UI
                    $('.patient-edit-form').empty();
                    $('.patient-edit-form').append(data);
                    if ($(".patient-details").hasClass('hide') && (id * 1 > 0)) {
                        $(".patient-details").removeClass('hide');
                    }
                    initilizeControls();
                },
                error: function (xhr, status, error) {
                    // Handle error
                }
            });
        }
        loadDoctor = function (doctorId) {
            if (doctorId > 0) {
                _$isLoading = true;
                var param = { id: doctorId };
                _doctorsService.getDoctorForEdit(param)
                    .done(function (data) {
                        var doctor = data.doctor;
                        var user = data.user;
                        var billTo = data.billTo;
                        var deliveryType = data.deliveryType;
                        var address = data.address;
                        if (doctor != null) {
                            if (doctor.licenseNumber == null || doctor.licenseNumber == "") {
                                abp.notify.error("Doctor has no NPI!");
                                $("#hdnDoctorId").val("");//don't allow for next step
                            }
                            if (doctor.signatureFileID == null || doctor.signatureFileID == "") {
                                //abp.notify.error("Doctor has no signature!");

                                //$("#hdnDoctorId").val("");//don't allow for next step
                                $("#signature-img").attr("src", "");
                            }
                            else {
                                loadSignature(doctor.signatureFileID);
                            }

                            $("#DoctorName").val(user.name);
                            $("#DoctorNPI").val(doctor.licenseNumber);
                            $("#PersonFaxing").val(data.personFaxing);
                            if (address != null) {
                                $("#DoctorAddress").val(address.labeledAddress);
                                $("#OfficePhone").val(address.phoneNo);
                                $("#OfficeFax").val(address.faxNo);
                            }
                            else {
                                $("#DoctorAddress").val('');
                                $("#OfficePhone").val('');
                                $("#OfficeFax").val('');
                            }

                            $("#hdnSignatureId").val(doctor.signatureFileID);
                            //Show
                            $(".prescriber-details").removeClass("hide");
                        }
                        _$isLoading = false;
                    })
                    .fail(function (error) {
                        abp.notify.error("Error loading prescriber information!");
                        _$isLoading = false;
                    });
            }
            else {
                $(".prescriber-details").addClass("hide");
            }

        }
        addPatient = function () {
            if (!validateDiv(".patient-details"))
                return false;

            var patientData = {};
            //var serializedData = "";
            patientData.Id = $("#hdnPatientID").val();
            $('.patient-details input').each(function () {
                patientData[$(this).attr('name')] = $(this).val();
            });

            patientData.AllergyIds = $(".patient-details").find('#AllergyId').select2('val').toString();
            patientData.StateId = $("#StateId").select2("val");
            patientData.PhoneNumber = $('#Patient_PhoneNumber').inputmask('unmaskedvalue');
            patientData.FaxNo = $('#FaxNo').inputmask('unmaskedvalue');
            patientData.GenderId = $('#Patient_Gender').val() * 1;

            $("#save-patient-button").prop('disabled', true);
            $("#save-patient-button").buttonBusy(true);
            _patientsService
                .createOrEdit(patientData)
                .done(function (data) {
                    if (data > 0) {
                        $("#hdnPatientID").val(data);
                    }
                    abp.notify.success('Saved Successfully');
                })
                .always(function () {
                    $("#save-patient-button").prop('disabled', false);
                    $("#save-patient-button").buttonBusy(false);
                });

        }

        drugRouteChange = function (drugId) {
            $.ajax({
                url: '/Pharmacy/Prescriptions/GetMedicationList?id=' + drugId + '&prescriptionId=' + ($("#hdnPrescriptionId").val() * 1),  // Path to the action in the controller
                type: 'GET',  // You can use 'POST' if required
                success: function (data) {
                    // Handle success

                    console.log("Child action result:", data);
                    // Do something with the result, e.g., display it in the UI
                    $('.medicationItems').empty();
                    $('.medicationItems').append(data);
                    $('.divNotes').show();
                },
                error: function (xhr, status, error) {
                    // Handle error
                    console.log("Error calling child action:", error);
                }
            });
        }

        reloadDoctor = function () {
            _doctorsService
                .getAllPharmacyDoctor()
                .done(function (data) {
                    if (data != null) {
                        $('#DoctorId').empty();

                        // Populate the dropdown with doctor data
                        $.each(data, function (index, doctor) {
                            $('#DoctorId').append('<option value="' + doctor.value + '">' + doctor.text + '</option>');
                        });

                        // Reinitialize the select2 plugin (if you are using it)
                        initializeDoctor();
                        $("#hdnDoctorId").val("");
                        loadDoctor(0);
                    }
                })
                .always(function () {
                });
        }
        nextStepButtons.each(function () {
            $(this).on("click", nextStep);
        });

        submitButton.on("click", function () {
            submit();
        });

        prevStepButtons.each(function () {
            $(this).on("click", prevStep);
        });

        savePatientButton.on("click", addPatient);


        //   if ($("#doctorFacilityDdl") != null && $("#doctorFacilityDdl") != undefined) {
        //       $("#doctorFacilityDdl").on("change", function () {
        //       //Save the default facility
        //            _$isDefaultFacilityChanged = true;
        //        _doctorsService
        //                .setDefaultFacility($("#doctorFacilityDdl").val() * 1)
        //           .done(function () {
        //                cleanPatient();
        //                reloadDoctor();
        //             })
        //             .always(function () {
        //             });
        //    });
        // }
        initializeDoctor = function () {
            var modal = _modalManager.getModal();
            modal.find('#DoctorId').select2({
                theme: 'bootstrap5',
                dropdownParent: '.modal',
                selectionCssClass: 'form-select',
                language: abp.localization.currentCulture.name,
                width: '100%',
                dropdownCssClass: "long-select2",
            }).on('change', function () {
                debugger;
                var doctorId = $(this).select2('val');
                $("#hdnSignatureId").val("");
                $("#hdnDoctorId").val(doctorId);
                if (doctorId !== '') {

                    loadDoctor(doctorId);
                }
                else {
                    $(".prescriber-details").addClass("hide");
                }

            });
        }
        loadSignature = function (fileId) {
            // Perform AJAX request to get the signature
            $.ajax({
                url: abp.appPath + 'api/services/app/Doctors/GetSignature',
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(fileId),
                success: function (response) {
                    if (response.result.length > 0) {
                        // Load the signature into the pad
                        $("#signature-img").attr("src", response.result);
                    }
                    else {
                        //abp.notify.error("Doctor has no signature!");
                        //$("#hdnDoctorId").val("");//don't allow for next step
                        $("#signature-img").attr("src", "");
                    }
                },
                error: function () {
                    //abp.notify.error("Failed to load signature!");
                    $("#signature-img").attr("src", "");
                    //$("#hdnDoctorId").val("");//don't allow for next step
                }
            });
        };

        initilizeControls = function () {
            var modal = _modalManager.getModal();
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
            modal.find('#Patient_PhoneNumber').inputmask('(999) 999-9999');
            modal.find('#FaxNo').inputmask('(999) 999-9999');
            modal.find('.date-picker').daterangepicker(
                {
                    locale: {
                        format: 'MM-DD-YYYY' // Change the format here
                    },
                    singleDatePicker: true
                }
            );

        };
        this.init = function (modalManager) {
            _modalManager = modalManager;
            var modal = _modalManager.getModal();
            _modalManager.onClose(function () {
                if (_$isDefaultFacilityChanged)
                    window.location.reload();
            });

            initializeDoctor();

            if ($("#hdnPrescriptionId").val() * 1 == 0)
                showStep(0);
            else {
                currentStep = 3;
                showStep(3);
                updateProgress();
                loadSubmit();
            }




            $('#select2Patient').typeahead({
                minLength: 2,  // Minimum characters to trigger search
                //highlight: true,
                //}, {
                //name: 'items',
                display: 'Label',
                //    function (item) {
                //    return item.name;  // Display name in the dropdown
                //},
                source: function (query, process) {

                    var matches = [];
                    mainObj = [];
                    $.ajax({
                        url: abp.appPath + 'api/services/app/Patients/GetPatients',
                        data: { searchText: query },
                        dataType: 'json',
                        success: function (data) {
                            $.each(data.result, function (i, doc) {
                                matches.push(doc.label);  // Display label in the suggestion list
                                mainObj.push(doc);           // Keep the full object for processing
                            });
                            process(matches);  // Bind the result to the typeahead dropdown
                        }
                    });
                }
            });

            reloadPatient($("#hdnPatientID").val() * 1);

            if ($("#hdnDoctorId").val() * 1 > 0) {
                //Load prescriber data
                loadDoctor($("#hdnDoctorId").val() * 1);
            }

            $('input[name="drugRouteIds"]:checked').each(function () {
                debugger
                var $this = $(this);
                dosageRouteId = $this.attr('data-id');
                drugRouteChange(dosageRouteId);
            });

            $('input[name="drugRouteIds"]').on('change', function () {
                debugger
                var $this = $(this);
                dosageRouteId = $this.attr('data-id');

                drugRouteChange(dosageRouteId);
            });

            $('#select2Patient').on('change', function () {
                var selected = $(this).val();
                var selectedPatient = mainObj.find(function (city) {
                    return city.label === selected;
                });

                if (selectedPatient != undefined && selectedPatient != null && selectedPatient != "") {
                    debugger;
                    $("#hdnPatientID").val(selectedPatient.id);
                    reloadPatient(selectedPatient.id);
                }

            });

            modal.find('#select2Patient').bind('typeahead:selected', function (event, selectedItem) {

                console.log('Item selected:', selectedItem);
                // You can now perform actions based on the selected item
                alert('You selected: ' + selectedItem);
            });
            //    .bind('typeahead:selected', function (e, selectedItem) {
            //    // When an item is selected, you can get the ID and Name.
            //    console.log('Selected Item ID: ' + selectedItem.value);
            //    console.log('Selected Item Name: ' + selectedItem.name);
            //    $("#PatientID").val(selectedItem.value);
            //    // You can also update a hidden field with the selected ID if needed.
            //});

            modal.find('.date-picker').daterangepicker(app.createDateTimePickerOptions());

            _$prescriptionEle = modal.find("#hdnPrescriptionId");

            //if (_$prescriptionEle.length > 0) {
            //  _$deleteButton.show();
            //}
            //else {
            //  _$deleteButton.hide();
            //}
            //Delete Doctor
            _$deleteButton.click(function () {

                var prescription = _$prescriptionInformationForm.serializeFormToObject();
                abp.message.confirm('', app.localize('AreYouSure'), function (isConfirmed) {
                    if (isConfirmed) {
                        _modalManager.setBusy(true);
                        _prescriptionsService
                            .delete({
                                id: $("#hdnPrescriptionId").val() * 1
                            })
                            .done(function () {
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                                _modalManager.close();
                                abp.event.trigger('app.prescriptionDataDeleted');
                            })
                            .always(function () {
                                _modalManager.setBusy(false);
                            });

                    }
                });
            });

            _$saveSubmitLaterButton.click(function () {
                abp.notify.success("Prescription saved successfully you can submit it later!");
                _modalManager.close();
                abp.event.trigger('app.createOrEditPrescriptionModalSaved');
            });

            _$prescriptionInformationForm = _modalManager.getModal().find('form[name=PrescriptionInformationsForm]');
            _$prescriptionInformationForm.validate();
        };
        this.save = function () {
            if (currentStep != 2)
                return;
            var medicationList = [];
            $.each($('.prescriptionItems'), function () {
                var $this = $(this);
                if ($this.find('.chkMedicines').prop("checked")) {
                    var medication = {};
                    medication["medicationId"] = $this.find('.chkMedicines').attr('data-id') * 1;
                    //medication["quantity"] = $this.find('.medicationQuantity').val() * 1;
                    medication["refillsAllowed"] = $this.find('.medicationRefilledQuantity').val() * 1;
                    medicationList.push(medication);
                }
            });
            $.each($('.prescriptionItemsNausea'), function () {
                var $this = $(this);
                if ($this.find('.chkMedicinesNausea').prop("checked")) {
                    var medication = {};
                    medication["medicationId"] = $this.find('.chkMedicinesNausea').attr('data-id') * 1;
                    //medication["quantity"] = $this.find('.medicationQuantity').val() * 1;
                    medication["refillsAllowed"] = $this.find('.medicationRefilledQuantity').val() * 1;
                    medicationList.push(medication);
                }
            });
            $.each($('.prescriptionItemsOther'), function () {
                var $this = $(this);
                if ($this.find('.chkMedicinesOther').prop("checked")) {
                    var medication = {};
                    medication["medicationId"] = $this.find('.chkMedicinesOther').attr('data-id') * 1;
                    //medication["quantity"] = $this.find('.medicationQuantity').val() * 1;
                    medication["refillsAllowed"] = $this.find('.medicationRefilledQuantity').val() * 1;
                    medicationList.push(medication);
                }
            });

            if (medicationList.length < 1) {
                abp.notify.warn(app.localize('PleaseAddMedicineToThePrescription'));
                return;
            }

            if (!validatePrescriber())
                return;

            //if (!_$prescriptionInformationForm.valid()) {
            //    return;
            //}
            var prescription = _$prescriptionInformationForm.serializeFormToObject();
            prescription["prescriptionItems"] = medicationList;
            prescription["dosageRouteId"] = dosageRouteId;
            prescription["id"] = $("#hdnPrescriptionId").val() * 1;
            prescription["doctorID"] = $("#hdnDoctorId").val();
            prescription["patientID"] = $("#hdnPatientID").val();
            _modalManager.setBusy(true);
            _prescriptionsService
                .createOrEdit(prescription)
                .done(function (data) {
                    abp.notify.success(app.localize('SavedSuccessfully'));
                    //_modalManager.close();
                    abp.event.trigger('app.createOrEditPrescriptionModalSaved');
                    if (data > 0) {
                        $("#hdnPrescriptionId").val(data);
                        loadSubmit();
                        nextStep();
                    }
                })
                .always(function () {
                    _modalManager.setBusy(false);
                });
        };
    };
})(jQuery);
