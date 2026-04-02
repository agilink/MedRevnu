(function ($) {
    app.modals.CreateOrEditProcedureTransactionModal = function () {
        var _transactionsService = abp.services.app.procedureTransactions;
        var _modalManager;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$form = _modalManager.getModal().find('form[name=ProcedureTransactionCreateOrEditForm]');
            _$form.validate();

            // Initialize form logic
            initializeFormLogic();

            // If editing and hospital/product are selected, load price on page load
            var hospitalId = $('#HospitalId').val();
            var productId = $('#ProductId').val();
            if (hospitalId && productId) {
                updateBasePrice();
            }
        };

        function initializeFormLogic() {
            // When hospital changes, update base price
            $('#HospitalId').on('change', function () {
                updateBasePrice();
            });

            // When physician changes, load facility and update base price
            $('#PhysicianId').on('change', function () {
                var physicianId = $(this).val();
                if (physicianId) {
                    loadPhysicianFacility(physicianId);
                } else {
                    $('#HospitalId').val('');
                }
            });

            // When product changes, update base price
            $('#ProductId').on('change', function () {
                updateBasePrice();
            });

            // When quantity or unit price changes, update total amount
            $('#Quantity, #UnitPrice').on('input change', function () {
                calculateTotalAmount();
            });

            // Manual total amount can be modified (allow user override)
            $('#TotalAmount').on('input', function () {
                // Allow manual override - do nothing
            });
        }

        function loadPhysicianFacility(physicianId) {
            $.ajax({
                url: abp.appPath + 'Revenue/ProcedureTransactions/GetPhysicianFacility',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ physicianId: parseInt(physicianId) }),
                success: function (result) {
                    if (result.success) {
                        $('#HospitalId').val(result.facilityId || '');
                        // After setting hospital, update the price
                        updateBasePrice();
                    } else {
                        abp.notify.error('Failed to load physician facility: ' + result.message);
                    }
                },
                error: function (xhr) {
                    console.error('Failed to load physician facility', xhr);
                    abp.notify.error('Failed to load physician facility');
                }
            });
        }

        function updateBasePrice() {
            var hospitalId = $('#HospitalId').val();
            var productId = $('#ProductId').val();

            if (!hospitalId || !productId) {
                return;
            }

            $.ajax({
                url: abp.appPath + 'Revenue/ProcedureTransactions/GetProductPriceByHospital',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    hospitalId: parseInt(hospitalId),
                    productId: parseInt(productId)
                }),
                success: function (result) {
                    if (result.success) {
                        $('#UnitPrice').val(result.unitPrice.toFixed(2));
                        calculateTotalAmount();
                    } else {
                        abp.notify.error('Failed to load price: ' + result.message);
                    }
                },
                error: function (xhr) {
                    console.error('Failed to load price', xhr);
                    abp.notify.error('Failed to load price');
                }
            });
        }

        function calculateTotalAmount() {
            var quantity = parseFloat($('#Quantity').val()) || 0;
            var unitPrice = parseFloat($('#UnitPrice').val()) || 0;
            var totalAmount = quantity * unitPrice;
            $('#TotalAmount').val(totalAmount.toFixed(2));
        }

        this.save = function () {
            if (!_$form.valid()) {
                return;
            }

            var procedureTransaction = _$form.serializeFormToObject();

            _modalManager.setBusy(true);
            _transactionsService
                .createOrEdit(procedureTransaction)
                .done(function () {
                    abp.notify.info(app.localize('SavedSuccessfully'));
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditProcedureTransactionModalSaved');
                })
                .always(function () {
                    _modalManager.setBusy(false);
                });
        };
    };
})(jQuery);
