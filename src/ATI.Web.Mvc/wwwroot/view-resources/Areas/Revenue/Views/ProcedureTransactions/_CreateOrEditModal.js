(function ($) {
    app.modals.CreateOrEditProcedureTransactionModal = function () {
        var _transactionsService = abp.services.app.procedureTransactions;
        var _modalManager;
        var _$form = null;
        var _$linesBody = null;
        var _products = [];

        this.init = function (modalManager) {
            _modalManager = modalManager;
            var $modal = _modalManager.getModal();

            _$form = $modal.find('form[name=ProcedureTransactionCreateOrEditForm]');
            _$form.validate();
            _$linesBody = $modal.find('#DeviceLinesBody');

            _products = readJson($modal.find('#DeviceLineProducts'), []);
            var existing = readJson($modal.find('#DeviceLineExisting'), []);

            bindEvents($modal);

            // An existing case shows its devices; a new one starts with one blank row so
            // there is somewhere to type.
            if (existing.length) {
                existing.forEach(function (line) { addLine(line); });
            } else {
                addLine();
            }

            recalculateAll();

            $modal.find('.save-button').click(function (e) {
                e.preventDefault();
                save();
            });
        };

        function readJson($el, fallback) {
            if (!$el.length) {
                return fallback;
            }
            try {
                return JSON.parse($el.text()) || fallback;
            } catch (e) {
                return fallback;
            }
        }

        function bindEvents($modal) {
            $modal.find('#AddDeviceLineButton').click(function (e) {
                e.preventDefault();
                addLine();
            });

            $modal.find('#RecalculateTotalButton').click(function (e) {
                e.preventDefault();
                var $total = $modal.find('#TotalAmount');
                $total.removeData('overridden');
                $total.val(sumLines().toFixed(2));
            });

            // The physician determines the hospital, and the hospital determines each
            // device's contracted price, so re-price every line when it changes.
            $modal.find('#PhysicianId').on('change', function () {
                loadPhysicianFacility($(this).val());
            });

            $modal.find('#HospitalId, #ProcedureDate').on('change', function () {
                repriceAllLines();
            });

            $modal.find('input[name=ImplantType]').on('change', function () {
                flagMismatchedLines();
            });

            $modal.find('#TotalAmount').on('input', function () {
                $(this).data('overridden', true);
            });

            // Delegated, because rows come and go.
            _$linesBody.on('change', '.device-product', function () {
                repriceLine($(this).closest('tr'), true);
            });

            _$linesBody.on('input change', '.device-quantity, .device-unit-price', function () {
                updateLineTotal($(this).closest('tr'));
                recalculateAll();
            });

            _$linesBody.on('click', '.remove-device', function (e) {
                e.preventDefault();
                $(this).closest('tr').remove();
                recalculateAll();
            });
        }

        function addLine(line) {
            var options = ['<option value=""></option>'];

            _products.forEach(function (p) {
                var selected = line && line.productId === p.id ? ' selected' : '';
                var implant = (p.implantType === null || p.implantType === undefined) ? '' : p.implantType;
                options.push('<option value="' + p.id + '" data-implant-type="' + implant + '"' + selected + '></option>');
            });

            var html = '<tr>'
                + '<td>'
                + '<input type="hidden" class="device-line-id" value="' + (line && line.id ? line.id : 0) + '" />'
                + '<select class="form-select form-select-sm device-product">' + options.join('') + '</select>'
                + '<div class="text-danger small device-warning" style="display:none;"></div>'
                + '</td>'
                + '<td><input type="number" class="form-control form-control-sm device-quantity" min="1" value="'
                + (line && line.quantity ? line.quantity : 1) + '" /></td>'
                + '<td><input type="number" class="form-control form-control-sm device-unit-price" step="0.01" min="0" value="'
                + (line && line.unitPrice ? line.unitPrice : 0) + '" /></td>'
                + '<td class="device-line-total text-end">$0.00</td>'
                + '<td class="text-end"><button type="button" class="btn btn-sm btn-icon btn-light-danger remove-device">'
                + '<i class="fa fa-times"></i></button></td>'
                + '</tr>';

            var $row = $(html);

            // Option labels are set with text() so a product name cannot inject markup.
            $row.find('.device-product option').each(function (i) {
                $(this).text(i === 0 ? app.localize('SelectProduct') : _products[i - 1].name);
            });

            _$linesBody.append($row);

            if (line && line.productId && !line.unitPrice) {
                repriceLine($row, false);
            }

            updateLineTotal($row);
            flagMismatchedLines();
            recalculateAll();
        }

        function repriceAllLines() {
            _$linesBody.find('tr').each(function () {
                repriceLine($(this), false);
            });
        }

        // Asks the server for the hospital's contracted price for this device, leaving a
        // manually entered price alone unless the product itself changed.
        function repriceLine($row, productChanged) {
            var hospitalId = $('#HospitalId').val();
            var productId = $row.find('.device-product').val();

            flagMismatchedLines();

            if (!productId || !hospitalId) {
                updateLineTotal($row);
                recalculateAll();
                return;
            }

            _transactionsService
                .getProductPriceByHospital(parseInt(hospitalId, 10), parseInt(productId, 10))
                .done(function (price) {
                    var $price = $row.find('.device-unit-price');
                    var current = parseFloat($price.val()) || 0;

                    if (productChanged || current === 0) {
                        $price.val((price || 0).toFixed(2));
                    }

                    updateLineTotal($row);
                    recalculateAll();
                })
                .fail(function () {
                    abp.notify.error(app.localize('FailedToLoadPrice'));
                });
        }

        // Every device on a case must match the case's implant type. The server enforces
        // it; this shows why before the user tries to save.
        function flagMismatchedLines() {
            var caseType = parseInt($('input[name=ImplantType]:checked').val(), 10);

            _$linesBody.find('tr').each(function () {
                var $row = $(this);
                var raw = $row.find('.device-product option:selected').attr('data-implant-type');
                var $warning = $row.find('.device-warning');

                if (!raw || isNaN(caseType)) {
                    $warning.hide().text('');
                    return;
                }

                if (parseInt(raw, 10) !== caseType) {
                    $warning.text(app.localize('DeviceImplantTypeMismatch')).show();
                } else {
                    $warning.hide().text('');
                }
            });
        }

        function updateLineTotal($row) {
            var quantity = parseInt($row.find('.device-quantity').val(), 10) || 0;
            var unitPrice = parseFloat($row.find('.device-unit-price').val()) || 0;
            var total = quantity * unitPrice;

            $row.find('.device-line-total').text('$' + total.toFixed(2));
            $row.data('line-total', total);
        }

        function sumLines() {
            var sum = 0;
            _$linesBody.find('tr').each(function () {
                sum += parseFloat($(this).data('line-total')) || 0;
            });
            return sum;
        }

        function recalculateAll() {
            var $modal = _modalManager.getModal();
            $modal.find('#DeviceLinesEmpty').toggle(_$linesBody.find('tr').length === 0);

            // Keep the total in step unless it was deliberately overridden.
            var $total = $modal.find('#TotalAmount');
            if (!$total.data('overridden')) {
                $total.val(sumLines().toFixed(2));
            }
        }

        function loadPhysicianFacility(physicianId) {
            if (!physicianId) {
                return;
            }

            $.ajax({
                url: abp.appPath + 'Revenue/ProcedureTransactions/GetPhysicianFacility',
                type: 'POST',
                data: JSON.stringify({ physicianId: parseInt(physicianId, 10) }),
                contentType: 'application/json',
                success: function (result) {
                    if (result && result.success && result.facilityId) {
                        $('#HospitalId').val(result.facilityId);
                        repriceAllLines();
                    }
                }
            });
        }

        function collectLines() {
            var lines = [];

            _$linesBody.find('tr').each(function () {
                var $row = $(this);
                var productId = $row.find('.device-product').val();

                if (!productId) {
                    return; // a blank row is simply not submitted
                }

                lines.push({
                    id: parseInt($row.find('.device-line-id').val(), 10) || 0,
                    productId: parseInt(productId, 10),
                    quantity: parseInt($row.find('.device-quantity').val(), 10) || 1,
                    unitPrice: parseFloat($row.find('.device-unit-price').val()) || 0
                });
            });

            return lines;
        }

        function save() {
            if (!_$form.valid()) {
                return;
            }

            var lines = collectLines();

            if (!lines.length) {
                abp.message.warn(app.localize('CaseNeedsAtLeastOneDevice'));
                return;
            }

            var transaction = {
                id: parseInt(_$form.find('input[name=Id]').val(), 10) || 0,
                caseNumber: $('#CaseNumber').val(),
                procedureDate: $('#ProcedureDate').val(),
                hospitalId: $('#HospitalId').val() ? parseInt($('#HospitalId').val(), 10) : null,
                physicianId: parseInt($('#PhysicianId').val(), 10),
                implantType: parseInt($('input[name=ImplantType]:checked').val(), 10),
                status: parseInt($('#Status').val(), 10),
                description: $('#Description').val() || null,
                totalAmount: parseFloat($('#TotalAmount').val()) || 0,
                products: lines
            };

            _modalManager.setBusy(true);
            _transactionsService
                .createOrEdit(transaction)
                .done(function () {
                    abp.notify.info(app.localize('SavedSuccessfully'));
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditProcedureTransactionModalSaved');
                })
                .always(function () {
                    _modalManager.setBusy(false);
                });
        }

        this.save = save;
    };
})(jQuery);
