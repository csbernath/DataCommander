namespace DataCommander.Providers
{
    using System;

    public sealed class SingleField
    {
        private readonly Single value;

        public SingleField(Single value)
        {
            this.value = value;
        }

        public Single Value => this.value;

        public override string ToString()
        {
            return this.value.ToString("N16");
        }
    }
}
