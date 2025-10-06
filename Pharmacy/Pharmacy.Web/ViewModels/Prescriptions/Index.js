(function () {
    $(function () {
        var _$prescriptionTable = $('#PrescriptionTable');
        var _$prescriptionTable = $('#PrescriptionTable');
        var _prescription = abp.services.app.prescriptions;
        var _entityTypeFullName = 'ATI.Pharmacy.Prescriptions';


        var _permissions = {
            create: abp.auth.hasPermission('Pages.Prescriptions.Create'),
            edit: abp.auth.hasPermission('Pages.Prescriptions.Edit'),
            delete: abp.auth.hasPermission('Pages.Prescriptions.Delete'),
            special: abp.auth.hasPermission('Pages.Prescriptions.Grid.View.Company'),
            download: abp.auth.hasPermission('Pages.Prescriptions.Download'),
            view: abp.auth.hasPermission('Pages.Prescriptions.View'),
            complete: abp.auth.hasPermission('Pages.Prescriptions.Complete'),
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Pharmacy/Prescriptions/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Pharmacy/Views/Prescriptions/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditPrescriptionModal',
            modalSize: 'modal-fullscreen'
        });

        var _viewPrescriptionModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Pharmacy/Prescriptions/ViewPrescriptionModal',
            modalClass: 'ViewPrescriberModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Pharmacy/Views/Prescriptions/_ViewAndPrintModal.js',
        });

        var _createOrEditModalRefill = new app.ModalManager({
            viewUrl: abp.appPath + 'Pharmacy/Prescriptions/CreateRefillModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Pharmacy/Views/Prescriptions/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditPrescriptionModal',
            modalSize: 'modal-fullscreen'
        });

        var _entityTypeHistoryModal = app.modals.EntityTypeHistoryModal.create();
        function entityHistoryIsEnabled() {
            return (
                abp.auth.hasPermission('Pages.Administration.AuditLogs') &&
                abp.custom.EntityHistory &&
                abp.custom.EntityHistory.IsEnabled &&
                _.filter(abp.custom.EntityHistory.EnabledEntities, (entityType) => entityType === _entityTypeFullName)
                    .length === 1
            );
        }


        //var _excelColumnSelectionModal = new app.ModalManager({
        //  viewUrl: abp.appPath + 'Pharmacy/Prescriptions/ExcelColumnSelectionModal',
        //  scriptUrl: abp.appPath + 'view-resources/Areas/Pharmacy/Views/Prescriptions/_ExcelColumnSelectionModal.js',
        //  modalClass: 'PrescriptionExcelColumnSelectionModal',
        //});

        var columnDefs = [
            {
                className: 'control responsive',
                orderable: false,
                render: function () {
                    return '';
                },
                targets: 0,
            },
            {
                data: 'prescription.id',
                targets: 1,
                render: function (data, type, row) {
                    debugger
                    return '<input type="checkbox" class="row-select" data-id="' + data + '" />';
                },
                orderable: false,  // Disable sorting for this column
                searchable: false  // Disable search for this column
            },
            {
                targets: 2,
                data: 'prescription.patientName',
                name: 'Patient.User.name',
                className: 'dt-left'
            },
            {
                targets: 3,
                data: 'prescription.patientsurName',
                name: 'Patient.User.surName',
                className: 'dt-left'
            },
            {
                targets: 4,
                data: 'prescription.dateOfBirth',
                name: 'Patient.dateOfBirth',
                className: 'dt-left',
                render: function (data, type, row) {
                    if (type === 'display' && data) {
                        // Format the date using JavaScript
                        const date = new Date(data);
                        return `${(date.getMonth() + 1).toString().padStart(2, '0')}/${date.getDate().toString().padStart(2, '0')}/${date.getFullYear()}`;
                    }
                    return data;
                }
            },
            {
                orderable: false,
                targets: 5,
                data: 'prescription.patientContact',
                name: 'Patient.User.phoneNumber',
                className: 'dt-left',
                render: function (data, type, row) {
                    // Format phone number (e.g., (123) 456-7890)
                    return formatPhoneNumber(data);
                }
            },
            {
                orderable: false,
                targets: 6,
                data: 'prescription.drugs',
                name: 'Medication',
                className: 'dt-left',
                width: 250,
                render: function (data, type, row) {
                    return data.replace(/&lt;/g, "<").replace(/&gt;/g, ">");
                }
            },
            {
                targets: 7,
                data: 'prescription.doctorName',
                name: 'Doctor.User.name',
                className: 'dt-left'
            },
            {
                targets: 8,
                data: 'prescription.deliveryType',
                name: 'deliveryTypeId',
                className: 'dt-left'
            },
            {
                targets: 9,
                data: 'prescription.billToName',
                name: 'billingTo',
                className: 'dt-left'
            },
            {
                targets: 10,
                orderable: false,
                data: 'doctor.faxNumber',
                name: 'Doctor.Address.faxNo',
                className: 'dt-left',
                render: function (data, type, row) {
                    // Format phone number (e.g., (123) 456-7890)
                    return formatPhoneNumber(data);
                }
            },
            {
                orderable: true,
                defaultContent: '',
                data: 'prescription.lastModificationTime',
                name: 'lastModificationTime',
                className: 'dt-left',
                render: function (data, type, row) {
                    if (type === 'display' && data) {
                        // Format the date using JavaScript
                        const date = new Date(data);
                        return `${(date.getMonth() + 1).toString().padStart(2, '0')}/${date.getDate().toString().padStart(2, '0')}/${date.getFullYear()}`;
                    }
                    return data;
                },
                targets: _permissions.special ? 12 : 11
            },
            {
                orderable: true,
                defaultContent: '',
                data: 'prescription.prescriptionDate',
                name: 'prescriptionDate',
                className: 'dt-left',
                render: function (data, type, row) {
                    if (type === 'display' && data) {
                        // Format the date using JavaScript
                        const date = new Date(data);
                        return `${(date.getMonth() + 1).toString().padStart(2, '0')}/${date.getDate().toString().padStart(2, '0')}/${date.getFullYear()}  ${date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', hour12: true })}`;
                    }
                    return data;
                },
                targets: _permissions.special ? 13 : 12
            },
            {
                data: null,
                orderable: true,
                defaultContent: '',
                name: 'prescriptionStatusId',
                render: function (data) {
                    return "<label>" + data.prescription.prescriptionStatus + "</label>";
                },
                targets: _permissions.special ? 14 : 13
            },
            {
                targets: _permissions.special ? 15 : 14,
                data: null,
                orderable: false,
                autoWidth: false,
                defaultContent: '',
                rowAction: {
                    text:
                        '<i class="fa fa-cog"></i> <span class="d-none d-md-inline-block d-lg-inline-block d-xl-inline-block">' +
                        app.localize('Actions') +
                        '</span> <span class="caret"></span>',
                    items: [
                        {
                            text: "View (R/O)",
                            visible: function (data) {
                                if (data.record.prescription.prescriptionStatusId >= 2) {
                                    return _permissions.view;
                                }
                                else {
                                    return false;
                                }
                            },
                            action: function (data) {
                                _viewPrescriptionModal.open({ id: data.record.prescription.id });
                            },
                        },
                        {
                            text: "Edit",
                            visible: function (data) {
                                if (data.record.prescription.prescriptionStatusId < 2) {
                                    return _permissions.edit;
                                }
                                else {
                                    return false;
                                }
                            },
                            action: function (data) {
                                _createOrEditModal.open({ id: data.record.prescription.id });
                            },
                        },
                        {
                            text: "Refill",
                            visible: function (data) {
                                debugger;
                                if (data.record.prescription.prescriptionStatusId > 1) {
                                    return _permissions.create;
                                }
                                else {
                                    return false;
                                }
                            },
                            action: function (data) {
                                openRefillModal(data.record.prescription.id, data.record.prescription.patientName + " " + data.record.prescription.patientsurName);
                            },
                        },
                        {
                            text: "Delete",
                            visible: function (data) {
                                if (data.record.prescription.prescriptionStatusId < 2) {
                                    return _permissions.delete;
                                }
                                else {
                                    return false;
                                }
                            },
                            action: function (data) {
                                deletePrescription(data.record.prescription, data.record.prescription.patientName + " " + data.record.prescription.patientsurName);
                            },
                        },
                        {
                            text: "Complete",
                            visible: function (data) {
                                if (data.record.prescription.prescriptionStatusId == 2) {
                                    return _permissions.complete;
                                }
                                else {
                                    return false;
                                }
                            },
                            action: function (data) {
                                completePrescription(data.record.prescription, data.record.prescription.patientName + " " + data.record.prescription.patientsurName);
                            },
                        },
                        {
                            text: "Download",
                            visible: function (data) {
                                if (data.record.prescription.prescriptionStatusId >= 2) {
                                    return _permissions.download;
                                }
                                else {
                                    return false;
                                }
                            },
                            action: function (data) {
                                download(data.record.prescription.id);
                            },
                        },
                    ],
                },
            },
        ]

        // Conditionally add the 'companyName' column if _permissions.special is true
        if (_permissions.special) {
            columnDefs.push({
                targets: 11,
                width: 120,
                data: 'prescription.companyName',
                name: 'companyName',
                className: 'dt-left'
            });
        }


        var dataTable = _$prescriptionTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _prescription.getAll,
                inputFilter: function () {
                    return {
                        filter: $('#PrescriptionsTableFilter').val() || '',
                        prescriptionStatus: $('#PrescriptionStatus').val(),
                    };
                },
            },
            columnDefs: columnDefs
        });

        function formatPhoneNumber(phoneNumber) {
            // Remove non-numeric characters
            const cleaned = ('' + phoneNumber).replace(/\D/g, '');
            const match = cleaned.match(/^(\d{3})(\d{3})(\d{4})$/);
            if (match) {
                return '(' + match[1] + ') ' + match[2] + '-' + match[3];
            }
            return phoneNumber; // Return as is if not valid
        }
        function getPrescriptions() {
            dataTable.ajax.reload();
        }

        function download(id) {
            window.location.href = "/Pharmacy/Prescriptions/Download/" + id;
        }
        function submitPrescription(prescriptionId) {
            abp.message.confirm('', app.localize('AreYouSureYouWantToSubmitThePrescription'), function (isConfirmed) {
                if (isConfirmed) {
                    _prescription
                        .submit({ id: prescriptionId })
                        .done(function (res) {
                            if (res > 0) {
                                getPrescriptions(true);
                                abp.notify.success("Submitted Successfully!");
                            }
                            else {
                                abp.notify.error("Some error occured while submitting.");
                            }
                        })
                        .always(function () {
                        });
                }
            });
        }
        function getSelectedRows() {
            var selectedIds = [];
            $('.row-select:checked').each(function () {
                selectedIds.push($(this).data('id'));
            });
            return selectedIds;
        }

        function deletePrescription(prescription, name) {
            abp.message.confirm('', app.localize('AreYouSureDeletePrescription') + " for " + name, function (isConfirmed) {
                if (isConfirmed) {
                    _prescription
                        .delete({
                            id: prescription.id,
                        })
                        .done(function () {
                            getPrescriptions(true);
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                }
            });
        }
        function completePrescription(prescription, name) {
            abp.message.confirm('', app.localize('AreYouSureCompletePrescription') + " for " + name, function (isConfirmed) {
                if (isConfirmed) {
                    _prescription
                        .complete({
                            id: prescription.id,
                        })
                        .done(function () {
                            getPrescriptions(true);
                            abp.notify.success("Completed successfully!");
                        });
                }
            });
        }

        function openRefillModal(id, name) {
            abp.message.confirm('', app.localize('AreYouSureYouWantToRefill') + " for " + name, function (isConfirmed) {
                if (isConfirmed) {
                    _createOrEditModalRefill.open({ id: id });
                }
            });
        }

        // Initialize click event listeners for action buttons
        _$prescriptionTable.on('draw.dt', function () {
            // Re-initialize button click actions after data has been loaded/redrawn           
            $('.edit-prescription').on('click', function () {
                var id = $(this).attr("data-id");
                _createOrEditModal.open({ id: id });

            });
            $('.view-prescription').on('click', function () {
                var id = $(this).attr("data-id");
                _viewPrescriptionModal.open({ id: id });
            });
            $('.download-prescription').on('click', function () {
                var id = $(this).attr("data-id");
                download(id);
            });
            $('.submit-prescription').on('click', function () {
                var id = $(this).attr("data-id");
                submitPrescription(id);
            });
        });
        $('#ShowAdvancedFiltersSpan').click(function () {
            $('#ShowAdvancedFiltersSpan').hide();
            $('#HideAdvancedFiltersSpan').show();
            $('#AdvacedAuditFiltersArea').slideDown();
        });

        $('#HideAdvancedFiltersSpan').click(function () {
            $('#HideAdvancedFiltersSpan').hide();
            $('#ShowAdvancedFiltersSpan').show();
            $('#AdvacedAuditFiltersArea').slideUp();
        });

        $('#CreateNewPrescriptionButton').click(function () {
            _createOrEditModal.open();
        });

        $('#BulkSubmitPrescription').click(function () {
            var selectedIds = getSelectedRows();
            if (selectedIds.length > 0) {
                // Make AJAX request to delete selected products
                abp.message.confirm('', app.localize('AreYouSureBulkSubmit'), function (isConfirmed) {
                    if (isConfirmed) {
                        _prescription
                            .bulkSubmit(selectedIds)
                            .done(function (res) {
                                getPrescriptions(true);
                                abp.notify.success(res);

                            })
                            //.error(function () {
                            //    abp.notify.error('An error occurred while submitting the selected prescriptions.');

                            //})
                            .always(function () {
                            });
                    }
                });

            } else {
                abp.notify.error('Please select prescription to submit.');
            }
        });

        $('#BulkCompletePrescription').click(function () {
            var selectedIds = getSelectedRows();
            if (selectedIds.length > 0) {
                // Make AJAX request to delete selected products
                abp.message.confirm('', app.localize('AreYouSureBulkComplete'), function (isConfirmed) {
                    if (isConfirmed) {
                        _prescription
                            .bulkComplete(selectedIds)
                            .done(function (res) {
                                getPrescriptions(true);
                                abp.notify.success(res);

                            })
                            //.error(function () {
                            //    abp.notify.error('An error occurred while submitting the selected prescriptions.');

                            //})
                            .always(function () {
                            });
                    }
                });
            } else {
                abp.notify.error('Please select prescription to complete.');
            }
        });

        abp.event.on('app.createOrEditPrescriptionModalSaved', function () {
            getPrescriptions();
        });
        abp.event.on('app.prescriptionDataDeleted', function () {
            getPrescriptions();
        });

        $('#GetPrescriptionsButton').click(function (e) {
            e.preventDefault();
            getPrescriptions();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getPrescriptions();
            }
        });

        $('.reload-on-change').change(function (e) {
            getPrescriptions();
        });

        $('.reload-on-keyup').keyup(function (e) {
            getPrescriptions();
        });

        $('#btn-reset-filters').click(function (e) {
            $('.reload-on-change,.reload-on-keyup,#MyEntsTableFilter,#PrescriptionStatus').val('');
            getPrescriptions();
        });

        $('#PrescriptionStatus').on("change", function () {
            getPrescriptions();
        });
        //$('#FacilityList').on("change", function () {
        //    getPrescriptions();
        //});
    });
})();
