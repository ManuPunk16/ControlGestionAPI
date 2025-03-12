using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ControlGestionAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<object>> GetUsuarios()
        {
            Console.WriteLine("EntradasController");
            var usuarios = new List<object>
            {
                new { Id = 1, Nombre = "Usuario 1" },
                new { Id = 2, Nombre = "Usuario 3" }
            };

            return Ok(usuarios);
        }
    }
}