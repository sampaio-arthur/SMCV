using System.Text;
using SMCV.Application.Interfaces;
using SMCV.Domain.Entities;

namespace SMCV.Infrastructure.ExternalServices;

public class CsvExportService : ICsvExportService
{
    public byte[] GenerateContactsCsv(IEnumerable<Contact> contacts)
    {
        var sb = new StringBuilder();

        sb.AppendLine("CompanyName,Email,Domain,ContactName,Position,Source,CampaignId");

        foreach (var c in contacts)
        {
            sb.AppendLine(string.Join(",",
                Escape(c.CompanyName),
                Escape(c.Email),
                Escape(c.Domain),
                Escape(c.ContactName ?? ""),
                Escape(c.Position ?? ""),
                Escape(c.Source),
                c.CampaignId));
        }

        var preamble = Encoding.UTF8.GetPreamble();
        var csvBytes = Encoding.UTF8.GetBytes(sb.ToString());

        var result = new byte[preamble.Length + csvBytes.Length];
        preamble.CopyTo(result, 0);
        csvBytes.CopyTo(result, preamble.Length);

        return result;
    }

    private static string Escape(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
