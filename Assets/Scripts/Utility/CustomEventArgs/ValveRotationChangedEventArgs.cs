using System;
using UnityEngine;

namespace System
{
    public class ValveRotationChangedEventArgs : EventArgs
    {
        public float ValveRotation;
        public GameObject Valve;
    }
}
