using Datos.Core;
using Datos.Modelo;
using System;
using System.Windows.Forms;

namespace Presentacion
{
    public partial class PPEdidos : Form
    {
        private readonly UnitOfWork unitOfWork;

        public PPEdidos()
        {
            InitializeComponent();
            unitOfWork = new UnitOfWork(); // Asumiendo que tienes una clase UnitOfWork para manejar las operaciones de base de datos

            BtnGuardar.Click += BtnGuardar_Click;
        }

        private void PPEdidos_Load(object sender, EventArgs e)
        {
            // TODO: esta línea de código carga datos en la tabla 'proyectoRadDataSet5.Pedidoes'. Puede moverla o quitarla según sea necesario.
            this.pedidoesTableAdapter.Fill(this.proyectoRadDataSet5.Pedidoes);
        }

        private void LimpiarCampos()
        {
            TxtID.Text = "";
            CBXCodCliente.SelectedIndex = -1;
            DTFechaCreación.Value = DateTime.Now;
            DTFechaPedido.Value = DateTime.Now;
            CBEstado.Checked = false;
            TxtSubTotal.Text = "";
            TxtDescuento.Text = "";
            TxtTotal.Text = "";
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            string codigo = TxtID.Text;
            int clienteId = int.Parse(CBXCodCliente.SelectedItem.ToString());
            DateTime fechaCreacion = DTFechaCreación.Value;
            DateTime fechaPedido = DTFechaPedido.Value;
            decimal subtotal = decimal.Parse(TxtSubTotal.Text);
            decimal descuento = decimal.Parse(TxtDescuento.Text);
            decimal total = decimal.Parse(TxtTotal.Text);
            byte estadoByte = CBEstado.Checked ? (byte)1 : (byte)0; // Convertir a byte

            DialogResult result = MessageBox.Show("¿Desea guardar el pedido?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Pedido pedido = new Pedido
                {
                    ClienteId = clienteId,
                    FechaCreacion = fechaCreacion,
                    FechaPedido = fechaPedido,
                    Estado = estadoByte, // Asignar el valor convertido
                    Subtotal = subtotal,
                    Descuento = descuento,
                    Total = total
                };

                try
                {
                    unitOfWork.Repository<Pedido>().Agregar(pedido);
                    unitOfWork.Guardar();

                    this.pedidoesTableAdapter.Fill(this.proyectoRadDataSet5.Pedidoes);
                    LimpiarCampos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar el pedido: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
