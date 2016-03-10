namespace DataCommander.Providers
{
    using System;

    internal sealed class ItemSelectedEventArgs : EventArgs
    {
        private readonly int startIndex;
        private readonly int length;
        private readonly IObjectName objectName;

        public ItemSelectedEventArgs(int startIndex, int length, IObjectName objectName)
        {
            this.startIndex = startIndex;
            this.length = length;
            this.objectName = objectName;
        }

        public int StartIndex => this.startIndex;

        public int Length => this.length;

        public IObjectName ObjectName => this.objectName;
    }
}