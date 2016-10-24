using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;

namespace MinistryPlatform.Translation.Extensions
{
    public static class MpEventGroupDtoExtensions
    {
        public static bool HasMatchingBirthMonth(this List<MpEventGroupDto> eventGroups, int ageGroupId, int birthMonthId)
        {
            return
                eventGroups.Exists(
                    e => e.Group != null && e.Group.HasAgeRange() && e.Group.AgeRange.Id == ageGroupId && e.Group.HasBirthMonth() && e.Group.BirthMonth.Id == birthMonthId);
        }

        public static bool HasMatchingNurseryMonth(this List<MpEventGroupDto> eventGroups, int nurseryMonthId)
        {
            return eventGroups.Exists(e => e.Group != null && e.Group.HasNurseryMonth() && e.Group.NurseryMonth.Id == nurseryMonthId);
        }

        public static bool HasMatchingGradeGroup(this List<MpEventGroupDto> eventGroups, int gradeId)
        {
            return eventGroups.Exists(e => e.Group != null && e.Group.HasGrade() && e.Group.Grade.Id == gradeId);
        }
    }
}
