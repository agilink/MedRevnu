(function ($) {
    $(function () {
        var _$productsTable = $('#ProductsTable');
        var _productsService = abp.services.app.products;

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Products.Create'),
            edit: abp.auth.hasPermission('Pages.Products.Edit'),
            'delete': abp.auth.hasPermission('Pages.Products.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Revenue/Products/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Revenue/Views/Products/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditProductModal'
        });

        var _viewProductModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Revenue/Products/ViewProductModal',
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
                        maxResultCount: 10,
                        skipCount: 0
                    };
                }
            },
            buttons: [
                {
                    name: 'refresh',
                    text: '<i class="fas fa-redo-alt"></i>',
                    action: function () {
                        dataTable.draw(false);
                    }
                }
            ],
            responsive: {
                details: {
                    type: 'column'
                }
            },
            columnDefs: [
                {
                    targets: 0,
                    className: 'control',
                    defaultContent: '',
                },
                {
                    targets: 1,
                    data: 'name'
                },
                {
                    targets: 2,
                    data: 'manufacturer'
                },
                {
                    targets: 3,
                    data: 'modelNo'
                },
                {
                    targets: 4,
                    data: 'productCategoryName',
                    render: function (categoryName) {
                        return categoryName || '-';
                    }
                },
                {
                    targets: 5,
                    data: 'price',
                    render: function (price) {
                        return ' + price.toFixed(2);
                    }
                },
                {
                    targets: 6,
                    data: 'isActive',
                    render: function (isActive) {
                        var badgeClass = isActive ? 'success' : 'secondary';
                        var text = isActive ? app.localize('Active') : app.localize('Inactive');
                        return '<span class="badge bg-' + badgeClass + '">' + text + '</span>';
                    }
                },
                {
                    targets: 7,
                    data: null,
                    orderable: false,
                    render: function (data) {
                        var actions = [
                            '<button class="btn btn-sm btn-info view-product" data-product-id="' + data.id + '" data-bs-toggle="tooltip" title="' + app.localize('View') + '">',
                            '   <i class="fa fa-eye"></i>',
                            '</button>'
                        ];

                        if (_permissions.edit) {
                            actions.push(
                                '<button class="btn btn-sm btn-primary edit-product ms-1" data-product-id="' + data.id + '" data-bs-toggle="tooltip" title="' + app.localize('Edit') + '">',
                                '   <i class="fa fa-edit"></i>',
                                '</button>'
                            );
                        }

                        if (_permissions.delete) {
                            actions.push(
                                '<button class="btn btn-sm btn-danger delete-product ms-1" data-product-id="' + data.id + '" data-product-name="' + data.name + '" data-bs-toggle="tooltip" title="' + app.localize('Delete') + '">',
                                '   <i class="fa fa-trash"></i>',
                                '</button>'
                            );
                        }

                        return actions.join('');
                    }
                }
            ]
        });

        function getProducts() {
            dataTable.ajax.reload();
        }

        function deleteProduct(productId, productName) {
            abp.message.confirm(
                abp.utils.formatString(app.localize('AreYouSureWantToDelete'), productName),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _productsService.delete({
                            id: productId
                        }).done(function () {
                            getProducts();
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                    }
                }
            );
        }

        $('#CreateNewProductButton').click(function () {
            _createOrEditModal.open();
        });

        $(document).on('click', '.edit-product', function () {
            var productId = $(this).data('product-id');
            _createOrEditModal.open({ id: productId });
        });

        $(document).on('click', '.delete-product', function () {
            var productId = $(this).data('product-id');
            var productName = $(this).data('product-name');
            deleteProduct(productId, productName);
        });

        $(document).on('click', '.view-product', function () {
            var productId = $(this).data('product-id');
            _viewProductModal.open({ id: productId });
        });

        abp.event.on('app.createOrEditProductModalSaved', function () {
            getProducts();
        });
    });
})(jQuery);