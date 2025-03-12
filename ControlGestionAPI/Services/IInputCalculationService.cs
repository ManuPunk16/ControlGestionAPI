using ControlGestionAPI.Controllers;
using ControlGestionAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ControlGestionAPI.Services
{
    public interface IInputCalculationService
    {
        InputDto CalculateInputDto(Input input);
    }
}
