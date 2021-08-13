using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaKudanAR.Interfaces
{
    public interface IKudanARService
    {
        //Task Init();
        Task StartMarkerARActivityAsync(IKudanImageSource marker, IKudanImageSource node);
        //Task StartMarkerlessARActivityAsync(IKudanImageSource marker, IKudanImageSource node);
        //Task StartMarkerlessWallActivityAsync(IKudanImageSource marker, IKudanImageSource node);
    }
}
