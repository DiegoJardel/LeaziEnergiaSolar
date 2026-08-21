using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class OpcaoRelatorioComissaoDto
{
    public TipoRelatorioComissao Tipo { get; init; }

    public string Descricao { get; init; } =
        string.Empty;
}