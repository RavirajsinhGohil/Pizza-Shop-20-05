$(document).ready(function () {
    let currentPage = parseInt("@Model.CurrentPage") || 1;
    let searchTerm = '';
    let pageSize = parseInt("@Model.PageSize") || 5;
    let fromRecord = 1;
    let totalItems = $('#totalItemssss').val() || 5;
    let totalPages = $('#totalPagessss').val() || 1;
    let sortBy = 'OrderId';
    let sortOrder = 'asc';
    let searchTimeout = 0;
    let currentSortBy = "OrderId";
    let currentSortOrder = "asc";
    let statusLog = '';
    let timeLog = '';
    let fromDate = '';
    let toDate = '';

    $(document).on('click', "#searchOrdersBtn", function () {
        searchTerm = $("#searchBox").val().trim();
        statusLog = $("#statusLog").val();
        timeLog = $("#timeLog").val();
        fromDate = $("#fromDate").val();
        toDate = $("#toDate").val();

        fetchItems(searchTerm, 1, pageSize, sortBy, sortOrder, statusLog, timeLog, fromDate, toDate);
    });

    fetchItems(searchTerm, 1, pageSize, sortBy, sortOrder, statusLog, timeLog, fromDate, toDate);

    function fetchItems(search = "", page = 1, pageSizeValue = 5, sortBy, sortOrder, statusLog, timeLog, fromDate, toDate) {

        $.ajax({
            url: '/Orders/GetOrderByPagination',
            type: 'GET',
            data: {
                searchTerm: search,
                page: page,
                pageSize: pageSizeValue,
                sortBy: sortBy,
                sortOrder: sortOrder,
                statusLog: statusLog,
                timeLog: timeLog,
                fromDate: fromDate,
                toDate: toDate
            },
            success: function (data) {
                $("#orderList").html(data);
                let $data = $(data);
                totalPages = parseInt($('#totalPageess').val()) || 1;
                totalItems = parseInt($('#totalItems').val()) || 5;

                //  Update UI with new data
                $("#orderList").html(data);

                //  Update current page & page size
                currentPage = page;
                pageSize = pageSizeValue;

                // updatePaginationButtons();
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", status, error);
            }
        });
    }

    //  Handle Previous Page Click
    $(document).on('click', '#prevPage', function () {
        if (currentPage > 1) {
            fetchItems(searchTerm, currentPage - 1, pageSize, sortBy, sortOrder, statusLog, timeLog, fromDate, toDate);
        }
    });

    $(document).on('click', '#nextPage', function () {
        if (currentPage < totalPages) {
            fetchItems(searchTerm, currentPage + 1, pageSize, sortBy, sortOrder, statusLog, timeLog, fromDate, toDate);
        }
    });

    $(document).on('change', '#pageSizes', function () {
        pageSize = parseInt($(this).val()) || 5;
        fetchItems(searchTerm, 1, pageSize, sortBy, sortOrder, statusLog, timeLog, fromDate, toDate);
    });

    $(document).on('keyup', '#searchBox', function () {
        clearTimeout(searchTimeout);
        searchTerm = $(this).val();
    });

    //For sorting according to sortBy and sortOrder
    $(document).on("click", ".sort-items", function (e) {
        e.preventDefault();

        currentSortBy = $(this).data("column");
        // @* let pageNumber = $(this).data("page"); *@

            if (sortBy === currentSortBy) {
            currentSortOrder = currentSortOrder === "asc" ? "desc" : "asc";
        }
        else {
            sortBy = currentSortBy;
            sortOrder = "asc";
        }

        fetchItems(searchTerm, 1, pageSize, sortBy, currentSortOrder, statusLog, timeLog, fromDate, toDate)
        sortOrder = currentSortOrder === "asc" ? "desc" : "asc";
    });

    //For filter data according to status log
    $(document).on('change', "#statusLog", function (e) {
        statusLog = $(this).val();
    });

    //For filter data according to time log
    $(document).on('change', "#timeLog", function (e) {
        timeLog = $(this).val();
    });

    //For clicking on clear button to reset filters
    $(document).on('click', '#resetFilters', function () {
        $("#searchBox").val("");
        $("#statusLog").val("All Status");
        $("#timeLog").val("All Time");
        $("#fromDate").val('');
        $("#toDate").val('');

        searchTerm = '';
        statusLog = 'All Status';
        timeLog = 'All Time';
        fromDate = '';
        toDate = '';
        sortBy = 'OrderId';
        sortOrder = 'asc';
        pageSize = parseInt($("#pageSizes").val()) || 5;
        currentPage = 1;

        fetchItems('', 1, pageSize, sortBy, sortOrder, "All Status", "All Time", '', '');
    });

    $(document).on('click', "#exportOrdersBtn", function () {
        var searchTerm = $("#searchBox").val()|| "";
        var statusLog = $("#statusLog").val();
        var timeLog = $("#timeLog").val();

        var url = `/Orders/ExportOrdersInExcel?searchTerm=${encodeURIComponent(searchTerm)}&statusLog=${encodeURIComponent(statusLog)}&timeLog=${encodeURIComponent(timeLog)}`;

        window.location.href = url;
    });
});

$("#fromDate").on('change', function () {
    var fromDate = $(this).val();
    if (fromDate) {
        // Set minimum selectable date for To Date
        $("#toDate").attr("min", fromDate);
    } else {
        $("#toDate").removeAttr("min");
    }
});

$("#toDate").on('change', function () {
    var toDate = $(this).val();
    if (toDate) {
        // Set maximum selectable date for From Date
        $("#fromDate").attr("max", toDate);
    } else {
        $("#fromDate").removeAttr("max");
    }
});
