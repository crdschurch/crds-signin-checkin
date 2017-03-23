using System.Collections.Generic;
using System.Linq;
using MinistryPlatform.Translation.Extensions;
using MinistryPlatform.Translation.Models.DTO;
using NUnit.Framework;

namespace MinistryPlatform.Translation.Test.Extensions
{
    public class MpEventGroupDtoExtensionsTest
    {
        private List<MpEventGroupDto> _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new List<MpEventGroupDto>();
        }

        [Test]
        public void TestHasMatchingBirthMonth()
        {
            _fixture.Add(new MpEventGroupDto
            {
                Group = new MpGroupDto
                {
                    AgeRange = new MpAttributeDto
                    {
                        Id = 456
                    },
                    BirthMonth = new MpAttributeDto
                    {
                        Id = 123
                    }
                }
            });

            Assert.IsTrue(_fixture.HasMatchingBirthMonth(456, 123));
            Assert.IsFalse(_fixture.HasMatchingBirthMonth(456, 789));
            Assert.IsFalse(_fixture.HasMatchingBirthMonth(789, 123));
        }

        [Test]
        public void TestGetMatchingBirthMonths()
        {
            _fixture.Add(new MpEventGroupDto
            {
                Group = new MpGroupDto
                {
                    AgeRange = new MpAttributeDto
                    {
                        Id = 456
                    },
                    BirthMonth = new MpAttributeDto
                    {
                        Id = 123
                    }
                }
            });

            Assert.AreEqual(1, _fixture.GetMatchingBirthMonths(456, 123).Count);
            Assert.IsFalse(_fixture.GetMatchingBirthMonths(456, 789).Any());
            Assert.IsFalse(_fixture.GetMatchingBirthMonths(789, 123).Any());
        }

        [Test]
        public void TestHasMatchingNurseryMonth()
        {
            _fixture.Add(new MpEventGroupDto
            {
                Group = new MpGroupDto
                {
                    NurseryMonth = new MpAttributeDto
                    {
                        Id = 123
                    }
                }
            });

            Assert.IsTrue(_fixture.HasMatchingNurseryMonth(123));
            Assert.IsFalse(_fixture.HasMatchingNurseryMonth(456));
        }

        [Test]
        public void TestGetMatchingNurseryMonths()
        {
            _fixture.Add(new MpEventGroupDto
            {
                Group = new MpGroupDto
                {
                    NurseryMonth = new MpAttributeDto
                    {
                        Id = 123
                    }
                }
            });

            Assert.AreEqual(1, _fixture.GetMatchingNurseryMonths(123).Count);
            Assert.IsFalse(_fixture.GetMatchingNurseryMonths(456).Any());
        }

        [Test]
        public void TestHasMatchingGradeGroup()
        {
            _fixture.Add(new MpEventGroupDto
            {
                Group = new MpGroupDto
                {
                    Grade = new MpAttributeDto
                    {
                        Id = 123
                    }
                }
            });

            Assert.IsTrue(_fixture.HasMatchingGradeGroup(123));
            Assert.IsFalse(_fixture.HasMatchingGradeGroup(456));
        }

        [Test]
        public void TestGetMatchingGradeGroups()
        {
            _fixture.Add(new MpEventGroupDto
            {
                Group = new MpGroupDto
                {
                    Grade = new MpAttributeDto
                    {
                        Id = 123
                    }
                }
            });

            Assert.AreEqual(1, _fixture.GetMatchingGradeGroups(123).Count);
            Assert.IsFalse(_fixture.GetMatchingGradeGroups(456).Any());
        }
    }
}
