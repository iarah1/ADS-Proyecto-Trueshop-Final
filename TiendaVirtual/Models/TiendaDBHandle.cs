using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TiendaVirtual.Models
{
    public class TiendaDBHandle
    {
        private SqlConnection con;
        private string conex;
        private void connection()
        {
            
            string constring = @"Server= localhost; Database= PickingADS;User Id=useraspnet;Password=Fr4m3w0rk;";
            conex = constring;
            con = new SqlConnection(constring);
        }

        public PickingUser TrueshopLogin(string username, string password, int type)
        {
            string sp_name = "";
            if (type == 1)
            {
                sp_name = "spERPLogin";
            }
            else
            {
                sp_name = "spPickerLogin";
            }
              
            PickingUser pickingUser = new PickingUser();
            connection();
            SqlCommand cmd = new SqlCommand(sp_name, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    pickingUser.EmpleadoId = dr.GetInt32(0);
                    pickingUser.Empleado = dr.GetString(1);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }

            //close data reader
            dr.Close();

            //close connection
            con.Close();

            return pickingUser;
        }

        public int GetTotalPages()
        {
            int TotalPages = 0;
            connection();
            SqlCommand cmd = new SqlCommand("spGetTotalPage", con);
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    TotalPages = dr.GetInt32(0);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }

            //close data reader
            dr.Close();

            //close connection
            con.Close();

            return TotalPages;
        }

        public List<Product> GetProducts(int pageIndex, int categoryId, string search)
        {
            connection();
            List<Product> products = new List<Product>();

            SqlCommand cmd = new SqlCommand("spGetProducts", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
            cmd.Parameters.AddWithValue("@CategoryId", categoryId);
            cmd.Parameters.AddWithValue("@Search", search);

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                products.Add(
                    new Product
                    {
                        ProductId = Convert.ToInt32(dr["ProductId"]),
                        ItemCode = Convert.ToString(dr["ItemCode"]),
                        ProductName = Convert.ToString(dr["ProductName"]),
                        Price = Convert.ToDecimal(dr["Price"]),
                        CategoryId = Convert.ToInt32(dr["CategoryId"]),
                        CategoryName = Convert.ToString(dr["CategoryName"]),
                        ImageUrl = Convert.ToString(dr["ImageUrl"])
                    });
            }

            return products;
        }

        public List<Product> GetListProducts(string productIds)
        {
            connection();
            List<Product> products = new List<Product>();

            SqlCommand cmd = new SqlCommand("spGetListProducts", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@productIds", productIds);
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                products.Add(
                    new Product
                    {
                        ProductId = Convert.ToInt32(dr["ProductId"]),
                        ItemCode = Convert.ToString(dr["ItemCode"]),
                        ProductName = Convert.ToString(dr["ProductName"]),
                        Price = Convert.ToDecimal(dr["Price"]),
                        CategoryId = Convert.ToInt32(dr["CategoryId"]),
                        CategoryName = Convert.ToString(dr["CategoryName"]),
                        ImageUrl = Convert.ToString(dr["ImageUrl"])
                    });
            }

            return products;
        }

        public int ProcesarPedido(Cliente cliente, List<PedidoDetalle> detalle)
        {
            int PedidoId = InsertPedido(cliente);

            if(PedidoId > 0)
            {
                foreach(var d in detalle)
                {
                    int DetalleId = InsertDetallePedido(PedidoId, d);
                }
            }

            return PedidoId;
        }

        private int InsertPedido(Cliente cliente)
        {
            string _cliente = cliente.Nombres + " " + cliente.Apellidos;

            int PedidoId = 0;
            connection();
            SqlCommand cmd = new SqlCommand("spCrearPedido", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Cliente", _cliente);
            cmd.Parameters.AddWithValue("@Direccion", cliente.Direccion);
            cmd.Parameters.AddWithValue("@Email", cliente.Email);
            cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono);
            cmd.Parameters.AddWithValue("@Total", cliente.TotalPedido);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    PedidoId = dr.GetInt32(0);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }

            //close data reader
            dr.Close();

            //close connection
            con.Close();

            return PedidoId;
        }

        private int InsertDetallePedido(int PedidoId, PedidoDetalle detalle)
        {
            int DetalleId = 0;
            connection();
            SqlCommand cmd = new SqlCommand("spAddDetallePedido", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PedidoId", PedidoId);
            cmd.Parameters.AddWithValue("@ProductId", detalle.ProductId);
            cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    DetalleId = dr.GetInt32(0);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }

            //close data reader
            dr.Close();

            //close connection
            con.Close();

            return DetalleId;
        }


        public int UpdatePedidoPick(int PedidoId, int EstadoId, List<PedidoDetalle> detalle)
        {
            int rPedidoId = UpdatePedido(PedidoId, EstadoId);

            if (PedidoId > 0)
            {
                foreach (var d in detalle)
                {
                    int rProductId = UpdateDetallePedido(PedidoId, d.ProductId, d.Cantidad);
                }
            }

            return PedidoId;
        }

        private int UpdatePedido(int PedidoId, int EstadoId)
        {

            int rPedidoId = 0;
            connection();
            SqlCommand cmd = new SqlCommand("spUpdatePedido", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PedidoId", PedidoId);
            cmd.Parameters.AddWithValue("@EstadoId", EstadoId);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    rPedidoId = dr.GetInt32(0);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }

            //close data reader
            dr.Close();

            //close connection
            con.Close();

            return rPedidoId;
        }

        private int UpdateDetallePedido(int PedidoId, int ProductId, int Cantidad)
        {
            int rProductId = 0;
            connection();
            SqlCommand cmd = new SqlCommand("spUpdateDetallePedido", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PedidoId", PedidoId);
            cmd.Parameters.AddWithValue("@ProductId", ProductId);
            cmd.Parameters.AddWithValue("@Cantidad", Cantidad);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    rProductId = dr.GetInt32(0);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }

            //close data reader
            dr.Close();

            //close connection
            con.Close();

            return rProductId;
        }


        public List<PickingPedido> GetListPedidosPick(int empleadoId)
        {
            connection();
            List<PickingPedido> listPedidos = new List<PickingPedido>();

            SqlCommand cmd = new SqlCommand("spGetPickPedidos", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@empleadoId", empleadoId);

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                listPedidos.Add(
                    new PickingPedido
                    {
                        PedidoId = Convert.ToInt32(dr["PedidoId"]),
                        TotalProductos = Convert.ToInt32(dr["TotalProductos"]),
                        Fecha = Convert.ToString(dr["Fecha"]),
                        Estado = Convert.ToString(dr["Estado"]),
                        EstadoId = Convert.ToInt32(dr["EstadoId"])
                    });
            }

            return listPedidos;
        }

        public List<PickingPedidoDetalle> GetDetallePedidosPick(int PedidoId)
        {
            connection();
            List<PickingPedidoDetalle> listDetallePedido = new List<PickingPedidoDetalle>();

            SqlCommand cmd = new SqlCommand("spGetPickPedidoDetalle", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PedidoId", PedidoId);

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                listDetallePedido.Add(
                    new PickingPedidoDetalle
                    {
                        PedidoId = Convert.ToInt32(dr["PedidoId"]),
                        ProductId = Convert.ToInt32(dr["ProductId"]),
                        ItemCode = Convert.ToString(dr["ItemCode"]),
                        UpcCode = Convert.ToString(dr["UpcCode"]),
                        ProductName = Convert.ToString(dr["ProductName"]),
                        LocationCode = Convert.ToString(dr["LocationCode"]),
                        Cantidad = Convert.ToInt32(dr["Cantidad"]),
                        CantidadPick = Convert.ToInt32(dr["CantidadPick"])
                    });
            }

            return listDetallePedido;
        }


        //Metodo para listar los pedidos en estado completado
        public List<Orden> GetListPedidos()
        {
            connection();
            List<Orden> listPedidos = new List<Orden>();

            SqlCommand cmd = new SqlCommand("spGetListOrden", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                listPedidos.Add(
                    new Orden
                    {
                        IdPedido = Convert.ToInt32(dr["PedidoId"]),
                        Cliente = Convert.ToString(dr["Cliente"]),
                        Direccion = Convert.ToString(dr["Direccion"]),
                        Email = Convert.ToString(dr["Email"]),
                        Telefono = Convert.ToString(dr["Telefono"]),
                        TotalOrden = Convert.ToDecimal(dr["TotalOrder"]),
                        FechaCreacion = Convert.ToString(dr["FechaCreacion"]),
                        PickerName = Convert.ToString(dr["EmpleadoNombre"]),
                        Estado = Convert.ToString(dr["Estado"])
                    });
            }

            return listPedidos;
        }

        //Metodo para detalles del pedido completado
        public List<PedidoCompleto> GetListPedidosByIdPedido(int PedidoId)
        {
            connection();
            List<PedidoCompleto> listDetallePedido = new List<PedidoCompleto>();

            SqlCommand cmd = new SqlCommand("spGetListOrdenByPedidoId", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PedidoId", PedidoId);

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                listDetallePedido.Add(
                    new PedidoCompleto
                    {
                        PedidoId = Convert.ToInt32(dr["PedidoId"]),
                        NombreProduct = Convert.ToString(dr["ProductName"]),
                        Precio = Convert.ToDecimal(dr["Price"]),
                        Cantidad = Convert.ToInt32(dr["Cantidad"]),
                        Total = Convert.ToDecimal(dr["Total"])
                    });
            }

            return listDetallePedido;
        }

        //Metodo para Crear Factura
        public int InsertFactura(Factura factura)
        {

            int FacturaId = 0;
            connection();
            SqlCommand cmd = new SqlCommand("spCrearFactura", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PedidoId", factura.PedidoId);
            cmd.Parameters.AddWithValue("@Cliente", factura.Cliente);
            cmd.Parameters.AddWithValue("@Direccion", factura.Direccion);
            cmd.Parameters.AddWithValue("@Email", factura.Email);
            cmd.Parameters.AddWithValue("@Telefono", factura.Telefono);
            cmd.Parameters.AddWithValue("@Total", factura.TotalOrden);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    FacturaId = dr.GetInt32(0);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }

            //close data reader
            dr.Close();

            //close connection
            con.Close();

            return FacturaId;
        }

        //Metodo para listar facturas
        public List<Factura> GetListFactura()
        {
            connection();
            List<Factura> listFactura = new List<Factura>();

            SqlCommand cmd = new SqlCommand("spGetFacturas", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                listFactura.Add(
                    new Factura
                    {
                        FacturaId = Convert.ToInt32(dr["FacturaId"]),
                        PedidoId = Convert.ToInt32(dr["PedidoId"]),
                        Cliente = Convert.ToString(dr["Cliente"]),
                        Direccion = Convert.ToString(dr["Direccion"]),
                        Email = Convert.ToString(dr["Email"]),
                        Telefono = Convert.ToString(dr["Telefono"]),
                        TotalOrden = Convert.ToDecimal(dr["TotalOrder"]),
                        FechaCreacion = Convert.ToString(dr["FechaCreacion"]),
                        Estado = "Facturado"
                    });
            }

            return listFactura;
        }

        //Metodo para Actualizar Estado de Pedido
        public int UpdateEstadoByPedidoId(int PedidoId)
        {
            connection();
            SqlCommand cmd = new SqlCommand("spChangeEstadoPedido", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PedidoId", PedidoId);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    PedidoId = dr.GetInt32(0);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }

            //close data reader
            dr.Close();

            //close connection
            con.Close();

            return PedidoId;
        }

        //Metodo para listar los pedidos en estado facturado
        public List<Orden> GetListPedidosFacturado(int PedidoId)
        {
            connection();
            List<Orden> listPedidos = new List<Orden>();

            SqlCommand cmd = new SqlCommand("spGetListPedidoFacturado", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PedidoId", PedidoId);

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                listPedidos.Add(
                    new Orden
                    {
                        IdPedido = Convert.ToInt32(dr["PedidoId"]),
                        Cliente = Convert.ToString(dr["Cliente"]),
                        Direccion = Convert.ToString(dr["Direccion"]),
                        Email = Convert.ToString(dr["Email"]),
                        Telefono = Convert.ToString(dr["Telefono"]),
                        TotalOrden = Convert.ToDecimal(dr["TotalOrder"]),
                        FechaCreacion = Convert.ToString(dr["FechaCreacion"]),
                        Estado = Convert.ToString(dr["Estado"])
                    });
            }

            return listPedidos;
        }

        public List<DashboardPicker> GetListDashboardPicker()
        {
            connection();
            List<DashboardPicker> dashboardPickers = new List<DashboardPicker>();

            SqlCommand cmd = new SqlCommand("spDashboardTotalASignados", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                dashboardPickers.Add(
                    new DashboardPicker
                    {
                        EmpleadoNombre = Convert.ToString(dr["EmpleadoNombre"]),
                        TotalOrder = Convert.ToInt32(dr["TotalOrder"]),
                        
                    });
            }

            return dashboardPickers;
        }

    }
}

