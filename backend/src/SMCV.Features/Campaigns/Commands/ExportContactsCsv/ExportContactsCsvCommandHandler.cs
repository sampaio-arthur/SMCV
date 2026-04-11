using MediatR;
using SMCV.Application.Interfaces;

namespace SMCV.Features.Campaigns.Commands.ExportContactsCsv;

public class ExportContactsCsvCommandHandler : IRequestHandler<ExportContactsCsvCommand, byte[]>
{
    private readonly IContactRepository _contactRepository;
    private readonly ICsvExportService _csvExportService;

    public ExportContactsCsvCommandHandler(
        IContactRepository contactRepository,
        ICsvExportService csvExportService)
    {
        _contactRepository = contactRepository;
        _csvExportService = csvExportService;
    }

    public async Task<byte[]> Handle(ExportContactsCsvCommand request, CancellationToken cancellationToken)
    {
        var contacts = await _contactRepository.GetByCampaignIdAsync(request.CampaignId);

        if (!contacts.Any())
            throw new InvalidOperationException("Nenhum contato encontrado para exportar.");

        return _csvExportService.GenerateContactsCsv(contacts);
    }
}
