// For toggle the side tabs of menu categories and modifier groups in small screen
function toggleTabs() {
    var sideTabs = document.querySelectorAll(".side-tab");
    sideTabs.forEach(function (tab) {
        if (tab.style.display === "none" || tab.style.display === "") {
            tab.style.display = "block";
        } else {
            tab.style.display = "none";
        }
    });
}

var selectedModifiers = new Map();
var selectedModifiersForAdd = new Map();
var existingModifiers = [];
var existingModifiersForAdd = [];
var jsonString = [];
var jsonStringForAdd = [];


//For open Add Modifier Item Modal
$(document).on("click", "#addmodifier", function () {
    var MyModal = new bootstrap.Modal(document.getElementById('addModifierModal'));
    MyModal.show();
});

//modifiers
$(document).ready(function () {

    $(".category-item").first().trigger("click");


    let currentPage = parseInt("@Model.CurrentPage") || 1;
    let currentPageModifiers = 1;
    let pageSize = parseInt("@Model.PageSize") || 5;
    let pageSizeModifiers = parseInt("@Model.PageSizeModifiers") || 5;
    let modifierGroupId = $('#mod-cat-id').val();
    let totalItems = $('#totalItemssss').val();
    let totalPages = $('#totalPagessss').val();
    let totalItemsModifiers = $('#TotalItemsModifiersss').val();
    let totalPagesModifiers = $('#TotalPagesModifiersss').val();
    let searchTerm = '';
    let searchTermModifiers = '';
    let isLoading = false;
    let searchTimeout;
    let categoryId = $('#catgory-id').val();

    if ($("#pills-item-tab").hasClass('active')) {
        $("#pills-modifires-tab").removeClass("tab-active");
        $("#pills-item-tab").addClass("tab-active");
    }

    $(".nav-link").on('click', function () {
        if ($("#pills-item-tab").hasClass('active')) {
            $("#pills-modifires-tab").removeClass("tab-active");
            $("#pills-item-tab").addClass("tab-active");
        }

        if ($("#pills-modifires-tab").hasClass('active')) {
            $("#pills-item-tab").removeClass("tab-active");
            $("#pills-modifires-tab").addClass("tab-active");
        }
    });

    function fetchItems(search = "", page = 1, pageSizeValue = 5) {
        if (isLoading) return;
        isLoading = true;

        $.ajax({
            url: 'Menu/GetItemsByCategory',
            type: 'GET',
            data: {
                categoryid: categoryId,
                searchTerm: search,
                page: page,
                pageSize: pageSizeValue
            },
            success: function (data) {
                $("#ItemListContainer").html(data);
                let $data = $(data);
                totalPages = parseInt($('#totalPageess').val()) || 1;
                totalItems = parseInt($('#totalItems').val());

                //  Update UI with new data
                $("#ItemListContainer").html(data);

                //  Update current page & page size
                currentPage = page;
                pageSize = pageSizeValue;

                updatePaginationButtons();
                // $(".cat").first().trigger("click");
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", status, error);
            },
            complete: function () {
                isLoading = false;
            }
        });
    }


    function fetchModifiers(search = "", page = 1, pageSizeValue = 5) {
        $.ajax({
            url: 'Menu/GetModifierItemsByModifierGroup',
            type: 'GET',
            data: {
                categoryid: modifierGroupId,
                searchTerm: search,
                page: page,
                pageSize: pageSizeValue
            },
            success: function (data) {
                $("#ModifiersListContainer").html(data);
                let $data = $(data);
                totalPagesModifiers = parseInt($('#TotalPagesModifiers').val()) || 1;
                totalItemsModifiers = parseInt($('#TotalItemsModifiers').val());

                //  Update UI with new data
                $("#ModifiersListContainer").html(data);

                //  Update current page & page size
                currentPageModifiers = page;
                pageSizeModifiers = pageSizeValue;

                updatePaginationButtons();
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", status, error);
            },
            complete: function () {
                isLoading = false;
            }
        });
    }

    function updatePaginationButtons() {
        // @* $('#prevPage').prop('disabled', currentPage <= 1);
        // $('#nextPage').prop('disabled', currentPage >= totalPages); *@
        // @* $('#currentPageDisplay').text(`Showing 1- ${pageSize} of ${totalItems}`); *@
    }

    //  Handle Previous Page Click
    $(document).on('click', '#prevPage', function () {
        fetchItems(searchTerm, currentPage - 1, pageSize);
    });

    $(document).on('click', '#nextPage', function () {
        fetchItems(searchTerm, currentPage + 1, pageSize);
    });

    $(document).on('change', '#pageSizes', function () {
        pageSize = parseInt($(this).val()) || 5;
        fetchItems(searchTerm, 1, pageSize);
    });

    //  Debounced Search Functionality
    $(document).on('keyup', '#searchBox', function () {
        clearTimeout(searchTimeout);
        searchTerm = $(this).val();

        searchTimeout = setTimeout(() => {
            fetchItems(searchTerm, 1, pageSize);
        }, 100);
    });

    $("#deleteAll").on("click", function (e) {
        var selectedItems = document.querySelectorAll(".ItemsCheck:checked");
        if (selectedItems.length === 0) {
            e.preventDefault();
            toastr.error("Please select Item(s) to delete.");
        }
        else {
            var MyModal = new bootstrap.Modal(document.getElementById('bulkDeleteModal'));
            MyModal.show();
        }
    });

    $("#deleteAllModifiers").on("click", function (e) {
        var selectedItems = document.querySelectorAll(".ModifiersCheck:checked");
        if (selectedItems.length === 0) {
            e.preventDefault();
            toastr.error("Please select Modifier(s) to delete.");
        }
        else {
            var MyModal = new bootstrap.Modal(document.getElementById('bulkDeleteModifiersModal'));
            MyModal.show();
        }
    });

    //  Handle Previous Page Click
    $(document).on('click', '#prevPageModifiers', function () {
        fetchModifiers(searchTermModifiers, currentPageModifiers - 1, pageSizeModifiers);
    });

    $(document).on('click', '#nextPageModifiers', function () {
        fetchModifiers(searchTermModifiers, currentPageModifiers + 1, pageSizeModifiers);
    });

    $(document).on('change', '#pageSizesModifiers', function () {
        pageSizeModifiers = parseInt($(this).val()) || 5;
        fetchModifiers(searchTermModifiers, 1, pageSizeModifiers);
    });

    //  Debounced Search Functionality
    $(document).on('keyup', '#searchBoxModifiers', function () {
        clearTimeout(searchTimeout);
        searchTermModifiers = $(this).val();

        searchTimeout = setTimeout(() => {
            fetchModifiers(searchTermModifiers, 1, pageSizeModifiers);
        }, 100);
    });

    //  Handle Category Selection
    $(document).on('click', '.cat', function (e) {
        e.preventDefault();

        let newCategoryId = $(this).data('category-id');

        $('.cat').each(function () {
            $(this).removeClass('custom-blue-color');
        });
        $(this).removeClass('text-secondary');
        $(this).addClass("custom-blue-color");

        if (!newCategoryId) {
            console.error("Category ID is undefined!");
            return;
        }

        categoryId = newCategoryId;
        fetchItems(searchTerm, 1, pageSize);
    });

    //  Handle Modifier Group Selection
    $(document).on('click', '.mod', function (e) {
        e.preventDefault();

        $('.mod').each(function () {
            $(this).removeClass('custom-blue-color');
        });
        $(this).addClass("custom-blue-color");

        let newCategoryId = $(this).data('modifiergroup-id');

        if (!newCategoryId) {
            console.error("Category ID is undefined!");
            return;
        }

        modifierGroupId = newCategoryId;
        fetchModifiers(searchTermModifiers, 1, pageSizeModifiers);
    });

    //  Initial Load
    fetchItems(searchTerm, 1, pageSize);
    fetchModifiers(searchTermModifiers, 1, pageSizeModifiers);

});

$('body').on('click', '.editCategoryBtn', function (e) {
    e.preventDefault();
    var itemId = $(this).data('id');
    $.ajax({
        url: 'Menu/EditCategory',
        type: 'GET',
        data: { id: itemId },
        success: function (response) {
            var data = response.data;
            $('#editCategoryModal').modal('show');
            $("#editCategoryId").val(data.menucategoryid);
            $("#editCategoryName").val(data.categoryname);
            $("#editCategoryDescription").val(data.description);

        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });
});

$(document).on('submit', '#editCategoryForm', function (e) {
    e.preventDefault();

    if (!$(this).valid()) {
        return;
    }

    var editCategoryData = {
        Categoryid: $("#editCategoryId").val(),
        Name: $("#editCategoryName").val(),
        Description: $("#editCategoryDescription").val(),
    };

    $.ajax({
        url: 'Menu/EditCategory',
        type: 'POST',
        data: JSON.stringify(editCategoryData),
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
                $('.modal-backdrop').remove();
                $('body').removeClass('modal-open');
                toastr.success(response.message);
                location.reload();
            } else {
                toastr.success(response.message);
                location.reload();
            }
        },
        error: function (xhr, status, error) {
            // console.error(error);
            // console.error(xhr.responseText);
            toastr.error('An error occurred while updating the category.');
        }
    });
});

$(document).off("change", ".rowModifierCheck"); // For remove event listners from previous edit modifierGroup
$(document).on("change", ".rowModifierCheck", function () {
    let modifierId = $(this).data('id');
    let modifierName = $(this).data('name');

    if ($(this).is(":checked")) {
        selectedModifiers.set(modifierId, modifierName);
        jsonString.push(modifierId);
    } else {
        selectedModifiers.delete(modifierId);
        jsonString = jsonString.filter(id => id !== modifierId);
    }
});

$(document).on('click', '#editModifierGroupBtn', function (e) {
    e.preventDefault();
    var ModifierGroupId = $(this).data('id');
    existingModifiers = [];
    selectedModifiers.clear();
    // var url = `/Orders/ExportOrders?status=${encodeURIComponent(status)}&date=${encodeURIComponent(date)}&searchText=${encodeURIComponent(searchText)}`;
    // '@Url.Action("EditModifierGroup", "Menu")' + '?id=' + encodeURIComponent(ModifierGroupId)
    $.ajax({
        url: 'Menu/EditModifierGroup',
        type: 'GET',
        data: { id: ModifierGroupId },
        success: function (response) {
            if (response.success) {
                $("#modifierGroupName-error").remove();
                $('#ModifierGroupId').val(response.data.modifierGroupId);
                $('#modifierGroupName').val(response.data.modifierGroupName);
                $('#modifierGroupDescription').val(response.data.modifierGroupDescription);
                $('#existingModifiersContainer').empty();
                response.data.existingModifiers.forEach(checkBox => {
                    existingModifiers.push(checkBox.modifierId);
                    selectedModifiers.set(checkBox.modifierId, checkBox.name);
                    // jsonString.push(checkBox.modifierId);
                });

                // Append new modifiers
                selectedModifiers.forEach(function (modifierName, modifierId) {
                    $('#existingModifiersContainer').append(
                        `<div class="d-flex align-items-center modifierInEditModal_${modifierId} border rounded-pill mb-2 me-2">
                        <div class="d-flex m-2 existingModifier ">${modifierName}</div>
                        <button type="button" class="btn-close removeModifierBtnEdit_${modifierId}" data-id="${modifierId}" aria-label="Close"></button>
                    </div>`
                    );

                    $(document).on('click', `.removeModifierBtnEdit_${modifierId}`, function (e) {
                        e.preventDefault();
                        let unCheckModifierId = $(this).data('id');
                        selectedModifiers.delete(unCheckModifierId);
                        existingModifiers = existingModifiers.filter(id => id !== unCheckModifierId);
                        $(`.modifierInEditModal_${unCheckModifierId}`).remove();
                    });
                });

                //For adding the selected Modifiers in Edit Modifier Group
                $("#editModifierGroupForm").validate({
                    rules: {
                        modifierGroupName: {
                            required: true
                        }
                    },
                    messages: {
                        modifierGroupName: {
                            required: "Name is required"
                        }
                    },
                    errorPlacement: function (error, element) {
                        // Place error message outside the floating wrapper
                        if (element.parent().hasClass("form-floating")) {
                            error.addClass("text-danger");
                            error.insertAfter(element.parent());
                        } else {
                            error.addClass("text-danger");
                            error.insertAfter(element);
                        }
                    },
                    // highlight: function (element) {
                    //     $(element).addClass("is-invalid");
                    // },
                    unhighlight: function (element) {
                        $(element).removeClass("is-invalid");
                    },
                    submitHandler: function (form) {
                        let modifierGroupName = $("#modifierGroupName").val();
                        let modifierGroupDescription = $("#modifierGroupDescription").val();
                        var formData = JSON.stringify(existingModifiers);

                        $.ajax({
                            url: `/Menu/AddSelectedModifiers?modifierGroupId=${encodeURIComponent(response.data.modifierGroupId)}&name=${modifierGroupName}&description=${modifierGroupDescription}`,
                            type: 'POST',
                            data: formData,
                            contentType: 'application/json',
                            success: function (response) {
                                $('#editModifierGroup').modal('hide');
                                toastr.success(response.message);
                            },
                            error: function () {
                                $('#editModifierGroup').modal('hide');
                                console.error("error");
                            }
                        });
                    }
                });
            }
            else {
                toastr.error(response.message);
            }
        },
        error: function (xhr, status, error) {
            toastr.error('An error occurred while edit the modifiergroup.');
        }
    });
});

// Trigger the AJAX call when the edit link is clicked
$('body').on('click', '.edit-icon-modifier', function (e) {
    e.preventDefault();
    var itemId = $(this).data('id');
    $.ajax({
        url: 'Menu/EditModifier',
        type: 'GET',
        data: { id: itemId },
        success: function (response) {
            $('#EditModifierModal').modal('show');
            $('#EditModifierModal').html(response);
            $.validator.unobtrusive.parse('#EditModifierModal .modal-content');
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });
});

//Post method for EditModifierForm
$(document).on("submit", "#EditModifierForm", function (e) {
    e.preventDefault();

    if (!$(this).valid()) {
        return;
    }

    var formData = $(this).serialize();
    var actionUrl = $(this).attr("action");

    $.ajax({
        url: 'Menu/EditModifier',
        type: "POST",
        data: formData,
        success: function (response) {
            if (response.success) {
                $("#EditModifierModal").modal("hide");
                $(".modal-backdrop").remove();
                $("body").removeClass("modal-open");
                toastr.success(response.message);
                location.reload();
            }
        },
        error: function (xhr, status, error) {
            console.error("Error:", xhr.responseText);
        }
    });
});

var modifiersGroups = [];

$(document).on('change', '.modifier-checkbox', function () {
    var modifierGroupId = $(this).data('id');

    // Ensure modifiersGroups is an array
    if (!Array.isArray(modifiersGroups)) {
        console.warn("Fixing corrupted modifiersGroups variable.");
        modifiersGroups = [];
    }

    if ($(this).is(':checked')) {
        if (!modifiersGroups.includes(modifierGroupId)) {
            modifiersGroups.push(modifierGroupId);
        }
    } else {
        modifiersGroups = modifiersGroups.filter(id => id !== modifierGroupId);
    }

});

var selectedMenuItems = [];
//For Adding Items to the array for Mass Delete
$(document).on('change', "#ItemMainCheck", function (e) {
    selectedMenuItems = [];
    let chechBoxes = document.querySelectorAll(".ItemsCheck");
    chechBoxes.forEach((checkBox) => {
        checkBox.checked = e.target.checked;
        let itemId123 = $(checkBox).data('id');

        if ($(this).is(':checked')) {
            if (!selectedMenuItems.includes(itemId123)) {
                selectedMenuItems.push(itemId123);
            }
        }
        else {
            selectedMenuItems = selectedMenuItems.filter(id => id !== itemId123);
        }
    });
});

//For adding particular items to the array for delete
$(document).on('change', '.ItemsCheck', function () {
    var itemId = $(this).data('id');

    if (!Array.isArray(selectedMenuItems)) {
        console.warn("Fixing corrupted modifiersGroups variable.");
        selectedMenuItems = [];
    }

    if ($(this).is(':checked')) {
        if (!selectedMenuItems.includes(itemId)) {
            selectedMenuItems.push(itemId);
        }
    }
    else {
        selectedMenuItems = selectedMenuItems.filter(id => id !== itemId);
    }
    // Check if all checkboxes are checked
    let total = $(".ItemsCheck").length;
    let checked = $(".ItemsCheck:checked").length;

    // If all individual checkboxes are checked, check the master checkbox
    // Otherwise, uncheck it
    if (total > 0 && total === checked) {
        $("#ItemMainCheck").prop('checked', true);
    } else {
        $("#ItemMainCheck").prop('checked', false);
    }

});

$(document).on('click', "#confirmBulkDeleteBtn", function () {
    selectedMenuItems.forEach((item) => {
        $.ajax({
            url: 'Menu/DeleteMenuItem',
            type: 'GET',
            data: { itemId: item },
            success: function () {
                location.reload();
            },
            error: function () {
                console.error("error")
            }
        });
    });
});

var selectedModifierItems = [];
//For Adding Modifiers to the array for Mass Delete 
$(document).on('change', "#ModifierMainCheck", function (e) {
    selectedModifierItems = [];
    let chechBoxes = document.querySelectorAll(".ModifiersCheck");
    chechBoxes.forEach((checkBox) => {
        checkBox.checked = e.target.checked;
        let modifierId123 = $(checkBox).data('id');
        // @* selectedMenuItems.push(itemId123); *@

        if ($(this).is(':checked')) {
            // Add if not already present
            if (!selectedModifierItems.includes(modifierId123)) {
                selectedModifierItems.push(modifierId123);
            }
        }
        else {
            // Remove if unchecked
            selectedModifierItems = selectedModifierItems.filter(id => id !== modifierId123);
        }
    });
});

//For adding particular modifiers to the array for delete
$(document).on('change', '.ModifiersCheck', function () {
    let modifierId = $(this).data('id');

    // Ensure modifiersGroups is an array
    if (!Array.isArray(selectedModifierItems)) {
        console.warn("Fixing corrupted modifiersGroups variable.");
        selectedModifierItems = [];
    }

    if ($(this).is(':checked')) {
        // Add if not already present
        if (!selectedModifierItems.includes(modifierId)) {
            selectedModifierItems.push(modifierId);
        }
    }
    else {
        // Remove if unchecked
        selectedModifierItems = selectedModifierItems.filter(id => id !== modifierId);
    }
    // Check if all checkboxes are checked
    let total = $(".ModifiersCheck").length;
    let checked = $(".ModifiersCheck:checked").length;

    // If all individual checkboxes are checked, check the master checkbox
    // Otherwise, uncheck it
    if (total > 0 && total === checked) {
        $("#ModifierMainCheck").prop('checked', true);
    } else {
        $("#ModifierMainCheck").prop('checked', false);
    }
});

$(document).on('click', "#confirmBulkDeleteModifiersBtn", function () {
    selectedModifierItems.forEach((item) => {
        $.ajax({
            url: 'Menu/DeleteMenuItem',
            type: 'GET',
            data: { itemId: item },
            success: function () {
                location.reload();
            },
            error: function () {
                console.error("error");
            }
        });
    });
});

// @* ************* *@

// Modal open add category
function openAddCategoryModal() {
    var addCategoryModal = new bootstrap.Modal(document.getElementById('addCategoryModal'));
    addCategoryModal.show();
}

var modifierGroupForAdd = [];
//For append Modifier Groups in New Item Modal
$(document).on('change', "#ModifierGroupsInNewItemSelect", function () {
    var modifierGroupId = $(this).val();
    modifierGroupForAdd.push(modifierGroupId);

    $.ajax({
        url: 'Menu/GetModifierGroupForNewItem',
        type: 'GET',
        data: { modifierGroupId: modifierGroupId },
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {

                let modifiersHTML = response.data.existingModifiers.map(modifier => `
                    <div class="modifier-item w-100 d-flex justify-content-between">
                        <div>
                            ${modifier.name}
                        </div>
                        <div class="me-2">
                            ${modifier.rate}
                        </div>
                    </div>
                `).join("");

                $("#ModifierGroupsInNewItemModal").append(
                    `<div  class="mt-2 appendModifierGroupInNewItem_${response.data.modifierGroupId}">
                    <input type="hidden" name="ModifierGroupIds" value="${response.data.modifierGroupId}">
                    <div class="modifierHeading fw-bold d-flex justify-content-between">
                        ${response.data.modifierGroupName}
                        <button type="button"  data-id="${response.data.modifierGroupId}" class="btn bg-transparent deleteModifierGroupInNewModal_${response.data.modifierGroupId} border-none">
                            <img src="/images/Download/trash.svg" width="20px" alt="delete">
                        </button>
                    </div>
                    <div>
                        <div class="row">
                            <div class="col-6">
                                <select class="form-select rounded-pill modifierForNewItemSelectMin" data-id="${response.data.modifierGroupId}">
                                    <option value="" selected>0</option>
                                    <option value="1">1</option>
                                    <option value="2">2</option>
                                    <option value="3">3</option>
                                </select>
                            </div>
                            <div class="col-6">
                                <select class="form-select rounded-pill modifierForNewItemSelectMax" data-id="${response.data.modifierGroupId}">
                                    <option value="" selected>0</option>
                                    <option value="1">1</option>
                                    <option value="2">2</option>
                                    <option value="3">3</option>
                                </select>
                            </div>
                        </div>
                        <div id="modifiersInNewItem">
                            ${modifiersHTML}
                        </div>
                    </div>
                </div>`
                );

                $(document).on('click', `.deleteModifierGroupInNewModal_${response.data.modifierGroupId}`, function () {
                    let modifierGroupId = $(this).data('id');
                    modifierGroupForAdd.filter(id => id !== modifierGroupId);
                    $(`.appendModifierGroupInNewItem_${modifierGroupId}`).remove();
                });
            } else {
                toastr.error(response.message);
            }
        },
        error: function () {
            console.error("error");
        }
    });
});



// @* Validations *@
$("#addEditCategoryForm").submit(function (e) {
    e.preventDefault();

    if (!$(this).valid()) {
        return;
    }

    if ($(this).data("edit-mode") === true) {
        updateCategory();
    } else {
        addCategory();
    }
});

function addCategory() {
    let categoryData = {
        name: $("#addCategoryName").val(),
        description: $("#addCategoryDescription").val()
    };

    $.ajax({
        url: 'Menu/AddCategory',
        type: 'POST',
        contentType: "application/json",
        data: JSON.stringify(categoryData),
        success: function (response) {
            if (response.success) {
                toastr.success(response.message);
                $("#addCategoryModal").modal("hide");
                $("#addEditCategoryForm").trigger('reset');
                location.reload();
                // $("#categoryname, #categorydescription").val("");
            } else {
                toastr.error(response.message);
            }
        },
        error: function () {
            console.error("error");
        }
    });
}



$("#addMenuItemForm").submit(function (e) {
    e.preventDefault();

    if (!$(this).valid()) {
        return;
    }

    if ($(this).data("edit-mode") === true) {
        updateItem();
    } else {
        addItem();
    }
});

function addItem() {
    var customSwitch = $(".newItemAvailability").is(":checked");
    var modifierGroupDataArray = [];

    modifierGroupForAdd.forEach(function (modifierGroupId) {
        var min = parseInt($(`.modifierForNewItemSelectMin[data-id="${modifierGroupId}"]`).val()) || 0;
        var max = parseInt($(`.modifierForNewItemSelectMax[data-id="${modifierGroupId}"]`).val()) || 0;

        modifierGroupDataArray.push({
            Id: modifierGroupId,
            Min: min,
            Max: max
        });
    });


    let customSwitchNewItem = $("#newItemAvailability").is(":checked");
    // let uploadImageForNewItem = $("#newItemImage");
    // let addImagePath = uploadImageForNewItem[0].files[0].name

    let newItemData = {
        categoryId: $("#newItemCategoryId").val(),
        Name: $("#newItemName").val(),
        Itemtype: $("#newItemType option:selected").val(),
        Rate: $("#newItemRate").val(),
        Quantity: $("#newItemQuantity").val(),
        Unit: $("#newItemUnit").val(),
        Isavailable: customSwitch,
        Tax: $("#newItemTaxPercentage").val(),
        ItemShortCode: $("#newItemShortCode").val(),
        // uploadimage: $("#ItemType").val(),
        Description: $("#newItemDescription").val(),
        ModifierGroupData: modifierGroupDataArray,
        Isavailable: customSwitchNewItem,
        // ItemPhoto: addImagePath
    };
    debugger;

    let itemPhoto = $("#newItemImage")[0].files[0];
    if (itemPhoto) {
        newItemData.append("ItemPhoto", itemPhoto);
    }

    $.ajax({
        url: 'Menu/AddNewItem',
        type: 'POST',
        contentType: "application/json",
        data: JSON.stringify(newItemData),

        success: function (response) {
            if (response.success) {
                $("#addMenuItemModal").modal("hide");
                toastr.success(response.message);
                $("#addMenuItemForm").trigger('reset');
                location.reload();

                // $("#categoryname, #categorydescription").val(""); 
            } else {
                toastr.error(response.message);
            }
        },
        error: function () {
            console.error("error");
        }
    });
}

$("#addModifierGroupForm").submit(function (e) {
    e.preventDefault();

    if (!$(this).valid()) {
        return;
    }

    if ($(this).data("edit-mode") === true) {
        updateModifierGroup();
    } else {
        addModifierGroup();
    }
});

function addModifierGroup() {
    var formData = JSON.stringify(jsonStringForAdd);

    let newModifierGroupData = {
        modifierGroupName: $("#newModifierGroupName").val(),
        modifierGroupDescription: $("#newModifierGroupDescription").val(),
        ModifiersIds: existingModifiersForAdd
    };

    var formData = new FormData();
    formData = JSON.stringify(newModifierGroupData);

    $.ajax({
        url: 'Menu/AddModifierGroup',
        type: 'POST',
        data: formData,
        contentType: 'application/json',
        success: function () {
            $('#addModifierGroupModal').modal('hide');
            $("#addModifierGroupForm").trigger('reset');
            location.reload();
        },
        error: function () {
            $('#addModifierGroupModal').modal('hide');
            console.error("error");
        }
    });
}

var modifierGroupForEdit = [];
// Trigger the AJAX call when the edit link is clicked
$('body').on('click', '.edit-icon', function (e) {
    e.preventDefault();
    var itemId = $(this).data('id');
    $.ajax({
        url: 'Menu/GetMenuItemForEdit',
        type: 'GET',
        data: { id: itemId },
        success: function (response) {
            var editItemModal = new bootstrap.Modal(document.getElementById('EditItemModal'));
            editItemModal.show();
            $("#EditItemModal").html(response);
            $.validator.unobtrusive.parse('#EditItemModal .modal-content');
            var ids = $("#editItemModifierGroupIdsForEdit").val() || [];
            var idList = [];
            if (ids.length !== 0) {
                idList = ids.split(',').map(ids => parseInt(ids));
                idList.forEach(function (modifierGroupId) {
                    modifierGroupForEdit.push(modifierGroupId);
                    $(`.deleteModifierGroupInEditModal_${modifierGroupId}`).on('click', function () {
                        let modifierGroupId = $(this).data('id');
                        modifierGroupForEdit = modifierGroupForEdit.filter(id => id !== modifierGroupId);
                        $(`.appendModifierGroupInEditItem_${modifierGroupId}`).remove();
                    });
                });
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });
});

//For append Modifier Groups in Edit Item Modal
$(document).on('change', "#ModifierGroupsInEditItemSelect", function () {
    var modifierGroupIdForEdit = $(this).val();
    modifierGroupForEdit.push(modifierGroupIdForEdit);
    console.error("value: ", modifierGroupIdForEdit);

    $.ajax({
        url: 'Menu/GetModifierGroupForEditItem',
        type: 'GET',
        data: { modifierGroupId: modifierGroupIdForEdit },
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
                let modifiersHTML = response.data.existingModifiers.map(modifier => `
                    <div class="modifier-item w-100 d-flex justify-content-between">
                        <div>
                            ${modifier.name}
                        </div>
                        <div class="me-2">
                            ${modifier.rate}
                        </div>
                    </div>
                `).join("");

                $("#ModifierGroupsInEditItemModal").append(
                    `<div  class="mt-2 appendModifierGroupInEditItem_${response.data.modifierGroupId}">
                    <input type="hidden" name="ModifierGroupIds" value="${response.data.modifierGroupId}">
                    <div class="modifierHeading fw-bold d-flex justify-content-between">
                        ${response.data.modifierGroupName}
                        <button type="button"  data-id="${response.data.modifierGroupId}" class="btn bg-transparent deleteModifierGroupInEditModal_${response.data.modifierGroupId} border-none">
                            <img src="/images/Download/trash.svg" width="20px" alt="delete">
                        </button>
                    </div>
                    <div>
                        <div class="row">
                            <div class="col-6">
                                <select class="form-select rounded-pill modifierForEditItemSelectMin modifierForItemSelect" data-id="${response.data.modifierGroupId}">
                                    <option value="" selected>0</option>
                                    <option value="1">1</option>
                                    <option value="2">2</option>
                                    <option value="3">3</option>
                                </select>
                            </div>
                            <div class="col-6">
                                <select class="form-select rounded-pill modifierForEditItemSelectMax modifierForItemSelect" data-id="${response.data.modifierGroupId}">
                                    <option value="" selected>0</option>
                                    <option value="1">1</option>
                                    <option value="2">2</option>
                                    <option value="3">3</option>
                                </select>
                            </div>
                        </div>
                        <div id="modifiersInNewItem">
                            ${modifiersHTML}
                        </div>
                    </div>
                </div>`
                );

                $(document).on('click', `.deleteModifierGroupInEditModal_${response.data.modifierGroupId}`, function () {
                    let modifierGroupId = $(this).data('id');
                    modifierGroupForEdit = modifierGroupForEdit.filter(id => id !== modifierGroupId);
                    $(`.appendModifierGroupInEditItem_${modifierGroupId}`).remove();
                });
            } else {
                console.error(response.message);
            }
        },
        error: function () {
            console.error("error");
        }
    });
});

$('body').on('submit', '#editMenuItemForm', function (e) {
    e.preventDefault();

    if (!$(this).valid()) {
        return;
    }

    var formData = new FormData(this);

    var internChoice = $('#editItemType option:selected').val();
    formData.set("Item.Itemtype", internChoice);

    let customSwitch = $("#editItemAvailable").is(":checked");
    formData.set("Item.Isavailable", customSwitch);
    console.log("customSwitch: ", customSwitch);

    modifierGroupForEdit.forEach(function (modifierGroupId, index) {
        var min = parseInt($(`.modifierForEditItemSelectMin[data-id="${modifierGroupId}"]`).val()) || 0;
        var max = parseInt($(`.modifierForEditItemSelectMax[data-id="${modifierGroupId}"]`).val()) || 0;

        formData.append(`Item.ModifierGroupData[${index}].Id`, modifierGroupId);
        formData.append(`Item.ModifierGroupData[${index}].Min`, min);
        formData.append(`Item.ModifierGroupData[${index}].Max`, max);
    });

    //modifierGroupIds without deletion even if click on trash icon
    var modifierGroupIds = [];
    var abc = $("#editItemModifierGroupIdsForEdit").val();
    if (abc.length !== 0) {
        modifierGroupIds = abc.split(',').map(ids => parseInt(ids));
        let count = 0;
        modifierGroupIds.forEach(function (modifierGroupId) {
            formData.append(`Item.ModifierGroupIds[${count}]`, modifierGroupId);
            count++;
        });
    }
    
    debugger;

    let itemPhoto = $("#editItemImage")[0].files[0];
    if (itemPhoto) {
        formData.append("Item.ItemPhoto", itemPhoto);
    }

    // let editImagePath = $("#editItemImage").val();
    // formData.append("Itemimage", editImagePath);

    $.ajax({
        url: 'Menu/EditMenuItem',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success) {
                $('#EditItemModal').modal('hide');
                $('.modal-backdrop').remove();
                $('body').removeClass('modal-open');
                toastr.success(response.message);
                location.reload();
            } else {
                toastr.error(response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });
});

let selectedIds = [];
$(".modifier-checkbox").on('change', function () {
    if ($(this).is(":checked")) {
        selectedIds.push(parseInt($(this).val()));
        $("#modifierDropdown").text(
            selectedIds.length > 0 ? selectedIds.length + " selected" : "Select Modifier Group(s)"
        );
    }
    else {
        selectedIds = selectedIds.filter(id => id !== parseInt($(this).val()));
        $("#modifierDropdown").text(
            selectedIds.length > 0 ? selectedIds.length + " selected" : "Select Modifier Group(s)"
        );
    }
});

$("#addNewModifierForm").submit(function (e) {
    e.preventDefault();


    if (!$(this).valid()) {
        return;
    }

    if (selectedIds.length === 0) {
        $("#ModifierGroupIds").addClass("is-invalid");
        $("#ModifierGroupIdsError").text("At least one Modifier Group must be selected.");
        return;
    }

    if ($(this).data("edit-mode") === true) {
        updateModifier();
    } else {
        addModifier();
    }
});



function addModifier() {
    var customSwitch = $(".newItemAvailability").is(":checked");

    let newModifierData = {
        Name: $("#newModifierName").val(),
        Unit: $("#newModifierUnit").val(),
        Rate: $("#newModifierRate").val(),
        Quantity: $("#newModifierQuantity").val(),
        Description: $("#newModifierDescription").val(),
        Ids: selectedIds
    };
    let ab = $("#addNewModifierForm").serialize();

    var formData = new FormData();
    formData = JSON.stringify(newModifierData);

    $.ajax({
        url: 'Menu/AddNewModifier',
        type: 'POST',
        contentType: 'application/json',
        data: formData,
        success: function (response) {
            if (response.success) {
                $('#addModifierModal').modal('hide');
                $('.modal-backdrop').remove();
                $('body').removeClass('modal-open');
                toastr.success(response.message);
            }
            else {
                $('#addModifierModal').modal('hide');
                $('.modal-backdrop').remove();
                $('body').removeClass('modal-open');
                toastr.error(response.message);
            }
        },
        error: function (xhr, status, error) {
            $('#addModifierModal').modal('hide');
            $('.modal-backdrop').remove();
            $('body').removeClass('modal-open');
            console.error('Error:', error);
        }
    });
}

function loadModifiers(pageNumber, pageSize) {
    $.ajax({
        url: '/Menu/GetModifiersForExistingModifiersForAdd',
        type: 'GET',
        data: { pageNumber: pageNumber, pageSize: pageSize },
        success: function (response) {
            $("#existingModifiersForAddGroupTable").empty();
            $('#existingModifiersForAddGroupTable').html(response);
            bindPaginationEvents();

            //For check all existing Modifiers
            existingModifiersForAdd.forEach(function (modifierId) {
                var modifierId = $(`.modifiersIdsForAdd[data-id="${modifierId}"]`).data('id');
                // var modifierInput = $(`.rowModifierCheckForAdd_"${modifierId}"]`);
                // modifierInput.checked = true;
                if (modifierId !== undefined) {
                    document.querySelector(`.modifiersIdsForAdd[data-id="${modifierId}"]`).checked = true;
                }
            }
            );
        },
        error: function () {
            console.error('Failed to load data.');
        }
    });
}

function bindPaginationEvents() {
    var currentPage = $("#currentPageForExistingModifiers").val();
    var totalPages = $("#totalPagesForExistingModifiers").val();
    var totalItems = $("#totalItemsForExistingModifiers").val();

    $('#prevPageForAddExistingModifiersAdd').off('click').on('click', function () {
        if (currentPage > 1) {
            loadModifiers(currentPage - 1, $('#pageSizesForAddExistingModifiers').val());
        }
    });

    $('#nextPageForAddExistingModifiersAdd').off('click').on('click', function () {
        if (currentPage < totalPages) {
            currentPage = parseInt(currentPage) + 1;
            loadModifiers(currentPage, $('#pageSizesForAddExistingModifiers').val());
        }
    });

    $('#pageSizesForAddExistingModifiers').off('change').on('change', function () {
        loadModifiers(1, $(this).val());
    });
}

var allModifierIds = [];
$('.modifiersIdsForAdd').each(function () {
    var modifierId = $(this).data('id');
    if (modifierId !== undefined) {
        allModifierIds.push(modifierId);
    }
});

allModifierIds.forEach(function (modifierId) {
    $(document).off("change", `.rowModifierCheckForAdd_${modifierId}`); // For remove event listners from previous edit modifierGroup
    $(document).on("change", `.rowModifierCheckForAdd_${modifierId}`, function () {
        let modifierId = $(this).data('id');
        let modifierName = $(this).data('name');

        if ($(this).is(":checked")) {
            selectedModifiersForAdd.set(modifierId, modifierName);
            jsonStringForAdd.push(modifierId);
            existingModifiersForAdd.push(modifierId);
        }
        else {
            selectedModifiersForAdd.delete(modifierId);
            jsonStringForAdd = jsonStringForAdd.filter(id => id !== modifierId);
            existingModifiersForAdd = existingModifiersForAdd.filter(id => id !== modifierId);
        }
    });
});

//For open modal on add existing modifiers in add modifier group modal
$(document).on('click', "#addModifiersInGroup", function () {
    $('#addModifierGroupModal').modal('hide');
    $("#addExistingModifiersForm").trigger('reset');
    $('#addExistingModal').modal('show');

    loadModifiers(1, $('#pageSizesForAddExistingModifiers').val());

    // For check all existing Modifiers
    selectedModifiersForAdd.forEach(function (modifierName, modifierId) {
        if (existingModifiersForAdd.includes(modifierId)) {
            if (modifierId !== undefined) {
                document.querySelector(`.modifiersIdsForAdd[data-id="${modifierId}"]`).checked = true;
            }
        }
    });
});

//On save modal of addExisting Modifiers in Add Modifier Group
$(document).on('click', "#saveAddExistingModifiersForAdd", function () {
    $('#addExistingModal').modal('hide');
    $('#addModifierGroupModal').modal('show');

    $('#existingModifiersContainerForAdd').empty();

    // Append new modifiers For Add
    selectedModifiersForAdd.forEach(function (modifierName, modifierId) {
        $('#existingModifiersContainerForAdd').append(
            `<div class="d-flex align-items-center modifierInAddModal_${modifierId} border rounded-pill mb-2 me-2">
                <div class="d-flex m-2 existingModifier">${modifierName}</div>
                <button type="button" class="btn-close removeModifierBtn_${modifierId}" data-id="${modifierId}" aria-label="Close"></button>
            </div>`
        );
        $(document).on('click', `.removeModifierBtn_${modifierId}`, function (e) {
            e.preventDefault();
            let unCheckModifierId = $(this).data('id');
            selectedModifiersForAdd.delete(unCheckModifierId);
            existingModifiersForAdd = existingModifiersForAdd.filter(id => id !== unCheckModifierId);
            $(`.modifierInAddModal_${unCheckModifierId}`).remove();
        });
    });
});

// Initialize pagination when the modal is shown for add modifier group
// $('#addExistingModal').on('shown.bs.modal', function () {
//     debugger;
//     loadModifiers(1, $('#pageSizesForAddExistingModifiers').val());
//     // bindPaginationEvents();
// });

function openAddModifierGroupModal() {
    $("#addModifierGroupModal").modal("show");
}

// function openAddExistingModifiersModal() {
//     $("#addModifierGroupModal").modal("hide");
//     $("#addExistingModal").modal("show");

//     //For checking the checkboxes for row of the addExistingModifiers
//     var rowModifierGroupCheckBoxes = document.querySelectorAll('.rowModifierCheckForAdd');
// }

function openEditModifierGroupModal(modifierGroup) {
    console.log(modifierGroup);
    $("#editModifierGroup").modal("show");
}


// @* $('#addExistingModifiersModal').modal('hide'); *@
// $(document).on('click', '#addExistingModifiersBtn', function () {
//     addExistingModifiers(selectedModifiers);
// });

// function addExistingModifiers(selectedModifiers) {
//     $('#editModifierGroup').modal('hide'); // Close Edit Modifier Group Modal
//     $("#addExistingModifiersForm").trigger('reset');
//     $('#addExistingModifiersModal').modal('show'); // Open Add Existing Modifiers Modal

//     //For checking the checkboxes for row of the addExistingModifiers
//     var rowModifierGroupCheckBoxes = document.querySelectorAll('.rowModifierCheck');

//     document.querySelectorAll("rowModifierCheck").forEach(checkBox => {
//         checkBox.checked = false;
//     });

//     // For check all existing Modifiers
//     selectedModifiers.forEach(function (modifierName, modifierId) {
//         if (existingModifiers.includes(modifierId)) {
//             document.querySelector(`.rowModifierCheck[data-id="${modifierId}"]`).checked = true;
//         }
//     });
// }

function openAddExistingModifiersForEdit() {
    $('#editModifierGroup').modal('hide'); // Close Edit Modifier Group Modal
    $("#addExistingModifiersForm").trigger('reset');
    $('#addExistingModifiersModal').modal('show'); // Open Add Existing Modifiers Modal

    loadModifiersForEdit(1, $("#pageSizesForAddExistingModifiersEdit").val());

    // For check all existing Modifiers when append the selected checkboxes
    selectedModifiers.forEach(function (modifierName, modifierId) {
        if (existingModifiersForAdd.includes(modifierId)) {
            if (modifierId !== undefined) {
                document.querySelector(`.modifiersIdsForAdd[data-id="${modifierId}"]`).checked = true;
            }
        }
    });
}

function loadModifiersForEdit(pageNumber, pageSize) {
    $.ajax({
        url: '/Menu/GetModifiersForExistingModifiersForEdit',
        type: 'GET',
        data: { pageNumber: pageNumber, pageSize: pageSize },
        success: function (response) {
            console.log("Hello");

            $("#existingModifiersForEditGroupTable").empty();
            $('#existingModifiersForEditGroupTable').html(response);
            bindPaginationEventsForEdit();

            // For check all existing Modifiers
            existingModifiers.forEach(function (modifierId) {
                var modifierId = $(`.modifiersIdsForEdit[data-id="${modifierId}"]`).data('id');
                // var modifierInput = $(`.rowModifierCheckForAdd_"${modifierId}"]`);
                // modifierInput.checked = true;
                if (modifierId !== undefined) {
                    document.querySelector(`.modifiersIdsForEdit[data-id="${modifierId}"]`).checked = true;
                }
            }
            );
        },
        error: function () {
            console.error('Failed to load data.');
        }
    });
}

function bindPaginationEventsForEdit() {
    var currentPage = $("#currentPageForExistingModifiersEdit").val();
    var totalPages = $("#totalPagesForExistingModifiersEdit").val();
    var totalItems = $("#totalItemsForExistingModifiersEdit").val();

    $('#prevPageForAddExistingModifiersEdit').off('click').on('click', function () {
        if (currentPage > 1) {
            loadModifiersForEdit(currentPage - 1, $('#pageSizesForAddExistingModifiersEdit').val());
        }
    });

    $('#nextPageForAddExistingModifiersEdit').off('click').on('click', function () {
        if (currentPage < totalPages) {
            currentPage = parseInt(currentPage) + 1;
            loadModifiersForEdit(currentPage, $('#pageSizesForAddExistingModifiersEdit').val());
        }
    });

    $('#pageSizesForAddExistingModifiersEdit').off('change').on('change', function () {
        loadModifiersForEdit(1, $(this).val());
    });
}

var allModifierIdsForEdit = [];
$('.modifiersIdsForEdit').each(function () {
    var modifierId = $(this).data('id');
    if (modifierId !== undefined) {
        allModifierIdsForEdit.push(modifierId);
    }
});

allModifierIdsForEdit.forEach(function (modifierId) {
    $(document).off("change", `.rowModifierCheck_${modifierId}`); // For remove event listners from previous edit modifierGroup
    $(document).on("change", `.rowModifierCheck_${modifierId}`, function () {
        let modifierId = $(this).data('id');
        let modifierName = $(this).data('name');

        if ($(this).is(":checked")) {
            selectedModifiers.set(modifierId, modifierName);
            jsonString.push(modifierId);
            existingModifiers.push(modifierId);
        }
        else {
            selectedModifiers.delete(modifierId);
            jsonString = jsonStringForAdd.filter(id => id !== modifierId);
            existingModifiers = existingModifiers.filter(id => id !== modifierId);
        }
    });
});

$(document).on('click', "#saveAddExistingModifiers", function () {
    $('#addExistingModifiersModal').modal('hide');
    $('#editModifierGroup').modal('show');
    $('#existingModifiersContainer').empty();

    // Append new modifiers
    selectedModifiers.forEach(function (modifierName, modifierId) {
        $('#existingModifiersContainer').append(
            `<div class="d-flex align-items-center modifierInEditModal_${modifierId} border rounded-pill mb-2 me-2">
                <div class="d-flex m-2 existingModifier">${modifierName}</div>
                <button type="button" class="btn-close removeModifierBtnEdit_${modifierId}" data-id="${modifierId}" aria-label="Close"></button>
            </div>`
        );

        console.log("selectedModifiers", selectedModifiers);
        console.log("existingModifiers", existingModifiers);
        console.log("selectedModifiers", selectedModifiers);

        $(document).on('click', `.removeModifierBtnEdit_${modifierId}`, function (e) {
            e.preventDefault();
            let unCheckModifierId = $(this).data('id');
            selectedModifiers.delete(unCheckModifierId);
            existingModifiers = existingModifiers.filter(id => id !== unCheckModifierId);
            $(`.modifierInEditModal_${unCheckModifierId}`).remove();
        });
    });


});

//On save modal of addExisting Modifiers in Edit Modifier Group
$(document).on('click', "#saveAddExistingModifiersForAdd", function () {
    $('#addExistingModal').modal('hide');
    $('#addModifierGroupModal').modal('show');
    $('#existingModifiersContainerForAdd').empty();

    // Append new modifiers For Add
    selectedModifiersForAdd.forEach(function (modifierName, modifierId) {
        $('#existingModifiersContainerForAdd').append(
            `<div class="d-flex align-items-center modifierInAddModal_${modifierId} border rounded-pill mb-2 me-2">
                <div class="d-flex m-2 existingModifier">${modifierName}</div>
                <button type="button" class="btn-close removeModifierBtn_${modifierId}" data-id="${modifierId}" aria-label="Close"></button>
            </div>`
        );
        $(document).on('click', `.removeModifierBtn_${modifierId}`, function (e) {
            e.preventDefault();
            let unCheckModifierId = $(this).data('id');
            selectedModifiersForAdd.delete(unCheckModifierId);
            existingModifiersForAdd = existingModifiersForAdd.filter(id => id !== unCheckModifierId);
            $(`.modifierInAddModal_${unCheckModifierId}`).remove();
        });
    });
});

function openNewItemInputFile() {
    const fileUpload = document.getElementById('newItemImage');
    fileUpload.click();
}

// Validation for Image upload in Add Item modal
document.getElementById('newItemImage').addEventListener('change', function () {
    var reader = new FileReader();
    var fileForAdd = this.files[0];
    var allowedExtensions = /(\.jpg|\.jpeg|\.png|\.svg)$/i;
    var messageDivForAdd = document.getElementById('uploadValidationMsgForAddItem');
    if (!allowedExtensions.exec(fileForAdd.name)) {
        if (!messageDivForAdd.classList.contains('text-danger')) {
            messageDivForAdd.classList.add('text-danger');
        }
        document.getElementById("addItemImagePreview").src = "/images/Download/cloud-arrow-up.svg";
        document.getElementById("addItemImagePreview").width = 30;
        messageDivForAdd.textContent = 'Please upload file having extensions .jpeg/.jpg/.png/.svg only';
        return false;
    }

    if (this.files && this.files.length > 0) {
        reader.onload = function (e) {
            document.getElementById("addItemImagePreview").src = e.target.result;
            // document.getElementById("addItemImagePreview").width = 70;
        };
        reader.readAsDataURL(fileForAdd);
        var fileName = this.files[0].name;
        if (messageDivForAdd.classList.contains('text-danger')) {
            messageDivForAdd.classList.remove('text-danger');
        }
        messageDivForAdd.textContent = '';
    }
});

function openEditItemInputFile() {
    const fileUploadForEdit = document.getElementById('editItemImage');
    fileUploadForEdit.click();
}

// Validation for Image upload in Edit Item modal
$(document).on('change','.editItemImage', function () {
    debugger;
    var reader = new FileReader();
    var fileForEdit = this.files[0];
    var allowedExtensions = /(\.jpg|\.jpeg|\.png|\.svg)$/i;
    var messageDivForEdit = document.getElementById('uploadValidationMsgForEditItem');
    if (!allowedExtensions.exec(fileForEdit.name)) {
        if (!messageDivForEdit.classList.contains('text-danger')) {
            messageDivForEdit.classList.add('text-danger');
        }
        document.getElementById("editItemImagePreview").src = "/images/Download/cloud-arrow-up.svg";
        document.getElementById("editItemImagePreview").width = 30;
        messageDivForEdit.textContent = 'Please upload file having extensions .jpeg/.jpg/.png/.svg only';
        return false;
    }

    if (this.files && this.files.length > 0) {
        reader.onload = function (e) {
            document.getElementById("editItemImagePreview").src = e.target.result;
            // document.getElementById("editItemImagePreview").width = 70;
        };
        reader.readAsDataURL(file);
        var fileName = this.files[0].name;
        if (messageDivForEdit.classList.contains('text-danger')) {
            messageDivForEdit.classList.remove('text-danger');
        }
        messageDivForEdit.textContent = '';
    }
});

function openDeleteCategoryModal(categoryId) {
    $("#deleteCategoryModal").modal("show");
    $("#deleteCategoryId").val(categoryId);
    $("#deleteCategoryLink").attr("href", `/Menu/DeleteCategory?categoryId=${categoryId}`);
}

function openAddMenuItemModal() {
    $("#addMenuItemModal").modal("show");
}

function openDeleteItemModal(itemId) {
    $("#deleteItemModal").modal("show");
    $("#deleteItemLink").attr("href", `/Menu/DeleteMenuItem?itemId=${itemId}`);
}

function openDeleteModifierGroupModal(modifierGroupId) {
    debugger;
    $("#deleteModifierGroupModal").modal("show");
    $("#deleteModifierGroupLink").attr("href", `/Menu/DeleteModifierGroup?modifierGroupId=${modifierGroupId}`);
}

function openDeleteModifierModal(modifierId) {
    $("#deleteModifierModal").modal("show");
    // let modifierGroupId = e.target
    $("#deleteModifierLink").attr("href", `/Menu/DeleteMenuItem?itemId=${modifierId}`);
    // $("#deleteModifierLink").attr("href", `/Menu/DeleteModifier?modifierId=${modifierId}&modifierGroupId=${modifierGroupId}`);
}