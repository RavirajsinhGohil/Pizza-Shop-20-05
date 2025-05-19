$(document).ready(function () {

    // get countries on page load
    $.getJSON('/GetData/GetCountries', function (data) {
        // set the selected country, state, and city from the model
        var country = $("#hiddenCountry").val();
        var state = $("#hiddenState").val();
        var city = $("#hiddenCity").val();
        $("#Country").html(`<option selected>${country}</option>`);
        $("#State").html(`<option selected>${state}</option>`);
        $("#City").html(`<option selected>${city}</option>`);
        $.each(data, function (index, country) {
            $("#Country").append(`<option value="${country.countryId}">${country.countryName}</option>`);
        });
    });

    // get states when a country is selected
    $("#Country").change(function () {
        var countryId = $(this).val();
        $("#State").html('<option value="">Select State</option>').prop("disabled", true);
        $("#City").html('<option value="">Select City</option>').prop("disabled", true);

        if (countryId) {
            $.getJSON(`/GetData/GetStates?countryId=${countryId}`, function (data) {
                $("#State").prop("disabled", false);
                $.each(data, function (index, state) {
                    $("#State").append(`<option value="${state.stateId}">${state.stateName}</option>`);
                });
            });
        }
    });

    // get cities when a state is selected
    $("#State").change(function () {
        var stateId = $(this).val();
        $("#City").html('<option value="">Select City</option>').prop("disabled", true);

        if (stateId) {
            $.getJSON(`/GetData/GetCities?stateId=${stateId}`, function (data) {
                $("#City").prop("disabled", false);
                $.each(data, function (index, city) {
                    $("#City").append(`<option value="${city.cityId}">${city.cityName}</option>`);
                });
            });
        }
    });

    // document.getElementById("inputFile").addEventListener("change", function (event) {
    //     debugger
    //     var file = event.target.files[0];
    //     if (file) {
    //         var reader = new FileReader();
    //         reader.onload = function (e) {
    //             document.getElementById("ProfileImagePreview").src = e.target.result;
    //             document.getElementById("ProfileImagePreview").width = 70;
    //         };
    //         reader.readAsDataURL(file);
    //     }
    // });
});

document.getElementById('inputFile').addEventListener('change', function () {
    var reader = new FileReader();
    var file = this.files[0];
    var allowedExtensions = /(\.jpg|\.jpeg|\.png|\.svg)$/i;
    var messageDiv = document.getElementById('uploadValidationMsg');
    if (!allowedExtensions.exec(file.name)) {
        if (!messageDiv.classList.contains('text-danger')) {
            messageDiv.classList.add('text-danger');
        }
        document.getElementById("ProfileImagePreview").src = "/images/Download/cloud-arrow-up.svg";
        document.getElementById("ProfileImagePreview").width = 30;
        messageDiv.textContent = 'Please upload file having extensions .jpeg/.jpg/.png/.svg only';
        return false;
    }

    if (this.files && this.files.length > 0) {
        reader.onload = function (e) {
            document.getElementById("ProfileImagePreview").src = e.target.result;
            document.getElementById("ProfileImagePreview").width = 70;
        };
        reader.readAsDataURL(file);
        var fileName = this.files[0].name;
        if (messageDiv.classList.contains('text-danger')) {
            messageDiv.classList.remove('text-danger');
        }
        messageDiv.textContent = '';
    }
});