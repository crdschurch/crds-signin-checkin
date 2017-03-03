using System;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Crossroads.Utilities.Services.Interfaces;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services;

namespace SignInCheckIn.Tests.Services
{
    public class SignInLogicTest
    {
        private Mock<IEventRepository> _eventRepository;
        private Mock<IRoomRepository> _roomRepository;
        private Mock<IAttributeRepository> _attributeRepository;
        private Mock<IGroupRepository> _groupRepository;
        private Mock<IApplicationConfiguration> _applicationConfiguration;
        private Mock<IApiUserRepository> _apiUserRepository;
        private Mock<IConfigRepository> _configRepository;

        private const int AgesAttributeTypeId = 102;
        private const int BirthMonthsAttributeTypeId = 103;
        private const int GradesAttributeTypeId = 104;
        private const int NurseryAgeAttributeId = 9014;
        private const int NurseryAgesAttributeTypeId = 105;

        private List<MpAttributeDto> _ageList;
        private List<MpAttributeDto> _gradeList;
        private List<MpAttributeDto> _birthMonthList;
        private List<MpAttributeDto> _nurseryMonthList;

        private SignInLogic _fixture;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();

            _eventRepository = new Mock<IEventRepository>();
            _roomRepository = new Mock<IRoomRepository>();
            _attributeRepository = new Mock<IAttributeRepository>(MockBehavior.Strict);
            _groupRepository = new Mock<IGroupRepository>(MockBehavior.Strict);
            _applicationConfiguration = new Mock<IApplicationConfiguration>();
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _configRepository = new Mock<IConfigRepository>();
            _applicationConfiguration.SetupGet(mocked => mocked.AgesAttributeTypeId).Returns(AgesAttributeTypeId);
            _applicationConfiguration.SetupGet(mocked => mocked.BirthMonthsAttributeTypeId).Returns(BirthMonthsAttributeTypeId);
            _applicationConfiguration.SetupGet(mocked => mocked.GradesAttributeTypeId).Returns(GradesAttributeTypeId);
            _applicationConfiguration.SetupGet(mocked => mocked.NurseryAgeAttributeId).Returns(NurseryAgeAttributeId);
            _applicationConfiguration.SetupGet(mocked => mocked.NurseryAgesAttributeTypeId).Returns(NurseryAgesAttributeTypeId);

            MpConfigDto earlyCheckInPeriodConfig = new MpConfigDto
            {
                ApplicationCode = "CHECKIN",
                KeyName = "DefaultEarlyCheckIn",
                ConfigurationSettingId = 1,
                Value = "60"
            };

            _configRepository.Setup(m => m.GetMpConfigByKey("DefaultEarlyCheckIn")).Returns(earlyCheckInPeriodConfig);

            MpConfigDto lateCheckInPeriodConfig = new MpConfigDto
            {
                ApplicationCode = "CHECKIN",
                KeyName = "DefaultLateCheckIn",
                ConfigurationSettingId = 1,
                Value = "30"
            };

            _configRepository.Setup(m => m.GetMpConfigByKey("DefaultLateCheckIn")).Returns(lateCheckInPeriodConfig);


            _ageList =
                JsonConvert.DeserializeObject<List<MpAttributeDto>>(
                    "[{'Attribute_ID':9014,'Attribute_Name':'Nursery','Sort_Order':1000,'Attribute_Type_ID':102,'Attribute_Type':'KC eCheck Age'},{'Attribute_ID':9015,'Attribute_Name':'First Year','Sort_Order':1,'Attribute_Type_ID':102,'Attribute_Type':'KC eCheck Age'},{'Attribute_ID':9016,'Attribute_Name':'Second Year','Sort_Order':2,'Attribute_Type_ID':102,'Attribute_Type':'KC eCheck Age'},{'Attribute_ID':9017,'Attribute_Name':'Third Year','Sort_Order':3,'Attribute_Type_ID':102,'Attribute_Type':'KC eCheck Age'},{'Attribute_ID':9018,'Attribute_Name':'Fourth Year','Sort_Order':4,'Attribute_Type_ID':102,'Attribute_Type':'KC eCheck Age'},{'Attribute_ID':9019,'Attribute_Name':'Fifth Year','Sort_Order':5,'Attribute_Type_ID':102,'Attribute_Type':'KC eCheck Age'}]");
            _gradeList =
                JsonConvert.DeserializeObject<List<MpAttributeDto>>(
                    "[{'Attribute_ID':9032,'Attribute_Name':'Kindergarten','Sort_Order':1000,'Attribute_Type_ID':104,'Attribute_Type':'KC eCheck Grade'},{'Attribute_ID':9033,'Attribute_Name':'First Grade','Sort_Order':1,'Attribute_Type_ID':104,'Attribute_Type':'KC eCheck Grade'},{'Attribute_ID':9034,'Attribute_Name':'Second Grade','Sort_Order':2,'Attribute_Type_ID':104,'Attribute_Type':'KC eCheck Grade'},{'Attribute_ID':9035,'Attribute_Name':'Third Grade','Sort_Order':3,'Attribute_Type_ID':104,'Attribute_Type':'KC eCheck Grade'},{'Attribute_ID':9036,'Attribute_Name':'Fourth Grade','Sort_Order':4,'Attribute_Type_ID':104,'Attribute_Type':'KC eCheck Grade'},{'Attribute_ID':9037,'Attribute_Name':'Fifth Grade','Sort_Order':5,'Attribute_Type_ID':104,'Attribute_Type':'KC eCheck Grade'},{'Attribute_ID':9038,'Attribute_Name':'Sixth Grade','Sort_Order':6,'Attribute_Type_ID':104,'Attribute_Type':'KC eCheck Grade'},{'Attribute_ID':9039,'Attribute_Name':'CSM','Sort_Order':7,'Attribute_Type_ID':104,'Attribute_Type':'KC eCheck Grade'}]");
            _birthMonthList =
                JsonConvert.DeserializeObject<List<MpAttributeDto>>(
                    "[{'Attribute_ID':9002,'Attribute_Name':'January','Sort_Order':1000,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9003,'Attribute_Name':'February','Sort_Order':1,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9004,'Attribute_Name':'March','Sort_Order':2,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9005,'Attribute_Name':'April','Sort_Order':3,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9006,'Attribute_Name':'May','Sort_Order':4,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9007,'Attribute_Name':'June','Sort_Order':5,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9008,'Attribute_Name':'July','Sort_Order':6,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9009,'Attribute_Name':'August','Sort_Order':7,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9010,'Attribute_Name':'September','Sort_Order':8,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9011,'Attribute_Name':'October','Sort_Order':9,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9012,'Attribute_Name':'November','Sort_Order':10,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9013,'Attribute_Name':'December','Sort_Order':11,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'}]");
            _nurseryMonthList =
                JsonConvert.DeserializeObject<List<MpAttributeDto>>(
                    "[{'Attribute_ID':9020,'Attribute_Name':'0-1','Sort_Order':1000,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9021,'Attribute_Name':'1-2','Sort_Order':1,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9022,'Attribute_Name':'2-3','Sort_Order':2,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9023,'Attribute_Name':'3-4','Sort_Order':3,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9024,'Attribute_Name':'4-5','Sort_Order':4,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9025,'Attribute_Name':'5-6','Sort_Order':5,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9026,'Attribute_Name':'6-7','Sort_Order':6,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9027,'Attribute_Name':'7-8','Sort_Order':7,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9028,'Attribute_Name':'8-9','Sort_Order':8,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9029,'Attribute_Name':'9-10','Sort_Order':9,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9030,'Attribute_Name':'10-11','Sort_Order':10,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9031,'Attribute_Name':'11-12','Sort_Order':11,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'}]");


            //_fixture = new RoomService(_eventRepository.Object, _roomRepository.Object, _attributeRepository.Object, _groupRepository.Object, _applicationConfiguration.Object, _apiUserRepository.Object);
            _fixture = new SignInLogic(_eventRepository.Object, _applicationConfiguration.Object, _configRepository.Object,
                _groupRepository.Object, _roomRepository.Object);
        }

        [Test]
        public void ShouldGetSingleEventForSignIn()
        {
            // Arrange
            int siteId = 1;
            bool underThreeSignIn = false;
            bool adventureClubSignIn = false;

            var signInTime = new DateTime(2017, 3, 3, 0, 00, 00);

            _eventRepository.Setup(r => r.GetEvents(signInTime, signInTime, 1, true)).Returns(GetTestEventSet());

            // Act
            var result = _fixture.GetSignInEvents(siteId, underThreeSignIn, adventureClubSignIn);

            // Assert
            Assert.AreEqual(result[0].EventId, 1234567);
            Assert.AreEqual(result.Count, 1);
        }

        [Test]
        public void ShouldGetAdventureClubEventsForSignIn()
        {
            // Arrange
            int siteId = 1;
            bool underThreeSignIn = false;
            bool adventureClubSignIn = false;

            var signInTime = new DateTime(2017, 3, 3, 0, 00, 00);

            _eventRepository.Setup(r => r.GetEvents(signInTime, signInTime, 1, true)).Returns(GetTestEventSet());

            // Act
            var result = _fixture.GetSignInEvents(siteId, underThreeSignIn, adventureClubSignIn);

            // Assert
            var serviceEventCount = result.Count(r => r.ParentEventId == null);
            var adventureClubEventCount = result.Count(r => r.ParentEventId != null);
            Assert.AreEqual(serviceEventCount, 2);
            Assert.AreEqual(adventureClubEventCount, 2);
            Assert.AreEqual(result.Count, 4);
        }


        private List<MpEventDto> GetTestEventSet()
        {
            // we need to set times dynamically on the test data set, to avoid having unit tests
            // break when run at later or earlier hours - this may still be a trouble spot
            var currentStartTime = System.DateTime.Now;
            var futureStartTime = System.DateTime.Now.AddHours(2);
            var invalidFutureStartTime = System.DateTime.Now.AddHours(4);

            List<MpEventDto> mpEventDtos = new List<MpEventDto>
            {
                new MpEventDto
                {
                    EventId = 1234567,
                    EventStartDate = currentStartTime,
                    EventTitle = "First Non Ac Event",
                    Cancelled = false,
                    ParentEventId = null
                },
                new MpEventDto
                {
                    EventId = 7654321,
                    EventStartDate = currentStartTime,
                    EventTitle = "First Ac Event",
                    Cancelled = false,
                    ParentEventId = 1234567
                },
                new MpEventDto
                {
                    EventId = 2345678,
                    EventStartDate = futureStartTime,
                    EventTitle = "Second Non Ac Event",
                    Cancelled = false,
                    ParentEventId = null
                },
                new MpEventDto
                {
                    EventId = 8765432,
                    EventStartDate = futureStartTime,
                    EventTitle = "Second Non Ac Event",
                    Cancelled = false,
                    ParentEventId = 2345678
                },
                new MpEventDto
                {
                    EventId = 3456789,
                    EventStartDate = futureStartTime,
                    EventTitle = "Second Non Ac Event",
                    Cancelled = false,
                    ParentEventId = null
                },
                new MpEventDto
                {
                    EventId = 9876543,
                    EventStartDate = futureStartTime,
                    EventTitle = "Second Non Ac Event",
                    Cancelled = false,
                    ParentEventId = 3456789
                }
            };

            return mpEventDtos;
        } 
    }
}
