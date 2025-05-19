document.getElementById('forgotPasswordLink').addEventListener('click', function () {
    var email = document.getElementById("email").value;
    if(email != "")
    {
        this.href = `Login/ForgotPassword?email=${encodeURIComponent(email)}`;
    }
    else
    {
        this.href = `Login/ForgotPassword`;
    }
});

const eyebtn = document.getElementById("togglePassword");
const passfield = document.getElementById('password');
togglePassword.addEventListener('click', function (e) {
    const type = passfield.getAttribute('type') === 'password' ? 'text' : 'password';
    passfield.setAttribute('type', type);
    this.classList.toggle('fa-eye-slash');
});