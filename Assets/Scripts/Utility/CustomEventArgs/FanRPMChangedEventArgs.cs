using System;
using UnityEngine;

namespace System
{
    public class FanRPMChangedEventArgs : EventArgs
    {
        public int FanRPM;
        public int MaxFanRPM;
    }
}
