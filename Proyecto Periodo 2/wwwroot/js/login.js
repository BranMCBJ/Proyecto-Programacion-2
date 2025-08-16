// Login functionality
function togglePassword() {
    const input = document.getElementById('password-login');
    const icon = document.querySelector('.input-group-text i'); // Selecciona el ícono del ojo
    if (input.type === 'password') {
        input.type = 'text';
        icon.classList.remove('bi-eye-slash');
        icon.classList.add('bi-eye');
    } else {
        input.type = 'password';
        icon.classList.remove('bi-eye');
        icon.classList.add('bi-eye-slash');
    }
}