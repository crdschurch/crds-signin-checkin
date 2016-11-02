using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Components.DictionaryAdapter;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using SignInCheckIn.App_Start;
using SignInCheckIn.Services;

namespace SignInCheckIn.Tests.Services
{
    public class ChildSigninServiceTest
    {
        private Mock<IChildSigninRepository> _childCheckinRepository;
        private Mock<IConfigRepository> _configRepository;
        private Mock<IEventRepository> _eventRepository;

        private ChildSigninService _fixture;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();

            _childCheckinRepository = new Mock<IChildSigninRepository>();
            _configRepository = new Mock<IConfigRepository>();
            _eventRepository = new Mock<IEventRepository>();

            MpConfigDto mpConfigDtoEarly = new MpConfigDto
            {
                ApplicationCode = "COMMON",
                ConfigurationSettingId = 1,
                KeyName = "DefaultEarlyCheckIn",
                Value = "60"
            };

            MpConfigDto mpConfigDtoLate = new MpConfigDto
            {
                ApplicationCode = "COMMON",
                ConfigurationSettingId = 1,
                KeyName = "DefaultLateCheckIn",
                Value = "60"
            };

            _configRepository.Setup(m => m.GetMpConfigByKey("DefaultEarlyCheckIn")).Returns(mpConfigDtoEarly);
            _configRepository.Setup(m => m.GetMpConfigByKey("DefaultLateCheckIn")).Returns(mpConfigDtoLate);

            _fixture = new ChildSigninService(_childCheckinRepository.Object, _configRepository.Object, _eventRepository.Object);
        }

        [Test]
        public void ShouldGetChildrenByPhoneNumber()
        {
            var siteId = 1;
            var phoneNumber = "812-812-8877";
            var primaryHouseholdId = 123;

            List<MpEventDto> events = new List<MpEventDto>
            {
                new MpEventDto
                {
                    CongregationId = 1,
                    CongregationName = "Oakley",
                    EarlyCheckinPeriod = 30,
                    EventEndDate = DateTime.Now.AddDays(1),
                    EventId = 1234567,
                    EventStartDate = DateTime.Now,
                    EventTitle = "test event",
                    EventType = "type test",
                    LateCheckinPeriod = 30,
                    LocationId = 3
                }
            };

            List<MpParticipantDto> mpParticipantDto = new List<MpParticipantDto>
            {
                new MpParticipantDto
                {
                    ParticipantId = 12,
                    ContactId = 1443,
                    HouseholdId = primaryHouseholdId,
                    HouseholdPositionId = 2,
                    FirstName = "First1",
                    LastName = "Last1",
                    DateOfBirth = new DateTime()
                }
            };

            _childCheckinRepository.Setup(m => m.GetChildrenByPhoneNumber(phoneNumber)).Returns(mpParticipantDto);
            _eventRepository.Setup(m => m.GetEvents(It.IsAny<DateTime>(), It.IsAny<DateTime>(), siteId)).Returns(events);
            var result = _fixture.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId);
            _childCheckinRepository.VerifyAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(mpParticipantDto[0].ParticipantId, result.Participants[0].ParticipantId);
            Assert.AreEqual(mpParticipantDto[0].ContactId, result.Participants[0].ContactId);
        }

        [Test]
        public void ShouldNotGetChildrenByPhoneNumber()
        {
            var siteId = 1;
            var phoneNumber = "812-812-8877";

            List<MpEventDto> events = new List<MpEventDto>
            {
                new MpEventDto
                {
                    CongregationId = 1,
                    CongregationName = "Oakley",
                    EarlyCheckinPeriod = 30,
                    EventEndDate = DateTime.Now.AddDays(1),
                    EventId = 1234567,
                    EventStartDate = DateTime.Now,
                    EventTitle = "test event",
                    EventType = "type test",
                    LateCheckinPeriod = 30,
                    LocationId = 3
                }
            };

            List<MpParticipantDto> mpParticipantDto = new List<MpParticipantDto>();

            _eventRepository.Setup(m => m.GetEvents(It.IsAny<DateTime>(), It.IsAny<DateTime>(), siteId)).Returns(events);
            _childCheckinRepository.Setup(m => m.GetChildrenByPhoneNumber(phoneNumber)).Returns(mpParticipantDto);

            var result = _fixture.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId);
            _childCheckinRepository.VerifyAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.Participants.Any());
        }
    }
}
