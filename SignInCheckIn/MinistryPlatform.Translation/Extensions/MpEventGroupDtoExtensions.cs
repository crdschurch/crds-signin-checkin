using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;

namespace MinistryPlatform.Translation.Extensions
{
    public static class MpEventGroupDtoExtensions
    {
        #region Birth Month Matching
        public static bool HasMatchingBirthMonth(this List<MpEventGroupDto> eventGroups, int ageGroupId, int birthMonthId)
        {
            return eventGroups.Exists(e => MatchesBirthMonth(e, ageGroupId, birthMonthId));
        }
        public static List<MpEventGroupDto> GetMatchingBirthMonths(this List<MpEventGroupDto> eventGroups, int ageGroupId, int birthMonthId)
        {
            return eventGroups.FindAll(e => MatchesBirthMonth(e, ageGroupId, birthMonthId));
        }

        private static bool MatchesBirthMonth(MpEventGroupDto e, int ageGroupId, int birthMonthId)
        {
            return e.Group != null && e.Group.HasAgeRange() && e.Group.AgeRange.Id == ageGroupId && e.Group.HasBirthMonth() && e.Group.BirthMonth.Id == birthMonthId;
        }
        #endregion

        #region Nursery Month Matching
        public static bool HasMatchingNurseryMonth(this List<MpEventGroupDto> eventGroups, int nurseryMonthId)
        {
            return eventGroups.Exists(e => MatchesNurseryMonth(e, nurseryMonthId));
        }

        public static List<MpEventGroupDto> GetMatchingNurseryMonths(this List<MpEventGroupDto> eventGroups, int nurseryMonthId)
        {
            return eventGroups.FindAll(e => MatchesNurseryMonth(e, nurseryMonthId));
        }

        private static bool MatchesNurseryMonth(MpEventGroupDto e, int nurseryMonthId)
        {
            return e.Group != null && e.Group.HasNurseryMonth() && e.Group.NurseryMonth.Id == nurseryMonthId;
        }
        #endregion

        #region Grade Group Matching
        public static bool HasMatchingGradeGroup(this List<MpEventGroupDto> eventGroups, int gradeId)
        {
            return eventGroups.Exists(e => MatchesGradeGroup(e, gradeId));
        }

        public static List<MpEventGroupDto> GetMatchingGradeGroups(this List<MpEventGroupDto> eventGroups, int gradeId)
        {
            return eventGroups.FindAll(e => MatchesGradeGroup(e, gradeId));
        }

        private static bool MatchesGradeGroup(MpEventGroupDto e, int gradeId)
        {
            return e.Group != null && e.Group.HasGrade() && e.Group.Grade.Id == gradeId;
        }
        #endregion
    }
}
