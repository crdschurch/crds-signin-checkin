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
            config.CreateMap<MpEventDto, EventDto>().ReverseMap();
        }
    }
}