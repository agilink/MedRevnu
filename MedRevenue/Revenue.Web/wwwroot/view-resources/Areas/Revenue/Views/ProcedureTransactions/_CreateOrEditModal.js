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

            // narrow on open: editing a case shows only its hospital's physicians, keeping
            // the one already recorded.
            var openingHospitalId = $('#HospitalId').val();
            if (openingHospitalId) {
                _syncing = true;
                reloadPhysicians(openingHospitalId, $('#PhysicianId').val())
                    .always(function () { _syncing = false; });
            }

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
                if (_syncing) {
                    return;
                }
                loadPhysicianFacility($(this).val());
            });

            $modal.find('#ProcedureDate').on('change', function () {
                repriceAllLines();
            });

            $modal.find('#HospitalId').on('change', function () {
                if (_syncing) {
                    return;
                }

                _syncing = true;
                reloadPhysicians($(this).val()).always(function () {
                    _syncing = false;
                    repriceAllLines();
                });
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
            var procedureDate = $('#ProcedureDate').val();

            flagMismatchedLines();

            // Only the product is needed to price a line. Previously this bailed out
            // whenever no hospital was chosen, so changing the product produced no price
            // at all; the service falls back to the product's base price when the hospital
            // has no contracted price, or when there is no hospital yet.
            if (!productId) {
                updateLineTotal($row);
                recalculateAll();
                return;
            }

            _transactionsService
                .getEffectiveUnitPrice(
                    hospitalId ? parseInt(hospitalId, 10) : null,
                    parseInt(productId, 10),
                    procedureDate || null)
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

        // Hospital and physician cascade. Choosing a hospital narrows the physician list
        // to that hospital's physicians; choosing a physician fills in their hospital.
        // _syncing stops the two handlers retriggering each other.
        var _syncing = false;

        function reloadPhysicians(hospitalId, keepPhysicianId) {
            // Query string, not a JSON body: a simple int? parameter does not bind from
            // the body, so this always arrived null and returned every physician.
            // abp.ajax, not $.ajax: ABP wraps a JsonResult in an AjaxResponse envelope,
            // so read raw the payload sits at result.result and result.physicians was
            // undefined - the list never narrowed even once the parameter bound.
            return abp.ajax({
                url: abp.appPath + 'Revenue/ProcedureTransactions/GetPhysiciansByHospital'
                     + (hospitalId ? '?hospitalId=' + encodeURIComponent(hospitalId) : ''),
                type: 'GET'
            }).then(function (result) {
                if (!result || !result.success) {
                    return;
                }

                var $physician = $('#PhysicianId');
                var previous = keepPhysicianId || $physician.val();

                $physician.empty().append(
                    $('<option></option>').attr('value', '').text(app.localize('SelectPhysician')));

                var stillValid = false;
                result.physicians.forEach(function (p) {
                    $physician.append($('<option></option>').attr('value', p.id).text(p.name));
                    if (String(p.id) === String(previous)) {
                        stillValid = true;
                    }
                });

                // Keep the chosen physician when they still work at the chosen hospital.
                $physician.val(stillValid ? previous : '');
            });
        }

        function loadPhysicianFacility(physicianId) {
            if (!physicianId) {
                return;
            }

            abp.ajax({
                url: abp.appPath + 'Revenue/ProcedureTransactions/GetPhysicianFacility'
                     + '?physicianId=' + encodeURIComponent(physicianId),
                type: 'GET'
            }).done(function (result) {
                    if (!result || !result.success || !result.facilityId) {
                        return;
                    }

                    // The hospital leads: it is chosen first and narrows this list. Only
                    // fill it in when it was left blank, so picking a physician never
                    // overrides a hospital the user already chose.
                    if (!$('#HospitalId').val()) {
                        _syncing = true;
                        $('#HospitalId').val(result.facilityId);
                        _syncing = false;
                    }

                    repriceAllLines();
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
