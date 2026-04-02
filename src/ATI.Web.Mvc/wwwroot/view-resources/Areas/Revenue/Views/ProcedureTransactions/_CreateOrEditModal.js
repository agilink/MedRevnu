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

            // If editing, load physician's facility on page load
            var physicianId = $('#PhysicianId').val();
            if (physicianId) {
                loadPhysicianFacility(physicianId);
            }
        };

        function initializeFormLogic() {
            // When physician changes, load facility and update base price
            $('#PhysicianId').on('change', function () {
                var physicianId = $(this).val();
                if (physicianId) {
                    loadPhysicianFacility(physicianId);
                    updateBasePrice();
                } else {
                    $('#HospitalId').val('');
                    $('#HospitalName').val('');
                }
            });

            // When product or procedure type changes, update base price
            $('#ProductId, input[name="ProcedureType"]').on('change', function () {
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
                        $('#HospitalName').val(result.facilityName || '(No facility assigned)');
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
            var productId = $('#ProductId').val();
            var procedureType = $('input[name="ProcedureType"]:checked').val();

            if (!productId || !procedureType) {
                return;
            }

            $.ajax({
                url: abp.appPath + 'Revenue/ProcedureTransactions/GetProductBasePrice',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    productId: parseInt(productId),
                    procedureType: procedureType
                }),
                success: function (result) {
                    if (result.success) {
                        $('#UnitPrice').val(result.basePrice.toFixed(2));
                        calculateTotalAmount();
                    } else {
                        abp.notify.error('Failed to load base price: ' + result.message);
                    }
                },
                error: function (xhr) {
                    console.error('Failed to load base price', xhr);
                    abp.notify.error('Failed to load base price');
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
