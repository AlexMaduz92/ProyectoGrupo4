﻿using Datos.Base_de_Dato;
using Datos.Core;
using Datos.Modelo;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Presentacion
{
    public partial class PCondicionesPago : Form
    {
        private readonly UnitOfWork unitOfWork;

        public PCondicionesPago()
        {
            InitializeComponent();
            unitOfWork = new UnitOfWork();
            DGVCPago.CellDoubleClick += dataGridView1_CellDoubleClick;
            BtnGuardar.Click += BtnGuardar_Click;
            BtnModificar.Click += BtnModificar_Click;
            BtnEliminar.Click += BtnEliminar_Click;
            CBXFiltro.SelectedIndexChanged += CBXFiltro_SelectedIndexChanged;
            BtnCerrar.Click += BtnCerrar_Click;
        }

        private void PCondicionesPago_Load(object sender, EventArgs e)
        {
            // Obtener el próximo ID que se generará al guardar
            int proximoId = ObtenerProximoId();

            // Mostrar el próximo ID en el TXTID
            TXTID.Text = proximoId.ToString();

            this.condiccionPagoesTableAdapter.Fill(this.proyectoRadDataSet4.CondiccionPagoes);

            // Crear un DataView y aplicar un filtro para mostrar solo los registros con Estado en true
            DataView dv = new DataView(this.proyectoRadDataSet4.CondiccionPagoes);
            dv.RowFilter = "Estado = true";

            // Asignar el DataView filtrado al DataGridView
            DGVCPago.DataSource = dv;

            CBXFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            CBXFiltro.Items.Add("Activos");
            CBXFiltro.Items.Add("No Activos");
        }

        private int ObtenerProximoId()
        {
            int maxId = 0;
            using (var context = new Exaconection())
            {
                maxId = context.Set<CondiccionPago>().Max(c => c.CondicionPagoId);
            }
            return maxId + 1;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = DGVCPago.Rows[e.RowIndex];
                TXTID.Text = row.Cells["condicionPagoIdDataGridViewTextBoxColumn"].Value.ToString();
                TXTCodigo.Text = row.Cells["codigoDataGridViewTextBoxColumn"].Value.ToString();
                TXTDescripcion.Text = row.Cells["descripcionDataGridViewTextBoxColumn"].Value.ToString();
                CBEstado.Checked = Convert.ToBoolean(row.Cells["estadoDataGridViewCheckBoxColumn"].Value);
                TxtDias.Text = row.Cells["diasDataGridViewTextBoxColumn"].Value.ToString();
                DTFCreacion.Value = Convert.ToDateTime(row.Cells["fechaCreacionDataGridViewTextBoxColumn"].Value);
            }
        }



        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            string codigo = TXTCodigo.Text;
            string descripcion = TXTDescripcion.Text;
            int dias = int.Parse(TxtDias.Text);
            bool estado = CBEstado.Checked;
            DateTime fechaCreacion = DTFCreacion.Value;

            DialogResult result = MessageBox.Show("¿Desea guardar la condición de pago?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(descripcion))
                {
                    MessageBox.Show("Debe completar todos los campos.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                CondiccionPago condicionPago = new CondiccionPago
                {
                    Codigo = codigo,
                    Descripcion = descripcion,
                    Dias = dias,
                    Estado = estado,
                    FechaCreacion = fechaCreacion
                };

                try
                {
                    unitOfWork.Repository<CondiccionPago>().Agregar(condicionPago);
                    unitOfWork.Guardar();

                    this.condiccionPagoesTableAdapter.Fill(this.proyectoRadDataSet4.CondiccionPagoes);
                    LimpiarCampos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar la condición de pago: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnModificar_Click(object sender, EventArgs e)
        {
            int id = int.Parse(TXTID.Text);
            string codigo = TXTCodigo.Text;
            string descripcion = TXTDescripcion.Text;
            int dias = int.Parse(TxtDias.Text);
            bool estado = CBEstado.Checked;
            DateTime fechaCreacion = DTFCreacion.Value;

            DialogResult result = MessageBox.Show("¿Desea modificar la condición de pago?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(descripcion))
                {
                    MessageBox.Show("Debe completar todos los campos.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                CondiccionPago condicionPago = unitOfWork.Repository<CondiccionPago>().ObtenerPorId(id);
                if (condicionPago != null)
                {
                    condicionPago.Codigo = codigo;
                    condicionPago.Descripcion = descripcion;
                    condicionPago.Dias = dias;
                    condicionPago.Estado = estado;
                    condicionPago.FechaCreacion = fechaCreacion;

                    try
                    {
                        unitOfWork.Repository<CondiccionPago>().Editar(condicionPago);
                        unitOfWork.Guardar();

                        this.condiccionPagoesTableAdapter.Fill(this.proyectoRadDataSet4.CondiccionPagoes);
                        LimpiarCampos();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al modificar la condición de pago: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            int id = int.Parse(TXTID.Text);

            DialogResult result = MessageBox.Show("¿Desea eliminar la condición de pago?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                CondiccionPago condicionPago = unitOfWork.Repository<CondiccionPago>().ObtenerPorId(id);
                if (condicionPago != null)
                {
                    condicionPago.Estado = false; // Desactivar en lugar de eliminar

                    try
                    {
                        unitOfWork.Repository<CondiccionPago>().Editar(condicionPago);
                        unitOfWork.Guardar();

                        this.condiccionPagoesTableAdapter.Fill(this.proyectoRadDataSet4.CondiccionPagoes);

                        LimpiarCampos();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar la condición de pago: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void CargarDatosEstadoActivo()
        {
            // Crear una nueva vista de datos filtrada por Estado = true
            DataView view = new DataView(this.proyectoRadDataSet4.CondiccionPagoes);
            view.RowFilter = "Estado = true";
            DGVCPago.DataSource = view;
        }

        private void CargarDatosEstadoNoActivo()
        {
            // Crear una nueva vista de datos filtrada por Estado = false
            DataView view = new DataView(this.proyectoRadDataSet4.CondiccionPagoes);
            view.RowFilter = "Estado = false";
            DGVCPago.DataSource = view;
        }

        private void CBXFiltro_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Obtener el filtro seleccionado
            string filtro = CBXFiltro.SelectedItem.ToString();

            // Actualizar el DataGridView según el filtro seleccionado
            if (filtro == "Activos")
            {
                CargarDatosEstadoActivo();
            }
            else if (filtro == "No Activos")
            {
                CargarDatosEstadoNoActivo();
            }
        }

        private void LimpiarCampos()
        {
            TXTID.Text = string.Empty;
            TXTCodigo.Text = string.Empty;
            TXTDescripcion.Text = string.Empty;
            CBEstado.Checked = false;
            TxtDias.Text = string.Empty;
            DTFCreacion.Value = DateTime.Now; // Puedes establecer otra fecha inicial si lo prefieres
        }


        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
