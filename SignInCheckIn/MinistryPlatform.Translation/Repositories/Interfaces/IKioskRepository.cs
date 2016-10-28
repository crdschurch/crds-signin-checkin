﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Translation.Models.DTO;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IKioskRepository
    {
        MpKioskConfigDto GetMpKioskConfigByIdentifier(Guid kioskConfigId);
    }
}
