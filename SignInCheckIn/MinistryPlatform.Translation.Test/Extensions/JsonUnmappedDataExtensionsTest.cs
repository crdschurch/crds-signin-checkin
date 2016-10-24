using System.Collections.Generic;
using FluentAssertions;
using MinistryPlatform.Translation.Extensions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace MinistryPlatform.Translation.Test.Extensions
{
    public class JsonUnmappedDataExtensionsTest
    {
        private IDictionary<string, JToken> _fixture;

        private const string FieldName = "field1";
        private const string FieldValue = "field1 value";

        [SetUp]
        public void SetUp()
        {
            _fixture = new Dictionary<string, JToken>
            {
                {FieldName, JToken.FromObject(FieldValue)}
            };
        }

        [Test]
        public void TestGetUnmappedDataFieldExists()
        {
            var field = _fixture.GetUnmappedDataField<string>(FieldName);
            Assert.IsNotNull(field);
            Assert.AreEqual(FieldValue, field);
        }

        [Test]
        public void TestGetUnmappedDataFieldDoesNotExist()
        {
            var field = _fixture.GetUnmappedDataField<int>(FieldName + "1");
            Assert.IsNotNull(field);
            Assert.AreEqual(default(int), field);
        }

        [Test]
        public void TestGetAttribute()
        {
            const string prefix = "prefix";
            _fixture.Add($"{prefix}_Attribute_Name", JToken.FromObject("attr name"));
            _fixture.Add($"{prefix}_Attribute_ID", JToken.FromObject(1));
            _fixture.Add($"{prefix}_Attribute_Sort_Order", JToken.FromObject(2));

            _fixture.Add($"{prefix}_Attribute_Type_Name", JToken.FromObject("attr type name"));
            _fixture.Add($"{prefix}_Attribute_Type_ID", JToken.FromObject(3));

            var attr = _fixture.GetAttribute(prefix);
            attr.Should().NotBeNull();
            attr.Name.Should().Be("attr name");
            attr.Id.Should().Be(1);
            attr.SortOrder.Should().Be(2);
            attr.Type.Should().NotBeNull();
            attr.Type.Name.Should().Be("attr type name");
            attr.Type.Id.Should().Be(3);
        }
    }
}
