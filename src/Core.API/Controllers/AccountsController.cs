using Core.Application.DTOs;
using Core.Application.UseCases;
using Core.Domain.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controllers;

/// <summary>
/// Controlador REST para operações de contas bancárias
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly CreateBankAccountUseCase _createAccountUseCase;
    private readonly TransferUseCase _transferUseCase;
    private readonly IBankAccountRepository _accountRepository;

    public AccountsController(
        CreateBankAccountUseCase createAccountUseCase,
        TransferUseCase transferUseCase,
        IBankAccountRepository accountRepository)
    {
        _createAccountUseCase = createAccountUseCase;
        _transferUseCase = transferUseCase;
        _accountRepository = accountRepository;
    }

    /// <summary>
    /// Criar nova conta corrente
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAccount([FromBody] CreateBankAccountDTO dto)
    {
        try
        {
            var result = await _createAccountUseCase.ExecuteAsync(dto);
            return CreatedAtAction(nameof(CreateAccount), result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao criar conta", details = ex.Message });
        }
    }

    /// <summary>
    /// Obter saldo da conta
    /// </summary>
    [HttpGet("{accountNumber}/balance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBalance(string accountNumber)
    {
        try
        {
            var account = await _accountRepository.GetByAccountNumberAsync(accountNumber);
            if (account == null)
                return NotFound(new { error = "Conta não encontrada" });

            return Ok(new
            {
                accountNumber = account.AccountNumber,
                balance = account.Balance.Amount,
                currency = account.Balance.Currency
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao buscar saldo", details = ex.Message });
        }
    }

    /// <summary>
    /// Obter extrato da conta
    /// </summary>
    [HttpGet("{accountNumber}/statement")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatement(string accountNumber)
    {
        try
        {
            var account = await _accountRepository.GetByAccountNumberAsync(accountNumber);
            if (account == null)
                return NotFound(new { error = "Conta não encontrada" });

            var transactions = account.Transactions.Select(t => new TransactionDTO
            {
                Id = t.Id,
                Type = t.Type.ToString(),
                Amount = t.Amount.Amount,
                Description = t.Description,
                CreatedAt = t.CreatedAt
            }).ToList();

            return Ok(new
            {
                accountNumber = account.AccountNumber,
                balance = account.Balance.Amount,
                transactions = transactions
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao buscar extrato", details = ex.Message });
        }
    }

    /// <summary>
    /// Realizar transferência (TED/PIX)
    /// </summary>
    [HttpPost("transfer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Transfer([FromBody] TransferRequestDTO dto)
    {
        try
        {
            await _transferUseCase.ExecuteAsync(dto.FromAccountNumber, dto.ToAccountNumber, dto.Amount);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao processar transferência", details = ex.Message });
        }
    }
}

public class TransferRequestDTO
{
    public string FromAccountNumber { get; set; } = string.Empty;
    public string ToAccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
