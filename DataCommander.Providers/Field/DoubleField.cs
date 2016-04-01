namespace DataCommander.Providers
{
    using System.Globalization;

    public sealed class DoubleField
	{
		public DoubleField( double value )
		{
			this.Value = value;
		}

		public double Value { get; }

        public override string ToString()
		{
			return this.Value.ToString( "N16", CultureInfo.InvariantCulture );
		}
	}
}