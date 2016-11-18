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
                //.ForMember(dest => dest.AssignedRoomName, opts => opts.MapFrom(src => src.))
                .ReverseMap()
                .ForMember(dest => dest.RoomId, opts => opts.MapFrom(src => src.AssignedRoomId));
            config.CreateMap<MpRoomDto, RoomDto>().ReverseMap();
        }
    }
}
