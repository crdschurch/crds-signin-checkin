using Crossroads.Utilities.Services.Interfaces;

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
        public int KidsClubCongretationId { get; }

        public int HeadOfHouseHoldId { get; }
        public int OtherAdultId { get; }
        public int AdultChildId { get; }
        public int MinorChildId { get; }
        public string HouseHoldIdsThatCanCheckIn { get; }

        public ApplicationConfiguration(IConfigurationWrapper configurationWrapper)
        {
            AgesAttributeTypeId = configurationWrapper.GetConfigIntValue("KidsClubAgesAttributeTypeId");
            GradesAttributeTypeId = configurationWrapper.GetConfigIntValue("KidsClubGradesAttributeTypeId");
            BirthMonthsAttributeTypeId = configurationWrapper.GetConfigIntValue("KidsClubBirthMonthsAttributeTypeId");
            NurseryAgesAttributeTypeId = configurationWrapper.GetConfigIntValue("KidsClubNurseryAgesAttributeTypeId");
            NurseryAgeAttributeId = configurationWrapper.GetConfigIntValue("KidsClubNurseryAgeAttributeId");


            KidsClubGroupTypeId = configurationWrapper.GetConfigIntValue("KidsClubGroupTypeId");
            KidsClubMinistryId = configurationWrapper.GetConfigIntValue("KidsClubMinistryId");
            KidsClubCongretationId = configurationWrapper.GetConfigIntValue("KidsClubCongretationId");

            HeadOfHouseHoldId = configurationWrapper.GetConfigIntValue("HeadOfHouseHoldId");
            OtherAdultId = configurationWrapper.GetConfigIntValue("OtherAdultId");
            AdultChildId = configurationWrapper.GetConfigIntValue("AdultChildId");
            MinorChildId = configurationWrapper.GetConfigIntValue("MinorChildId");
            HouseHoldIdsThatCanCheckIn = $"{HeadOfHouseHoldId},{OtherAdultId},{AdultChildId}";
        }
    }
}
