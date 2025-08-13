// Register functionality
function togglePassword() {
    const input = document.getElementById('password');
    const icon = document.querySelector('.input-group-text i');
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

function previewImage(event) {
    const file = event.target.files[0];
    const preview = document.getElementById('imagePreview');

    if (file && file.type.startsWith("image/")) {
        const reader = new FileReader();
        reader.onload = function (e) {
            preview.innerHTML = `<img src="${e.target.result}" class="img-fluid w-100 h-100 object-fit-cover rounded" alt="Imagen previa">`;
        };
        reader.readAsDataURL(file);
    }
}