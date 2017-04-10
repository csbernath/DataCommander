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

			const string format = @"select	argument_name,
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
		    var commandText = string.Format(format, owner, packageName, objectName, overload);
		    var command = new OracleCommand(commandText, connection);

			using (var dataReader = command.ExecuteReader())
			{
				var first = true;

				while (dataReader.Read())
				{
					var argumentName = dataReader.GetStringOrDefault(0);
					var dataType = dataReader.GetString(1);
				    var inOut = dataReader.GetString(2);
					var dataLength = dataReader.GetNullableDecimal(3);
				    var precision = dataReader.GetNullableDecimal(4);
				    var scale = dataReader.GetNullableDecimal(5);
				    ParameterDirection direction;

					switch (inOut)
					{
						case "IN":
							direction = ParameterDirection.Input;
							break;

						case "OUT":
							direction = first ? ParameterDirection.ReturnValue : ParameterDirection.Output;
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

				    var parameter = new OracleParameter
				    {
				        ParameterName = argumentName,
				        Direction = direction,
				        OracleDbType = dbType,
				        Size = (int)dataLength.GetValueOrDefault(),
				        Precision = (byte)precision.GetValueOrDefault(),
				        Scale = (byte)scale.GetValueOrDefault()
				    };

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