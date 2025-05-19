// @* Pagination section *@

// @* -Pagination For Menu Items List *@

function TableListPaginationAjax(pageSize, pageNumber, sectionid) {
    // Get the dropdown element
    let id = $("#section-list ").attr("section-id");
    let searchkeyword = $("#tableitem-search-field").val();

    $.ajax({
        url: "/Section/GetDiningTableList",
        data: { 'id': sectionid, 'pageSize': pageSize, 'pageNumber': pageNumber, 'searchKeyword': searchkeyword },
        type: "GET",
        dataType: "html",
        success: function (data) {
            let $partialView = $(data);

            let checkboxes = $partialView.find(".tablelist_inner_checkbox");

            // // When main checkbox is clicked
            // maincb.on("change", function () {
            //     checkboxes.prop("checked", this.checked);
            // });

            // checkboxes.on("change", function () {
            //     if (!this.checked) {
            //         maincb.prop("checked", false);
            //     } else if (checkboxes.length === checkboxes.filter(":checked").length) {
            //         maincb.prop("checked", true);
            //     }
            // });

            $('#diningtablelistcontainer').html($partialView);
        },
        error: function () {
            $("#diningtablelistcontainer").html('An error has occurred');
        }
    });
}

document.getElementById("tableitem-search-field").addEventListener('keyup', () => {
    TableListPaginationAjax();
});

function loadsection(ele) {
    var id = ele?.getAttribute("section-id");
    var selectedSection = $("#loadFunctionParameter").data('selected-section') ?? 1;

    if (id == null) {
        id = selectedSection;
    }

    $.ajax({
        url: "/Section/GetSections",
        type: "GET",
        data: { id: id },
        success: function (data) {
            $('#section-list').html(data);
            $('#section-list-for-smallscreen').html(data);

            const activeSection = document.querySelector("#section-list .category-active-option");
            const sectionid = activeSection.getAttribute("section-id");
        }
    });

    $.ajax({
        url: '/Section/GetDiningTableList',
        type: 'GET',
        data: { id: id },
        success: function (data) {
            let $partialView = $(data);

            // let maincb = $("#main_table_checkbox");
            let checkboxes = $(".modifieritemcheckbox");
            // // When main checkbox is clicked
            // maincb.on("change", function () {
            //     checkboxes.prop("checked", this.checked); // Set all inner checkboxes same as main
            // });

            // If any inner checkbox is unchecked, uncheck the main checkbox
            // checkboxes.on("change", function () {
            //     if (!this.checked) {
            //         maincb.prop("checked", false);
            //     } else if (checkboxes.length === checkboxes.filter(":checked").length) {
            //         maincb.prop("checked", true);
            //     }
            // });

            $('#diningtablelistcontainer').html($partialView);
        }
    });
}

$(document).on("click", "#main_table_checkbox", function () {
    const checkboxes = document.querySelectorAll(".tablelist_inner_checkbox");
    checkboxes.forEach(checkbox => {
        checkbox.checked = this.checked; // Set all inner checkboxes same as main
    });
});

// @* set edit data for section *@
function setEditSectionData(ele) {
    var c = JSON.parse(ele.getAttribute("item-obj"));
    var editsectionitem = document.getElementById("Editsectionmodal");
    editsectionitem.querySelector("#Sectionid").value = c.sectionId;
    editsectionitem.querySelector("#SectionName").value = c.sectionName;
    editsectionitem.querySelector("#Description").value = c.description;
}

// @* set delete data for section *@

function setDeleteSectionId(element) {
    var Id = element.getAttribute("section-id");
    var deleteBtn = document.getElementById("deleteSectionBtn");
    deleteBtn.href = `/Section/DeleteSection?id=${Id}`;
}

// @* set edit data for table *@
function setEditTabledata(ele) {
    var c = JSON.parse(ele.getAttribute("item-obj"));
    var editTableItem = document.getElementById("EditTablemodal");
    editTableItem.querySelector("#NameofTableforedit").value = c.name;
    editTableItem.querySelector("#TableidForEdit").value = c.tableId;
    editTableItem.querySelector("#capacityoftableforedit").value = c.capacity;
    editTableItem.querySelector("#statusoftableforedit").value = c.status ? "Available" : "Occupied";
    editTableItem.querySelector("#sectionidforedit").value = c.sectionId;
}

// @* Delete single Dining Table *@
function setDeleteTableData(element) {
    var Id = element.getAttribute("table-id");
    var deleteBtn = document.getElementById("deleteTableBtn");
    deleteBtn.href = `/Section/DeleteTable?id=${Id}`;
}

$(document).on('change', '.tablelist_inner_checkbox', function () {
    // Check if all checkboxes are checked
    let total = $(".tablelist_inner_checkbox").length;
    let checked = $(".tablelist_inner_checkbox:checked").length;

    // If all individual checkboxes are checked, check the master checkbox
    // Otherwise, uncheck it
    if (total > 0 && total === checked) {
        $("#main_table_checkbox").prop('checked', true);
    } else {
        $("#main_table_checkbox").prop('checked', false);
    }
});

// @* Mass Delete Of Table *@
$("#deletemultipleTableBtn").click(function (e) {
    var idlist = [];
    const checkboxes = document.querySelectorAll(".tablelist_inner_checkbox");

    checkboxes.forEach(checkbox => {
        if (checkbox.checked) {
            idlist.push(checkbox.value);
        }
    });

    $.ajax({
        url: "/Section/DeleteTables",
        method: "POST",
        data: {
            ids: idlist
        },
        success: function (response) {
            const activeSection = document.querySelector("#section-list .category-active-option");
            const sectionid = activeSection.getAttribute("section-id");
            window.location.href = '/Section/Index?id=' + sectionid;
        },
        error: function (xhr, status, error) {
            console.error("Error deleting items:", error);
        }
    });
});

$("#DeleteTables").on("click", function (e) {
    var selectedItems = document.querySelectorAll(".tablelist_inner_checkbox:checked");
    if (selectedItems.length === 0) {
        e.preventDefault();
        toastr.error("Please select table(s) to delete.");
    }
    else {
        var MyModal = new bootstrap.Modal(document.getElementById('deletemultipletablemodal'));
        MyModal.show();
    }
});