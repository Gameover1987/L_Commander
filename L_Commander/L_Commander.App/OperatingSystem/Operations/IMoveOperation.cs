﻿using System;

namespace L_Commander.App.OperatingSystem.Operations
{
    public interface IMoveOperation : IFileSystemOperation
    {
        public event EventHandler<CopyProgressEventArgs> ActiveItemsProgress;

        void Initialize(FileSystemEntryDescriptor[] entries, string destDirectory);
    }
}