(function () {
    app.modals.CreateOrEditProductModal = function () {
        var _modalManager;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$form = _modalManager.getModal().find('form[name=ProductCreateOrEditForm]');
            _$form.validate();
            loadCategories();
        };

        function loadCategories() {
            abp.ajax({
                url: abp.appPath + 'Revenue/ProductCategories/GetAll',
                type: 'GET',
                abpHandleError: false
            }).done(function (result) {
                var $select = _$form.find('#ProductCategoryId');
                var currentId = parseInt($select.data('current-category-id')) || 0;

                if (result && result.items && result.items.length > 0) {
                    $.each(result.items, function (i, cat) {
                        $select.append($('<option>').val(cat.id).text(cat.name));
                    });
                }

                if (currentId) {
                    $select.val(currentId);
                }
            }).fail(function () {
                abp.message.error('Failed to load categories');
            });
        }

        this.save = function () {
            if (!_$form.valid()) {
                return;
            }

            var productId = parseInt(_$form.find('input[name=Id]').val()) || 0;
            var categoryId = _$form.find('#ProductCategoryId').val();
            var isActiveChecked = _$form.find('#IsActive').is(':checked');

            var productData = {
                name: _$form.find('#Name').val(),
                manufacturer: _$form.find('#Manufacturer').val() || null,
                modelNo: _$form.find('#ModelNo').val() || null,
                productCategoryId: categoryId ? parseInt(categoryId) : null,
                cost: parseFloat(_$form.find('#Cost').val()) || 0,
                price: parseFloat(_$form.find('#Price').val()) || 0,
                description: _$form.find('#Description').val() || null,
                isActive: isActiveChecked
            };

            var isCreate = productId === 0;
            if (!isCreate) {
                productData.id = productId;
            }

            _modalManager.setBusy(true);

            abp.ajax({
                url: abp.appPath + 'api/services/app/Products/' + (isCreate ? 'Create' : 'Update'),
                type: isCreate ? 'POST' : 'PUT',
                data: JSON.stringify(productData),
                contentType: 'application/json'
            })
            .done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
                abp.event.trigger('app.createOrEditProductModalSaved');
            })
            .always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})();
