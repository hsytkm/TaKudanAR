﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaKudanAR.Interfaces
{
    public interface IKudanARService
    {
        //Task Init();
        Task StartMarkerARActivityAsync(IKudanImageSource marker, IKudanImageSource node);
        Task StartMarkerlessARFloorActivityAsync(IKudanImageSource node);
        Task StartMarkerlessARWallActivityAsync(IKudanImageSource node);
    }
}
