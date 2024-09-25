// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function ObtenerColorDeFondo() {
    document.getElementById('encNom').style.backgroundColor = $("body").css("backgroundColor");
    document.getElementById('encValorDocIdent').style.backgroundColor = $("body").css("backgroundColor");
    document.getElementById('encAccion').style.backgroundColor = $("body").css("backgroundColor");
}

document.addEventListener(onload, ObtenerColorDeFondo());

$(document).ready(function () {
    // Desaparece después de 4 segundos
    setTimeout(function () {
        $(".alert").css("opacity", "0"); // Inicia el desvanecimiento
        setTimeout(function () {
            $(".alert").remove(); // Elimina el elemento después del desvanecimiento
        }, 2000); // Tiempo para que el desvanecimiento complete
    }, 4000);

    // Permite cerrar el alert manualmente
    $(".alert .close").click(function () {
        $(this).parent().css("opacity", "0");
        setTimeout(function () {
            $(this).parent().remove();
        }, 2000);
    });
});

//Para la _VistaparcialFiltro
$(document).ready(function() {
    $("#formFiltrar").submit(function(event) {
        event.preventDefault();  // Evita que se recargue toda la página

        var form = $(this);
        var url = form.attr('action');
        var formData = form.serialize();  // Serializa los datos del formulario

        $.post(url, formData, function(data) {
            $("#div-lista").html(data);  // Actualiza solo la parte de la vista parcial
            $("#tablaEmpleados").onload(ObtenerColorDeFondo());
        });
    });
});

//Para la _VistaparcialSeleccionDePuesto
$(document).ready(function () {
    $("#formAgregarEmp").submit(function (event) {
        event.preventDefault();  // Evita que se recargue toda la página

        var form = $(this);
        var url = form.attr('action');
        var formData = form.serialize();  // Serializa los datos del formulario

        $.post(url, formData, function (data) {
            $("#div-PartialView").html(data);  // Actualiza solo la parte de la vista parcial
        });
    });
});


//$("formAgregarEmp").ready(function () {
//    // Realizar una solicitud AJAX para cargar la partial view
//    $("#div-PartialView").load('@Url.Action("ObtenerPuestos", "Empleado")');
//});


//$(document).ready(function () {
//    // Cargar el datalist mediante AJAX cuando la página esté completamente cargada
//    $("#datalistContainer").load('@Url.Action("ObtenerPuestos")');
//});






/* Desde aqui el JS para el popup de avisos */

//const abrirPopup = document.getElementById('btnAgregar');
const cerrarPopup = document.getElementById('btnCerrarPopup');
const contenedorPopup = document.getElementById('elPopup');
const taparContenido = document.getElementById('overlay');

// Funciones para abrir y cerrar el popup
function showPopup() {
    taparContenido.style.display = 'block';
    contenedorPopup.style.display = 'flex';
}

function hidePopup() {
    taparContenido.style.display = 'none';
    contenedorPopup.style.display = 'none';
}

// Event listeners para abrir y cerrar el popup
//abrirPopup.addEventListener('click', showPopup);
cerrarPopup.addEventListener('click', hidePopup);

document.addEventListener('click', function (event) {
    // Valida si el clic sucedio en el contenedor overlay
    if (taparContenido.contains(event.target)) {
        hidePopup();
    }
});

/* Hasta aqui el JS para el popup de avisos */








