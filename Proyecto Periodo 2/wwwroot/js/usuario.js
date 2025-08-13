// Usuario functionality
function toggleDropdown() {
    document.getElementById('dropdownContent').classList.toggle('show');
}

function selectRole(rol) {
    // Cambiar el texto visible
    document.getElementById('rolSeleccionado').textContent = rol;

    // Establecer el valor del input oculto
    document.getElementById('rolInput').value = rol;

    // Cerrar el dropdown
    document.getElementById('dropdownContent').classList.remove('show');
}

// Cerrar el dropdown si se hace clic fuera
window.onclick = function(event) {
    if (!event.target.matches('.dropdown-btn') && !event.target.closest('.dropdown-content')) {
        var dropdowns = document.getElementsByClassName("dropdown-content");
        for (let i = 0; i < dropdowns.length; i++) {
            dropdowns[i].classList.remove('show');
        }
    }
}

function previewImageEdit(event) {
    const file = event.target.files[0];
    const profileImage = document.querySelector('.profile-image');

    if (file && file.type.startsWith("image/")) {
        const reader = new FileReader();
        reader.onload = function (e) {
            profileImage.src = e.target.result;
        };
        reader.readAsDataURL(file);
    }
}

function previewImageCreate(event) {
    const file = event.target.files[0];
    const imageContainer = document.querySelector('#imagePreview');

    if (file && file.type.startsWith("image/")) {
        const reader = new FileReader();
        reader.onload = function (e) {
            imageContainer.innerHTML = `<img src="${e.target.result}" class="img-fluid w-100 h-100 object-fit-cover rounded" alt="Imagen previa" />`;
        };
        reader.readAsDataURL(file);
    }
}