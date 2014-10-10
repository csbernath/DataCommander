namespace DataCommander
{

    internal sealed class DecimalString
	{
		public DecimalString( string str )
		{
			//string separator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
			const char separator = '.';
			int index = str.IndexOf( separator );

			string intValue;

			if (index >= 0)
			{
				intValue = str.Substring( 0, index );
				string fracValue = str.Substring( index + 1 );
				scale = (byte) fracValue.Length;
			}
			else
			{
				intValue = str;
				scale = 0;
			}

			precision = (byte) (intValue.Length + scale);
		}

		public byte Precision
		{
			get
			{
				return precision;
			}
		}

		public byte Scale
		{
			get
			{
				return scale;
			}
		}

		private byte precision;
		private byte scale;
	}
}