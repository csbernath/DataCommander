namespace DataCommander.Providers.Field
{
    public sealed class BooleanField
    {
		private readonly bool value;

        public BooleanField(bool value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            int int32value;

            if (this.value)
            {
                int32value = 1;
            }
            else
            {
                int32value = 0;
            }

            return int32value.ToString();
        }
    }
}