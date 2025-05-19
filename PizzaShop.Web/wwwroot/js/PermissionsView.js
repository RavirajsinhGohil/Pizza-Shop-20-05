$(document).ready(function () {
    //Check if all checkboxes are checked then selectAll checkbox will be checked
    // (".custom-switch").any(function () {


    var urlPramas = new URLSearchParams(window.location.search);
    var role = urlPramas.get("role");

    var row = $(".custom-switch").closest("tr");
    var allSwitches = row.find(".custom-switch");
    var rowCheckbox = row.find(".row-checkbox");
    console.log("Row Checkbox", rowCheckbox);

    // rowCheckbox.each(function () {
    //     var eachRowCheckboxes = $(this).closest('custom-switch');
    //     if (eachRowCheckboxes.is(":checked")) {
    //         $(this).prop("checked", true);
    //     } else {
    //         $(this).prop("checked", false);
    //     }
    // });

    $(".permission_table tr").each(function () {
        var $row = $(this);
        var $allSwitches = $row.find(".custom-switch");
        var $rowCheckbox = $row.find(".row-checkbox");

        // Check if at least one custom-switch is checked in the row
        if ($allSwitches.filter(":checked").length > 0) {
            $rowCheckbox.prop("checked", true);
        } else {
            $rowCheckbox.prop("checked", false);
        }
    });

    // Check if Can Add/Edit check box is checked then check the can view checkbox
    $(".custom-switch").each(function () {
        var $row = $(this).closest("tr");
        var $canViewSwitch = $row.find(".custom-switch[data-type='CanView']");
        var $canAddEditSwitch = $row.find(".custom-switch[data-type='CanAddEdit']");

        if ($canAddEditSwitch.is(":checked")) {
            $canViewSwitch.prop("checked", true);
        }
    });


    // if (allSwitches.filter(":checked").length > 0) {
    //     rowCheckbox.prop("checked", true);
    //     allRowCheckboxes = $(".row-checkbox");
    //     allChecked = allRowCheckboxes.length === allRowCheckboxes.filter(":checked").length;
    //     $("#selectAll").prop("checked", allChecked);
    // } else {
    //     rowCheckbox.prop("checked", false);
    //     allRowCheckboxes = $(".row-checkbox");
    //     allChecked = allRowCheckboxes.length === allRowCheckboxes.filter(":checked").length;
    //     $("#selectAll").prop("checked", allChecked);

    // }

    $(document).on('change', "#selectAll", function () {
        var isChecked = $(this).is(":checked");
        // Check for RoleAndPermission toggel 
        $(".row-checkbox").each(function () {
            var row = $(this).closest('tr');
            if (row.find('td:nth-child(2)').text() !== "RoleAndPermission") {
                $(this).prop("checked", isChecked);
            }
        });

        $(".custom-switch").each(function () {
            var row = $(this).closest('tr');
            if (row.find('td:nth-child(2)').text() !== "RoleAndPermission") {
                $(this).prop("checked", isChecked);
            }
        });
    });

    // Check if all row-checkboxes are checked
    var allRowCheckboxes = $(".row-checkbox");
    var allChecked = allRowCheckboxes.length === allRowCheckboxes.filter(":checked").length;
    var allCustomSwitch = $(".custom-switch");
    var allCustomChecked = allCustomSwitch.length === allCustomSwitch.filter(":checked").length;
    $("#selectAll").prop("checked", allCustomChecked);


    $(".row-checkbox").on('change', function () {
        var isChecked = $(this).is(":checked");
        $(this).closest("tr").find(".custom-switch").prop("checked", isChecked);

        allRowCheckboxes = $(".row-checkbox");
        allChecked = allRowCheckboxes.length === allRowCheckboxes.filter(":checked").length;
        $("#selectAll").prop("checked", allChecked);
    });

    $(".custom-switch").change(function () {
        var row = $(this).closest("tr");
        var allSwitches = row.find(".custom-switch");
        var rowCheckbox = row.find(".row-checkbox");

        if (allSwitches.filter(":checked").length > 0) {
            var abc = allSwitches.filter(":checked").length;
            rowCheckbox.prop("checked", true);

            allRowCheckboxes = $(".row-checkbox");
            allChecked = allRowCheckboxes.length === allRowCheckboxes.filter(":checked").length;
            $("#selectAll").prop("checked", allChecked);
        } else {
            var abc = allSwitches.filter(":checked").length;
            rowCheckbox.prop("checked", false);

            allRowCheckboxes = $(".row-checkbox");
            allChecked = allRowCheckboxes.length === allRowCheckboxes.filter(":checked").length;
            $("#selectAll").prop("checked", allChecked);
        }
    });

    $("#saveBtn").click(function () {
        var updatedPermissions = [];

        $(".custom-switch").each(function () {
            var permissionId = $(this).data("id");
            var Rolename = $(this).data("role")
            var permissionName = $(this).data("name");
            var type = $(this).data("type");
            var isChecked = $(this).is(":checked");

            if (permissionId && Rolename && type) {
                var existing = updatedPermissions.find(p => p.PermissionId === permissionId);
                if (!existing) {
                    existing = {
                        Rolename: Rolename,
                        PermissionId: permissionId,
                        PermissionName: permissionName,
                        CanView: false,
                        CanAddEdit: false,
                        CanDelete: false
                    };
                    updatedPermissions.push(existing);
                }
                existing[type] = isChecked;
            }
        });

        console.log("Sending Data:", JSON.stringify(updatedPermissions)); // **Important**

        $.ajax({
            url: "/User/Permissions?role=" + encodeURIComponent(role),
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(updatedPermissions),
            success: function (response) {
                if (response.success) {
                    toastr.success("Permissions updated successfully.");
                } else {
                    toastr.error("Failed to update permissions: ");
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", status, error);
                toastr.error("Error updating permissions.");
            }
        });
    });
});