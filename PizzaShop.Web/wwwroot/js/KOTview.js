


function loadKOTCards(categoryId, inProgress) {
    const url = `/OrderApp/LoadKOTCards?categoryId=${encodeURIComponent(categoryId)}&inProgress=${encodeURIComponent(inProgress)}`;

    $.ajax({
        url: url,
        type: 'GET',
        success: function (data) {
            $('#kotTabContent').html(data);
            // $('#inProgressBtn').removeClass('custom-btn-bg text-white').addClass('custom-blue-border custom-blue-color');
            if (inProgress) {
                $('#readyBtn').removeClass('custom-btn-bg text-white').addClass('custom-blue-border custom-blue-color');
                $(this).removeClass('custom-blue-border custom-blue-color').addClass('custom-btn-bg text-white active');
                $('#markInProgressBtn').addClass('d-none');
                $('#markReadyBtn').removeClass('d-none');
            }
            else {
                $('#inProgressBtn').removeClass('custom-btn-bg text-white').addClass('custom-blue-border custom-blue-color');
                $('#readyBtn').removeClass('custom-blue-border custom-blue-color').addClass('custom-btn-bg text-white active');
                $('#markReadyBtn').addClass('d-none');
                $('#markInProgressBtn').removeClass('d-none');
            }
            initLiveTimers();
        },
        error: function () {
            $('#kotTabContent').empty();
        }
    });
}

function setupKOTTabs() {
    const initialCategoryId = $('#kotTabs button.active').data('id');
    loadKOTCards(initialCategoryId, true);

    $('#kotTabs').on('click', 'button', function () {
        $('#kotTabs button').removeClass('active');
        $(this).addClass('active');
        $(this).css("border", "2px solid #007bff !important");
        // $(this).style.border = "2px solid #007bff !important";
        // var categoryName = $(this).data('category-name');
        // $("#categoryName").text(categoryName);

        const categoryId = $(this).data('id');
        loadKOTCards(categoryId, true);
    });
}

let currentTab = 'inprogress';
function setupKOTCardButtons(categoryId) {
    $('#readyBtn').on('click', function () {
        currentTab = 'ready';
        loadKOTCards(categoryId, false);

        let tab = $('#inProgressModal').attr('data-tab');
    });

    $('#inProgressBtn').on('click', function () {
        currentTab = 'inprogress';
        loadKOTCards(categoryId, true);

        let tab = $('#inProgressModal').attr('data-tab');
        if (tab === 'inprogress') {
        }

        // $('#readyBtn').removeClass('custom-btn-bg text-white').addClass('custom-blue-border custom-blue-color');
        // $(this).removeClass('custom-blue-border custom-blue-color').addClass('custom-btn-bg text-white active');
    });
}

function increment(btn) {
    const input = btn.parentNode.querySelector('input');
    input.stepUp();
}

function decrement(btn) {
    const input = btn.parentNode.querySelector('input');
    if (parseInt(input.value) > parseInt(input.min || 1)) {
        input.stepDown();
    }
}

var orderId;
$(document).on('click', '.openOrderModal', function () {
    const orderData = $(this).data('order');
    orderId = orderData.orderId;
    if (!orderData) return;

    $('#inProgressModal .modal-title').text("Order Id: #" + orderData.orderId);

    let rowsHtml = '';
    orderData.items.forEach(item => {
        let modifiersHtml = '';
        item.modifiers.forEach(mod => {
            modifiersHtml += `<li class="text-secondary  small">${mod.modifierName}</li>`;
        });

        rowsHtml += `
            <tr class="d-flex justify-content-between">
                <td class="d-flex flex-column">
                    <div>
                        <input class="form-check-input ItemsCheck" type="checkbox" data-order-id="${orderData.orderId}" data-id="${item.itemId}" />
                        ${item.itemName}
                    </div>
                    ${modifiersHtml}
                </td>
                <td class="input-group quantity-control" style="width: 120px;">
                    <button class="btn btn-sm" type="button" onclick="decrement(this)">-</button>
                    <input type="number" class="form-control p-0 border-none bg-white text-center" value="0" min="1" max="${item.inProgressQuantity}"  />
                    <button class="btn btn-sm" type="button" onclick="increment(this)">+</button>
                </td>
            </tr>
        `;
    });

    $('#orderDetailsModalBody').html(rowsHtml);

});

$(document).on('click', '#markReadyBtn', function () {
    const selectedItems = $(".ItemsCheck:checked").map(function () {
        const itemId = $(this).data('id'); // Get the item ID
        const quantity = $(this).closest('tr').find('input[type="number"]').val(); // Get the quantity
        return { id: itemId, quantity: parseInt(quantity) }; // Return an object with ID and quantity
    }).get();

    if (selectedItems.length === 0) {
        toastr.error("Please select at least one item to Mark as Ready.");
        return;
    }

    debugger;

    $.ajax({
        url: `/OrderApp/MarkAsReady?orderId=${orderId}`,
        type: 'POST',
        data: JSON.stringify({
            orderId: orderId,
            OrderDetailsIds: selectedItems
        }),
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
                // Close the modal
                $('#inProgressModal').modal('hide');
                loadKOTCards($('#kotTabs button.active').data('id'), false);
            } else {
                console.error("Failed to mark items as ready.");
            }
        },
        error: function () {
            console.error("Error marking items as ready.");
        }
    });
});

$(document).on('click', '#markInProgressBtn', function () {
    const selectedItems = $(".ItemsCheck:checked").map(function () {
        const itemId = $(this).data('id'); // Get the item ID
        const quantity = $(this).closest('tr').find('input[type="number"]').val(); // Get the quantity
        return { id: itemId, quantity: parseInt(quantity) }; // Return an object with ID and quantity
    }).get();

    if (selectedItems.length === 0) {
        toastr.error("Please select at least one item to Mark as In Progress.");
        return;
    }

    $.ajax({
        url: '/OrderApp/MarkAsInProgress',
        type: 'POST',
        data: JSON.stringify({
            orderId: orderId,
            orderDetailsIds: selectedItems
        }),
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
                // Close the modal
                $('#inProgressModal').modal('hide');
                loadKOTCards($('#kotTabs button.active').data('id'), true);
            } else {
                console.error("Failed to mark items as in progress.");
            }
        },
        error: function () {
            console.error("Error marking items as in progress.");
        }
    });
});

function initLiveTimers() {
    $('.updatedLiveTimeKOT').each(function () {
        var $this = $(this);
        var startTimeStrr = $this.data('starttime');

        // Ensure startTime is a valid date
        var startTime = new Date(startTimeStrr);

        var updateTimer = function () {
            var now = new Date();
            var diff = now - startTime;
            var totalSeconds = Math.floor(diff / 1000);
            var days = Math.floor(totalSeconds / (3600 * 24));
            var hours = Math.floor((totalSeconds % (3600 * 24)) / 3600);
            var minutes = Math.floor((totalSeconds % 3600) / 60);
            var seconds = totalSeconds % 60;

            $this.text(days + " days " + hours + " hours " + minutes + " min " + seconds + " sec");
        };
        updateTimer();
        setInterval(updateTimer, 1000);
    });
}


