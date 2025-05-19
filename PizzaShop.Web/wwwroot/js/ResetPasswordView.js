document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".toggle-password").forEach(icon => {
        icon.addEventListener("click", function () {
            const targetId = this.getAttribute("data-target");
            const passwordField = document.getElementById(targetId);

            // Toggle password visibility
            if (passwordField.type === "password") {
                passwordField.type = "text";
                this.classList.remove("fa-eye");
                this.classList.add("fa-eye-slash");
            } else {
                passwordField.type = "password";
                this.classList.remove("fa-eye-slash");
                this.classList.add("fa-eye");
            }
        });
    });
});

const eyebtn = document.getElementById("togglePassword");
const passfield = document.getElementById('password');
togglePassword.addEventListener('click', function (e) {
    const type = passfield.getAttribute('type') === 'password' ? 'text' : 'password';
    passfield.setAttribute('type', type);
    this.classList.toggle('fa-eye-slash');
});
const eyebtn2 = document.getElementById("togglePassword2");
const passfield2 = document.getElementById('password2');
togglePassword2.addEventListener('click', function (e) {
    const type = passfield.getAttribute('type') === 'password' ? 'text' : 'password';
    passfield.setAttribute('type', type);
    this.classList.toggle('fa-eye-slash');
});