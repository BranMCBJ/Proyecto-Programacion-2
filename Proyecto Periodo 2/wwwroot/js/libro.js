// Libro functionality
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