using Crossroads.Web.Common.MinistryPlatform;
using FluentAssertions;
using MinistryPlatform.Translation.Models;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace MinistryPlatform.Translation.Test.Repositories
{
    public class AttributeRepositoryTest
    {
        private AttributeRepository _fixture;

        private Mock<IApiUserRepository> _apiUserRepository;
        private Mock<IMinistryPlatformRestRepository> _ministryPlatformRestRepository;

        private List<string> _attributeColumns;


        [SetUp]
        public void SetUp()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _ministryPlatformRestRepository = new Mock<IMinistryPlatformRestRepository>(MockBehavior.Strict);
            _attributeColumns = new List<string>
            {
                "Attributes.[Attribute_ID]",
                "Attributes.[Attribute_Name]",
                "Attributes.[Sort_Order]",
                "Attribute_Type_ID_Table.[Attribute_Type_ID]",
                "Attribute_Type_ID_Table.[Attribute_Type]"
            };

            _fixture = new AttributeRepository(_apiUserRepository.Object, _ministryPlatformRestRepository.Object);
        }

        [Test]
        public void TestGetAttributesByAttributeTypeIdSingleIdWithAuthenticationToken()
        {
            const int typeId = 123;
            const string token = "456";

            var attrs = new List<MpAttributeDto>();
            _apiUserRepository.Setup(m => m.GetApiClientToken("CRDS.Service.SignCheckIn")).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpAttributeDto>($"Attribute_Type_ID_Table.[Attribute_Type_ID] IN ({typeId})", _attributeColumns, null, false)).Returns(attrs);

            var result = _fixture.GetAttributesByAttributeTypeId(typeId);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();

            result.Should().BeSameAs(attrs);
        }

        [Test]
        public void TestGetAttributesByAttributeTypeIdSingleIdWithoutAuthenticationToken()
        {
            const int typeId = 123;
            const string token = "456";

            _apiUserRepository.Setup(mocked => mocked.GetApiClientToken("CRDS.Service.SignCheckIn")).Returns(token);

            var attrs = new List<MpAttributeDto>();
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpAttributeDto>($"Attribute_Type_ID_Table.[Attribute_Type_ID] IN ({typeId})", _attributeColumns, null, false)).Returns(attrs);

            var result = _fixture.GetAttributesByAttributeTypeId(typeId);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();

            result.Should().BeSameAs(attrs);
        }

        [Test]
        public void TestGetAttributesByAttributeTypeIdMultipleIdWithAuthenticationToken()
        {
            var typeIds = new[] { 123, 456 };
            const string token = "456";

            var attrs = new List<MpAttributeDto>();
            _apiUserRepository.Setup(m => m.GetApiClientToken("CRDS.Service.SignCheckIn")).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(
                mocked => mocked.Search<MpAttributeDto>($"Attribute_Type_ID_Table.[Attribute_Type_ID] IN ({string.Join(",", typeIds)})", _attributeColumns, null, false)).Returns(attrs);

            var result = _fixture.GetAttributesByAttributeTypeId(typeIds);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();

            result.Should().BeSameAs(attrs);
        }

        [Test]
        public void TestGetAttributesByAttributeTypeIdMultipleIdWithoutAuthenticationToken()
        {
            var typeIds = new[] { 123, 456 };
            const string token = "456";

            _apiUserRepository.Setup(mocked => mocked.GetApiClientToken("CRDS.Service.SignCheckIn")).Returns(token);

            var attrs = new List<MpAttributeDto>();
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpAttributeDto>($"Attribute_Type_ID_Table.[Attribute_Type_ID] IN ({string.Join(",", typeIds)})", _attributeColumns, null, false)).Returns(attrs);

            var result = _fixture.GetAttributesByAttributeTypeId(typeIds);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();

            result.Should().BeSameAs(attrs);
        }

        [Test]
        public void TestCreateContactAttribute()
        {
            const string token = "456";
            var attributeColumns = new List<string>
            {
                "Contact_ID",
                "Attribute_ID"
            };
            var contractAttributeDto = new MpContactAttributeDto()
            {
                Attribute_ID = 33,
                Contact_ID = 44
            };

            _apiUserRepository.Setup(mocked => mocked.GetApiClientToken("CRDS.Service.SignCheckIn")).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(m => m.Create(contractAttributeDto, attributeColumns)).Returns(contractAttributeDto);

            var result = _fixture.CreateContactAttribute(contractAttributeDto);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();

            result.Should().BeSameAs(contractAttributeDto);
        }
    }
}
