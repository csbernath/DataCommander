namespace DataCommander.Providers.FieldNamespace
{
    public sealed class BooleanField
    {
		private readonly bool _value;

        public BooleanField(bool value)
        {
            _value = value;
        }

        public override string ToString()
        {
            int int32Value;

            if (_value)
            {
                int32Value = 1;
            }
            else
            {
                int32Value = 0;
            }

            return int32Value.ToString();
        }
    }
}