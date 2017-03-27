﻿using System;
using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IChildSigninRepository
    {
        MpHouseholdParticipantsDto GetChildrenByPhoneNumber(string phoneNumber, bool includeOtherHousehold = true, int? groupTypeId = null);

        [Obsolete("This should not be used, and should eventually be removed.  It has been replaced by GetChildrenByPhoneNumber.")]
        List<MpParticipantDto> GetChildrenByHouseholdId(int? householdId, MpEventDto eventDto);

        List<MpEventParticipantDto> CreateEventParticipants(List<MpEventParticipantDto> mpEventParticipantDtos);

        [Obsolete("This should not be used, and should eventually be removed.  It was only needed when calling GetChildrenByHouseholdId, which is Obsolete.")]
        int? GetHouseholdIdByPhoneNumber(string phoneNumber);
    }
}
