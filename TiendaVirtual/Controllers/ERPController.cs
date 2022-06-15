using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TiendaVirtual.Models;


namespace TiendaVirtual.Controllers
{
    public class ERPController : Controller
    {
        // GET: ERP
        public ActionResult ERPlogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ERPLogin(string userName, string password)
        {
            TiendaDBHandle tienda = new TiendaDBHandle();
            PickingUser empleado = tienda.TrueshopLogin(userName, password, 2);

            if (empleado.EmpleadoId > 0)
            {
                Session["User"] = userName;
                Session["EmpleadoId"] = empleado.EmpleadoId;
                Session["Empleado"] = empleado.Empleado;

                return RedirectToAction("Dashboard");
            }
            else
            {
                return RedirectToAction("ERPLogin");
            }
        }

        public ActionResult Dashboard()
        {
            string user = Convert.ToString(Session["User"]);
            int empleadoId = Convert.ToInt32(Session["EmpleadoId"]);

            if (string.IsNullOrEmpty(user))
            {
                return RedirectToAction("ERPLogin");
            }

            TiendaDBHandle tienda = new TiendaDBHandle();
            var dashboardPicker = tienda.GetListDashboardPicker();

            ViewBag.DashboardPicker = dashboardPicker;
            return View();
        }

        //Vista de Pedidos
        public ActionResult Ordenes()
        {            
            return View();
        }

        // GET: ERP/DetallesOrden - JsonResult
        public JsonResult DetalleOrden()
        {
            List<Orden> OListOrden = new List<Orden>();
            TiendaDBHandle tienda = new TiendaDBHandle();
            OListOrden = tienda.GetListPedidos().ToList();

            return Json(new { data = OListOrden }, JsonRequestBehavior.AllowGet);
        }

        // GET: ERP/DetalleOrdenCompleta - JsonResult
        public JsonResult DetalleOrdenCompleta(int PedidoId)
        {
            List<PedidoCompleto> OListPedido = new List<PedidoCompleto>();
            TiendaDBHandle tienda = new TiendaDBHandle();
            OListPedido = tienda.GetListPedidosByIdPedido(PedidoId).ToList();

            return Json(new { data = OListPedido }, JsonRequestBehavior.AllowGet);
        }


        //Vista de Facturas
        public ActionResult Facturas()
        {
            return View();
        }

        // GET: ERP/DetalleFactura - JsonResult
        public JsonResult DetalleFactura()
        {
            List<Factura> oListFactura = new List<Factura>();
            TiendaDBHandle tienda = new TiendaDBHandle();
            oListFactura = tienda.GetListFactura().ToList();

            return Json(new { data = oListFactura }, JsonRequestBehavior.AllowGet);
        }

        // POST: ERP/CrearFactura - JsonResult
        [HttpPost]
        public JsonResult CrearFactura(Factura oFactura)
        {
            bool resultado = true;
            try
            {
                if (oFactura.FacturaId == 0)
                {
                    TiendaDBHandle tienda = new TiendaDBHandle();
                    var save = tienda.InsertFactura(oFactura);
                }
            }
            catch
            {
                resultado = false;
            }

            return Json(new { respuesta = resultado }, JsonRequestBehavior.AllowGet);
        }

        // POST: ERP/UpdateEstadoPedido - JsonResult
        [HttpPost]
        public JsonResult UpdateEstadoPedido(int PedidoId)
        {
            bool resultado = true;
            try
            {
                if (PedidoId != 0)
                {
                    TiendaDBHandle tienda = new TiendaDBHandle();
                    var update = tienda.UpdateEstadoByPedidoId(PedidoId);
                }
            }
            catch
            {
                resultado = false;
            }

            return Json(new { respuesta = resultado }, JsonRequestBehavior.AllowGet);
        }

        // GET: ERP/DetallePedidoFacturado - JsonResult
        public JsonResult DetallePedidoFacturado(int PedidoId)
        {
            List<PedidoCompleto> OListPedido = new List<PedidoCompleto>();
            TiendaDBHandle tienda = new TiendaDBHandle();
            OListPedido = tienda.GetListPedidosByIdPedido(PedidoId).ToList();

            return Json(new { data = OListPedido }, JsonRequestBehavior.AllowGet);
        }

    }
}