$(document).ready(function () {
    $('.assignBtnForSection').on('click', function () {
        var sectionId = $(this).data('section-id');  // Get the section ID from the button
        // var availableSections = @Html.Raw(Json.Serialize(assignModel.AvailableSections));  // Get the available sections
        var availableSections = JSON.parse(document.getElementById('assignModal').dataset.sections);

        // Clear the dropdown options first
        $('#SectionDropdown').empty().append('<option selected disabled>Choose a Section</option>');

        // Populate the dropdown with available sections
        availableSections.forEach(function (section) {
            $('#SectionDropdown').append('<option value="' + section + '">' + section + '</option>');
        });

        // Optionally, set the current section as selected in the dropdown
        if (sectionId) {
            $('#sectionDropdown').val(sectionId);
        }
    });

    $('#waitingTokenModal').on('shown.bs.modal', function (event) {
        const button = event.relatedTarget; // Button that triggered the modal
        const sectionId = button.getAttribute('data-section-id'); // Get section id from data attribute

        // Reset the form
        const form = document.getElementById("waitingTokenForm");
        if (form) {
            form.reset();
            form.classList.remove('waitingTokenValidation');
            form.classList.remove('was-validated');
        }

        // Set the dropdown value in modal
        $('#floatingSection').val(sectionId).change();
    });

    //for click assign button only when available table is selected
    $(".table-card.available-clickable").click(function () {
        const $selectedCard = $(this);
        const sectionBody = $selectedCard.closest(".accordion-body");
        const assignButton = sectionBody.find(".assignBtnForSection");
        const tableId = $selectedCard.data("table-id");
        const sectionName = $selectedCard.data("section");
        const sectionId = $selectedCard.data("sectionid");


        // If already selected, unselect it
        if ($selectedCard.hasClass("selected-table")) {
            // Unselect the card
            $selectedCard.removeClass("selected-table").css("border", "2px solid transparent");

            // Clear hidden fields
            $("#SelectedTableId").val("");
            $("#SectionName").val("");

            // Disable the assign button again
            assignButton.prop("disabled", true);
            assignButton.removeAttr("data-table-id").removeAttr("data-section");
        } else {
            // Unselect all
            // @* $(".table-card.available-clickable").removeClass("selected-table").css("border", "2px solid transparent"); *@

            // Highlight new selection
            $selectedCard.addClass("selected-table").css("border", "2px solid #0d6efd");

            $(`#SelectedTableId_${sectionId}`).val(tableId);
            $("#SectionName").val(sectionName);

            $(".assignBtnForSection").prop("disabled", true);
            assignButton.prop("disabled", false)
                .attr("data-table-id", tableId)
                .attr("data-section", sectionName);
        }
    });
});

$(".accordion-button").click(function (e) {
    const target = $(this).data("bs-target");
    $(".accordion-collapse").not(target).collapse("hide");
    $(target).collapse("toggle");
});

document.addEventListener("DOMContentLoaded", () => {
    // const assignButtons = document.querySelectorAll(".open-assign-offcanvas"); 
    const assignButtons = document.querySelectorAll(".assignBtnForSection");


    assignButtons.forEach(button => {
        button.addEventListener("click", handleAssignButtonClick);
    });

    function handleAssignButtonClick() {
        const sectionId = $(this).data('section-id');
        const sectionName = this.getAttribute("data-section");
        // const tableId = this.getAttribute("data-table-id");
        const selectedCards = document.querySelectorAll('.table-card.selected-table');
        const selectedTableIds = Array.from(selectedCards).map(card => card.getAttribute('data-table-id'));

        const tableIdsString = selectedTableIds.join(',');

        const assignOffcanvas = document.querySelector("#assignOffcanvas");
        if (!assignOffcanvas) {
            console.warn("Offcanvas element not found.");
            return;
        }

        // Fill basic table and section inputs
        setInput(assignOffcanvas, "#SectionName", sectionName);
        setInput(assignOffcanvas, "#SelectedTableId", tableIdsString);


        // Load waiting list for section
        fetch(`/OrderApp/GetWaitingCustomers?sectionId=${sectionId}`)
            .then(response => response.json())
            .then(customers => {
                const listContainer = document.querySelector("#waitinglistCustomerDeatil");
                if (!listContainer) {
                    console.warn("Customer list container not found.");
                    return;
                }

                if (!Array.isArray(customers) || customers.length === 0) {
                    listContainer.innerHTML = `<p class="text-muted">No customers found.</p>`;
                    return;
                }

                listContainer.innerHTML = renderCustomerTable(customers, tableIdsString, sectionName);
                setupCustomerSelection(assignOffcanvas);
            })
            .catch(err => {
                console.error("Failed to fetch waiting list:", err);
            });
    }

    function setInput(container, selector, value) {
        const input = container.querySelector(selector);
        if (input) {
            input.value = value || "";
        } else {
            console.warn(`Input ${selector} not found in container.`);
        }
    }

    function renderCustomerTable(customers, selectedTableId, sectionName) {
        return `
                    <table class="table">
                        <thead>
                            <tr>
                                <th></th>
                                <th class="text-center">ID</th>
                                <th class="text-center">Name</th>
                                <th class="text-center">No Of Person</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${customers.map(customer => {
            const customerData = {
                tokenId: customer.waitingTicketId,
                customerId: customer.id,
                name: customer.name,
                email: customer.email,
                mobile: customer.mobile,
                noOfPersons: customer.noOfPersons,
                sectionId: customer.sectionId,
                sectionName: sectionName,
                selectedTableId: selectedTableId,
            };

            return `
                                    <tr>
                                        <td class="text-center align-middle">
                                            <input class="form-check-input RadionBtn" type="radio" name="radioDefault"
                                                id="radioDefault${customer.id}"
                                                data-obj='${JSON.stringify(customerData)}'>
                                        </td>
                                        <td class="table-row text-center">#${customer.id}</td>
                                        <td class="table-row text-center">${customer.name}</td>
                                        <td class="table-row text-center">${customer.noOfPersons}</td>
                                    </tr>
                                `;
        }).join("")}
                        </tbody>
                    </table>
            `;
    }


    function setupCustomerSelection(assignOffcanvas) {
        const radios = document.querySelectorAll(".RadionBtn");

        radios.forEach(radio => {
            radio.addEventListener("change", () => {
                const dataJson = radio.getAttribute("data-obj");
                if (!dataJson) {
                    console.warn("No customer data attached to radio button.");
                    return;
                }

                let customerData;
                try {
                    customerData = JSON.parse(dataJson);
                } catch (e) {
                    console.error("Failed to parse customer data JSON:", e);
                    return;
                }

                setInput(assignOffcanvas, "#Email", customerData.email);
                setInput(assignOffcanvas, "#Name", customerData.name);
                setInput(assignOffcanvas, "#Mobile", customerData.mobile);
                setInput(assignOffcanvas, "#NoOfPersons", customerData.noOfPersons);
                setInput(assignOffcanvas, "#SectionId", customerData.sectionId);
                setInput(assignOffcanvas, "#SelectedTableId", customerData.selectedTableId);
                setInput(assignOffcanvas, "#SectionName", customerData.sectionName);
                setInput(assignOffcanvas, "#WaitingTokenId", customerData.tokenId);
                setInput(assignOffcanvas, "#CustomerId", customerData.customerId);
                setInput(assignOffcanvas, "#OrderId", customerData.orderId);

                // Set dropdown if exists
                const sectionDropdown = document.querySelector("#SectionDropdown");
                if (sectionDropdown && customerData.sectionName) {
                    sectionDropdown.value = customerData.sectionName;
                }
            });
        });
    }
});

$("#assignOffcanvas").on("shown.bs.offcanvas", function () {
    const selectedTableId = $("#SelectedTableId").val();
    const sectionName = $("#SectionName").val();

    // Set the values in the offcanvas
    $("#SelectedTableId").val(selectedTableId);
    $("#SectionName").val(sectionName);
    $("#SectionDropdown").val(sectionName);
});

$("#accordion").on("shown.bs.collapse", function () {
    selectedTableIds = [];
    $(".table-card").removeClass("selected-table").css("border", "2px solid transparent");
    // $(".assignBtnForSection").prop("disabled", true);
    $("#SelectedTableId").val("");
});