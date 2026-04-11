namespace SMCV.Application.Interfaces;

using SMCV.Domain.Entities;

public interface ICsvExportService
{
    /// <summary>
    /// Gera CSV em memória a partir de uma lista de contatos.
    /// Retorna byte[] pronto para download.
    /// </summary>
    byte[] GenerateContactsCsv(IEnumerable<Contact> contacts);
}
