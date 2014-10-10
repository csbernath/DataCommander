namespace DataCommander.Providers
{
	using System.Globalization;

	public sealed class DoubleField
	{
		public DoubleField( double value )
		{
			this.value = value;
		}

		public double Value
		{
			get
			{
				return value;
			}
		}

		public override string ToString()
		{
			return value.ToString( "N16", CultureInfo.InvariantCulture );
		}

		private double value;
	}
}