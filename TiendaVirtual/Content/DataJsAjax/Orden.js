var tablaOrdenes;

$(document).ready(function () {
    //Cargar pedidos
    tablaOrdenes = $('#tbPedidos').DataTable({

        "ajax": {
            "url": "/ERP/DetalleOrden",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "IdPedido" },
            { "data": "Cliente" },
            { "data": "Direccion" },
            { "data": "Email" },
            { "data": "Telefono" },
            { "data": "TotalOrden" },
            { "data": "FechaCreacion" },
            { "data": "PickerName" },
            { "data": "Estado" },
            {
                "data": "IdPedido", "render": function (data) {
                    return "<button type='button' class='btn btn-secondary btn-sm mr-1' data-toggle='tooltip' title='Detalles' onclick='openModal(" + data + ")'><i class='fa fa-eye'></i></button>" +
                           "<button type='button' class='btn btn-success btn-sm' data-toggle='tooltip' title='Facturar' onclick='Facturar(" + data + ")'><i class='fa fa-boxes-packing'></i></button>"
                },
            }
        ],
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.15/i18n/Spanish.json"
        }
    });
});

//Mostrar detalles de pedidos
function openModal($IdPedido) {
    //mostrar modal
    var modal = document.getElementById("ModalDetalles");
    $(modal).modal('show');
    console.log($IdPedido);
    $('#Identificador').text($IdPedido);
    //limpiar tabla
    $("#ordenCompleta").dataTable().fnDestroy();
    LoadTablePedido($IdPedido);
}

//cargar detalle pedidos en el modal
function LoadTablePedido($IdPedido) {
    $("#ordenCompleta").dataTable({
        "ajax": {
            "url": "/ERP/DetalleOrdenCompleta?PedidoId=" + $IdPedido,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "NombreProduct",
                "width": "auto"
            },
            {
                "data": "Precio",
                "width": "auto"
            },
            {
                "data": "Cantidad",
                "width": "auto"
            },
            {
                "data": "Total",
                "width": "auto"
            },

        ],
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.15/i18n/Spanish.json"
        }
    });

}

function Facturar($IdPedido) {
    LoadDatosPedido($IdPedido);
    $("#ProductoFactura").dataTable().fnDestroy();
    LoadPedidoProductos($IdPedido);
    //mostrar modal
    var modal = document.getElementById("ModalFacturar");
    $(modal).modal('show');
}

function LoadPedidoProductos($IdPedido) {
    $("#ProductoFactura").dataTable({
        "ajax": {
            "url": "/ERP/DetalleOrdenCompleta?PedidoId=" + $IdPedido,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "Cantidad",
                "width": "auto"
            },
            {
                "data": "NombreProduct",
                "width": "auto"
            },
            {
                "data": "Precio",
                "width": "auto"
            },
            {
                "data": "Total",
                "width": "auto"
            },

        ],
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.15/i18n/Spanish.json"
        }
    });
}

//datos a facturar
var pedidoId;
var cliente;
var direccion;
var total;
var estado;
var email;
var telefono;

function LoadDatosPedido($IdPedido) {
    //limpiando datos
    $('#IdentificadorFa').text("");
    $('#Cliente').text("");
    //cargando datos
    jQuery.ajax({
        url: "/ERP/DetalleOrden",
        type: "GET",
        dataType: "json",
        contentType: "application/json, charset=utf-8",
        success: function (data) {

            if (data.data[0] == null) {
                alert("Lo sentimos, no encotramos este pedido...");
                return;
            } else {

                for (var i = 0; i < data.data.length; i++) {
                    if ($IdPedido == data.data[i].IdPedido) {

                        //console.log(data.data[0].PedidoId);
                        $('#IdentificadorFa').text(data.data[i].IdPedido);
                        $('#fechaPedido').text(data.data[i].FechaCreacion);
                        $('#Cliente').text(data.data[i].Cliente);
                        $('#Direccion').text(data.data[i].Direccion);
                        $('#Total').text(data.data[i].TotalOrden);
                        $('#Estado').text(data.data[i].Estado);

                        ////datos a setear
                        pedidoId = data.data[i].IdPedido;
                        cliente = data.data[i].Cliente;
                        direccion = data.data[i].Direccion;
                        total = data.data[i].TotalOrden;
                        estado = data.data[i].Estado;
                        email = data.data[i].Email;
                        telefono = data.data[i].Telefono;

                        console.log(data.data[i]);
                        $('#buttonModalFac').html("<button type='button' class='btn btn-success' onclick='CrearFactura()'>Facturar</button>" +
                            "<button type='button' class='btn btn-danger' data-dismiss='modal'>Cerrar</button>");
                    }
                }
            }
        }
    });
}


function CrearFactura() {
    //facha actual - No se usa
    var hoy = new Date();
    var fecha = hoy.getDate() + '-' + (hoy.getMonth() + 1) + '-' + hoy.getFullYear();
    var hora = hoy.getHours() + ':' + hoy.getMinutes() + ':' + hoy.getSeconds();
    var fechaYHora = fecha + ' ' + hora;
    //Obteniendo una variable con la máscara d-m-Y H:i:s
    //alert('La fecha es: '+fechaYHora);


    var $data = {
        oFactura: {
            PedidoId: pedidoId,
            Cliente: cliente,
            Direccion: direccion,
            TotalOrden: total,
            Estado: estado,
            Email: email,
            Telefono: telefono
        }
    }

    console.log($data);

    jQuery.ajax({
        url: "/ERP/CrearFactura",
        type: "POST",
        data: JSON.stringify($data),
        dataType: "json",
        contentType: "application/json, charset=utf-8",
        success: function (data) {
            if ($data != null) {
                actualizaEstado(pedidoId);
                tablaOrdenes.ajax.reload();
                //ocultar modal
                var modal = document.getElementById("ModalFacturar");
                $(modal).modal('hide');
                //alert("Registro Guardado Existosamente.");
            } else {
                alert("No se pudo guardar el registro...");
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function actualizaEstado($IdPedido) {
    //alert($IdPedido);
    jQuery.ajax({
        url: "/ERP/UpdateEstadoPedido?PedidoId="+$IdPedido,
        type: "POST",
        data: JSON.stringify($IdPedido),
        dataType: "json",
        contentType: "application/json, charset=utf-8",
        success: function (data) {
            if ($IdPedido != null) {
                tablaOrdenes.ajax.reload();
                //ocultar modal
                //alert("Registro Actualizado Existosamente.");
            } else {
                alert("No se pudo actualizar el registro...");
                return;
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}
