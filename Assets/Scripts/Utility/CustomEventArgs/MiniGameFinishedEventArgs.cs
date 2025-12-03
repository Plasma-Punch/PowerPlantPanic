using System;
using UnityEngine;

namespace System
{
    public class MiniGameFinishedEventArgs : EventArgs
    {
        public MiniGame FinishedMiniGame;
    }

    public enum MiniGame
    {
        PowerRegulating,
        FanBlock,
        PipeBroke,
        WasteManagement
    }
}

