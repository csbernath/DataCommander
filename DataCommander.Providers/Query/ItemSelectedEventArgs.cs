namespace DataCommander.Providers
{
    using System;

    internal sealed class ItemSelectedEventArgs : EventArgs
    {
        public ItemSelectedEventArgs(int startIndex, int length, IObjectName objectName)
        {
            this.StartIndex = startIndex;
            this.Length = length;
            this.ObjectName = objectName;
        }

        public int StartIndex { get; }

        public int Length { get; }

        public IObjectName ObjectName { get; }
    }
}