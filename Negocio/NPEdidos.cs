using Datos.Core;
using Datos.Modelo;
using System;

namespace Negocio
{
    public class PedidoNegocio
    {
        private readonly UnitOfWork unitOfWork;

        public PedidoNegocio()
        {
            unitOfWork = new UnitOfWork(); 
        }

        public void GuardarPedido(int clienteId, DateTime fechaCreacion, DateTime fechaPedido, decimal subtotal, decimal descuento, decimal total, bool estado)
        {
            byte estadoByte = estado ? (byte)1 : (byte)0; // Convertir estado a byte
            Pedido pedido = new Pedido
            {
                ClienteId = clienteId,
                FechaCreacion = fechaCreacion,
                FechaPedido = fechaPedido,
                Subtotal = subtotal,
                Descuento = descuento,
                Total = total,
                Estado = estadoByte // Asignar el valor convertido
            };

            unitOfWork.Repository<Pedido>().Agregar(pedido);
            unitOfWork.Guardar();
        }
    }
}
