using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Translation.Models.DTO;

namespace MinistryPlatform.Translation.Extensions
{
    public static class MpEventGroupDtoExtensions
    {
        public static bool HasMatchingAgeGroupBirthMonth(this List<MpEventGroupDto> eventGroups, int birthMonthId)
        {
            return HasMatchingBirthMonth(eventGroups, birthMonthId, false);
        }
        public static bool HasMatchingNurseryBirthMonth(this List<MpEventGroupDto> eventGroups, int birthMonthId)
        {
            return HasMatchingBirthMonth(eventGroups, birthMonthId, true);
        }

        private static bool HasMatchingBirthMonth(List<MpEventGroupDto> eventGroups, int birthMonthId, bool nursery)
        {
            return
                eventGroups.Exists(
                    e => ((nursery && e.Group.HasNurseryMonth()) || (!nursery && !e.Group.HasNurseryMonth())) && e.Group.HasBirthMonth() && e.Group.BirthMonth.Id == birthMonthId);
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
