// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// document.addEventListener('DOMContentLoaded', function () {
//     const loginForm = document.getElementById('loginForm');
//     const email = document.getElementById('email');
//     const password = document.getElementById('password');
//     const emailError = document.getElementById('emailError');
//     const passwordError = document.getElementById('passwordError');


//     loginForm.addEventListener('submit', function (e) {
//         e.preventDefault();
//         let isValid = true;


//         if (!email.value.trim()) {
//             emailError.classList.remove('d-none');
//             isValid = false;
//         }


//         if (!password.value.trim()) {
//             passwordError.classList.remove('d-none');
//             isValid = false;
//         }

//         if (isValid) {
//             alert('Login successful!');
//         }
//     });
// });

function openinputFile() {
    const fileUpload = document.getElementById('inputFile');
    fileUpload.click();
}

// const main_check_box = document.getElementById('mainCheckBox');
// main_check_box.addEventListener('change', function() {
//     if(main_check_box.ariaChecked){
//         const sub_check_box = document.querySelectorAll('subCheckBox');
//         sub_check_box.ariaChecked;
//     } 
// });

function sidebarResize() {
    let sidebar = document.getElementById("sidebar");
    let add_user_body = document.getElementById("add_user_body");

    if(window.innerWidth >= 768) {
        sidebar.classList.add("show");
        sidebar.classList.add("d-lg-block");
        sidebar.style.transform = "none";
        sidebar.style.visibility = "visible";
        add_user_body.style.marginLeft = "250px";
    }
    else {
        sidebar.classList.remove("show");
        sidebar.classList.remove("d-lg-block");
        sidebar.style.transform = "";
        sidebar.style.visibility = "";
        add_user_body.style.marginLeft = "0";
    }
}

// document.getElementById("LogoutBtn").addEventListener("click", function() => {
//     var modal = document.getElementById("modal");
//     modal.classList.remove("d-none");
// });

