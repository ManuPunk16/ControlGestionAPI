using ControlGestionAPI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ControlGestionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseController : ControllerBase
    {
        private readonly MongoDBSettings _mongoDBSettings;
        //private readonly string _collectionName = "inputsnuevos";
        private readonly string _collectionName = "institutions";

        public DatabaseController(IOptions<MongoDBSettings> mongoDBSettings)
        {
            _mongoDBSettings = mongoDBSettings.Value;
        }

        [HttpGet("checkConnection")]
        public ActionResult<string> CheckConnection()
        {
            try
            {
                var client = new MongoClient(_mongoDBSettings.ConnectionString);
                var database = client.GetDatabase(_mongoDBSettings.DatabaseName);
                var collection = database.GetCollection<object>(_collectionName);

                // Obtener el número de registros en la colección
                var recordCount = collection.EstimatedDocumentCount();

                return Ok(new
                {
                    DatabaseName = _mongoDBSettings.DatabaseName,
                    CollectionName = _collectionName,
                    RecordCount = recordCount,
                    Message = "Conexión a la base de datos exitosa."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al conectar a la base de datos: {ex.Message}");
            }
        }
    }
}