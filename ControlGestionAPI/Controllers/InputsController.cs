using ControlGestionAPI.Models;
using ControlGestionAPI.Services;
using ControlGestionAPI.Settings;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlGestionAPI.Controllers
{
    public class InputDto
    {
        public string Id { get; set; }
        public int Anio { get; set; }
        public long Folio { get; set; }
        public string NumOficio { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string Asignado { get; set; }
        public string Asunto { get; set; }
        public string Estatus { get; set; }
        public string Remitente { get; set; }
        public string InstitucionOrigen { get; set; }
        public SeguimientosDto Seguimientos { get; set; }
        public int? DiasAtraso { get; set; }
        public string MensajeAtraso { get; set; }
        public string EstadoSemaforo { get; set; }
        public string ColorSemaforo { get; set; }
    }

    public class SeguimientosDto
    {
        public string AtencionOtorgada { get; set; }
        public DateTime? FechaAcuseRecibido { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class InputsController : Controller
    {
        private readonly IInputService _inputService;

        public InputsController(IInputService inputService)
        {
            _inputService = inputService;
        }

        [HttpGet("currentYear")]
        public async Task<ActionResult<object>> GetNoDeletedInputsInCurrentYear()
        {
            try
            {
                int currentYear = DateTime.Now.Year;
                var inputs = await _inputService.GetInputsAsync(currentYear);

                long totalInputs = inputs.Count;

                if (inputs.Count == 0)
                {
                    return StatusCode(204, new { status = "error", message = "No existen registros!" });
                }

                var inputDtos = inputs.Select(input =>
                {
                    DateTime currentDate = DateTime.Now;
                    int? diasAtraso = null;
                    string mensajeAtraso = null;
                    string estadoSemaforo = null;
                    string colorSemaforo = null;

                    if (input.FechaVencimiento.HasValue)
                    {
                        TimeSpan diffTime = input.FechaVencimiento.Value - currentDate;
                        diasAtraso = (int)Math.Ceiling(diffTime.TotalDays);

                        if (input.Seguimientos?.FechaAcuseRecibido.HasValue == true)
                        {
                            TimeSpan diffTimeAcuse = input.FechaVencimiento.Value - input.Seguimientos.FechaAcuseRecibido.Value;
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
                }).ToList();

                return Ok(new { status = "success", totalInputs = totalInputs, inputs = inputDtos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = "Error al devolver el registro.", error = ex.Message });
            }
        }

        [HttpGet("currentYearByArea")]
        public async Task<ActionResult<object>> GetNoDeletedInputsInCurrentYearByArea([FromQuery] string area)
        {
            if (string.IsNullOrEmpty(area))
            {
                return BadRequest(new { status = "error", message = "El parámetro 'area' es requerido (query)." });
            }

            try
            {
                int currentYear = DateTime.Now.Year;
                var inputs = await _inputService.GetInputsByYearAndAreaAsync(currentYear, area);

                long totalInputs = inputs.Count;

                if (inputs.Count == 0)
                {
                    return StatusCode(204);
                }

                var inputDtos = inputs.Select(input =>
                {
                    DateTime currentDate = DateTime.Now;
                    int? diasAtraso = null;
                    string mensajeAtraso = null;
                    string estadoSemaforo = null;
                    string colorSemaforo = null;

                    if (input.FechaVencimiento.HasValue)
                    {
                        TimeSpan diffTime = input.FechaVencimiento.Value - currentDate;
                        diasAtraso = (int)Math.Ceiling(diffTime.TotalDays);

                        if (input.Seguimientos?.FechaAcuseRecibido.HasValue == true)
                        {
                            TimeSpan diffTimeAcuse = input.FechaVencimiento.Value - input.Seguimientos.FechaAcuseRecibido.Value;
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
                }).ToList();

                return Ok(new { status = "success", totalInputs = totalInputs, inputs = inputDtos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = "Error al devolver el registro.", error = ex.Message });
            }
        }

        [HttpGet("inputsByYear")]
        public async Task<ActionResult<object>> GetInputsByYear([FromQuery] string year)
        {
            if (string.IsNullOrEmpty(year))
            {
                return BadRequest(new { status = "error", message = "El año es requerido." });
            }

            if (!int.TryParse(year, out int anio))
            {
                return BadRequest(new { status = "error", message = "El año debe ser un número." });
            }

            int currentYearMinusOne = DateTime.Now.Year - 1;

            if (anio < 2021 || anio > currentYearMinusOne)
            {
                return BadRequest(new { status = "error", message = $"El año debe ser un número entre 2021 y {currentYearMinusOne}." });
            }

            try
            {
                var inputs = await _inputService.GetInputsByYearAsync(anio);

                long totalInputs = inputs.Count;

                if (inputs.Count == 0)
                {
                    return Ok(new { status = "success", message = "No se encontraron registros para el año especificado.", inputs = new List<InputDto>(), totalInputs = 0 });
                }

                var inputDtos = inputs.Select(input =>
                {
                    DateTime currentDate = DateTime.Now;
                    int? diasAtraso = null;
                    string mensajeAtraso = null;
                    string estadoSemaforo = null;
                    string colorSemaforo = null;

                    if (input.FechaVencimiento.HasValue)
                    {
                        TimeSpan diffTime = input.FechaVencimiento.Value - currentDate;
                        diasAtraso = (int)Math.Ceiling(diffTime.TotalDays);

                        if (input.Seguimientos?.FechaAcuseRecibido.HasValue == true)
                        {
                            TimeSpan diffTimeAcuse = input.FechaVencimiento.Value - input.Seguimientos.FechaAcuseRecibido.Value;
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
                }).ToList();

                return Ok(new { status = "success", totalInputs = totalInputs, inputs = inputDtos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = "Error al devolver el registro.", error = ex.Message });
            }
        }

        [HttpGet("inputsByYearByArea")]
        public async Task<ActionResult<object>> GetInputsByYearByArea([FromQuery] string year, [FromQuery] string area)
        {
            if (string.IsNullOrEmpty(area))
            {
                return BadRequest(new { status = "error", message = "El parámetro 'area' es requerido (query)." });
            }

            if (string.IsNullOrEmpty(year))
            {
                return BadRequest(new { status = "error", message = "El año es requerido." });
            }

            if (!int.TryParse(year, out int anio))
            {
                return BadRequest(new { status = "error", message = "El año debe ser un número." });
            }

            int currentYearMinusOne = DateTime.Now.Year - 1;

            if (anio < 2021 || anio > currentYearMinusOne)
            {
                return BadRequest(new { status = "error", message = $"El año debe ser un número entre 2021 y {currentYearMinusOne}." });
            }

            try
            {
                var inputs = await _inputService.GetInputsByYearAndAreaAsync(anio, area);

                long totalInputs = inputs.Count;

                if (inputs.Count == 0)
                {
                    return Ok(new { status = "success", message = "No se encontraron registros para el año especificado.", inputs = new List<InputDto>(), totalInputs = 0 });
                }

                var inputDtos = inputs.Select(input =>
                {
                    DateTime currentDate = DateTime.Now;
                    int? diasAtraso = null;
                    string mensajeAtraso = null;
                    string estadoSemaforo = null;
                    string colorSemaforo = null;

                    if (input.FechaVencimiento.HasValue)
                    {
                        TimeSpan diffTime = input.FechaVencimiento.Value - currentDate;
                        diasAtraso = (int)Math.Ceiling(diffTime.TotalDays);

                        if (input.Seguimientos?.FechaAcuseRecibido.HasValue == true)
                        {
                            TimeSpan diffTimeAcuse = input.FechaVencimiento.Value - input.Seguimientos.FechaAcuseRecibido.Value;
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
                }).ToList();

                return Ok(new { status = "success", totalInputs = totalInputs, inputs = inputDtos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = "Error al devolver el registro.", error = ex.Message });
            }
        }
    }
}