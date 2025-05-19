// $(document).ready(function () {
//     // Initialize the filter dropdown with the current filter type
//     var urlParams = new URLSearchParams(window.location.search).get('filter');
//     const filterType = urlParams || 'Current Month'; 
//     const filterSelect = document.getElementById('filterSelect');
//     filterSelect.value = filterType;
// });

// function applyFilter(filterType) {
//     const url = `@Url.Action("Index", "Dashboard")?filter=${encodeURIComponent(filterType)}`;
//     window.location.href = url;
    
// }

// const revenueCtx = document.getElementById('myChart').getContext('2d');
// const revenueChart = new Chart(revenueCtx, {
//     type: 'line',
//     data: {
//         labels: @Html.Raw(Json.Serialize(Model.RevenueChartData.Select(d => d.Label))),
//         datasets: [{
//             label: 'Revenue',
//             data: @Html.Raw(Json.Serialize(Model.RevenueChartData.Select(d => d.Value))),
//             borderColor: 'rgba(75, 192, 192, 1)',
//             backgroundColor: 'rgba(75, 192, 192, 0.2)',
//             tension: 0.4,
//             fill: true
//         }]
//     },
//     options: {
//         responsive: true,
//         maintainAspectRatio: false,
//         innerHeight: 350,
//         outerHeight: 350,
//         plugins: {
//             legend: { position: 'top' },
//             title: {
//                 display: true
//             }
//         },
//         scales: {
//             y: {
//                 beginAtZero: true,
//                 ticks: { stepSize: 1 }
//             }
//         }
//     }
// });


// const customerCtx = document.getElementById('customerGrowthChart').getContext('2d');
// const customerGrowthChart = new Chart(customerCtx, {
//     type: 'line',
//     data: {
//         labels: @Html.Raw(Json.Serialize(Model.CustomerGrowthData.Select(d => d.Label))),
//         datasets: [{

//             label: 'Customer Growth',
//             data: @Html.Raw(Json.Serialize(Model.CustomerGrowthData.Select(d => d.Value))),
//             borderColor: 'rgba(255, 99, 132, 1)',
//             backgroundColor: 'rgba(255, 99, 132, 0.2)',
//             tension: 0.4,
//             fill: true
//         }]
//     },
//     options: {
//         responsive: true,
//         maintainAspectRatio: false,
//         innerHeight: 350,
//         outerHeight: 350,
//         plugins: {
//             legend: { position: 'top' },
//             title: {
//                 display: true
//             }
//         },
//         scales: {
//             y: {
//                 beginAtZero: true,
//                 ticks: { stepSize: 1 }
//             }
//         }
//     }
// });