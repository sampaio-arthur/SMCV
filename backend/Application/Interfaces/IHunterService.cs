namespace SMCV.Application.Interfaces;

public interface IHunterService
{
    /// <summary>
    /// Busca e-mails de empresas por domínio/nicho em uma região.
    /// Retorna lista de contatos encontrados.
    /// </summary>
    Task<IEnumerable<HunterContactResult>> SearchContactsAsync(
        string niche,
        string region,
        int limit = 10);
}
