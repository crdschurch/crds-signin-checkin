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
    }
}
