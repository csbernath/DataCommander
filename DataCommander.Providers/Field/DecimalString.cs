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
			    this.scale = (byte) fracValue.Length;
			}
			else
			{
				intValue = str;
			    this.scale = 0;
			}

		    this.precision = (byte) (intValue.Length + this.scale);
		}

		public byte Precision => this.precision;

        public byte Scale => this.scale;

        private readonly byte precision;
		private readonly byte scale;
	}
}