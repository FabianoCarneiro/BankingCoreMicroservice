using Core.Application.DTOs;
using Core.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controllers;

/// <summary>
/// Controlador REST para operações de clientes (KYC)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CreateCustomerUseCase _createCustomerUseCase;

    public CustomersController(CreateCustomerUseCase createCustomerUseCase)
    {
        _createCustomerUseCase = createCustomerUseCase;
    }

    /// <summary>
    /// Criar novo cliente
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDTO dto)
    {
        try
        {
            var result = await _createCustomerUseCase.ExecuteAsync(dto);
            return CreatedAtAction(nameof(CreateCustomer), result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao criar cliente", details = ex.Message });
        }
    }
}
