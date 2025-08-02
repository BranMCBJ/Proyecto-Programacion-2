// ==========================
// Collapse de cualquier card - VERSIÓN ÚNICA Y CORREGIDA
// ==========================

document.addEventListener('DOMContentLoaded', function () {

    // Solución robusta usando eventos de Bootstrap
    document.addEventListener('show.bs.collapse', function (event) {
        const collapseElement = event.target;

        // Verificar si es una de nuestras cards
        if (collapseElement.id && collapseElement.id.startsWith('cardExpandida-')) {
            const cedula = collapseElement.id.replace('cardExpandida-', '');
            const preview = document.querySelector(`#preview-${cedula}`);

            if (preview) {
                preview.classList.add('d-none');
            }

            // Cambiar ícono a chevron-up en TODOS los botones de esta card
            const buttons = document.querySelectorAll(`button[data-bs-target="#${collapseElement.id}"]`);
            buttons.forEach(button => {
                const icon = button.querySelector('i');
                if (icon) {
                    icon.classList.remove('bi-chevron-down');
                    icon.classList.add('bi-chevron-up');
                }
            });
        }
    });

    document.addEventListener('hide.bs.collapse', function (event) {
        const collapseElement = event.target;

        // Verificar si es una de nuestras cards
        if (collapseElement.id && collapseElement.id.startsWith('cardExpandida-')) {
            const cedula = collapseElement.id.replace('cardExpandida-', '');
            const preview = document.querySelector(`#preview-${cedula}`);

            if (preview) {
                preview.classList.remove('d-none');
            }

            // Cambiar ícono a chevron-down en TODOS los botones de esta card
            const buttons = document.querySelectorAll(`button[data-bs-target="#${collapseElement.id}"]`);
            buttons.forEach(button => {
                const icon = button.querySelector('i');
                if (icon) {
                    icon.classList.remove('bi-chevron-up');
                    icon.classList.add('bi-chevron-down');
                }
            });
        }
    });

    // ==========================
    // Funcionalidad de botones - MEJORADA
    // ==========================

    // Función para configurar botones dinámicamente (útil si cargas contenido via AJAX)
    function configurarBotonesClientes() {
        // Eliminar event listeners existentes para evitar duplicados
        document.querySelectorAll('.btn-eliminar-cliente').forEach(button => {
            // Clonar el botón para eliminar event listeners existentes
            const newButton = button.cloneNode(true);
            button.parentNode.replaceChild(newButton, button);

            // Agregar el event listener
            newButton.addEventListener('click', function () {
                const cedula = this.getAttribute('data-cedula');
                if (confirm(`¿Está seguro de que desea eliminar el cliente con cédula ${cedula}?`)) {
                    console.log(`Eliminando cliente con cédula: ${cedula}`);
                    // Aquí va tu lógica para eliminar
                }
            });
        });

        // Botones de editar - también con limpieza de event listeners
        document.querySelectorAll('.btn-editar-cliente').forEach(button => {
            const newButton = button.cloneNode(true);
            button.parentNode.replaceChild(newButton, button);

            newButton.addEventListener('click', function () {
                // Rellenar el modal de editar
                const editarIdCliente = document.getElementById('EditarIdCliente');
                const editarNombre = document.getElementById('EditarNombre');
                const editarCedula = document.getElementById('EditarCedula');
                const editarTelefono = document.getElementById('EditarTelefono');
                const editarCorreo = document.getElementById('EditarCorreo');
                const editarPrestamos = document.getElementById('EditarPrestamos');

                if (editarIdCliente) editarIdCliente.value = this.dataset.id || '';
                if (editarNombre) editarNombre.value = this.dataset.nombre || '';
                if (editarCedula) editarCedula.value = this.dataset.cedula || '';
                if (editarTelefono) editarTelefono.value = this.dataset.telefono || '';
                if (editarCorreo) editarCorreo.value = this.dataset.correo || '';
                if (editarPrestamos) editarPrestamos.value = this.dataset.prestamos || '';

                // Manejo de imagen en editar
                const previewEditarImg = document.getElementById('EditarPreviewImg');
                const placeholderEditar = document.getElementById('EditarImgPlaceholder');

                if (previewEditarImg && placeholderEditar) {
                    if (this.dataset.img) {
                        previewEditarImg.src = this.dataset.img;
                        previewEditarImg.style.display = 'block';
                        placeholderEditar.style.display = 'none';
                    } else {
                        previewEditarImg.src = '';
                        previewEditarImg.style.display = 'none';
                        placeholderEditar.style.display = 'flex';
                    }
                }

                console.log('Editando cliente:', {
                    id: this.dataset.id,
                    nombre: this.dataset.nombre,
                    cedula: this.dataset.cedula,
                    telefono: this.dataset.telefono,
                    correo: this.dataset.correo,
                    prestamos: this.dataset.prestamos
                });
            });
        });
    }

    // Ejecutar al cargar la página
    configurarBotonesClientes();

    // ==========================
    // Vista previa de imagen en modal AGREGAR
    // ==========================
    const inputAgregarFoto = document.getElementById('AgregarImagen') || document.getElementById('modalImagen');
    const previewAgregarImg = document.getElementById('AgregarPreviewImg') || document.getElementById('previewImg');
    const placeholderAgregar = document.getElementById('AgregarImgPlaceholder') || document.getElementById('imgPlaceholder');

    if (inputAgregarFoto && previewAgregarImg && placeholderAgregar) {
        inputAgregarFoto.addEventListener('change', function (e) {
            const file = e.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = ev => {
                    previewAgregarImg.src = ev.target.result;
                    previewAgregarImg.style.display = 'block';
                    placeholderAgregar.style.display = 'none';
                };
                reader.readAsDataURL(file);
            } else {
                previewAgregarImg.src = '';
                previewAgregarImg.style.display = 'none';
                placeholderAgregar.style.display = 'flex';
            }
        });
    }

    // ==========================
    // Vista previa de imagen en modal EDITAR
    // ==========================
    const inputEditarFoto = document.getElementById('EditarImagen');
    const previewEditarImg = document.getElementById('EditarPreviewImg');
    const placeholderEditar = document.getElementById('EditarImgPlaceholder');

    if (inputEditarFoto && previewEditarImg && placeholderEditar) {
        inputEditarFoto.addEventListener('change', function (e) {
            const file = e.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = ev => {
                    previewEditarImg.src = ev.target.result;
                    previewEditarImg.style.display = 'block';
                    placeholderEditar.style.display = 'none';
                };
                reader.readAsDataURL(file);
            } else {
                previewEditarImg.src = '';
                previewEditarImg.style.display = 'none';
                placeholderEditar.style.display = 'flex';
            }
        });
    }

    // ==========================
    // Botones de cancelar específicos
    // ==========================
    const btnCancelarAgregar = document.getElementById('btnCancelarAgregar') || document.getElementById('btnCancelarModal');
    if (btnCancelarAgregar) {
        btnCancelarAgregar.addEventListener('click', function () {
            const modalElement = document.getElementById('agregarClienteModal');
            if (modalElement) {
                const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
                modal.hide();
            }
        });
    }

    const btnCancelarEditar = document.getElementById('btnCancelarEditar');
    if (btnCancelarEditar) {
        btnCancelarEditar.addEventListener('click', function () {
            const modalElement = document.getElementById('editarClienteModal');
            if (modalElement) {
                const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
                modal.hide();
            }
        });
    }

    // ==========================
    // Cerrar modales con botones de cerrar (X)
    // ==========================
    document.querySelectorAll('[data-bs-dismiss="modal"]').forEach(btn => {
        btn.addEventListener('click', function () {
            const modalEl = this.closest('.modal');
            if (modalEl) {
                const modal = bootstrap.Modal.getInstance(modalEl);
                if (modal) {
                    modal.hide();
                }
            }
        });
    });
});

// Debug - eliminar después de probar
console.log('Script cargado correctamente');
console.log('Bootstrap disponible:', typeof bootstrap !== 'undefined');
console.log('Cards encontradas:', document.querySelectorAll('.cliente-card').length);