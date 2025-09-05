/**
 * Script para manejar la funcionalidad de los clientes
 * Incluye: collapse de cards dropdowns de Bootstrap validaciones de formularios
 */

/* ==========================================
   Variables Globales
========================================== */
// Objeto para mantener el estado expandido/colapsado de cada card de cliente
const estadosBotones = {};

/* ==========================================
   Inicialización del DOM
========================================== */
// Ejecutar cuando el DOM este completamente cargado
document.addEventListener("DOMContentLoaded", function () {

    // Timeout para asegurar que Bootstrap este completamente cargado
    setTimeout(() => {
        // Verificar que la libreria Bootstrap este disponible globalmente
        if (typeof bootstrap === 'undefined') {
            console.error('Bootstrap no está cargado correctamente');
            return;
        }

        // Buscar todos los elementos con atributo data-bs-toggle="dropdown"
        const dropdownElements = document.querySelectorAll('[data-bs-toggle="dropdown"]');

        // Inicializar cada elemento dropdown encontrado
        dropdownElements.forEach((element, index) => {
            // Verificar si ya tiene una instancia de Bootstrap para evitar duplicados
            const existingInstance = bootstrap.Dropdown.getInstance(element);
            if (!existingInstance) {
                try {
                    // Crear nueva instancia de Bootstrap Dropdown
                    new bootstrap.Dropdown(element);
                } catch (error) {
                    console.error(`Error inicializando dropdown ${index}:`, error);
                }
            }

            // Agregar event listener manual como respaldo por si Bootstrap falla
            element.addEventListener('click', function (e) {
                // Prevenir que el evento se propague a elementos padre
                e.stopPropagation();

                // Buscar el menu dropdown que sigue al boton trigger
                const menu = this.nextElementSibling;
                if (menu && menu.classList.contains('dropdown-menu')) {
                    // Alternar visibilidad del menu
                    menu.classList.toggle('show');
                }
            });
        });

        // Cerrar todos los dropdowns cuando se hace click fuera de ellos
        document.addEventListener('click', function (e) {
            // Si el click no fue dentro de un dropdown cerrar todos los menus abiertos
            if (!e.target.closest('.dropdown')) {
                document.querySelectorAll('.dropdown-menu.show').forEach(menu => {
                    menu.classList.remove('show');
                });
            }
        });

        // Configurar event listeners para los botones de collapse de cards de clientes
        document.querySelectorAll(".btnCollapseCard").forEach(boton => {
            boton.addEventListener("click", function (event) {
                // Prevenir comportamiento por defecto del link
                event.preventDefault();
                // Prevenir propagacion del evento
                event.stopPropagation();

                // Extraer ID del boton para identificar el cliente
                const idBoton = this.id;
                // Separar por guion y tomar la segunda parte que es el ID del cliente
                const idCliente = idBoton.split("-")[1];

                // Inicializar estado del boton si no existe
                if (estadosBotones[idCliente] === undefined) {
                    estadosBotones[idCliente] = false;
                }

                // Invertir estado actual del boton
                estadosBotones[idCliente] = !estadosBotones[idCliente];

                if (estadosBotones[idCliente]) {
                    // Colapsar todas las cards expandidas antes de expandir la nueva
                    document.querySelectorAll('[id^="cardExpandida-"]').forEach(elem => {
                        elem.classList.replace('d-flex', 'd-none');
                    });
                    // Mostrar todas las vistas preview
                    document.querySelectorAll('[id^="preview-"]').forEach(elem => {
                        elem.classList.replace('d-none', 'd-flex');
                    });
                    // Cambiar todos los iconos a estado colapsado
                    document.querySelectorAll('[id^="collapseCardIcon-"]').forEach(elem => {
                        elem.classList.replace('bi-chevron-up', 'bi-chevron-down');
                    });

                    // Expandir la card del cliente actual
                    const previewElement = document.getElementById(`preview-${idCliente}`);
                    const cardElement = document.getElementById(`cardExpandida-${idCliente}`);
                    const iconElement = document.getElementById(`collapseCardIcon-${idCliente}`);

                    // Verificar que todos los elementos existan antes de manipularlos
                    if (previewElement && cardElement && iconElement) {
                        previewElement.classList.replace('d-flex', 'd-none');
                        cardElement.classList.replace('d-none', 'd-flex');
                        iconElement.classList.replace('bi-chevron-down', 'bi-chevron-up');
                    }
                } else {
                    // Colapsar la card actual y mostrar el preview
                    const previewElement = document.getElementById(`preview-${idCliente}`);
                    const cardElement = document.getElementById(`cardExpandida-${idCliente}`);
                    const iconElement = document.getElementById(`collapseCardIcon-${idCliente}`);

                    // Verificar existencia de elementos antes de manipularlos
                    if (previewElement && cardElement && iconElement) {
                        previewElement.classList.replace('d-none', 'd-flex');
                        cardElement.classList.replace('d-flex', 'd-none');
                        // Cambiar icono a estado colapsado
                        iconElement.classList.replace('bi-chevron-up', 'bi-chevron-down');
                    }
                }
            });
        });

        /* ==========================================
           Manejo de Modales
        ========================================== */
        // Funcion para cerrar modal de cliente completamente y limpiar backdrop
        function cerrarModalCompleto() {
            const modalElement = document.getElementById('clienteModal');

            // Intentar cerrar usando la instancia de Bootstrap
            const modal = bootstrap.Modal.getInstance(modalElement);
            if (modal) {
                modal.hide();
            }

            // Limpieza agresiva del backdrop modal que a veces queda colgado
            setTimeout(() => {
                // Buscar y eliminar todos los elementos backdrop residuales
                document.querySelectorAll('.modal-backdrop').forEach(backdrop => {
                    backdrop.remove();
                });
            }, 50);
        }

        // Event listener para boton de cerrar modal (icono X)
        const btnCerrarModal = document.getElementById('btnCerrarModal');
        if (btnCerrarModal) {
            btnCerrarModal.addEventListener('click', function (e) {
                e.preventDefault();
                e.stopPropagation();
                cerrarModalCompleto();
            });
        }

        // Event listener para boton de cancelar en el modal
        const btnCancelarModal = document.getElementById('btnCancelarModal');
        if (btnCancelarModal) {
            btnCancelarModal.addEventListener('click', function (e) {
                e.preventDefault();
                e.stopPropagation();
                cerrarModalCompleto();
            });
        }

        // Cerrar modal cuando se presiona la tecla Escape
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                const modalElement = document.getElementById('clienteModal');
                // Verificar que el modal este visible antes de cerrarlo
                if (modalElement && modalElement.classList.contains('show')) {
                    cerrarModalCompleto();
                }
            }
        });

    }, 300); // Timeout de 300ms para asegurar carga completa

    /* ==========================================
       Manejo de Vista Previa de Imagenes
    ========================================== */
    // Event listener para cambio en el input de archivo de foto
    document.getElementById('inputFoto').addEventListener('change', function (e) {
        const file = e.target.files[0]; // Obtener primer archivo seleccionado
        const img = document.getElementById('previewImg'); // Elemento img para preview
        const placeholder = document.getElementById('imgPlaceholder'); // Placeholder cuando no hay imagen

        if (file) {
            // Crear FileReader para leer el archivo como Data URL
            const reader = new FileReader();
            reader.onload = function (ev) {
                // Asignar resultado como src de la imagen
                img.src = ev.target.result;
                img.style.display = 'block';
                placeholder.style.display = 'none';
            }
            // Iniciar lectura del archivo como Data URL (base64)
            reader.readAsDataURL(file);
        } else {
            // Si no hay archivo limpiar preview y mostrar placeholder
            img.src = '';
            img.style.display = 'none';
            placeholder.style.display = 'flex';
        }
    });

    /* ==========================================
       Configuracion de Botones de Edicion
    ========================================== */
    // Configurar comportamiento de todos los botones de editar cliente
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

