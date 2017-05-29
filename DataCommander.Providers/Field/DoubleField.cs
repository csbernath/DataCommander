namespace DataCommander.Providers.Field
{
    using System.Globalization;

    public sealed class DoubleField
	{
		public DoubleField( double value )
		{
			Value = value;
		}

		public double Value { get; }

        public override string ToString()
		{
			return Value.ToString( "N16", CultureInfo.InvariantCulture );
		}
	}
}