using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ControlGestionAPI.Models
{
    public class Input
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; }

        [BsonElement("anio")]
        public int Anio { get; set; }

        [BsonElement("folio")]
        public long Folio { get; set; }

        [BsonElement("num_oficio")]
        public string NumOficio { get; set; }

        [BsonElement("fecha_oficio")]
        public DateTime? FechaOficio { get; set; }

        [BsonElement("fecha_vencimiento")]
        public DateTime? FechaVencimiento { get; set; }

        [BsonElement("fecha_recepcion")]
        public DateTime? FechaRecepcion { get; set; }

        [BsonElement("hora_recepcion")]
        public string? HoraRecepcion { get; set; }

        [BsonElement("instrumento_juridico")]
        public string? InstrumentoJuridico { get; set; }

        [BsonElement("remitente")]
        public string Remitente { get; set; }

        [BsonElement("institucion_origen")]
        public string InstitucionOrigen { get; set; }

        [BsonElement("asunto")]
        public string Asunto { get; set; }

        [BsonElement("asignado")]
        public string Asignado { get; set; }

        [BsonElement("estatus")]
        public string Estatus { get; set; }

        [BsonElement("observacion")]
        public string? Observacion { get; set; }

        [BsonElement("archivosPdf")]
        public List<string> ArchivosPdf { get; set; }

        [BsonElement("create_user")]
        public UserRef CreateUser { get; set; }

        [BsonElement("editor_user")]
        public UserRef? EditorUser { get; set; }

        [BsonElement("edit_count")]
        public int EditCount { get; set; }

        [BsonElement("deleted")]
        public bool Deleted { get; set; }

        [BsonElement("seguimientos")]
        public Seguimientos? Seguimientos { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    public class UserRef
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string? Id { get; set; }

        [BsonElement("username")]
        public string? Username { get; set; }
    }

    public class Seguimientos
    {
        [BsonElement("oficio_salida")]
        public string Oficio_salida { get; set; }

        [BsonElement("fecha_respuesta")]
        public DateTime FechaRepuesta { get; set; }

        [BsonElement("usuario")]
        public UserRef Usuario { get; set; }

        [BsonElement("comentarios")]
        public string Comentarios { get; set; }

        [BsonElement("archivosPdf_seguimiento")]
        public List<string> ArchivosPdfSeguimiento { get; set; }

        [BsonElement("num_expediente")]
        public string? NumExpediente { get; set; }

        [BsonElement("fecha_oficio_salida")]
        public DateTime? FechaOficioSalida { get; set; }

        [BsonElement("fecha_acuse_recibido")]
        public DateTime? FechaAcuseRecibido { get; set; }

        [BsonElement("destinatario")]
        public string? Destinatario { get; set; }

        [BsonElement("cargo")]
        public string? Cargo { get; set; }

        [BsonElement("atencion_otorgada")]
        public string? AtencionOtorgada { get; set; }

        [BsonElement("anexo")]
        public string? Anexo { get; set; }

        [BsonElement("estatus")]
        public string? Estatus { get; set; }

        [BsonElement("firma_visado")]
        public string? FirmaVisado { get; set; }
    }
}