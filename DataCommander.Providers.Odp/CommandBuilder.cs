namespace DataCommander.Providers.Odp
{
    using System;
    using System.Data;
    using DataCommander.Foundation.Data;
    using Oracle.ManagedDataAccess.Client;

    /// <exclude/>
	/// <summary>
	/// Summary description for CommandBuilder.
	/// </summary>
	public static class CommandBuilder
	{
		public static void DeriveParameters(
			OracleConnection connection,
			string owner,
			string packageName,
			string objectName,
			string overload,
			OracleParameterCollection parameters )
		{
			if (overload == null)
			{
				overload = "is null";
			}
			else
			{
				overload = "= '" + overload + "'";
			}

			string commandText = @"select	argument_name,
	data_type,
	default_value,
	in_out,
	data_length,
	data_precision,
	data_scale
from	all_arguments a
where	owner			= '{0}'
	and package_name	= '{1}'
	and object_name		= '{2}'
    and overload		{3}
	--and data_level = 0
order by overload,position";
			commandText = string.Format( commandText, owner, packageName, objectName, overload );
			OracleCommand command = new OracleCommand( commandText, connection );

			using (OracleDataReader dataReader = command.ExecuteReader())
			{
				bool first = true;

				while (dataReader.Read())
				{
					string argumentName = dataReader.GetValueOrDefault<string>( "ARGUMENT_NAME" );
					string dataType = dataReader.GetValue<string>( "DATA_TYPE" );
					string inOut = dataReader.GetValue<string>( "IN_OUT" );
					decimal? dataLength = dataReader.GetValueOrDefault<decimal?>( "DATA_LENGTH" );
					decimal? precision = dataReader.GetValueOrDefault<decimal?>( "DATA_PRECISION" );
					decimal? scale = dataReader.GetValueOrDefault<decimal?>( "DATA_SCALE" );
					ParameterDirection direction;

					switch (inOut)
					{
						case "IN":
							direction = ParameterDirection.Input;
							break;

						case "OUT":
							if (first)
							{
								direction = ParameterDirection.ReturnValue;
							}
							else
							{
								direction = ParameterDirection.Output;
							}

							break;

						case "IN/OUT":
							direction = ParameterDirection.InputOutput;
							break;

						default:
							throw new NotImplementedException();
					}

					OracleDbType dbType;

					switch (dataType)
					{
						case "CHAR":
							dbType = OracleDbType.Char;
							break;

						case "NUMBER":
							dbType = OracleDbType.Decimal;

							if (precision == null && scale == null)
							{
								precision = 38;
								scale = 127;
							}

							break;

						case "VARCHAR2":
							dbType = OracleDbType.Varchar2;
							break;

						case "PL/SQL TABLE":
							dbType = OracleDbType.RefCursor;
							break;

						case "BINARY_INTEGER":
							//            dbType = OracleDbType.Decimal;
							//
							//            if (precision == 0 && scale == 0)
							//            {
							//              precision = 38;
							//              scale = 0;
							//            }
							//
							//            length = dataReader.IsDBNull(3) ? 22 : Convert.ToInt32(dataReader[3]);
							dbType = OracleDbType.Int64;
							break;

						case "RAW":
							dbType = OracleDbType.Raw;
							break;

						//case "OBJECT":
						//    dbType = OracleDbType.
						//    break;

						default:
							throw new NotImplementedException();
					}

					OracleParameter parameter = new OracleParameter();
					parameter.ParameterName = argumentName;
					parameter.Direction = direction;
					parameter.OracleDbType = dbType;
					parameter.Size = (int) dataLength.GetValueOrDefault();
					parameter.Precision = (byte) precision.GetValueOrDefault();
					parameter.Scale = (byte) scale.GetValueOrDefault();
					parameters.Add( parameter );

					if (first)
					{
						first = false;
					}
				}
			}
		}
	}
}