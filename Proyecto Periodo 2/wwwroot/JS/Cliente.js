/**
 * Script para manejar la funcionalidad de los clientes
 * Incluye: collapse de cards, dropdowns de Bootstrap, validaciones de formularios
 */

// Variable para mantener el estado de los botones de collapse
const estadosBotones = {};

// Inicialización cuando el DOM esté completamente cargado
document.addEventListener("DOMContentLoaded", function () {

    setTimeout(() => {
        // Verificar que Bootstrap esté disponible
        if (typeof bootstrap === 'undefined') {
            console.error('Bootstrap no está cargado correctamente');
            return;
        }

        // Inicializar todos los elementos dropdown de Bootstrap
        const dropdownElements = document.querySelectorAll('[data-bs-toggle="dropdown"]');

        dropdownElements.forEach((element, index) => {
            // Verificar si ya tiene una instancia de Bootstrap
            const existingInstance = bootstrap.Dropdown.getInstance(element);
            if (!existingInstance) {
                try {
                    new bootstrap.Dropdown(element);
                } catch (error) {
                    console.error(`Error inicializando dropdown ${index}:`, error);
                }
            }

            // Agregar event listener manual como backup
            element.addEventListener('click', function (e) {
                e.stopPropagation();

                // Obtener el menú dropdown
                const menu = this.nextElementSibling;
                if (menu && menu.classList.contains('dropdown-menu')) {
                    menu.classList.toggle('show');
                }
            });
        });

        // Cerrar dropdowns al hacer click fuera de ellos
        document.addEventListener('click', function (e) {
            if (!e.target.closest('.dropdown')) {
                document.querySelectorAll('.dropdown-menu.show').forEach(menu => {
                    menu.classList.remove('show');
                });
            }
        });

        // Configurar event listeners para los botones de collapse de cards
        document.querySelectorAll(".btnCollapseCard").forEach(boton => {
            boton.addEventListener("click", function (event) {
                event.preventDefault();
                event.stopPropagation();

                const idBoton = this.id;
                const idCliente = idBoton.split("-")[1];

                if (estadosBotones[idCliente] === undefined) {
                    estadosBotones[idCliente] = false;
                }

                estadosBotones[idCliente] = !estadosBotones[idCliente];

                if (estadosBotones[idCliente]) {
                    // Colapsar todas las cards
                    document.querySelectorAll('[id^="cardExpandida-"]').forEach(elem => {
                        elem.classList.replace('d-flex', 'd-none');
                    });
                    document.querySelectorAll('[id^="preview-"]').forEach(elem => {
                        elem.classList.replace('d-none', 'd-flex');
                    });
                    document.querySelectorAll('[id^="collapseCardIcon-"]').forEach(elem => {
                        elem.classList.replace('bi-chevron-up', 'bi-chevron-down');
                    });

                    // Expandir la card actual
                    const previewElement = document.getElementById(`preview-${idCliente}`);
                    const cardElement = document.getElementById(`cardExpandida-${idCliente}`);
                    const iconElement = document.getElementById(`collapseCardIcon-${idCliente}`);

                    if (previewElement && cardElement && iconElement) {
                        previewElement.classList.replace('d-flex', 'd-none');
                        cardElement.classList.replace('d-none', 'd-flex');
                        iconElement.classList.replace('bi-chevron-down', 'bi-chevron-up');
                    }
                } else {
                    // Colapsar la card actual
                    const previewElement = document.getElementById(`preview-${idCliente}`);
                    const cardElement = document.getElementById(`cardExpandida-${idCliente}`);
                    const iconElement = document.getElementById(`collapseCardIcon-${idCliente}`);

                    if (previewElement && cardElement && iconElement) {
                        previewElement.classList.replace('d-none', 'd-flex');
                        cardElement.classList.replace('d-flex', 'd-none');
                        iconElement.classList.replace('bi-chevron-up', 'bi-chevron-down');
                    }
                }
            });
        });

        // Función para cerrar modal completamente
        function cerrarModalCompleto() {
            const modalElement = document.getElementById('clienteModal');

            // Primero intentar cerrar con Bootstrap
            const modal = bootstrap.Modal.getInstance(modalElement);
            if (modal) {
                modal.hide();
            }

            // Limpieza inmediata y agresiva
            setTimeout(() => {
                // Remover TODOS los backdrops posibles
                document.querySelectorAll('.modal-backdrop').forEach(backdrop => {
                    backdrop.remove();
                });
            }, 50);
        }

        // Botón de cerrar (X)
        const btnCerrarModal = document.getElementById('btnCerrarModal');
        if (btnCerrarModal) {
            btnCerrarModal.addEventListener('click', function (e) {
                e.preventDefault();
                e.stopPropagation();
                cerrarModalCompleto();
            });
        }

        // Botón de cancelar
        const btnCancelarModal = document.getElementById('btnCancelarModal');
        if (btnCancelarModal) {
            btnCancelarModal.addEventListener('click', function (e) {
                e.preventDefault();
                e.stopPropagation();
                cerrarModalCompleto();
            });
        }

        // Cerrar modal con Escape
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                const modalElement = document.getElementById('clienteModal');
                if (modalElement && modalElement.classList.contains('show')) {
                    cerrarModalCompleto();
                }
            }
        });

    }, 300);

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
            const idCliente = btn.getAttribute('data-id');
            document.getElementById('clienteModalTitulo').textContent = 'Editar Cliente';
            document.getElementById('formCliente').action = '/Cliente/Edit';
            document.getElementById('inputIdCliente').value = idCliente;
            
            // Buscar la tarjeta del cliente
            const cardExpandida = document.getElementById(`cardExpandida-${idCliente}`);
            
            if (cardExpandida) {
                // Extraer el nombre completo
                const nombreCompletoElement = cardExpandida.querySelector('.row.text-center p');
                const nombreCompleto = nombreCompletoElement ? nombreCompletoElement.textContent.trim() : '';
                
                // Extraer los demás datos usando los selectores correctos
                const infoRows = cardExpandida.querySelectorAll('.ms-4 .row');
                let cedula = '', telefono = '', correo = '', prestamos = '';
                
                infoRows.forEach(row => {
                    const labelElement = row.querySelector('.text-primary');
                    const valueElement = row.querySelector('.text-secondary');
                    
                    if (labelElement && valueElement) {
                        const label = labelElement.textContent.trim();
                        const value = valueElement.textContent.trim();
                        
                        switch (label) {
                            case 'Cédula:':
                                cedula = value;
                                break;
                            case 'Teléfono:':
                                telefono = value;
                                break;
                            case 'Correo:':
                                correo = value;
                                break;
                            case 'Préstamos disponibles:':
                                prestamos = value;
                                break;
                        }
                    }
                });
                
                // Dividir el nombre completo en partes
                const partesNombre = nombreCompleto.split(' ').filter(parte => parte.length > 0);
                const nombre = partesNombre[0] || '';
                const apellido1 = partesNombre[1] || '';
                const apellido2 = partesNombre.slice(2).join(' ') || '';
                
                // Poblar los inputs
                document.getElementById('inputNombre').value = nombre;
                document.getElementById('inputApellido1').value = apellido1;
                document.getElementById('inputApellido2').value = apellido2;
                document.getElementById('inputCedula').value = cedula;
                document.getElementById('inputTelefono').value = telefono;
                document.getElementById('inputCorreo').value = correo;
                document.getElementById('inputCantidadDePrestamos').value = prestamos;
            } else {
                // Intentar obtener datos básicos del preview
                const preview = document.getElementById(`preview-${idCliente}`);
                if (preview) {
                    const nombreCompletoElement = preview.querySelector('.col-12:first-child');
                    const cedulaElement = preview.querySelector('.col-12.text-secondary');
                    
                    if (nombreCompletoElement && cedulaElement) {
                        const nombreCompleto = nombreCompletoElement.textContent.trim();
                        const cedula = cedulaElement.textContent.trim();
                        
                        const partesNombre = nombreCompleto.split(' ').filter(parte => parte.length > 0);
                        const nombre = partesNombre[0] || '';
                        const apellido1 = partesNombre[1] || '';
                        const apellido2 = partesNombre.slice(2).join(' ') || '';
                        
                        document.getElementById('inputNombre').value = nombre;
                        document.getElementById('inputApellido1').value = apellido1;
                        document.getElementById('inputApellido2').value = apellido2;
                        document.getElementById('inputCedula').value = cedula;
                    }
                }
            }
            
            // Limpiar solo la imagen de vista previa
            const previewImg = document.getElementById('previewImg');
            const placeholder = document.getElementById('imgPlaceholder');
            const inputFoto = document.getElementById('inputFoto');
            
            previewImg.src = '';
            previewImg.style.display = 'none';
            placeholder.style.display = 'flex';
            inputFoto.value = '';
        });
    });

    document.querySelector('#btnAgregarCliente button').addEventListener('click', function () {
        document.getElementById('clienteModalTitulo').textContent = 'Agregar Cliente';
        document.getElementById('formCliente').action = '/Cliente/Create';
        document.getElementById('inputIdCliente').value = '';
        
        // Limpiar todos los campos del formulario
        document.getElementById('formCliente').reset();
        
        // Limpiar específicamente cada campo
        document.getElementById('inputNombre').value = '';
        document.getElementById('inputApellido1').value = '';
        document.getElementById('inputApellido2').value = '';
        document.getElementById('inputCedula').value = '';
        document.getElementById('inputTelefono').value = '';
        document.getElementById('inputCorreo').value = '';
        document.getElementById('inputCantidadDePrestamos').value = '';
        
        // Limpiar la imagen
        const previewImg = document.getElementById('previewImg');
        const placeholder = document.getElementById('imgPlaceholder');
        const inputFoto = document.getElementById('inputFoto');
        
        previewImg.src = '';
        previewImg.style.display = 'none';
        placeholder.style.display = 'flex';
        inputFoto.value = '';
    });

    // Manejar botón de eliminar cliente
    document.querySelectorAll('.btn-eliminar-cliente').forEach(btn => {
        btn.addEventListener('click', function () {
            const idCliente = btn.getAttribute('data-id');
            const nombreCliente = btn.getAttribute('data-nombre');
            const imagenCliente = btn.getAttribute('data-imagen');
            
            // Actualizar el formulario con el ID del cliente
            document.getElementById('inputEliminarId').value = idCliente;
            
            // Actualizar la información del cliente en el modal
            document.getElementById('eliminarClienteNombre').textContent = nombreCliente;
            
            // Actualizar la imagen del cliente
            const imgElement = document.getElementById('eliminarClienteImagen');
            if (imagenCliente && imagenCliente.trim() !== '') {
                imgElement.src = `/Images/Cliente/${imagenCliente}`;
            } else {
                imgElement.src = '/Images/default-user.svg';
            }
            imgElement.onerror = function() {
                this.src = '/Images/default-user.svg';
            };
        });
    });

});

