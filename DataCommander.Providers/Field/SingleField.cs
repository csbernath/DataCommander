namespace DataCommander.Providers
{
    using System;

    public sealed class SingleField
    {
        public SingleField(Single value)
        {
            this.Value = value;
        }

        public Single Value { get; }

        public override string ToString()
        {
            return this.Value.ToString("N16");
        }
    }
}
