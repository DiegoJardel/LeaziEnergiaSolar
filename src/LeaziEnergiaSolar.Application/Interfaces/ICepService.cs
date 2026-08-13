using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Interfaces;

public interface ICepService
{
    Task<EnderecoCepDto?> ConsultarAsync(
        string cep,
        CancellationToken cancellationToken = default);
}
