using AutoMapper;
using SMCV.Application.DTOs.Campaigns;
using SMCV.Application.DTOs.Contacts;
using SMCV.Application.DTOs.EmailLogs;
using SMCV.Domain.Entities;

namespace SMCV.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // EmailLog → EmailLogResponse
        CreateMap<EmailLog, EmailLogResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // Contact → ContactResponse
        CreateMap<Contact, ContactResponse>()
            .ForMember(dest => dest.EmailLog, opt =>
            {
                opt.Condition(src => src.EmailLog != null);
                opt.MapFrom(src => src.EmailLog);
            });

        // Campaign → CampaignResponse
        CreateMap<Campaign, CampaignResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.TotalContacts, opt =>
                opt.MapFrom(src => src.Contacts != null ? src.Contacts.Count() : 0));

        // Campaign → CampaignDetailResponse
        CreateMap<Campaign, CampaignDetailResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => src.Contacts));
    }
}
