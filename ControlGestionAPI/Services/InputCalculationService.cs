using ControlGestionAPI.Controllers;
using ControlGestionAPI.Models;
using System;

namespace ControlGestionAPI.Services
{
    public class InputCalculationService : IInputCalculationService
    {
        public InputDto CalculateInputDto(Input input)
        {
            DateTime currentDate = DateTime.Now.Date; // Usar solo la fecha para comparación
            int? diasAtraso = null;
            string mensajeAtraso = null;
            string estadoSemaforo = null;
            string colorSemaforo = null;

            if (input.FechaVencimiento.HasValue)
            {
                DateTime fechaVencimiento = input.FechaVencimiento.Value.Date; // Usar solo la fecha para comparación
                TimeSpan diffTime = fechaVencimiento - currentDate;
                diasAtraso = (int)Math.Ceiling(diffTime.TotalDays);

                if (input.Seguimientos?.FechaAcuseRecibido.HasValue == true)
                {
                    DateTime fechaAcuseRecibido = input.Seguimientos.FechaAcuseRecibido.Value.Date; // Usar solo la fecha para comparación
                    TimeSpan diffTimeAcuse = fechaVencimiento - fechaAcuseRecibido;
                    diasAtraso = (int)Math.Ceiling(diffTimeAcuse.TotalDays);
                }
            }
            else
            {
                mensajeAtraso = "Fecha de vencimiento no establecida.";
            }

            if (diasAtraso.HasValue)
            {
                if (diasAtraso > 3)
                {
                    estadoSemaforo = "verde";
                    colorSemaforo = "#A5D6A7";
                }
                else if (diasAtraso >= 0)
                {
                    estadoSemaforo = "amarillo";
                    colorSemaforo = "#FFF59D";
                }
                else
                {
                    estadoSemaforo = "rojo";
                    colorSemaforo = "#EF9A9A";
                }
            }

            // Mapear el resto de las propiedades de Input a InputDto
            return new InputDto
            {
                Id = input.Id,
                Anio = input.Anio,
                Folio = input.Folio,
                NumOficio = input.NumOficio,
                FechaRecepcion = input.FechaRecepcion,
                FechaVencimiento = input.FechaVencimiento,
                Asignado = input.Asignado,
                Asunto = input.Asunto,
                Estatus = input.Estatus,
                Remitente = input.Remitente,
                InstitucionOrigen = input.InstitucionOrigen,
                Seguimientos = new SeguimientosDto
                {
                    AtencionOtorgada = input.Seguimientos?.AtencionOtorgada,
                    FechaAcuseRecibido = input.Seguimientos?.FechaAcuseRecibido
                },
                DiasAtraso = diasAtraso,
                MensajeAtraso = mensajeAtraso,
                EstadoSemaforo = estadoSemaforo,
                ColorSemaforo = colorSemaforo
            };
        }
    }
}