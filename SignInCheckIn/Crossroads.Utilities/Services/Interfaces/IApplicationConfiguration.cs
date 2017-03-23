using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossroads.Utilities.Services.Interfaces
{
    public interface IApplicationConfiguration
    {
        int AgesAttributeTypeId { get; }
        int GradesAttributeTypeId { get; }
        int BirthMonthsAttributeTypeId { get; }
        int NurseryAgesAttributeTypeId { get; }
        int NurseryAgeAttributeId { get; }

        int KidsClubGroupTypeId { get; }
        int KidsClubMinistryId { get; }
        int KidsClubCongregationId { get; }

        int HeadOfHouseholdId { get; }
        int OtherAdultId { get; }
        int AdultChildId { get; }
        int MinorChildId { get; }
        string HouseHoldIdsThatCanCheckIn { get; }

        int KidsClubRegistrationSourceId { get; }
        int AttendeeParticipantType { get; }
        int GroupRoleMemberId { get; }
        int AdventureClubEventTypeId { get; }

        int BumpingRoomTypePriorityId { get; }
        int BumpingRoomTypeVacancyId { get; }

        int SignedInParticipationStatusId { get; }
        int CheckedInParticipationStatusId { get; }
        int CancelledParticipationStatusId { get; }
        int CapacityParticipationStatusId { get; }
        int ErrorParticipationStatusId { get; }
        string MachineConfigDetails { get; }

        int GuestHouseholdId { get; set; }

        int RoomUsageTypeKidsClub { get; set; }

        string ManageRoomsChannel { get; set; }
        string CheckinParticipantsChannel { get; set; }
        string CheckinCapacityChannel { get; set; }

        // groups for new families
        int KcJan0To1 { get; }
        int KcJan1To2 { get; }
        int KcJan2To3 { get; }
        int KcJan3To4 { get; }
        int KcJan4To5 { get; }
        int KcJan5To6 { get; }
        int KcJan6To7 { get; }
        int KcJan7To8 { get; }
        int KcJan8To9 { get; }
        int KcJan9To10 { get; }
        int KcJan10To11 { get; }
        int KcJan11To12 { get; }

        int KcFeb0To1 { get; }
        int KcFeb1To2 { get; }
        int KcFeb2To3 { get; }
        int KcFeb3To4 { get; }
        int KcFeb4To5 { get; }
        int KcFeb5To6 { get; }
        int KcFeb6To7 { get; }
        int KcFeb7To8 { get; }
        int KcFeb8To9 { get; }
        int KcFeb9To10 { get; }
        int KcFeb10To11 { get; }
        int KcFeb11To12 { get; }

        int KcMar0To1 { get; }
        int KcMar1To2 { get; }
        int KcMar2To3 { get; }
        int KcMar3To4 { get; }
        int KcMar4To5 { get; }
        int KcMar5To6 { get; }
        int KcMar6To7 { get; }
        int KcMar7To8 { get; }
        int KcMar8To9 { get; }
        int KcMar9To10 { get; }
        int KcMar10To11 { get; }
        int KcMar11To12 { get; }

        int KcApr0To1 { get; }
        int KcApr1To2 { get; }
        int KcApr2To3 { get; }
        int KcApr3To4 { get; }
        int KcApr4To5 { get; }
        int KcApr5To6 { get; }
        int KcApr6To7 { get; }
        int KcApr7To8 { get; }
        int KcApr8To9 { get; }
        int KcApr9To10 { get; }
        int KcApr10To11 { get; }
        int KcApr11To12 { get; }

        int KcMay0To1 { get; }
        int KcMay1To2 { get; }
        int KcMay2To3 { get; }
        int KcMay3To4 { get; }
        int KcMay4To5 { get; }
        int KcMay5To6 { get; }
        int KcMay6To7 { get; }
        int KcMay7To8 { get; }
        int KcMay8To9 { get; }
        int KcMay9To10 { get; }
        int KcMay10To11 { get; }
        int KcMay11To12 { get; }

        int KcJun0To1 { get; }
        int KcJun1To2 { get; }
        int KcJun2To3 { get; }
        int KcJun3To4 { get; }
        int KcJun4To5 { get; }
        int KcJun5To6 { get; }
        int KcJun6To7 { get; }
        int KcJun7To8 { get; }
        int KcJun8To9 { get; }
        int KcJun9To10 { get; }
        int KcJun10To11 { get; }
        int KcJun11To12 { get; }

        int KcJul0To1 { get; }
        int KcJul1To2 { get; }
        int KcJul2To3 { get; }
        int KcJul3To4 { get; }
        int KcJul4To5 { get; }
        int KcJul5To6 { get; }
        int KcJul6To7 { get; }
        int KcJul7To8 { get; }
        int KcJul8To9 { get; }
        int KcJul9To10 { get; }
        int KcJul10To11 { get; }
        int KcJul11To12 { get; }

        int KcAug0To1 { get; }
        int KcAug1To2 { get; }
        int KcAug2To3 { get; }
        int KcAug3To4 { get; }
        int KcAug4To5 { get; }
        int KcAug5To6 { get; }
        int KcAug6To7 { get; }
        int KcAug7To8 { get; }
        int KcAug8To9 { get; }
        int KcAug9To10 { get; }
        int KcAug10To11 { get; }
        int KcAug11To12 { get; }

        int KcSep0To1 { get; }
        int KcSep1To2 { get; }
        int KcSep2To3 { get; }
        int KcSep3To4 { get; }
        int KcSep4To5 { get; }
        int KcSep5To6 { get; }
        int KcSep6To7 { get; }
        int KcSep7To8 { get; }
        int KcSep8To9 { get; }
        int KcSep9To10 { get; }
        int KcSep10To11 { get; }
        int KcSep11To12 { get; }

        int KcOct0To1 { get; }
        int KcOct1To2 { get; }
        int KcOct2To3 { get; }
        int KcOct3To4 { get; }
        int KcOct4To5 { get; }
        int KcOct5To6 { get; }
        int KcOct6To7 { get; }
        int KcOct7To8 { get; }
        int KcOct8To9 { get; }
        int KcOct9To10 { get; }
        int KcOct10To11 { get; }
        int KcOct11To12 { get; }

        int KcNov0To1 { get; }
        int KcNov1To2 { get; }
        int KcNov2To3 { get; }
        int KcNov3To4 { get; }
        int KcNov4To5 { get; }
        int KcNov5To6 { get; }
        int KcNov6To7 { get; }
        int KcNov7To8 { get; }
        int KcNov8To9 { get; }
        int KcNov9To10 { get; }
        int KcNov10To11 { get; }
        int KcNov11To12 { get; }

        int KcDec0To1 { get; }
        int KcDec1To2 { get; }
        int KcDec2To3 { get; }
        int KcDec3To4 { get; }
        int KcDec4To5 { get; }
        int KcDec5To6 { get; }
        int KcDec6To7 { get; }
        int KcDec7To8 { get; }
        int KcDec8To9 { get; }
        int KcDec9To10 { get; }
        int KcDec10To11 { get; }
        int KcDec11To12 { get; }

        // one to five year old groups
        int KcOneYearJan { get; }
        int KcOneYearFeb { get; }
        int KcOneYearMar { get; }
        int KcOneYearApr { get; }
        int KcOneYearMay { get; }
        int KcOneYearJun { get; }
        int KcOneYearJul { get; }
        int KcOneYearAug { get; }
        int KcOneYearSep { get; }
        int KcOneYearOct { get; }
        int KcOneYearNov { get; }
        int KcOneYearDec { get; }

        int KcTwoYearJan { get; }
        int KcTwoYearFeb { get; }
        int KcTwoYearMar { get; }
        int KcTwoYearApr { get; }
        int KcTwoYearMay { get; }
        int KcTwoYearJun { get; }
        int KcTwoYearJul { get; }
        int KcTwoYearAug { get; }
        int KcTwoYearSep { get; }
        int KcTwoYearOct { get; }
        int KcTwoYearNov { get; }
        int KcTwoYearDec { get; }

        int KcThreeYearJan { get; }
        int KcThreeYearFeb { get; }
        int KcThreeYearMar { get; }
        int KcThreeYearApr { get; }
        int KcThreeYearMay { get; }
        int KcThreeYearJun { get; }
        int KcThreeYearJul { get; }
        int KcThreeYearAug { get; }
        int KcThreeYearSep { get; }
        int KcThreeYearOct { get; }
        int KcThreeYearNov { get; }
        int KcThreeYearDec { get; }

        int KcFourYearJan { get; }
        int KcFourYearFeb { get; }
        int KcFourYearMar { get; }
        int KcFourYearApr { get; }
        int KcFourYearMay { get; }
        int KcFourYearJun { get; }
        int KcFourYearJul { get; }
        int KcFourYearAug { get; }
        int KcFourYearSep { get; }
        int KcFourYearOct { get; }
        int KcFourYearNov { get; }
        int KcFourYearDec { get; }

        int KcFiveYearJan { get; }
        int KcFiveYearFeb { get; }
        int KcFiveYearMar { get; }
        int KcFiveYearApr { get; }
        int KcFiveYearMay { get; }
        int KcFiveYearJun { get; }
        int KcFiveYearJul { get; }
        int KcFiveYearAug { get; }
        int KcFiveYearSep { get; }
        int KcFiveYearOct { get; }
        int KcFiveYearNov { get; }
        int KcFiveYearDec { get; }

        // grade school groups
        int KcKindergarten { get; }
        int KcFirstGrade { get; }
        int KcSecondGrade { get; }
        int KcThirdGrade { get; }
        int KcFourthGrade { get; }
        int KcFifthGrade { get; }
    }
}
