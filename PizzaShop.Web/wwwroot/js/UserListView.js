// let searchTimeout;

//     function searchUsers() {
//         clearTimeout(searchTimeout);

//         searchTimeout = setTimeout(function () {
//             let searchTerm = $('#searchTerm').val();
//             loadUsers(1, '@Model.SortBy', '@Model.SortOrder', $('#itemsPerPage').val(), searchTerm);
//         }, 500); 
//     }

//     function loadUsers(page, sortBy, sortOrder, pageSize, searchTerm) {
//         $.ajax({
//             url: 'UserList' ,
//             type: 'GET',
//             data: {
//                 page: page,
//                 pageSize: pageSize || $('#itemsPerPage').val(),
//                 searchTerm: searchTerm || $('#searchTerm').val(),   
//                 sortBy: sortBy || '@Model.SortBy',
//                 sortOrder: sortOrder || '@Model.SortOrder'
//             },
//             headers: { "X-Requested-With": "XMLHttpRequest" }, 
//             success: function (data) {
//                 $("#userListContainer").html(data);
//             },
//             error: function (xhr, status, error) {
//                 console.error("Error: ", status, error);
//                 console.error(xhr.responseText);
//             }
//         });
//     }