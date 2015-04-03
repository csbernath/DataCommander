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

        public int StartIndex
        {
            get
            {
                return this.startIndex;
            }
        }

        public int Length
        {
            get
            {
                return this.length;
            }
        }

        public IObjectName ObjectName
        {
            get
            {
                return this.objectName;
            }
        }
    }
}