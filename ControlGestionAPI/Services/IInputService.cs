using ControlGestionAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlGestionAPI.Services
{
    public interface IInputService
    {
        Task<List<Input>> GetInputsAsync(int? year = null);
        Task<List<Input>> GetInputsByYearAndAreaAsync(int year, string area);
        Task<List<Input>> GetInputsByYearAsync(int year);
    }
}
