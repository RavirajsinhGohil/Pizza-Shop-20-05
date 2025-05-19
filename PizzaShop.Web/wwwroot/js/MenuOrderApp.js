let selectedCategoryId = null;

function loadItems(categoryId = null, searchText = "") {
    selectedCategoryId = categoryId;

    $.ajax({
        url: "/OrderApp/MenuMenu",
        type: 'GET',
        data: { categoryId, searchText },
        success: function (result) {
            $('#itemList').html(result);

            // Update active category highlight
            $('.category-link').removeClass('active');
            $(`.category-link[data-category-id="${categoryId}"]`).addClass('active');
        }
    });
}

$(document).ready(function () {
    // Remove padding from the tabContent
    $('#tab-content').removeClass('p-4');

    // Search input listener
    $('#SearchItems').on('keyup', function () {
        const searchText = $(this).val();
        loadItems(selectedCategoryId, searchText);
    });

    DefaultShowActiveCategory();
    // Click category links with data-category-id attribute
    $(document).on('click', '.category-link', function () {
        const categoryIdAttr = $(this).data('category-id');
        const categoryId = categoryIdAttr === "all" ? null : parseInt(categoryIdAttr);
        loadItems(categoryId, $('#SearchItems').val());
    });

    const urlParams = new URLSearchParams(window.location.search);
    const orderId = urlParams.get('orderId');

    if (!orderId) {
        // When modal opens, hide the Add To Order button if no orderId
        const itemModal = document.getElementById('ItemsDetails');
        itemModal.addEventListener('show.bs.modal', function () {
            const addToOrderBtn = document.getElementById('addToOrderBtn');
            if (addToOrderBtn) {
                addToOrderBtn.style.display = 'none';
            }
        });
    }
});

function toggleFavorite(itemId, icon) {
    event.stopPropagation();
    $.post("/OrderApp/ToggleFavorite", { itemId }, function () {
        const iconEl = $(icon);
        iconEl.toggleClass('bi-heart bi-heart-fill text-primary');

        // Refresh list if in favorites view
        if (selectedCategoryId === -1) {
            loadItems(-1, $('#SearchItems').val());
        }
    });
}

function DefaultShowActiveCategory() {
    const categoryId = 0;
    loadItems(categoryId, $('#SearchItems').val());
}

//Item modifierGroup modal data load
function loadModifiers(itemId) {
    $.get(`/OrderApp/GetItemModifiers?itemId=${itemId}`, function (data) {
        $('#ItemsDetails .modal-content').html(data);
        $('#ItemsDetails').modal('show');
    });
}

$(document).on("click", ".option-card", function () {
    const $card = $(this);
    const $group = $card.closest(".modifier-group");
    const max = parseInt($group.data("max")) || 0;
    const min = parseInt($group.data("min")) || 0;

    const selected = $group.find(".option-card.selected");

    if ($card.hasClass("selected")) {
        if (selected.length > min) {
            $card.removeClass("selected");
        } else {
            toastr.warning(`You must select at least ${min} option(s).`);
        }
    } else {
        if (max === 0 || selected.length < max) {
            $card.addClass("selected");
        } else {
            toastr.warning(`You can select up to ${max} option(s).`);
        }
    }
});


//for right side box of order
// -- cart registry { key : { index , qty } } 
const cartRegistry = new Map();   // key - { index , qty }
let itemIndex = 0;

// helper to build unique key
function buildKey(name, mods) {
    const ids = mods.map(m => m.ModifierId).sort((a, b) => a - b).join('_');
    return `${name}__${ids}`;          // e.g.  "Margherita__3_7"
}

$(document).on('click', '#addToOrderBtn', function () {
    // index++;

    /* 1. Collect item & selected modifiers  */
    const selectedModifiers = [];

    $('#ItemsDetails .row').each(function () {
        $(this).find('.option-card.selected').each(function () {
            selectedModifiers.push({
                ModifierId: $(this).data('id'),
                ModifierName: $(this).find('.fw-semibold').text(),
                Rate: parseFloat($(this).find('.text-muted').text()
                    .replace(/[^\d.]/g, ''))
            });
        });
    });

    const itemName = $('#ItemName').val();
    const basePrice = parseFloat($('#ItemRate').val());
    const qty = parseInt($('#itemQuantity').val()) || 1;
    const itemId = $("#ItemId").val();

    // const Instruction = $("#Instruction").val()

    /* 2. Build key & decide -- */
    const key = buildKey(itemName, selectedModifiers);

    if (cartRegistry.has(key)) {
        //  same combo already in cart → bump quantity by 1 
        const rowInfo = cartRegistry.get(key);

        // Only increase by 1, don't add the full qty
        // rowInfo.qty = 1;
        rowInfo.qty += 1;

        // Ensure the updated qty doesn't exceed max quantity (Model.Quantity)
        if (rowInfo.qty > rowInfo.maxQuantity) {
            rowInfo.qty = rowInfo.maxQuantity;
        }

        // Update qty input in DOM
        $(`[data-key='${key}'] .qty-input`).val(rowInfo.qty);

        updateTotals();
        $('#ItemsDetails').modal('hide');
        return;
    }

    /* 3. Brand‑new row → ask server for html  */
    $.ajax({
        url: '/OrderApp/RenderOrderItemRow',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            itemId: itemId,
            ItemName: itemName,
            BasePrice: basePrice,
            Quantity: 1,
            MaxQuantity: qty,
            Index: itemIndex,
            SelectedModifiers: selectedModifiers,
            OrderedQuantity: 1,
        }),
        success: function (html) {
            // tag wrapper with the key so we can find it later
            const $row = $(html).attr('data-key', key);
            $('#orderItemsContainer').append($row);
            // $('#orderItemsContainer').html(html);

            // remember it
            cartRegistry.set(key, { index: itemIndex, qty: qty });

            itemIndex++;
            updateTotals();
            $('#ItemsDetails').modal('hide');
        },
        error: function () {
            toastr.error("Something went wrong while adding the item.");
        }
    });
});



// Delete item in order
$(document).on('click', '.delete-item', function () {
    $(this).closest('.order-item').remove();
    updateTotals();
});


function updateTotals() {
    let subtotal = 0;

    // Calculate subtotal from all cart rows
    $('#orderItemsContainer .cart-row').each(function () {
        const qty = parseInt($(this).find('.qty-input').val()) || 1;
        const unit = parseFloat($(this).data('price')) || 0;
        const mod = parseFloat($(this).data('modprice')) || 0;
        subtotal += (unit + mod) * qty;
    });

    // Set Subtotal
    $('#subtotal').text(`₹${subtotal.toFixed(2)}`);

    let totalTaxes = 0;

    // Add default taxes (those without checkbox)
    // Add default taxes (those without checkbox)
    $('.default-tax-amount').each(function () {
        const $tax = $(this); // wrap `this` with jQuery

        if ($tax.hasClass('taxPercentage')) {
            const taxPercentage = parseFloat($tax.text().replace('%', '')) || 0;
            const taxAmount = (subtotal * taxPercentage) / 100;
            $tax.text(`₹${taxAmount.toFixed(2)}`);
            totalTaxes += taxAmount;
        } else {
            const taxAmount = parseFloat($tax.text().replace('₹', '')) || 0;
            totalTaxes += taxAmount;
        }
    });


    // Add optional taxes (those with checkbox only if checked)
    $('.tax-checkbox').each(function () {
        if ($(this).is(':checked')) {
            const taxValue = parseFloat($(this).val()) || 0;
            totalTaxes += taxValue;
        }
    });

    // Set GST (optional: you can show all taxes combined under GST or rename it to Taxes)
    $('#gst').text(`₹${totalTaxes.toFixed(2)}`);

    // Final Total
    const total = subtotal + totalTaxes;
    $('#total').text(`₹${total.toFixed(2)}`);
}

/**
 * Removes the element that matches #order-item-row-{index} from the DOM,
 * then calls your endpoint to delete it server‑side.
 */
function removeItem(index) {
    const row = document.getElementById(`order-item-row-${index}`);
    if (!row) return;

    const readyQty = parseInt(row.getAttribute("data-ready-qty")) || 0;
    if (readyQty > 0) {
        toastr.warning("This item has already been prepared and cannot be deleted.");
        return;
    }

    // Optimistic UI removal
    row.remove();
    updateTotals();
}


// validation of edit customer modal
// Bootstrap validation style
(() => {
    document
        .querySelector('#editCustomerForm')
        .addEventListener('submit', async e => {
            e.preventDefault();
            const form = e.target;
            if (!form.checkValidity()) {          // built‑in validation
                form.classList.add('was-validated');
                return;
            }
            const fd = new FormData(form);

            const r = await fetch('/OrderApp/UpdateCustomer', {
                method: 'POST',
                body: fd
            });

            if (r.ok) {
                bootstrap.Modal.getInstance('#editCustomerInMenuModal').hide();
                toastr.success("Customer Edited Successfully!");
                // TODO: refresh summary header with new name / phone
            } else {
                toastr.error('Update failed');
            }
        });
})();

// called by the v‑card button
async function OpenEditCustomerModal(orderId) {

    new bootstrap.Modal('#editCustomerInMenuModal').show();
    const r = await fetch(`/OrderApp/GetOrderCustomer?orderId=${orderId}`);
    if (!r.ok) { toastr.error('Could not load customer'); return; }
    const c = await r.json();

    // fill fields
    $('#editOrderId').val(orderId);
    $('#editCustomerId').val(c.customerId);
    $('#editName').val(c.name);
    $('#editPhone').val(c.phone);
    $('#editEmail').val(c.email);
    $('#editPersons').val(c.noOfPersons);

    // reset validation state
    $('#editCustomerForm').removeClass('was-validated');

}

// Change the quantity for a given row
function changeQuantity(index, delta, maxQuantity) {
    const qtyInput = document.querySelector(`#quantityInput_${index}`);
    let qty = parseInt(qtyInput.value) || 0;

    if (delta == 1) {
        if (qty < maxQuantity) {
            qty += delta;
        }
        else {
            toastr.warning(`You can select only ${maxQuantity} items`);
        }
    }
    else {
        if (qty > 1) {
            qty += delta;
        }
        else {
            toastr.warning("You must select at least 1 item");
        }
    }


    const row = document.getElementById(`order-item-row-${index}`);
    if (!row) return;

    const readyQty = parseInt(row.getAttribute("data-ready-qty")) || 0;

    /* clamp between 1 and maxQuantity */
    if (qty < 1) {
        qty = 1;
    }
    else if (qty < readyQty) {
        qty = readyQty;
        toastr.warning(`Here ${readyQty} items are Ready`);
    }

    /* set the new value … */
    qtyInput.value = qty;

    /* …and trigger the event so totals update */
    qtyInput.dispatchEvent(new Event('input', { bubbles: true }));
}


/* -------- delegated listener for any .qty-input ------- */
$(document).on('input', '.qty-input', function () {
    const $row = $(this).closest('.cart-row');
    const qty = parseInt(this.value) || 0;
    const unit = parseFloat($row.data('price')) || 0;
    const mod = parseFloat($row.data('modprice')) || 0;

    /* update per‑row display */
    // $row.find('.row-total')
    //     .html(₹${((unit + mod) * qty).toFixed(2)});

    /* update overall summary */
    updateTotals();
});


//Open Scanner in Order App Menu
function OpenScannerModal() {
    $("#ModalForMenuScanner").modal("show");
}

// open Order instruction modal
function openOrderInstructionModal(orderDetailId) {
    // Set the hidden input
    $('#orderDetailId').val(orderDetailId);
    // $('#ModalForOrderInstruction').modal('show');

    // Fetch existing comment
    $.ajax({
        url: '/OrderApp/GetAdminComment',
        type: 'GET',
        data: { id: orderDetailId },
        success: function (response) {
            $('#adminComment').val(response.adminComment);
            $('#ModalForOrderInstruction').modal('show');
        }
    });
}

$('#orderInstructionForm').submit(function (e) {
    e.preventDefault();

    var formData = {
        OrderDetailId: $('#orderDetailId').val(),
        AdminComment: $('#adminComment').val()
    };

    $.ajax({
        url: '/OrderApp/SaveAdminComment',
        type: 'POST',
        data: formData,
        success: function () {
            $('#ModalForOrderInstruction').modal('hide');
            toastr.success("Instruction is Saved!")
        }
    });
});

//itemWise Comments
function openItemInstructionModal(orderId, itemId) {
    debugger;
    $('#ItemDetailId').val(orderId);
    $('#instructionError').hide();
    let hiddenOrderId = $("#hiddenOrderId").val();

    $('#ModalForItemInstruction').modal('show');

    $.ajax({
        url: '/OrderApp/GetItemInstruction',
        type: 'GET',
        data: { orderId: orderId, itemId: itemId },
        success: function (response) {
            $('#Instruction').val(response.instruction);
            $('#ModalForItemInstruction').modal('show');
        },
        error: function () {
            toastr.error('Failed to load instruction.');
        }
    });
}

$('#ItemInstructionForm').submit(function (e) {
    e.preventDefault();

    //Check Order is saved or not

    var instructionText = $('#Instruction').val().trim();
    $('#instructionError').hide();

    if (instructionText === '') {
        $('#instructionError').text('Instruction is required.').show();
        return;
    }

    var formData = {
        orderId: $('#hiddenOrderId').val(),
        itemId: $('#ItemDetailId').val(),
        Instruction: instructionText
    };

    $.ajax({
        url: '/OrderApp/SaveItemInstruction',
        type: 'POST',
        data: formData,
        success: function () {
            $('#ModalForItemInstruction').modal('hide');
            toastr.success("Item Instruction is Updated!")
        },
        error: function (xhr) {
            if (xhr.status === 400) {
                $('#instructionError').text('Instruction is required.').show();
            } else {
                toastr.error('Failed to save instruction.');
            }
        }
    });
});


//For Save Order
function saveOrder() {
    var orderItems = [];
    var selectedTaxes = [];

    var urlParams = new URLSearchParams(window.location.search);
    var orderId = $("#hiddenOrderId").val();

    $(".items").each(function (index) {
        var itemId = $(this).data("item-id");

        var itemName = $(this).find(".item-name").text();

        var quantity = parseInt($(this).find(".qty-input").val());

        var availableQty = parseInt($(this).find(".available-qty").val());

        var rate = parseFloat($(this).find(".cart-row").data("price"));

        // var instruction = $(this).find(".item-instruction").val();

        var selectedModifiers = [];

        $(this).find(".modifier").each(function () {
            selectedModifiers.push({
                modifierId: $(this).data("modifier-id"),
                modifierName: $(this).data("modifier-name"),
                rate: parseFloat($(this).data("modifier-price"))
            });
        });

        orderItems.push({
            itemId: itemId,
            itemName: itemName,
            quantity: quantity,
            AvailableQuantity: availableQty,
            rate: rate,
            // instruction: instruction,
            selectedModifiers: selectedModifiers
        });
    });

    // Get Default Taxes
    $(".defaultTax").each(function () {
        // var taxAmount = parseFloat($(this).text().replace('₹', '')) || 0;
        var taxId = parseInt($(this).data("tax-id"));

        // if (taxAmount > 0) {
        selectedTaxes.push(taxId);
        // }
    });

    $(".tax-checkbox:checked").each(function () {
        selectedTaxes.push(parseInt($(this).val()));
    });

    $.ajax({
        type: "POST",
        url: "/OrderApp/SaveOrderItemsAsync",
        contentType: "application/json",
        data: JSON.stringify({
            orderId: orderId,
            orderItems: orderItems,
            selectedTaxIds: selectedTaxes
        }),
        success: function (response) {
            toastr.success("Order saved successfully!");
        },
        error: function (xhr, status, error) {
            toastr.error(xhr.responseText);
        }
    });
}

// Complete Order Ajax call
function completeOrder() {
    var orderId = $("#hiddenOrderId").val();

    $.ajax({
        type: "POST",
        url: "/OrderApp/CompleteOrder",
        data: { orderId: orderId },
        success: function (response) {
            if (response.success) {
                toastr.success(response.message);
                $('#completeOrderModal').modal('hide');

                // $('#customerReviewModal').modal('show');
            }
            else {
                toastr.error(response.message);
                $('#completeOrderModal').modal('hide');
            }
        },
        error: function (xhr) {
            toastr.error(xhr.responseText);
        }
    });
}

function CancelOrder() {
    var orderId = $("#hiddenOrderId").val();

    $.ajax({
        type: "POST",
        url: "/OrderApp/CancelOrder",
        data: { orderId: orderId },
        success: function (response) {
            if (response.success) {
                toastr.success(response.message);
                // $('#cancelOrderModal').modal('hide');
            }
            else {
                toastr.error(response.message);
                // $('#cancelOrderModal').modal('hide');
            }
        },
        error: function (xhr) {
            toastr.error(xhr.responseText);
        }
    });
}

// fetch data after save
$(document).ready(function () {
    var activeOrderId = $("#hiddenOrderId").val();
    if (activeOrderId) {
        loadOrderItems(activeOrderId);
    }
});

function loadOrderItems(orderId) {
    $.ajax({
        url: '/OrderApp/GetOrderDetailsById/' + orderId,
        type: 'GET',
        success: function (orderItems) {
            $('#orderItemsContainer').empty();

            const renderPromises = orderItems.map(function (item, index) {
                return renderOrderItem(item, index);
            });
            itemIndex = orderItems.length + 1;

            Promise.all(renderPromises).then(function () {
                updateTotals();
            });
        },
        error: function () {
            console.error('Failed to load order items.');
        }
    });
}

function renderOrderItem(item, index) {
    return $.ajax({
        url: '/OrderApp/RenderOrderItemRow',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            ItemId: item.itemId,
            OrderId: item.id,
            Index: index,
            ItemName: item.itemName,
            Instruction: item.instruction || "",
            BasePrice: item.rate,
            Quantity: item.Quantity,
            MaxQuantity: item.maxQuantity,
            SelectedModifiers: item.modifiers || []
        }),
        success: function (partialHtml) {
            $('#orderItemsContainer').append(partialHtml);
        },
        error: function () {
            console.error('Failed to render order item.');
        }
    });
}

function quantityChange(e) {
    var input = $(e.target);
    var maxQuantity = input.attr("data-maxQuantity");
    var currentQuantity = input.attr("data-currentValue");
    var updatedQuantity = input.val();

    if (updatedQuantity > 0) {
        if (+updatedQuantity > +maxQuantity) {
            toastr.warning(`You can select only ${maxQuantity} items`);
            $(input).val(currentQuantity);
        } else {
            $(input).attr("data-currentValue", updatedQuantity);
        }
    }
    else {
        if (+updatedQuantity < 1) {
            toastr.warning("You must select at least 1 item");
            $(input).val(1);
        } else {
            $(input).attr("data-currentValue", updatedQuantity);
        }
    }

}