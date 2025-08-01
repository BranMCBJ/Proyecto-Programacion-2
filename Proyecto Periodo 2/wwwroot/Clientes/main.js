// Collapse de la card
const collapse = document.getElementById('cardExpandida');
const elementoOculto = document.getElementById('preview');

if (collapse && elementoOculto) {
    collapse.addEventListener('show.bs.collapse', function () {
        elementoOculto.classList.add('d-none');
    });

    collapse.addEventListener('hide.bs.collapse', function () {
        elementoOculto.classList.remove('d-none');
    });
}

// Vista previa de imagen en el modal
document.getElementById('inputFoto').addEventListener('change', function (e) {
    const file = e.target.files[0];
    const img = document.getElementById('previewImg');
    const placeholder = document.getElementById('imgPlaceholder');

    if (file) {
        const reader = new FileReader();
        reader.onload = function (ev) {
            img.src = ev.target.result;
            img.style.display = 'block';
            placeholder.style.display = 'none';
        }
        reader.readAsDataURL(file);
    } else {
        img.src = '';
        img.style.display = 'none';
        placeholder.style.display = 'flex';
    }
});

// Cambiar el texto del modal según el origen
document.querySelectorAll('.btn-editar-cliente').forEach(btn => {
    btn.addEventListener('click', function () {
        document.getElementById('clienteModalTitulo').textContent = 'Editar Cliente';
    });
});

document.querySelector('#btnAgregarCliente button').addEventListener('click', function () {
    document.getElementById('clienteModalTitulo').textContent = 'Agregar Cliente';
});

// Forzar cierre del modal con JS para Cancelar y Cerrar (X)
document.getElementById('btnCancelarModal').addEventListener('click', function () {
    const modal = bootstrap.Modal.getOrCreateInstance(document.getElementById('clienteModal'));
    modal.hide();
});
document.getElementById('btnCerrarModal').addEventListener('click', function () {
    const modal = bootstrap.Modal.getOrCreateInstance(document.getElementById('clienteModal'));
    modal.hide();
});
