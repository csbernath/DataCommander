namespace DataCommander.Providers.Field
{
    public sealed class SingleField
    {
        public SingleField(float value)
        {
            this.Value = value;
        }

        public float Value { get; }

        public override string ToString()
        {
            return this.Value.ToString("N16");
        }
    }
}
