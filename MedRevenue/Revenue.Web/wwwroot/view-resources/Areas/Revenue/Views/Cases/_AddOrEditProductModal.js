(function () {
    app.modals.AddOrEditProductModal = function () {
        var _modalManager;
        var _casesService = abp.services.app.cases;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$form = _modalManager.getModal().find('form[name=CaseProductForm]');
            _$form.validate();

            // Load products dropdown
            loadProducts();

            // Calculate total when quantity, unit price, or discount changes
            $('#Quantity, #UnitPrice, #Discount').on('input change', function () {
                calculateTotal();
            });

            // Set unit price when product is selected
            $('#ProductId').on('change', function () {
                var selectedOption = $(this).find('option:selected');
                var price = selectedOption.data('price');
                if (price) {
                    $('#UnitPrice').val(price);
                    calculateTotal();
                }
            });
        };

        function loadProducts() {
            $.ajax({
                url: abp.appPath + 'api/services/app/Products/GetAll',
                type: 'GET',
                success: function (result) {
                    var $select = $('#ProductId');
                    $select.empty();
                    $select.append('<option value="">-- Select Product --</option>');

                    if (result.items && result.items.length > 0) {
                        $.each(result.items, function (index, product) {
                            $select.append(
                                $('<option></option>')
                                    .attr('value', product.id)
                                    .attr('data-price', product.price || 0)
                                    .text(product.name)
                            );
                        });

                        // If editing, select the current product
                        var currentProductId = _modalManager.getArgs().productId;
                        if (currentProductId) {
                            $select.val(currentProductId);
                            $select.trigger('change');
                        }
                    }
                },
                error: function () {
                    abp.notify.error('Failed to load products');
                }
            });
        }

        function calculateTotal() {
            var quantity = parseFloat($('#Quantity').val()) || 0;
            var unitPrice = parseFloat($('#UnitPrice').val()) || 0;
            var discount = parseFloat($('#Discount').val()) || 0;

            var total = (quantity * unitPrice) - discount;
            $('#TotalPrice').val(total.toFixed(2));
        }

        this.save = function () {
            if (!_$form.valid()) {
                return;
            }

            var caseProductData = {
                id: parseInt($('#Id').val()) || 0,
                caseId: parseInt($('#CaseId').val()),
                productId: parseInt($('#ProductId').val()),
                quantity: parseFloat($('#Quantity').val()),
                unitPrice: parseFloat($('#UnitPrice').val()),
                discount: parseFloat($('#Discount').val()) || 0,
                totalPrice: parseFloat($('#TotalPrice').val())
            };

            if (!caseProductData.caseId) {
                abp.notify.error('Case ID is required');
                return;
            }

            if (!caseProductData.productId) {
                abp.notify.error('Please select a product');
                return;
            }

            _modalManager.setBusy(true);

            _casesService
                .addOrUpdateCaseProduct(caseProductData)
                .done(function () {
                    abp.notify.info(app.localize('SavedSuccessfully'));
                    _modalManager.close();
                    abp.event.trigger('app.addOrEditProductModalSaved');
                })
                .fail(function (error) {
                    console.error('Save failed:', error);
                })
                .always(function () {
                    _modalManager.setBusy(false);
                });
        };
    };
})();
