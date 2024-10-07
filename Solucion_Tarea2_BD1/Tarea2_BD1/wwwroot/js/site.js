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

