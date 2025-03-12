using ControlGestionAPI.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlGestionAPI.Services
{
    public class InputService : IInputService
    {
        private readonly IMongoCollection<Input> _inputsCollection;

        public InputService(IMongoDatabase database)
        {
            _inputsCollection = database.GetCollection<Input>("inputsnuevos");
        }

        public async Task<List<Input>> GetInputsAsync(int? year = null)
        {
            var filterBuilder = Builders<Input>.Filter;
            var filter = filterBuilder.Eq(x => x.Deleted, false);

            if (year.HasValue)
            {
                filter = filter & filterBuilder.Eq(x => x.Anio, year.Value);
            }
            else
            {
                filter = filter & filterBuilder.Eq(x => x.Anio, DateTime.Now.Year);
            }

            var projection = Builders<Input>.Projection
                .Include(x => x.Id)
                .Include(x => x.Anio)
                .Include(x => x.Folio)
                .Include(x => x.NumOficio)
                .Include(x => x.FechaRecepcion)
                .Include(x => x.FechaVencimiento)
                .Include(x => x.Asignado)
                .Include(x => x.Asunto)
                .Include(x => x.Estatus)
                .Include(x => x.Remitente)
                .Include(x => x.InstitucionOrigen)
                .Include("seguimientos.atencion_otorgada") // Include nested field
                .Include("seguimientos.fecha_acuse_recibido"); // Include nested field

            return await _inputsCollection.Find(filter)
                .Project<Input>(projection)
                .SortByDescending(x => x.Anio)
                .ThenByDescending(x => x.Folio)
                .ThenByDescending(x => x.FechaRecepcion)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Input>> GetInputsByYearAndAreaAsync(int year, string area)
        {
            var filterBuilder = Builders<Input>.Filter;
            var filter = filterBuilder.Eq(x => x.Deleted, false) &
                         filterBuilder.Eq(x => x.Anio, year) &
                         filterBuilder.Eq(x => x.Asignado, area);

            var projection = Builders<Input>.Projection
                .Include(x => x.Id)
                .Include(x => x.Anio)
                .Include(x => x.Folio)
                .Include(x => x.NumOficio)
                .Include(x => x.FechaRecepcion)
                .Include(x => x.FechaVencimiento)
                .Include(x => x.Asignado)
                .Include(x => x.Asunto)
                .Include(x => x.Estatus)
                .Include(x => x.Remitente)
                .Include(x => x.InstitucionOrigen)
                .Include("seguimientos.atencion_otorgada")
                .Include("seguimientos.fecha_acuse_recibido");

            return await _inputsCollection.Find(filter)
                .Project<Input>(projection)
                .SortByDescending(x => x.Anio)
                .ThenByDescending(x => x.Folio)
                .ThenByDescending(x => x.FechaRecepcion)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Input>> GetInputsByYearAsync(int year)
        {
            var filterBuilder = Builders<Input>.Filter;
            var filter = filterBuilder.Eq(x => x.Deleted, false) &
                         filterBuilder.Eq(x => x.Anio, year);

            var projection = Builders<Input>.Projection
                .Include(x => x.Id)
                .Include(x => x.Anio)
                .Include(x => x.Folio)
                .Include(x => x.NumOficio)
                .Include(x => x.FechaRecepcion)
                .Include(x => x.FechaVencimiento)
                .Include(x => x.Asignado)
                .Include(x => x.Asunto)
                .Include(x => x.Estatus)
                .Include(x => x.Remitente)
                .Include(x => x.InstitucionOrigen)
                .Include("seguimientos.atencion_otorgada")
                .Include("seguimientos.fecha_acuse_recibido");

            return await _inputsCollection.Find(filter)
                .Project<Input>(projection)
                .SortByDescending(x => x.Anio)
                .ThenByDescending(x => x.Folio)
                .ThenByDescending(x => x.FechaRecepcion)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}