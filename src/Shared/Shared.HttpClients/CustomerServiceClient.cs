using Shared.DTOs.Customer;
using System.Net.Http.Json;
using System.Text.Json;

namespace Shared.HttpClients;

/// <summary>
/// Cliente HTTP para comunicação com o Customer Service
/// </summary>
public interface ICustomerServiceClient
{
    Task<CustomerDTO?> GetCustomerByIdAsync(Guid id);
    Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync();
    Task<CustomerDTO> CreateCustomerAsync(CreateCustomerRequest request);
}

public class CreateCustomerRequest
{
    public string CPF { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

/// <summary>
/// Implementação do cliente HTTP para Customer Service
/// </summary>
public class CustomerServiceClient : ICustomerServiceClient
{
    private readonly HttpClient _httpClient;

    public CustomerServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CustomerDTO?> GetCustomerByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/customers/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<CustomerDTO>(content);
            }
            
            return null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao buscar cliente {id} do Customer Service", ex);
        }
    }

    public async Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/customers");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<CustomerDTO>>(content) ?? new List<CustomerDTO>();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Erro ao buscar clientes do Customer Service", ex);
        }
    }

    public async Task<CustomerDTO> CreateCustomerAsync(CreateCustomerRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/customers", request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return (JsonSerializer.Deserialize<CustomerDTO>(content)) ?? throw new InvalidOperationException("Resposta vazia do Customer Service");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Erro ao criar cliente no Customer Service", ex);
        }
    }
}
