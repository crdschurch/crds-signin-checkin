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
        int KidsClubCongretationId { get; }

        int HeadOfHouseHoldId { get; }
        int OtherAdultId { get; }
        int AdultChildId { get; }
        int MinorChildId { get; }
        string HouseHoldIdsThatCanCheckIn { get; }
    }
}
