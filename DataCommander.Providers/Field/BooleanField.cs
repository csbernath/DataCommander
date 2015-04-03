namespace DataCommander.Providers
{
    using System;

    public sealed class BooleanField
    {
		private readonly bool value;

        public BooleanField(Boolean value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            Int32 int32value;

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