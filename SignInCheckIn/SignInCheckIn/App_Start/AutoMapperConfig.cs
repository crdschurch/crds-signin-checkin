using AutoMapper;
using MinistryPlatform.Translation.Models.DTO;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(CreateMappings);
        }

        private static void CreateMappings(IMapperConfigurationExpression config)
        {
            config.CreateMap<MpStateDto, StateDto>().ReverseMap();
            config.CreateMap<MpCountryDto, CountryDto>().ReverseMap();
            config.CreateMap<MpEventRoomDto, EventRoomDto>().ReverseMap();
            config.CreateMap<MpEventDto, EventDto>()
                .ForMember(dest => dest.EventSite, opts => opts.MapFrom(src => src.CongregationName))
                .ForMember(dest => dest.EventSiteId, opts => opts.MapFrom(src => src.CongregationId))
                .ReverseMap()
                .ForMember(dest => dest.CongregationName, opts => opts.MapFrom(src => src.EventSite))
                .ForMember(dest => dest.CongregationId, opts => opts.MapFrom(src => src.EventSiteId));

            config.CreateMap<MpKioskConfigDto, KioskConfigDto>().ReverseMap();
            config.CreateMap<MpParticipantDto, ParticipantDto>().ReverseMap();
            config.CreateMap<MpEventParticipantDto, ParticipantDto>()
                .ForMember(dest => dest.AssignedRoomId, opts => opts.MapFrom(src => src.RoomId))
                .ForMember(dest => dest.AssignedRoomName, opts => opts.MapFrom(src => src.RoomName))
                .ForMember(dest => dest.AssignedSecondaryRoomId, opts => opts.MapFrom(src => src.SecondaryRoomId))
                .ForMember(dest => dest.AssignedSecondaryRoomName, opts => opts.MapFrom(src => src.SecondaryRoomName))
                .ForMember(dest => dest.ParticipationStatusId, opts => opts.MapFrom(src => src.ParticipantStatusId))
                .ForMember(dest => dest.EventParticipantId, opts => opts.MapFrom(src => src.EventParticipantId))
                .ForMember(dest => dest.EventId, opts => opts.MapFrom(src => src.EventId))
                .ForMember(dest => dest.ParticipantId, opts => opts.MapFrom(src => src.ParticipantId))
                .ForMember(dest => dest.FirstName, opts => opts.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opts => opts.MapFrom(src => src.LastName))
                .ForMember(dest => dest.GroupId, opts => opts.MapFrom(src => src.GroupId))
                .ForMember(dest => dest.CallNumber, opts => opts.MapFrom(src => src.CallNumber))
                .ForMember(dest => dest.TimeIn, opts => opts.MapFrom(src => src.TimeIn))
                .ForMember(dest => dest.TimeConfirmed, opts => opts.MapFrom(src => src.TimeConfirmed))
                .ForMember(dest => dest.TimeOut, opts => opts.MapFrom(src => src.TimeOut))
                .ForMember(dest => dest.Notes, opts => opts.MapFrom(src => src.Notes))
                .ForMember(dest => dest.GroupParticipantId, opts => opts.MapFrom(src => src.GroupParticipantId))
                .ForMember(dest => dest.CheckinStation, opts => opts.MapFrom(src => src.CheckinStation))
                .ForMember(dest => dest.CallParents, opts => opts.MapFrom(src => src.CallParents))
                .ForMember(dest => dest.GroupRoleId, opts => opts.MapFrom(src => src.GroupRoleId))
                .ForMember(dest => dest.ResponseId, opts => opts.MapFrom(src => src.ResponseId))
                .ForMember(dest => dest.OpportunityId, opts => opts.MapFrom(src => src.OpportunityId))
                .ForMember(dest => dest.RegistrantMessageSent, opts => opts.MapFrom(src => src.RegistrantMessageSent))
                .ReverseMap()
                .ForMember(dest => dest.RoomId, opts => opts.MapFrom(src => src.AssignedRoomId))
                .ForMember(dest => dest.RoomName, opts => opts.MapFrom(src => src.AssignedRoomName))
                .ForMember(dest => dest.SecondaryRoomId, opts => opts.MapFrom(src => src.AssignedSecondaryRoomId))
                .ForMember(dest => dest.SecondaryRoomName, opts => opts.MapFrom(src => src.AssignedSecondaryRoomName))
                .ForMember(dest => dest.ParticipantStatusId, opts => opts.MapFrom(src => src.ParticipationStatusId))
                .ForMember(dest => dest.EventParticipantId, opts => opts.MapFrom(src => src.EventParticipantId))
                .ForMember(dest => dest.EventId, opts => opts.MapFrom(src => src.EventId))
                .ForMember(dest => dest.ParticipantId, opts => opts.MapFrom(src => src.ParticipantId))
                .ForMember(dest => dest.FirstName, opts => opts.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opts => opts.MapFrom(src => src.LastName))
                .ForMember(dest => dest.GroupId, opts => opts.MapFrom(src => src.GroupId))
                .ForMember(dest => dest.CallNumber, opts => opts.MapFrom(src => src.CallNumber))
                .ForMember(dest => dest.TimeIn, opts => opts.MapFrom(src => src.TimeIn))
                .ForMember(dest => dest.TimeConfirmed, opts => opts.MapFrom(src => src.TimeConfirmed))
                .ForMember(dest => dest.TimeOut, opts => opts.MapFrom(src => src.TimeOut))
                .ForMember(dest => dest.Notes, opts => opts.MapFrom(src => src.Notes))
                .ForMember(dest => dest.GroupParticipantId, opts => opts.MapFrom(src => src.GroupParticipantId))
                .ForMember(dest => dest.CheckinStation, opts => opts.MapFrom(src => src.CheckinStation))
                .ForMember(dest => dest.CallParents, opts => opts.MapFrom(src => src.CallParents))
                .ForMember(dest => dest.GroupRoleId, opts => opts.MapFrom(src => src.GroupRoleId))
                .ForMember(dest => dest.ResponseId, opts => opts.MapFrom(src => src.ResponseId))
                .ForMember(dest => dest.OpportunityId, opts => opts.MapFrom(src => src.OpportunityId))
                .ForMember(dest => dest.RegistrantMessageSent, opts => opts.MapFrom(src => src.RegistrantMessageSent))
                .ForMember(dest => dest.CheckinHouseholdId, opts => opts.MapFrom(src => src.CheckinHouseholdId))
                .ForMember(dest => dest.CheckinPhone, opts => opts.MapFrom(src => src.CheckinPhone));
            config.CreateMap<MpRoomDto, RoomDto>().ReverseMap();
            config.CreateMap<MpHouseholdDto, HouseholdDto>().ReverseMap();
            config.CreateMap<MpContactDto, ContactDto>().ReverseMap();
            config.CreateMap<MpCongregationDto, CongregationDto>()
                .ForMember(dest => dest.CongregationId, opts => opts.MapFrom(src => src.CongregationId))
                .ForMember(dest => dest.CongregationName, opts => opts.MapFrom(src => src.CongregationName))
                .ReverseMap()
                .ForMember(dest => dest.CongregationId, opts => opts.MapFrom(src => src.CongregationId))
                .ForMember(dest => dest.CongregationName, opts => opts.MapFrom(src => src.CongregationName));
        }
    }
}
