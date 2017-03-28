using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.HA
{
    public interface IHAComponent
    {
        bool IsRunning { get; }
        void Start(object sender, EventArgs e);
        void Pause(object sender, EventArgs e);
        void Resume(object sender, EventArgs e);
        void Stop(object sender, EventArgs e);
    }
}