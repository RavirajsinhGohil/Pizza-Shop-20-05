$(document).ready(function () {
        let currentPage = 1;
        let searchTerm = '';
        let pageSize = 5;
        let totalItems = $('#totalItemssss').val() || 5;
        let totalPages = $('#totalPagessss').val() || 1;
        let sortBy = 'Name';
        let sortOrder = 'asc';
        let searchTimeout = 0;
        let currentSortBy = "Name";
        let currentSortOrder = "asc";
        let timeLog = '';

        fetchItems(searchTerm, 1, 5, sortBy, sortOrder, timeLog);

        function fetchItems(search = "", page = 1, pageSizeValue = 5, sortBy, sortOrder, timeLog) {
            $.ajax({
                url: 'GetCustomerByPagination',
                type: 'GET',
                data: {
                    searchTerm: search,
                    page: page,
                    pageSize: pageSizeValue,
                    sortBy: sortBy,
                    sortOrder: sortOrder,
                    timeLog: timeLog,
                    CustomFromDate : $('#fromDateFilter').val(),
                    CustomToDate : $('#toDateFilter').val()
                },
                success: function (data) {
                    $("#customerList").html(data);
                    let $data = $(data);
                    totalPages = parseInt($('#totalPageess').val()) || 1;
                    totalItems = parseInt($('#totalItems').val()) || 5;

                    //  Update UI with new data
                    $("#customerList").html(data);

                    //  Update current page & page size
                    currentPage = page;
                    pageSize = pageSizeValue;

                    // @* updatePaginationButtons(); *@
                },
                error: function (xhr, status, error, data) {
                }
            });
        }

        //  Handle Previous Page Click
        $(document).on('click', '#prevPage', function () {
            if (currentPage > 1) {
                fetchItems(searchTerm, currentPage - 1, pageSize, currentSortBy, currentSortOrder, timeLog);
            }
        });

        //  Handle Next Page Click
        $(document).on('click', '#nextPage', function () {
            if (currentPage < totalPages) {
                fetchItems(searchTerm, currentPage + 1, pageSize, currentSortBy, currentSortOrder, timeLog);
            }
        });

        $(document).on('change', '#pageSizes', function () {
            pageSize = parseInt($(this).val()) || 2;
            fetchItems(searchTerm, 1, pageSize, currentSortBy, currentSortOrder, timeLog);
        });

        $(document).on('keyup', '#searchBox', function () {
            clearTimeout(searchTimeout);
            searchTerm = $(this).val();

            searchTimeout = setTimeout(() => {
                fetchItems(searchTerm, currentPage, pageSize, sortBy, sortOrder, timeLog);
            }, 100);
        });

        //For sorting according to sortBy and sortOrder
        $(document).on("click", ".sort-items", function (e) {
            debugger;
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

            fetchItems(searchTerm, 1, pageSize, sortBy, currentSortOrder, timeLog)
            sortOrder = currentSortOrder === "asc" ? "desc" : "asc";
        });

        //For filter data according to time log
        $(document).on('change', "#timeLog", function (e) {
            timeLog = $(this).val();
            if (timeLog === "Custom Date") {
                var MyModal = new bootstrap.Modal(document.getElementById('customDateModal'));
                MyModal.show();

                $("#fromDateFilter").on('change', function () {
                    var fromDate = $(this).val();
                    if (fromDate) {
                        // Set minimum selectable date for To Date
                        $("#toDateFilter").attr("min", fromDate);
                    } else {
                        $("#toDateFilter").removeAttr("min");
                    }
                });

                $("#toDateFilter").on('change', function () {
                    var toDate = $(this).val();
                    if (toDate) {
                        // Set maximum selectable date for From Date
                        $("#fromDateFilter").attr("max", toDate);
                    } else {
                        $("#fromDateFilter").removeAttr("max");
                    }
                });

                //  Handle Submit Button Click
                $(document).on('click', '#customDateSubmit', function () {
                    debugger;
                    var fromDate = $('#fromDateFilter').val();
                    var toDate = $('#toDateFilter').val();
                    timeLog = "Custom Date";

                    if (fromDate && toDate) {
                        fetchItems(searchTerm, 1, pageSize, sortBy, currentSortOrder, timeLog);
                        MyModal.hide();
                    }
                    else {
                        toastr.error("Please select both dates.");
                    }
                });
            }
            else {
                fetchItems(searchTerm, 1, pageSize, sortBy, currentSortOrder, timeLog)
            }
        });

        $(document).on('click', "#exportCustomersBtn", function () {
            var searchTerm = $("#searchBox").val() || "";
            var timeLog = $("#timeLog").val();

            var url = `/Customers/ExportCustomersInExcel?searchTerm=${encodeURIComponent(searchTerm)}&timeLog=${encodeURIComponent(timeLog)}`;
            window.location.href = url;
        });

        $(document).on("click", '.customerHistoryBtn', function () {
            let customerId = $(this).data('id');

            $.ajax({
                url: '@Url.Action("CustomerHistory")',
                type: 'GET',
                data: {
                    customerId: customerId
                },
                success: function (data) {
                    $("#customerName").text(data.firstname);
                    $("#customerPhone").text(data.phone);
                    $("#maxOrder").text(data.maxOrder);
                    $("#averageBill").text(data.averageBill);
                    $("#comingSince").text(data.comingSince);
                    console.log(data.comingSince);
                    $("#visitCount").text(data.visits);
                    // Clear previous rows if any
                    $("#customerOrdersTable tbody").empty();
                    // Append new rows
                    data.customerOrders.forEach(customerOrder => {
                        const row = `
                                    <tr>
                                        <td>${customerOrder.createdat}</td>
                                        <td>${customerOrder.orderType}</td>
                                        <td>${customerOrder.status}</td>
                                        <td>${customerOrder.itemsCount}</td>
                                        <td>${customerOrder.totalAmount}</td>
                                    </tr>
                                `;
                        $("#customerOrdersTable tbody").append(row);
                    });
                    var MyModal = new bootstrap.Modal(document.getElementById('customerHistoryModal'));
                    MyModal.show();
                },
                error: function (xhr, status, error, data) {
                    console.error("AJAX Error:", status, error);
                    console.error("Data :", data);
                }
            });
        });
    });