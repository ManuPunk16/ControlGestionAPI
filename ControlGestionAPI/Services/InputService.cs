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

        private FilterDefinition<Input> GetBaseFilter()
        {
            return Builders<Input>.Filter.Eq(x => x.Deleted, false);
        }

        private ProjectionDefinition<Input> GetBaseProjection()
        {
            return Builders<Input>.Projection
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
        }

        private SortDefinition<Input> GetBaseSort()
        {
            return Builders<Input>.Sort
                .Descending(x => x.Anio)
                .Descending(x => x.Folio)
                .Descending(x => x.FechaRecepcion)
                .Descending(x => x.CreatedAt);
        }

        public async Task<List<Input>> GetInputsAsync(int? year = null)
        {
            var filter = GetBaseFilter();

            if (year.HasValue)
            {
                filter &= Builders<Input>.Filter.Eq(x => x.Anio, year.Value);
            }
            else
            {
                filter &= Builders<Input>.Filter.Eq(x => x.Anio, DateTime.Now.Year);
            }

            var projection = GetBaseProjection();
            var sort = GetBaseSort();

            var findOptions = new FindOptions<Input> { Projection = projection, Sort = sort };

            using (var cursor = await _inputsCollection.FindAsync(filter, findOptions))
            {
                return await cursor.ToListAsync();
            }
        }

        public async Task<List<Input>> GetInputsByYearAndAreaAsync(int year, string area)
        {
            var filter = GetBaseFilter() & Builders<Input>.Filter.Eq(x => x.Anio, year) & Builders<Input>.Filter.Eq(x => x.Asignado, area);

            var projection = GetBaseProjection();
            var sort = GetBaseSort();

            var findOptions = new FindOptions<Input> { Projection = projection, Sort = sort };

            using (var cursor = await _inputsCollection.FindAsync(filter, findOptions))
            {
                return await cursor.ToListAsync();
            }
        }

        public async Task<List<Input>> GetInputsByYearAsync(int year)
        {
            var filter = GetBaseFilter() & Builders<Input>.Filter.Eq(x => x.Anio, year);

            var projection = GetBaseProjection();
            var sort = GetBaseSort();

            var findOptions = new FindOptions<Input> { Projection = projection, Sort = sort };

            using (var cursor = await _inputsCollection.FindAsync(filter, findOptions))
            {
                return await cursor.ToListAsync();
            }
        }
    }
}