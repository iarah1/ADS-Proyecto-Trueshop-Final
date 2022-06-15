using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TiendaVirtual.Models
{
    public class Factura
    {

        public int FacturaId { get; set; }
        public int PedidoId { get; set; }
        public string Cliente { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public decimal TotalOrden { get; set; }
        public string FechaCreacion { get; set; }
        public string Estado { get; set; }

    }
     
        public class FacturaDetalle
    {
        public int PedidoId { get; set; }
        public int ProductId { get; set; }
        public string ItemCode { get; set; }
        public string UpcCode { get; set; }
        public string ProductName { get; set; }
        public string LocationCode { get; set; }
        public int Cantidad { get; set; }
        public int CantidadPick { get; set; }

    }
}