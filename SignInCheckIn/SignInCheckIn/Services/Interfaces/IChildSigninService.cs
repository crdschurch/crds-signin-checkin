using System;
using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IChildSigninService
    {
        ParticipantEventMapDto GetChildrenAndEventByHouseholdId(int householdId, int siteId, string kioskId);
        ParticipantEventMapDto GetChildrenAndEventByPhoneNumber(string phoneNumber, int siteId, EventDto existingEventDto, bool newFamilyRegistration = false, string kioskId = "");
        ParticipantEventMapDto SigninParticipants(ParticipantEventMapDto participantEventMapDto);
        ParticipantEventMapDto PrintParticipant(int eventParticipantId, string kioskIdentifier, string token);
        ParticipantEventMapDto PrintParticipants(ParticipantEventMapDto participantEventMapDto, string kioskIdentifier);
        ParticipantEventMapDto CreateNewFamily(string token, NewFamilyDto newFamilyDto, string kioskIdentifier);
        List<MpNewParticipantDto> SaveNewFamilyData(string token, NewFamilyDto newFamilyDto);
        List<MpEventDto> GetEventsForSignin(ParticipantEventMapDto participantEventMapDto);
        MpNewParticipantDto CreateNewParticipantWithContact(string firstName, string lastName, DateTime dateOfBirth, int? gradeGroupId, int householdId, int householdPositionId, bool? isSpecialNeeds, int? genderId = 0);
        bool ReverseSignin(string token, int eventParticipantId);
        List<MpGroupParticipantDto> CreateGroupParticipants(string token, List<MpNewParticipantDto> mpParticipantDtos);
        MpGroupParticipantDto UpdateGradeGroupParticipant(string token, int participantId, DateTime dob, int gradeAttributeId, bool removeExisting);
    }
}
