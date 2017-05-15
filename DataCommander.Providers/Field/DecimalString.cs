namespace DataCommander.Providers.Field
{

    internal sealed class DecimalString
	{
		public DecimalString( string str )
		{
			//string separator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
			const char separator = '.';
			var index = str.IndexOf( separator );

			string intValue;

			if (index >= 0)
			{
				intValue = str.Substring( 0, index );
				var fracValue = str.Substring( index + 1 );
			    this.Scale = (byte) fracValue.Length;
			}
			else
			{
				intValue = str;
			    this.Scale = 0;
			}

		    this.Precision = (byte) (intValue.Length + this.Scale);
		}

		public byte Precision { get; }

        public byte Scale { get; }
	}
}