using System;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IGroupLookupRepository
    {
        int GetGroupId(DateTime birthDate, int? gradeGroupAttributeId);
        int GetGradeAttributeId(int gradeGroupId);
    }
}
