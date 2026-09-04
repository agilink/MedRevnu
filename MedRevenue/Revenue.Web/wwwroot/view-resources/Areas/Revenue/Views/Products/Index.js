(function () {
    $(function () {
        var _$productsTable = $('#ProductsTable');
        var _productsService = abp.services.app.products;

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Revenue/Products/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Revenue/Views/Products/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditProductModal'
        });

        var _viewModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Revenue/Products/DetailsModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Revenue/Views/Products/_ViewModal.js',
            modalClass: 'ViewProductModal'
        });

        var dataTable = _$productsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _productsService.getAll,
                inputFilter: function () {
                    return {
                        categoryIdFilter: $('#CategoryFilter').val() ? parseInt($('#CategoryFilter').val()) : null,
                        subcategoryIdFilter: $('#SubcategoryFilter').val() ? parseInt($('#SubcategoryFilter').val()) : null
                    };
                }
            },
            columnDefs: [
                {
                    targets: 0,
                    data: 'productCode',
                    name: 'productCode',
                    render: function (v) { return v || '-'; }
                },
                {
                    targets: 1,
                    data: 'name',
                    name: 'name'
                },
                {
                    targets: 2,
                    data: 'productCategoryName',
                    name: 'productCategoryName',
                    render: function (v) { return v || '-'; }
                },
                {
                    targets: 3,
                    data: 'subproductCategoryName',
                    name: 'subproductCategoryName',
                    render: function (v) { return v || '-'; }
                },
                {
                    targets: 4,
                    data: 'manufacturer',
                    name: 'manufacturer',
                    render: function (v) { return v || '-'; }
                },
                {
                    targets: 5,
                    data: 'modelNo',
                    name: 'modelNo',
                    render: function (v) { return v || '-'; }
                },
                {
                    targets: 6,
                    data: 'basePrice',
                    name: 'basePrice',
                    className: 'text-end',
                    render: function (v) {
                        return '$' + parseFloat(v).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    }
                },
                {
                    targets: 7,
                    data: 'cost',
                    name: 'cost',
                    className: 'text-end',
                    render: function (v) {
                        return '$' + parseFloat(v).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    }
                },
                {
                    targets: 8,
                    data: 'price',
                    name: 'price',
                    className: 'text-end',
                    render: function (v) {
                        return '$' + parseFloat(v).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    }
                },
                {
                    targets: 9,
                    data: 'isActive',
                    name: 'isActive',
                    className: 'text-center',
                    render: function (v) {
                        return v
                            ? '<span class="badge badge-light-success">' + app.localize('Active') + '</span>'
                            : '<span class="badge badge-light-danger">' + app.localize('Inactive') + '</span>';
                    }
                },
                {
                    // Icon buttons rather than a dropdown, and last rather than first: the actions
                    // belong beside the row they act on, and one click instead of two.
                    targets: 10,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    className: 'text-end ati-row-actions',
                    render: function (unused, type, row) {
                        var html = '';

                        if (true) {
                            html += '<button type="button" class="btn btn-sm btn-icon btn-light-primary ms-1 ati-act-edit" '
                                + 'title="' + app.localize('Edit') + '"><i class="fa fa-pen"></i></button>';
                        }

                        if (true) {
                            html += '<button type="button" class="btn btn-sm btn-icon btn-light-info ms-1 ati-act-details" '
                                + 'title="' + app.localize('Details') + '"><i class="fa fa-eye"></i></button>';
                        }

                        if (true) {
                            html += '<button type="button" class="btn btn-sm btn-icon btn-light-danger ms-1 ati-act-delete" '
                                + 'title="' + app.localize('Delete') + '"><i class="fa fa-trash"></i></button>';
                        }

                        return html;
                    }
                }
            ]
        });


        // Delegated: the grid redraws its rows, so per-row binding would not
        // survive a page change or a sort.

        _$productsTable.on('click', '.ati-act-edit', function () {
            var data = { record: dataTable.row($(this).closest('tr')).data() };
            _createOrEditModal.open({ id: data.record.id });
        });

        _$productsTable.on('click', '.ati-act-details', function () {
            var data = { record: dataTable.row($(this).closest('tr')).data() };
            _viewModal.open({ id: data.record.id });
        });

        _$productsTable.on('click', '.ati-act-delete', function () {
            var data = { record: dataTable.row($(this).closest('tr')).data() };
            deleteProduct(data.record);
        });

        function getProducts() {
            dataTable.ajax.reload();
        }

        function deleteProduct(product) {
            abp.message.confirm(
                app.localize('AreYouSure'),
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _productsService.delete({ id: product.id }).done(function () {
                            getProducts();
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                    }
                }
            );
        }

        // Category filter change — reload subcategories dynamically
        $('#CategoryFilter').change(function () {
            var categoryId = $(this).val();
            var $sub = $('#SubcategoryFilter');
            $sub.html('<option value="">All Subcategories</option>');

            if (categoryId) {
                $.getJSON('/Revenue/Products/GetSubcategoriesByCategory', { categoryId: categoryId }, function (data) {
                    $.each(data, function (i, item) {
                        $sub.append('<option value="' + item.id + '">' + item.name + '</option>');
                    });
                });
            }

            getProducts();
        });

        $('#SubcategoryFilter').change(function () {
            getProducts();
        });

        $('#GetProductsButton').click(function (e) {
            e.preventDefault();
            getProducts();
        });

        $('#ProductsTableFilter').on('keydown', function (e) {
            if (e.keyCode === 13) {
                e.preventDefault();
                getProducts();
            }
        });

        $('#ShowAdvancedFiltersSpan').click(function () {
            $('#ShowAdvancedFiltersSpan').hide();
            $('#HideAdvancedFiltersSpan').show();
            $('#AdvancedFiltersArea').slideDown();
        });

        $('#HideAdvancedFiltersSpan').click(function () {
            $('#HideAdvancedFiltersSpan').hide();
            $('#ShowAdvancedFiltersSpan').show();
            $('#AdvancedFiltersArea').slideUp();
        });

        $('#RefreshProductsButton').click(function (e) {
            e.preventDefault();
            getProducts();
        });

        $('#CreateNewProductButton').click(function () {
            _createOrEditModal.open();
        });

        abp.event.on('app.createOrEditProductModalSaved', function () {
            getProducts();
        });
    });
})();
