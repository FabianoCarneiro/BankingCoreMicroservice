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
    private readonly GetCustomerByIdUseCase _getCustomerByIdUseCase;
    private readonly ListAllCustomersUseCase _listAllCustomersUseCase;

    public CustomersController(
        CreateCustomerUseCase createCustomerUseCase,
        GetCustomerByIdUseCase getCustomerByIdUseCase,
        ListAllCustomersUseCase listAllCustomersUseCase)
    {
        _createCustomerUseCase = createCustomerUseCase;
        _getCustomerByIdUseCase = getCustomerByIdUseCase;
        _listAllCustomersUseCase = listAllCustomersUseCase;
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
            return CreatedAtAction(nameof(GetCustomerById), new { id = result.Id }, result);
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

    /// <summary>
    /// Obter cliente por ID
    /// </summary>
    /// <param name="id">ID do cliente</param>
    /// <returns>Dados do cliente</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCustomerById([FromRoute] Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest(new { error = "ID do cliente inválido" });

            var customer = await _getCustomerByIdUseCase.ExecuteAsync(id);
            
            if (customer == null)
                return NotFound(new { error = "Cliente não encontrado" });

            return Ok(customer);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao consultar cliente", details = ex.Message });
        }
    }

    /// <summary>
    /// Listar todos os clientes
    /// </summary>
    /// <returns>Lista de clientes</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAllCustomers()
    {
        try
        {
            var customers = await _listAllCustomersUseCase.ExecuteAsync();
            return Ok(customers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao listar clientes", details = ex.Message });
        }
    }
}
