using Crossroads.Utilities.Services.Interfaces;
using Crossroads.Web.Common.Configuration;

namespace Crossroads.Utilities.Services
{
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        public int AgesAttributeTypeId { get; }
        public int GradesAttributeTypeId { get; }
        public int BirthMonthsAttributeTypeId { get; }
        public int NurseryAgesAttributeTypeId { get; }
        public int NurseryAgeAttributeId { get; }

        public int KidsClubGroupTypeId { get; }
        public int KidsClubMinistryId { get; }
        public int KidsClubCongregationId { get; }

        public int PersonalCharacteristicsAttributeTypeId { get; }
        public int SpecialNeedsAttributeId { get; }

        public int HeadOfHouseholdId { get; }
        public int OtherAdultId { get; }
        public int AdultChildId { get; }
        public int MinorChildId { get; }
        public string HouseHoldIdsThatCanCheckIn { get; }

        public int KidsClubRegistrationSourceId { get; }
        public int AttendeeParticipantType { get; }
        public int GroupRoleMemberId { get; }
        public int AdventureClubEventTypeId { get; }

        public int BumpingRoomTypePriorityId { get; }
        public int BumpingRoomTypeVacancyId { get; }

        public int SignedInParticipationStatusId { get; }
        public int CheckedInParticipationStatusId { get; }
        public int CancelledParticipationStatusId { get; }
        public int CapacityParticipationStatusId { get; }
        public int ErrorParticipationStatusId { get; }
        public string MachineConfigDetails { get; }

        public int GuestHouseholdId { get; set; }

        public int RoomUsageTypeKidsClub { get; set; }

        public string ManageRoomsChannel { get; set; }
        public string CheckinParticipantsChannel { get; set; }
        public string CheckinCapacityChannel { get; set; }

        public int ChildcareEventTypeId { get; }
        public int ChildcareGroupTypeId { get; }

        public int KcJan0To1 { get; }
        public int KcJan1To2 { get; }
        public int KcJan2To3 { get; }
        public int KcJan3To4 { get; }
        public int KcJan4To5 { get; }
        public int KcJan5To6 { get; }
        public int KcJan6To7 { get; }
        public int KcJan7To8 { get; }
        public int KcJan8To9 { get; }
        public int KcJan9To10 { get; }
        public int KcJan10To11 { get; }
        public int KcJan11To12 { get; }

        public int KcFeb0To1 { get; }
        public int KcFeb1To2 { get; }
        public int KcFeb2To3 { get; }
        public int KcFeb3To4 { get; }
        public int KcFeb4To5 { get; }
        public int KcFeb5To6 { get; }
        public int KcFeb6To7 { get; }
        public int KcFeb7To8 { get; }
        public int KcFeb8To9 { get; }
        public int KcFeb9To10 { get; }
        public int KcFeb10To11 { get; }
        public int KcFeb11To12 { get; }

        public int KcMar0To1 { get; }
        public int KcMar1To2 { get; }
        public int KcMar2To3 { get; }
        public int KcMar3To4 { get; }
        public int KcMar4To5 { get; }
        public int KcMar5To6 { get; }
        public int KcMar6To7 { get; }
        public int KcMar7To8 { get; }
        public int KcMar8To9 { get; }
        public int KcMar9To10 { get; }
        public int KcMar10To11 { get; }
        public int KcMar11To12 { get; }

        public int KcApr0To1 { get; }
        public int KcApr1To2 { get; }
        public int KcApr2To3 { get; }
        public int KcApr3To4 { get; }
        public int KcApr4To5 { get; }
        public int KcApr5To6 { get; }
        public int KcApr6To7 { get; }
        public int KcApr7To8 { get; }
        public int KcApr8To9 { get; }
        public int KcApr9To10 { get; }
        public int KcApr10To11 { get; }
        public int KcApr11To12 { get; }

        public int KcMay0To1 { get; }
        public int KcMay1To2 { get; }
        public int KcMay2To3 { get; }
        public int KcMay3To4 { get; }
        public int KcMay4To5 { get; }
        public int KcMay5To6 { get; }
        public int KcMay6To7 { get; }
        public int KcMay7To8 { get; }
        public int KcMay8To9 { get; }
        public int KcMay9To10 { get; }
        public int KcMay10To11 { get; }
        public int KcMay11To12 { get; }

        public int KcJun0To1 { get; }
        public int KcJun1To2 { get; }
        public int KcJun2To3 { get; }
        public int KcJun3To4 { get; }
        public int KcJun4To5 { get; }
        public int KcJun5To6 { get; }
        public int KcJun6To7 { get; }
        public int KcJun7To8 { get; }
        public int KcJun8To9 { get; }
        public int KcJun9To10 { get; }
        public int KcJun10To11 { get; }
        public int KcJun11To12 { get; }

        public int KcJul0To1 { get; }
        public int KcJul1To2 { get; }
        public int KcJul2To3 { get; }
        public int KcJul3To4 { get; }
        public int KcJul4To5 { get; }
        public int KcJul5To6 { get; }
        public int KcJul6To7 { get; }
        public int KcJul7To8 { get; }
        public int KcJul8To9 { get; }
        public int KcJul9To10 { get; }
        public int KcJul10To11 { get; }
        public int KcJul11To12 { get; }

        public int KcAug0To1 { get; }
        public int KcAug1To2 { get; }
        public int KcAug2To3 { get; }
        public int KcAug3To4 { get; }
        public int KcAug4To5 { get; }
        public int KcAug5To6 { get; }
        public int KcAug6To7 { get; }
        public int KcAug7To8 { get; }
        public int KcAug8To9 { get; }
        public int KcAug9To10 { get; }
        public int KcAug10To11 { get; }
        public int KcAug11To12 { get; }

        public int KcSep0To1 { get; }
        public int KcSep1To2 { get; }
        public int KcSep2To3 { get; }
        public int KcSep3To4 { get; }
        public int KcSep4To5 { get; }
        public int KcSep5To6 { get; }
        public int KcSep6To7 { get; }
        public int KcSep7To8 { get; }
        public int KcSep8To9 { get; }
        public int KcSep9To10 { get; }
        public int KcSep10To11 { get; }
        public int KcSep11To12 { get; }

        public int KcOct0To1 { get; }
        public int KcOct1To2 { get; }
        public int KcOct2To3 { get; }
        public int KcOct3To4 { get; }
        public int KcOct4To5 { get; }
        public int KcOct5To6 { get; }
        public int KcOct6To7 { get; }
        public int KcOct7To8 { get; }
        public int KcOct8To9 { get; }
        public int KcOct9To10 { get; }
        public int KcOct10To11 { get; }
        public int KcOct11To12 { get; }

        public int KcNov0To1 { get; }
        public int KcNov1To2 { get; }
        public int KcNov2To3 { get; }
        public int KcNov3To4 { get; }
        public int KcNov4To5 { get; }
        public int KcNov5To6 { get; }
        public int KcNov6To7 { get; }
        public int KcNov7To8 { get; }
        public int KcNov8To9 { get; }
        public int KcNov9To10 { get; }
        public int KcNov10To11 { get; }
        public int KcNov11To12 { get; }

        public int KcDec0To1 { get; }
        public int KcDec1To2 { get; }
        public int KcDec2To3 { get; }
        public int KcDec3To4 { get; }
        public int KcDec4To5 { get; }
        public int KcDec5To6 { get; }
        public int KcDec6To7 { get; }
        public int KcDec7To8 { get; }
        public int KcDec8To9 { get; }
        public int KcDec9To10 { get; }
        public int KcDec10To11 { get; }
        public int KcDec11To12 { get; }

        // one to five year old groups
        public int KcOneYearJan { get; }
        public int KcOneYearFeb { get; }
        public int KcOneYearMar { get; }
        public int KcOneYearApr { get; }
        public int KcOneYearMay { get; }
        public int KcOneYearJun { get; }
        public int KcOneYearJul { get; }
        public int KcOneYearAug { get; }
        public int KcOneYearSep { get; }
        public int KcOneYearOct { get; }
        public int KcOneYearNov { get; }
        public int KcOneYearDec { get; }

        public int KcTwoYearJan { get; }
        public int KcTwoYearFeb { get; }
        public int KcTwoYearMar { get; }
        public int KcTwoYearApr { get; }
        public int KcTwoYearMay { get; }
        public int KcTwoYearJun { get; }
        public int KcTwoYearJul { get; }
        public int KcTwoYearAug { get; }
        public int KcTwoYearSep { get; }
        public int KcTwoYearOct { get; }
        public int KcTwoYearNov { get; }
        public int KcTwoYearDec { get; }

        public int KcThreeYearJan { get; }
        public int KcThreeYearFeb { get; }
        public int KcThreeYearMar { get; }
        public int KcThreeYearApr { get; }
        public int KcThreeYearMay { get; }
        public int KcThreeYearJun { get; }
        public int KcThreeYearJul { get; }
        public int KcThreeYearAug { get; }
        public int KcThreeYearSep { get; }
        public int KcThreeYearOct { get; }
        public int KcThreeYearNov { get; }
        public int KcThreeYearDec { get; }

        public int KcFourYearJan { get; }
        public int KcFourYearFeb { get; }
        public int KcFourYearMar { get; }
        public int KcFourYearApr { get; }
        public int KcFourYearMay { get; }
        public int KcFourYearJun { get; }
        public int KcFourYearJul { get; }
        public int KcFourYearAug { get; }
        public int KcFourYearSep { get; }
        public int KcFourYearOct { get; }
        public int KcFourYearNov { get; }
        public int KcFourYearDec { get; }

        public int KcFiveYearJan { get; }
        public int KcFiveYearFeb { get; }
        public int KcFiveYearMar { get; }
        public int KcFiveYearApr { get; }
        public int KcFiveYearMay { get; }
        public int KcFiveYearJun { get; }
        public int KcFiveYearJul { get; }
        public int KcFiveYearAug { get; }
        public int KcFiveYearSep { get; }
        public int KcFiveYearOct { get; }
        public int KcFiveYearNov { get; }
        public int KcFiveYearDec { get; }

        // grade school groups
        public int KcKindergarten { get; }
        public int KcFirstGrade { get; }
        public int KcSecondGrade { get; }
        public int KcThirdGrade { get; }
        public int KcFourthGrade { get; }
        public int KcFifthGrade { get; }

        public ApplicationConfiguration(IConfigurationWrapper configurationWrapper)
        {
            AgesAttributeTypeId = configurationWrapper.GetConfigIntValue("KidsClubAgesAttributeTypeId");
            GradesAttributeTypeId = configurationWrapper.GetConfigIntValue("KidsClubGradesAttributeTypeId");
            BirthMonthsAttributeTypeId = configurationWrapper.GetConfigIntValue("KidsClubBirthMonthsAttributeTypeId");
            NurseryAgesAttributeTypeId = configurationWrapper.GetConfigIntValue("KidsClubNurseryAgesAttributeTypeId");
            NurseryAgeAttributeId = configurationWrapper.GetConfigIntValue("KidsClubNurseryAgeAttributeId");

            PersonalCharacteristicsAttributeTypeId = configurationWrapper.GetConfigIntValue("PersonalCharacteristicsAttributeTypeId");
            SpecialNeedsAttributeId = configurationWrapper.GetConfigIntValue("SpecialNeedsAttributeId");

            KidsClubGroupTypeId = configurationWrapper.GetConfigIntValue("KidsClubGroupTypeId");
            KidsClubMinistryId = configurationWrapper.GetConfigIntValue("KidsClubMinistryId");
            KidsClubCongregationId = configurationWrapper.GetConfigIntValue("KidsClubCongregationId");

            HeadOfHouseholdId = configurationWrapper.GetConfigIntValue("HeadOfHouseholdId");
            OtherAdultId = configurationWrapper.GetConfigIntValue("OtherAdultId");
            AdultChildId = configurationWrapper.GetConfigIntValue("AdultChildId");
            MinorChildId = configurationWrapper.GetConfigIntValue("MinorChildId");
            HouseHoldIdsThatCanCheckIn = $"{HeadOfHouseholdId},{OtherAdultId},{AdultChildId}";

            KidsClubRegistrationSourceId = configurationWrapper.GetConfigIntValue("KidsClubRegistrationSourceId");
            AttendeeParticipantType = configurationWrapper.GetConfigIntValue("AttendeeParticipantType");
            GroupRoleMemberId = configurationWrapper.GetConfigIntValue("GroupRoleMemberId");
            AdventureClubEventTypeId = configurationWrapper.GetConfigIntValue("AdventureClubEventTypeId");

            BumpingRoomTypePriorityId = configurationWrapper.GetConfigIntValue("BumpingRoomTypePriorityId");
            BumpingRoomTypeVacancyId = configurationWrapper.GetConfigIntValue("BumpingRoomTypeVacancyId");

            SignedInParticipationStatusId = configurationWrapper.GetConfigIntValue("SignedInParticipationStatusId");
            CheckedInParticipationStatusId = configurationWrapper.GetConfigIntValue("CheckedInParticipationStatusId");
            CancelledParticipationStatusId = configurationWrapper.GetConfigIntValue("CancelledParticipationStatusId");
            CapacityParticipationStatusId = configurationWrapper.GetConfigIntValue("CapacityParticipationStatusId");
            ErrorParticipationStatusId = configurationWrapper.GetConfigIntValue("ErrorParticipationStatusId");
            MachineConfigDetails = configurationWrapper.GetConfigValue("MachineConfigDetails");

            GuestHouseholdId = configurationWrapper.GetConfigIntValue("GuestHouseholdId");

            RoomUsageTypeKidsClub = configurationWrapper.GetConfigIntValue("RoomUsageTypeKidsClub");

            ManageRoomsChannel = configurationWrapper.GetConfigValue("ManageRoomsChannel");
            CheckinParticipantsChannel = configurationWrapper.GetConfigValue("CheckinParticipantsChannel");
            CheckinCapacityChannel = configurationWrapper.GetConfigValue("CheckinCapacityChannel");

            ChildcareEventTypeId = configurationWrapper.GetConfigIntValue("ChildcareEventTypeId");
            ChildcareGroupTypeId = configurationWrapper.GetConfigIntValue("ChildcareGroupTypeId");

            // kc groups - January Birth Months
            KcJan0To1 = configurationWrapper.GetConfigIntValue("Jan_0-1");
            KcJan1To2 = configurationWrapper.GetConfigIntValue("Jan_1-2");
            KcJan2To3 = configurationWrapper.GetConfigIntValue("Jan_2-3");
            KcJan3To4 = configurationWrapper.GetConfigIntValue("Jan_3-4");
            KcJan4To5 = configurationWrapper.GetConfigIntValue("Jan_4-5");
            KcJan5To6 = configurationWrapper.GetConfigIntValue("Jan_5-6");
            KcJan6To7 = configurationWrapper.GetConfigIntValue("Jan_6-7");
            KcJan7To8 = configurationWrapper.GetConfigIntValue("Jan_7-8");
            KcJan8To9 = configurationWrapper.GetConfigIntValue("Jan_8-9");
            KcJan9To10 = configurationWrapper.GetConfigIntValue("Jan_9-10");
            KcJan10To11 = configurationWrapper.GetConfigIntValue("Jan_10-11");
            KcJan11To12 = configurationWrapper.GetConfigIntValue("Jan_11-12");

            // kc groups - February Birth Months
            KcFeb0To1 = configurationWrapper.GetConfigIntValue("Feb_0-1");
            KcFeb1To2 = configurationWrapper.GetConfigIntValue("Feb_1-2");
            KcFeb2To3 = configurationWrapper.GetConfigIntValue("Feb_2-3");
            KcFeb3To4 = configurationWrapper.GetConfigIntValue("Feb_3-4");
            KcFeb4To5 = configurationWrapper.GetConfigIntValue("Feb_4-5");
            KcFeb5To6 = configurationWrapper.GetConfigIntValue("Feb_5-6");
            KcFeb6To7 = configurationWrapper.GetConfigIntValue("Feb_6-7");
            KcFeb7To8 = configurationWrapper.GetConfigIntValue("Feb_7-8");
            KcFeb8To9 = configurationWrapper.GetConfigIntValue("Feb_8-9");
            KcFeb9To10 = configurationWrapper.GetConfigIntValue("Feb_9-10");
            KcFeb10To11 = configurationWrapper.GetConfigIntValue("Feb_10-11");
            KcFeb11To12 = configurationWrapper.GetConfigIntValue("Feb_11-12");

            // kc groups - March Birth Months
            KcMar0To1 = configurationWrapper.GetConfigIntValue("Mar_0-1");
            KcMar1To2 = configurationWrapper.GetConfigIntValue("Mar_1-2");
            KcMar2To3 = configurationWrapper.GetConfigIntValue("Mar_2-3");
            KcMar3To4 = configurationWrapper.GetConfigIntValue("Mar_3-4");
            KcMar4To5 = configurationWrapper.GetConfigIntValue("Mar_4-5");
            KcMar5To6 = configurationWrapper.GetConfigIntValue("Mar_5-6");
            KcMar6To7 = configurationWrapper.GetConfigIntValue("Mar_6-7");
            KcMar7To8 = configurationWrapper.GetConfigIntValue("Mar_7-8");
            KcMar8To9 = configurationWrapper.GetConfigIntValue("Mar_8-9");
            KcMar9To10 = configurationWrapper.GetConfigIntValue("Mar_9-10");
            KcMar10To11 = configurationWrapper.GetConfigIntValue("Mar_10-11");
            KcMar11To12 = configurationWrapper.GetConfigIntValue("Mar_11-12");

            // kc groups - April Birth Months
            KcApr0To1 = configurationWrapper.GetConfigIntValue("Apr_0-1");
            KcApr1To2 = configurationWrapper.GetConfigIntValue("Apr_1-2");
            KcApr2To3 = configurationWrapper.GetConfigIntValue("Apr_2-3");
            KcApr3To4 = configurationWrapper.GetConfigIntValue("Apr_3-4");
            KcApr4To5 = configurationWrapper.GetConfigIntValue("Apr_4-5");
            KcApr5To6 = configurationWrapper.GetConfigIntValue("Apr_5-6");
            KcApr6To7 = configurationWrapper.GetConfigIntValue("Apr_6-7");
            KcApr7To8 = configurationWrapper.GetConfigIntValue("Apr_7-8");
            KcApr8To9 = configurationWrapper.GetConfigIntValue("Apr_8-9");
            KcApr9To10 = configurationWrapper.GetConfigIntValue("Apr_9-10");
            KcApr10To11 = configurationWrapper.GetConfigIntValue("Apr_10-11");
            KcApr11To12 = configurationWrapper.GetConfigIntValue("Apr_11-12");

            // kc groups - May Birth Months
            KcMay0To1 = configurationWrapper.GetConfigIntValue("May_0-1");
            KcMay1To2 = configurationWrapper.GetConfigIntValue("May_1-2");
            KcMay2To3 = configurationWrapper.GetConfigIntValue("May_2-3");
            KcMay3To4 = configurationWrapper.GetConfigIntValue("May_3-4");
            KcMay4To5 = configurationWrapper.GetConfigIntValue("May_4-5");
            KcMay5To6 = configurationWrapper.GetConfigIntValue("May_5-6");
            KcMay6To7 = configurationWrapper.GetConfigIntValue("May_6-7");
            KcMay7To8 = configurationWrapper.GetConfigIntValue("May_7-8");
            KcMay8To9 = configurationWrapper.GetConfigIntValue("May_8-9");
            KcMay9To10 = configurationWrapper.GetConfigIntValue("May_9-10");
            KcMay10To11 = configurationWrapper.GetConfigIntValue("May_10-11");
            KcMay11To12 = configurationWrapper.GetConfigIntValue("May_11-12");

            // kc groups - June Birth Months
            KcJun0To1 = configurationWrapper.GetConfigIntValue("Jun_0-1");
            KcJun1To2 = configurationWrapper.GetConfigIntValue("Jun_1-2");
            KcJun2To3 = configurationWrapper.GetConfigIntValue("Jun_2-3");
            KcJun3To4 = configurationWrapper.GetConfigIntValue("Jun_3-4");
            KcJun4To5 = configurationWrapper.GetConfigIntValue("Jun_4-5");
            KcJun5To6 = configurationWrapper.GetConfigIntValue("Jun_5-6");
            KcJun6To7 = configurationWrapper.GetConfigIntValue("Jun_6-7");
            KcJun7To8 = configurationWrapper.GetConfigIntValue("Jun_7-8");
            KcJun8To9 = configurationWrapper.GetConfigIntValue("Jun_8-9");
            KcJun9To10 = configurationWrapper.GetConfigIntValue("Jun_9-10");
            KcJun10To11 = configurationWrapper.GetConfigIntValue("Jun_10-11");
            KcJun11To12 = configurationWrapper.GetConfigIntValue("Jun_11-12");

            // kc groups - July Birth Months
            KcJul0To1 = configurationWrapper.GetConfigIntValue("Jul_0-1");
            KcJul1To2 = configurationWrapper.GetConfigIntValue("Jul_1-2");
            KcJul2To3 = configurationWrapper.GetConfigIntValue("Jul_2-3");
            KcJul3To4 = configurationWrapper.GetConfigIntValue("Jul_3-4");
            KcJul4To5 = configurationWrapper.GetConfigIntValue("Jul_4-5");
            KcJul5To6 = configurationWrapper.GetConfigIntValue("Jul_5-6");
            KcJul6To7 = configurationWrapper.GetConfigIntValue("Jul_6-7");
            KcJul7To8 = configurationWrapper.GetConfigIntValue("Jul_7-8");
            KcJul8To9 = configurationWrapper.GetConfigIntValue("Jul_8-9");
            KcJul9To10 = configurationWrapper.GetConfigIntValue("Jul_9-10");
            KcJul10To11 = configurationWrapper.GetConfigIntValue("Jul_10-11");
            KcJul11To12 = configurationWrapper.GetConfigIntValue("Jul_11-12");

            // kc groups - August Birth Months
            KcAug0To1 = configurationWrapper.GetConfigIntValue("Aug_0-1");
            KcAug1To2 = configurationWrapper.GetConfigIntValue("Aug_1-2");
            KcAug2To3 = configurationWrapper.GetConfigIntValue("Aug_2-3");
            KcAug3To4 = configurationWrapper.GetConfigIntValue("Aug_3-4");
            KcAug4To5 = configurationWrapper.GetConfigIntValue("Aug_4-5");
            KcAug5To6 = configurationWrapper.GetConfigIntValue("Aug_5-6");
            KcAug6To7 = configurationWrapper.GetConfigIntValue("Aug_6-7");
            KcAug7To8 = configurationWrapper.GetConfigIntValue("Aug_7-8");
            KcAug8To9 = configurationWrapper.GetConfigIntValue("Aug_8-9");
            KcAug9To10 = configurationWrapper.GetConfigIntValue("Aug_9-10");
            KcAug10To11 = configurationWrapper.GetConfigIntValue("Aug_10-11");
            KcAug11To12 = configurationWrapper.GetConfigIntValue("Aug_11-12");

            // kc groups - September Birth Months
            KcSep0To1 = configurationWrapper.GetConfigIntValue("Sep_0-1");
            KcSep1To2 = configurationWrapper.GetConfigIntValue("Sep_1-2");
            KcSep2To3 = configurationWrapper.GetConfigIntValue("Sep_2-3");
            KcSep3To4 = configurationWrapper.GetConfigIntValue("Sep_3-4");
            KcSep4To5 = configurationWrapper.GetConfigIntValue("Sep_4-5");
            KcSep5To6 = configurationWrapper.GetConfigIntValue("Sep_5-6");
            KcSep6To7 = configurationWrapper.GetConfigIntValue("Sep_6-7");
            KcSep7To8 = configurationWrapper.GetConfigIntValue("Sep_7-8");
            KcSep8To9 = configurationWrapper.GetConfigIntValue("Sep_8-9");
            KcSep9To10 = configurationWrapper.GetConfigIntValue("Sep_9-10");
            KcSep10To11 = configurationWrapper.GetConfigIntValue("Sep_10-11");
            KcSep11To12 = configurationWrapper.GetConfigIntValue("Sep_11-12");

            // kc groups - October Birth Months
            KcOct0To1 = configurationWrapper.GetConfigIntValue("Oct_0-1");
            KcOct1To2 = configurationWrapper.GetConfigIntValue("Oct_1-2");
            KcOct2To3 = configurationWrapper.GetConfigIntValue("Oct_2-3");
            KcOct3To4 = configurationWrapper.GetConfigIntValue("Oct_3-4");
            KcOct4To5 = configurationWrapper.GetConfigIntValue("Oct_4-5");
            KcOct5To6 = configurationWrapper.GetConfigIntValue("Oct_5-6");
            KcOct6To7 = configurationWrapper.GetConfigIntValue("Oct_6-7");
            KcOct7To8 = configurationWrapper.GetConfigIntValue("Oct_7-8");
            KcOct8To9 = configurationWrapper.GetConfigIntValue("Oct_8-9");
            KcOct9To10 = configurationWrapper.GetConfigIntValue("Oct_9-10");
            KcOct10To11 = configurationWrapper.GetConfigIntValue("Oct_10-11");
            KcOct11To12 = configurationWrapper.GetConfigIntValue("Oct_11-12");

            // kc groups - November Birth Months
            KcNov0To1 = configurationWrapper.GetConfigIntValue("Nov_0-1");
            KcNov1To2 = configurationWrapper.GetConfigIntValue("Nov_1-2");
            KcNov2To3 = configurationWrapper.GetConfigIntValue("Nov_2-3");
            KcNov3To4 = configurationWrapper.GetConfigIntValue("Nov_3-4");
            KcNov4To5 = configurationWrapper.GetConfigIntValue("Nov_4-5");
            KcNov5To6 = configurationWrapper.GetConfigIntValue("Nov_5-6");
            KcNov6To7 = configurationWrapper.GetConfigIntValue("Nov_6-7");
            KcNov7To8 = configurationWrapper.GetConfigIntValue("Nov_7-8");
            KcNov8To9 = configurationWrapper.GetConfigIntValue("Nov_8-9");
            KcNov9To10 = configurationWrapper.GetConfigIntValue("Nov_9-10");
            KcNov10To11 = configurationWrapper.GetConfigIntValue("Nov_10-11");
            KcNov11To12 = configurationWrapper.GetConfigIntValue("Nov_11-12");

            // kc groups - December Birth Months
            KcDec0To1 = configurationWrapper.GetConfigIntValue("Dec_0-1");
            KcDec1To2 = configurationWrapper.GetConfigIntValue("Dec_1-2");
            KcDec2To3 = configurationWrapper.GetConfigIntValue("Dec_2-3");
            KcDec3To4 = configurationWrapper.GetConfigIntValue("Dec_3-4");
            KcDec4To5 = configurationWrapper.GetConfigIntValue("Dec_4-5");
            KcDec5To6 = configurationWrapper.GetConfigIntValue("Dec_5-6");
            KcDec6To7 = configurationWrapper.GetConfigIntValue("Dec_6-7");
            KcDec7To8 = configurationWrapper.GetConfigIntValue("Dec_7-8");
            KcDec8To9 = configurationWrapper.GetConfigIntValue("Dec_8-9");
            KcDec9To10 = configurationWrapper.GetConfigIntValue("Dec_9-10");
            KcDec10To11 = configurationWrapper.GetConfigIntValue("Dec_10-11");
            KcDec11To12 = configurationWrapper.GetConfigIntValue("Dec_11-12");

            // one year olds
            KcOneYearJan = configurationWrapper.GetConfigIntValue("Jan_1yr");
            KcOneYearFeb = configurationWrapper.GetConfigIntValue("Feb_1yr");
            KcOneYearMar = configurationWrapper.GetConfigIntValue("Mar_1yr");
            KcOneYearApr = configurationWrapper.GetConfigIntValue("Apr_1yr");
            KcOneYearMay = configurationWrapper.GetConfigIntValue("May_1yr");
            KcOneYearJun = configurationWrapper.GetConfigIntValue("Jun_1yr");
            KcOneYearJul = configurationWrapper.GetConfigIntValue("Jul_1yr");
            KcOneYearAug = configurationWrapper.GetConfigIntValue("Aug_1yr");
            KcOneYearSep = configurationWrapper.GetConfigIntValue("Sep_1yr");
            KcOneYearOct = configurationWrapper.GetConfigIntValue("Oct_1yr");
            KcOneYearNov = configurationWrapper.GetConfigIntValue("Nov_1yr");
            KcOneYearDec = configurationWrapper.GetConfigIntValue("Dec_1yr");

            // two year olds
            KcTwoYearJan = configurationWrapper.GetConfigIntValue("Jan_2yr");
            KcTwoYearFeb = configurationWrapper.GetConfigIntValue("Feb_2yr");
            KcTwoYearMar = configurationWrapper.GetConfigIntValue("Mar_2yr");
            KcTwoYearApr = configurationWrapper.GetConfigIntValue("Apr_2yr");
            KcTwoYearMay = configurationWrapper.GetConfigIntValue("May_2yr");
            KcTwoYearJun = configurationWrapper.GetConfigIntValue("Jun_2yr");
            KcTwoYearJul = configurationWrapper.GetConfigIntValue("Jul_2yr");
            KcTwoYearAug = configurationWrapper.GetConfigIntValue("Aug_2yr");
            KcTwoYearSep = configurationWrapper.GetConfigIntValue("Sep_2yr");
            KcTwoYearOct = configurationWrapper.GetConfigIntValue("Oct_2yr");
            KcTwoYearNov = configurationWrapper.GetConfigIntValue("Nov_2yr");
            KcTwoYearDec = configurationWrapper.GetConfigIntValue("Dec_2yr");

            // three year olds
            KcThreeYearJan = configurationWrapper.GetConfigIntValue("Jan_3yr");
            KcThreeYearFeb = configurationWrapper.GetConfigIntValue("Feb_3yr");
            KcThreeYearMar = configurationWrapper.GetConfigIntValue("Mar_3yr");
            KcThreeYearApr = configurationWrapper.GetConfigIntValue("Apr_3yr");
            KcThreeYearMay = configurationWrapper.GetConfigIntValue("May_3yr");
            KcThreeYearJun = configurationWrapper.GetConfigIntValue("Jun_3yr");
            KcThreeYearJul = configurationWrapper.GetConfigIntValue("Jul_3yr");
            KcThreeYearAug = configurationWrapper.GetConfigIntValue("Aug_3yr");
            KcThreeYearSep = configurationWrapper.GetConfigIntValue("Sep_3yr");
            KcThreeYearOct = configurationWrapper.GetConfigIntValue("Oct_3yr");
            KcThreeYearNov = configurationWrapper.GetConfigIntValue("Nov_3yr");
            KcThreeYearDec = configurationWrapper.GetConfigIntValue("Dec_3yr");

            // four year olds
            KcFourYearJan = configurationWrapper.GetConfigIntValue("Jan_4yr");
            KcFourYearFeb = configurationWrapper.GetConfigIntValue("Feb_4yr");
            KcFourYearMar = configurationWrapper.GetConfigIntValue("Mar_4yr");
            KcFourYearApr = configurationWrapper.GetConfigIntValue("Apr_4yr");
            KcFourYearMay = configurationWrapper.GetConfigIntValue("May_4yr");
            KcFourYearJun = configurationWrapper.GetConfigIntValue("Jun_4yr");
            KcFourYearJul = configurationWrapper.GetConfigIntValue("Jul_4yr");
            KcFourYearAug = configurationWrapper.GetConfigIntValue("Aug_4yr");
            KcFourYearSep = configurationWrapper.GetConfigIntValue("Sep_4yr");
            KcFourYearOct = configurationWrapper.GetConfigIntValue("Oct_4yr");
            KcFourYearNov = configurationWrapper.GetConfigIntValue("Nov_4yr");
            KcFourYearDec = configurationWrapper.GetConfigIntValue("Dec_4yr");

            // five year olds
            KcFiveYearJan = configurationWrapper.GetConfigIntValue("Jan_5yr");
            KcFiveYearFeb = configurationWrapper.GetConfigIntValue("Feb_5yr");
            KcFiveYearMar = configurationWrapper.GetConfigIntValue("Mar_5yr");
            KcFiveYearApr = configurationWrapper.GetConfigIntValue("Apr_5yr");
            KcFiveYearMay = configurationWrapper.GetConfigIntValue("May_5yr");
            KcFiveYearJun = configurationWrapper.GetConfigIntValue("Jun_5yr");
            KcFiveYearJul = configurationWrapper.GetConfigIntValue("Jul_5yr");
            KcFiveYearAug = configurationWrapper.GetConfigIntValue("Aug_5yr");
            KcFiveYearSep = configurationWrapper.GetConfigIntValue("Sep_5yr");
            KcFiveYearOct = configurationWrapper.GetConfigIntValue("Oct_5yr");
            KcFiveYearNov = configurationWrapper.GetConfigIntValue("Nov_5yr");
            KcFiveYearDec = configurationWrapper.GetConfigIntValue("Dec_5yr");

            // grade groups
            KcKindergarten = configurationWrapper.GetConfigIntValue("KcKind");
            KcFirstGrade = configurationWrapper.GetConfigIntValue("KcFirst");
            KcSecondGrade = configurationWrapper.GetConfigIntValue("KcSecond");
            KcThirdGrade = configurationWrapper.GetConfigIntValue("KcThird");
            KcFourthGrade = configurationWrapper.GetConfigIntValue("KcFourth");
            KcFifthGrade = configurationWrapper.GetConfigIntValue("KcFifth");
        }
    }
}
