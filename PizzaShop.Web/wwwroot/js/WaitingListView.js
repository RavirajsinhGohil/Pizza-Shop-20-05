function fetchWaitingTokens(sectionId) {
    $.ajax({
        url: '/OrderApp/LoadWaitingListCards',
        type: 'GET',
        data: { sectionId: sectionId },
        success: function (data) {
            $('#waitingListContainer').html(data);
            initLiveTimers();
        },
        error: function (xhr, status, error) {
            console.error('Error fetching waiting tokens:', error);
        }
    });
}

$(".sectionBtn").on("click", function () {
    var sectionId = $(this).data("id");
    fetchWaitingTokens(sectionId);
});

// $('edit-token').on('click', function () {
//     debugger
//     console.log("Edit token clicked");
//     const tokenId = $(this).data("token-id");
// });

document.addEventListener("DOMContentLoaded", function () {
    document.body.addEventListener("click", function (event) {
        if (event.target.closest(".edit-token")) {
            const tokenId = event.target.closest(".edit-token").getAttribute("data-token-id");

            $.ajax({
                url: '/OrderApp/GetWaitingTokenById',
                type: 'GET',
                data: { tokenId: tokenId },
                success: function (response) {
                    // $('#waitingTokenModal').modal('show');
                    var MyModal = new bootstrap.Modal(document.getElementById('editWaitingTokenModal'));
                    MyModal.show();
                    $("#editTokenId").val(tokenId);
                    $('#waitingTokenForm').attr('action', '/OrderApp/UpdateWaitingToken');
                    $('#editTokenSectionId').val(response.data.SectionId123);
                    $("#editTokenCustomerId").val(response.data.customerId);
                    $('#editTokenEmail').val(response.data.email);
                    $('#editTokenName').val(response.data.name);
                    $('#editTokenPhone').val(response.data.phone);
                    $('#editTokenPersons').val(response.data.totalPersons);
                    $('#editTokenSection').val(response.data.sectionId123);
                },
                error: function (xhr, status, error) {
                    console.error('Error fetching waiting token:', error);
                }
            });
        }
    });
});

$("#deleteTokenBtn").on("click", function () {
    const tokenId = $(this).data("token-id");

    const deleteTokenLink = $("#deleteTokenLink");
    deleteTokenLink.attr("href", "/OrderApp/DeleteWaitingToken/?tokenId=" + tokenId);
});

function initLiveTimers() {
    $('.updatedLiveTimeToken').each(function () {
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

// get customer by email
$(document).on('blur', '#customerEmailWaitingToken', function () {
    var email = $(this).val();
    if (email) {
        $.ajax({
            url: '/OrderApp/GetCustomerByEmail',
            type: 'GET',
            data: { email: email },
            success: function (data) {
                if (data) {
                    $('#customerNameWaitingToken').val(data.firstname + ' ' + data.lastname);
                    $('#customerPhoneWaitingToken').val(data.phone);
                    // $('#customerTokenIdWaitingToken').val(data.id);
                    toastr.success("Customer details loaded successfully!");
                } else {
                    $('#customerNameWaitingToken, #customerPhoneWaitingToken').val('');

                }
            },
            error: function (xhr, status, error) {
                console.error('Error fetching customer details:', error);
            }
        });
    }
});

// get customer by email for edit waiting token
$(document).on('blur', '#editTokenEmail', function () {
    var email = $(this).val();
    if (email) {
        $.ajax({
            url: '/OrderApp/GetCustomerByEmail',
            type: 'GET',
            data: { email: email },
            success: function (data) {
                if (data) {
                    $('#editTokenName').val(data.firstname + ' ' + data.lastname);
                    $('#editTokenPhone').val(data.phone);
                    toastr.success("Customer details loaded successfully!");
                } else {
                    $('#editTokenName, #editTokenPhone').val('');
                }
            },
            error: function (xhr, status, error) {
                console.error('Error fetching customer details:', error);
            }
        });
    }
});