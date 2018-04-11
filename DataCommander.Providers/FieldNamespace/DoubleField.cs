using System.Globalization;

namespace DataCommander.Providers.FieldNamespace
{
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