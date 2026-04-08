using AutoMapper;
using SMCV.Application.DTOs.Campaigns;
using SMCV.Application.DTOs.Contacts;
using SMCV.Application.DTOs.EmailLogs;
using SMCV.Application.DTOs.UserProfiles;
using SMCV.Application.DTOs.Users;
using SMCV.Domain.Entities;

namespace SMCV.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User → UserResponse
        CreateMap<User, UserResponse>();

        // UserProfile → UserProfileResponse
        CreateMap<UserProfile, UserProfileResponse>();

        // EmailLog → EmailLogResponse
        CreateMap<EmailLog, EmailLogResponse>();

        // Contact → ContactResponse
        CreateMap<Contact, ContactResponse>()
            .ForMember(dest => dest.EmailStatus,
                       opt => opt.MapFrom(src => src.EmailStatus.ToString()));

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
