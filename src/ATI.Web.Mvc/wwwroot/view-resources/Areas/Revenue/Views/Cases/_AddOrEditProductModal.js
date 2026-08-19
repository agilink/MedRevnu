(function () {
    app.modals.AddOrEditProductModal = function () {
        var _modalManager;
        var _casesService = abp.services.app.cases;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$form = _modalManager.getModal().find('form[name=CaseProductForm]');
            _$form.validate();

            loadProducts();

            $('#Quantity, #UnitPrice, #Discount').on('input change', function () {
                calculateTotal();
            });

            $('#ProductId').on('change', function () {
                var price = $(this).find('option:selected').data('price');
                if (price) {
                    $('#UnitPrice').val(price);
                    calculateTotal();
                }
            });
        };

        function loadProducts() {
            abp.services.app.products.getAllActive()
                .done(function (result) {
                    var $select = $('#ProductId');
                    $select.empty().append('<option value="">-- Select Product --</option>');

                    if (result && result.items && result.items.length > 0) {
                        $.each(result.items, function (i, product) {
                            $select.append(
                                $('<option>').val(product.id)
                                    .attr('data-price', product.price || 0)
                                    .text(product.name)
                            );
                        });

                        var currentProductId = $select.data('current-product-id');
                        if (currentProductId) {
                            $select.val(currentProductId);
                            calculateTotal();
                        }
                    }
                })
                .fail(function () {
                    abp.message.error('Failed to load products');
                });
        }

        function calculateTotal() {
            var quantity = parseFloat($('#Quantity').val()) || 0;
            var unitPrice = parseFloat($('#UnitPrice').val()) || 0;
            var discount = parseFloat($('#Discount').val()) || 0;
            $('#TotalPrice').val(((quantity * unitPrice) - discount).toFixed(2));
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

            abp.ajax({
                url: abp.appPath + 'api/services/app/Cases/AddOrUpdateCaseProduct',
                type: 'POST',
                data: JSON.stringify(caseProductData),
                contentType: 'application/json'
            })
            .done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
                abp.event.trigger('app.addOrEditProductModalSaved');
            })
            .always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})();
