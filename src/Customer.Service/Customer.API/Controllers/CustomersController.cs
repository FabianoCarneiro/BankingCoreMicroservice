using Customer.Application.DTOs;
using Customer.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Customer.API.Controllers;

/// <summary>
/// Controller para gerenciar operações de clientes
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CreateCustomerUseCase _createCustomerUseCase;
    private readonly GetCustomerByIdUseCase _getCustomerByIdUseCase;
    private readonly ListAllCustomersUseCase _listAllCustomersUseCase;
    private readonly UpdateCustomerUseCase _updateCustomerUseCase;
    private readonly DeleteCustomerUseCase _deleteCustomerUseCase;

    public CustomersController(
        CreateCustomerUseCase createCustomerUseCase,
        GetCustomerByIdUseCase getCustomerByIdUseCase,
        ListAllCustomersUseCase listAllCustomersUseCase,
        UpdateCustomerUseCase updateCustomerUseCase,
        DeleteCustomerUseCase deleteCustomerUseCase
    )
    {
        _createCustomerUseCase = createCustomerUseCase;
        _getCustomerByIdUseCase = getCustomerByIdUseCase;
        _listAllCustomersUseCase = listAllCustomersUseCase;
        _updateCustomerUseCase = updateCustomerUseCase;
        _deleteCustomerUseCase = deleteCustomerUseCase;
    }

    /// <summary>
    /// Criar um novo cliente
    /// </summary>
    /// <param name="dto">Dados do cliente a criar</param>
    /// <returns>Cliente criado</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDTO dto)
    {
        try
        {
            var customer = await _createCustomerUseCase.ExecuteAsync(dto);
            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obter um cliente por ID
    /// </summary>
    /// <param name="id">ID do cliente</param>
    /// <returns>Cliente encontrado</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCustomerById([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest(new { error = "ID do cliente inválido" });

        var customer = await _getCustomerByIdUseCase.ExecuteAsync(id);
        
        if (customer == null)
            return NotFound(new { error = "Cliente não encontrado" });

        return Ok(customer);
    }

    /// <summary>
    /// Listar todos os clientes
    /// </summary>
    /// <returns>Lista de clientes</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListAllCustomers()
    {
        var customers = await _listAllCustomersUseCase.ExecuteAsync();
        return Ok(customers);
    }

    /// <summary>
    /// Atualizar um cliente
    /// </summary>
    /// <param name="id">ID do cliente</param>
    /// <param name="dto">Dados a atualizar</param>
    /// <returns>Cliente atualizado</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCustomer([FromRoute] Guid id, [FromBody] UpdateCustomerDTO dto)
    {
        if (id == Guid.Empty)
            return BadRequest(new { error = "ID do cliente inválido" });

        try
        {
            var customer = await _updateCustomerUseCase.ExecuteAsync(id, dto);
            return Ok(customer);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Deletar um cliente
    /// </summary>
    /// <param name="id">ID do cliente</param>
    /// <returns>Sem conteúdo</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCustomer([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest(new { error = "ID do cliente inválido" });

        try
        {
            await _deleteCustomerUseCase.ExecuteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
