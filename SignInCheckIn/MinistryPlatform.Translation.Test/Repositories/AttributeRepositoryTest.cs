using System.Collections.Generic;
using FluentAssertions;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;

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
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpAttributeDto>($"Attribute_Type_ID_Table.[Attribute_Type_ID] IN ({typeId})", _attributeColumns)).Returns(attrs);

            var result = _fixture.GetAttributesByAttributeTypeId(typeId, token);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();

            result.Should().BeSameAs(attrs);
        }

        [Test]
        public void TestGetAttributesByAttributeTypeIdSingleIdWithoutAuthenticationToken()
        {
            const int typeId = 123;
            const string token = "456";

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns(token);

            var attrs = new List<MpAttributeDto>();
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpAttributeDto>($"Attribute_Type_ID_Table.[Attribute_Type_ID] IN ({typeId})", _attributeColumns)).Returns(attrs);

            var result = _fixture.GetAttributesByAttributeTypeId(typeId);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();

            result.Should().BeSameAs(attrs);
        }

        [Test]
        public void TestGetAttributesByAttributeTypeIdMultipleIdWithAuthenticationToken()
        {
            var typeIds = new[] {123, 456};
            const string token = "456";

            var attrs = new List<MpAttributeDto>();
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(
                mocked => mocked.Search<MpAttributeDto>($"Attribute_Type_ID_Table.[Attribute_Type_ID] IN ({string.Join(",", typeIds)})", _attributeColumns)).Returns(attrs);

            var result = _fixture.GetAttributesByAttributeTypeId(typeIds, token);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();

            result.Should().BeSameAs(attrs);
        }

        [Test]
        public void TestGetAttributesByAttributeTypeIdMultipleIdWithoutAuthenticationToken()
        {
            var typeIds = new[] { 123, 456 };
            const string token = "456";

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns(token);

            var attrs = new List<MpAttributeDto>();
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpAttributeDto>($"Attribute_Type_ID_Table.[Attribute_Type_ID] IN ({string.Join(",", typeIds)})", _attributeColumns)).Returns(attrs);

            var result = _fixture.GetAttributesByAttributeTypeId(typeIds);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();

            result.Should().BeSameAs(attrs);
        }
    }
}
