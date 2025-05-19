
$(document).ready(function () {
    TableListPaginationAjax();
});

function TableListPaginationAjax(pageSize, pageNumber) {
    let id = $("#section-list .category-active-option").attr("section-id");
    let searchkeyword = $("#taxitem-search-field").val();

    $.ajax({
        url: "/Tax/GetTaxList",
        data: { 'pageSize': pageSize, 'pageNumber': pageNumber, 'searchKeyword': searchkeyword },
        type: "GET",
        dataType: "html",
        success: function (data) {
            $("#taxlistcontainer").html(data);
            // @* @ onPartialViewLoaded();  @ *@
        },
        error: function () {
            $("#taxlistcontainer").html('An error has occurred');
        }
    });
}

document.getElementById("taxitem-search-field").addEventListener('keyup', () => {
    TableListPaginationAjax();
})

// @* @ set data for edit tax @ *@
function setEditTaxData(ele) {
    var c = JSON.parse(ele.getAttribute("item-obj"));

    var editsectionitem = document.getElementById("EditTaxmodal");
    editsectionitem.querySelector("#taxIdForEdit").value = c.taxId;
    editsectionitem.querySelector("#taxNameForEdit").value = c.taxName;
    editsectionitem.querySelector("#typeOfTaxForEdit").value = c.type;
    editsectionitem.querySelector("#taxAmountForEdit").value = c.taxAmount;
    editsectionitem.querySelector("#isEnabledForEdit").checked = c.isenable;
    editsectionitem.querySelector("#isDefaultForEdit").checked = c.isdefault;
}

// @* @ Delete Tax @ *@
$(document).on('click', '#DeleteTaxBtn', function (e) {
    var MyModal = new bootstrap.Modal(document.getElementById("deletetaxmodal"));
    MyModal.show();
    var taxId = $(this).data('id');
    var deleteBtn = document.getElementById("deleteTaxBtn");
    deleteBtn.href = `Tax/DeleteTax?id=${taxId}`;
});