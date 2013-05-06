﻿using System;

namespace Morpheus
{
    public class StatusEventArgs : EventArgs
    {
        public string Status { get; private set; }

        public StatusEventArgs(string status)
        {
            Status = status;
        }
    }
}
