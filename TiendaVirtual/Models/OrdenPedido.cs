using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TiendaVirtual.Models
{
    public class Orden
    {
        public int IdPedido { get; set; }
        public string Cliente { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public decimal TotalOrden { get; set; }
        public string FechaCreacion { get; set; }
        public string PickerName { get; set; }
        public string Estado { get; set; }
    }

    public class PedidoCompleto
    {
        public int PedidoId { get; set; }
        public string NombreProduct { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }
}