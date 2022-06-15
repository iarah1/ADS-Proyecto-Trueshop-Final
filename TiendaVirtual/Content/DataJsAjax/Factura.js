var tablaFactura;

$(document).ready(function () {
  
    //Cargar pedidos
    tablaOrdenes = $('#tbFacturas').DataTable({
        "ajax": {
            "url": "/ERP/DetalleFactura",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "FacturaId" },
            { "data": "PedidoId" },
            { "data": "Cliente" },
            { "data": "Direccion" },
            { "data": "Email" },
            { "data": "Telefono" },
            { "data": "TotalOrden" },
            { "data": "FechaCreacion" },
            { "data": "Estado" },
            
            {
                "data": "PedidoId", "render": function (data) {
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
var PedidoId;
function openModal($PedidoId) {
    PedidoId = $PedidoId;
    //mostrar modal
    var modal = document.getElementById("ModalViewFacturar");
    $(modal).modal('show');
    LoadDatosFactura(PedidoId);
    $("#ProductoFactura").dataTable().fnDestroy();
    LoadPedidoProductos(PedidoId);
}

function LoadDatosFactura(PedidoId) {
    //console.log(PedidoId);
    //limpiando datos
    $('#IdentificadorFactura').text("");
    $('#Cliente').text("");
    //cargando datos
    jQuery.ajax({
        url: "/ERP/DetalleFactura",
        type: "GET",
        dataType: "json",
        contentType: "application/json, charset=utf-8",
        success: function (data) {
            //console.log(data.data[0].PedidoId);
            if (data.data[0] == null) {
                alert("Lo sentimos, no encotramos este pedido...");
                return;
            } else {
                for (var i = 0; i < data.data.length; i++) {
                    if (PedidoId == data.data[i].PedidoId) {
                        //console.log(data.data[i].PedidoId);
                        $('#IdentificadorFactura').text(data.data[i].FacturaId);
                        $('#fechaPedido').text(data.data[0].FechaCreacion);
                        $('#Cliente').text(data.data[0].Cliente);
                        $('#Direccion').text(data.data[0].Direccion);
                        $('#Total').text(data.data[0].TotalOrden);
                        $('#Estado').text(data.data[0].Estado);

                        //console.log(data.data[i]);
                        $('#buttonModalFac').html("<button type='button' class='btn btn-success' onclick='openAlert()'>Imprimir</button>" +
                            "<button type='button' class='btn btn-danger' data-dismiss='modal'>Cerrar</button>");
                    }
                }
            }
        }
    });
}


function LoadPedidoProductos(PedidoId) {
    $("#ProductoFactura").dataTable({
        "ajax": {
            "url": "/ERP/DetallePedidoFacturado?PedidoId=" + PedidoId,
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

function openAlert() {
    //ocultar modal
    var modal = document.getElementById("ModalViewFacturar");
    $(modal).modal('hide');
    alert("Imprimiendo factura");
    
}
