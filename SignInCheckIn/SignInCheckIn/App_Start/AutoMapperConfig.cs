using AutoMapper;
using MinistryPlatform.Translation.Models.DTO;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.App_Start
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(CreateMappings);
        }

        private static void CreateMappings(IMapperConfigurationExpression config)
        {
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
                .ReverseMap()
                .ForMember(dest => dest.RoomId, opts => opts.MapFrom(src => src.AssignedRoomId))
                .ForMember(dest => dest.RoomName, opts => opts.MapFrom(src => src.AssignedRoomName))
                .ForMember(dest => dest.SecondaryRoomId, opts => opts.MapFrom(src => src.AssignedSecondaryRoomId))
                .ForMember(dest => dest.SecondaryRoomName, opts => opts.MapFrom(src => src.AssignedSecondaryRoomName))
                .ForMember(dest => dest.ParticipantStatusId, opts => opts.MapFrom(src => src.ParticipationStatusId));
            config.CreateMap<MpRoomDto, RoomDto>().ReverseMap();
            config.CreateMap<MpContactDto, ContactDto>().ReverseMap();
        }
    }
}
