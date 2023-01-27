using System;

namespace L_Commander.App.OperatingSystem.Operations
{
    public class OperationProgressEventArgs : EventArgs
    {
        public long Processed { get; set; }

        public long Total { get; set; }

        public string CurrentItemName { get; set; }

    }
}
