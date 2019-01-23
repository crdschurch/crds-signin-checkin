using MinistryPlatform.Translation.Models.DTO;
using SignInCheckIn.Models.DTO;
using System;
using System.Collections.Generic;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IChildSigninService
    {
        ParticipantEventMapDto GetChildrenAndEventByHouseholdId(int householdId, int eventId, int siteId, string kioskId);
        ParticipantEventMapDto GetChildrenAndEventByPhoneNumber(string phoneNumber, int siteId, EventDto existingEventDto, bool newFamilyRegistration = false, string kioskId = "");
        ParticipantEventMapDto SigninParticipants(ParticipantEventMapDto participantEventMapDto, bool allowLateSignIn = false);
        ParticipantEventMapDto PrintParticipant(int eventParticipantId, string kioskIdentifier);
        ParticipantEventMapDto PrintParticipants(ParticipantEventMapDto participantEventMapDto, string kioskIdentifier);
        List<MpEventDto> GetEventsForSignin(ParticipantEventMapDto participantEventMapDto, int kisokTypeId, bool allowLateSignin = false);
        MpNewParticipantDto CreateNewParticipantWithContact(string firstName, string lastName, DateTime dateOfBirth, int? gradeGroupId, int householdId, int householdPositionId, bool? isSpecialNeeds = false, int? genderId = 0);
        bool ReverseSignin(int eventParticipantId);
        List<MpGroupParticipantDto> CreateGroupParticipants(List<MpNewParticipantDto> mpParticipantDtos);
        MpGroupParticipantDto UpdateGradeGroupParticipant(int participantId, DateTime dob, int gradeAttributeId, bool removeExisting);
    }
}
