using System;
using System.Data;

namespace DataCommander.Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class DataParameterExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static T GetValueOrDefault<T>(this IDataParameter parameter)
        {
            return Database.GetValueOrDefault<T>(parameter.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        public static void SetValue<T>( this IDataParameter parameter, DataParameterValue<T> value )
        {
#if CONTRACTS_FULL
            Contract.Requires( parameter != null );
            Contract.Requires( value.Type == DataParameterValueType.Value || value.Type == DataParameterValueType.Null || value.Type == DataParameterValueType.Default );
#endif

            object valueObject;

            switch (value.Type)
            {
                case DataParameterValueType.Value:
                    valueObject = value.Value;
                    break;

                case DataParameterValueType.Null:
                    valueObject = DBNull.Value;
                    break;

                case DataParameterValueType.Default:
                    valueObject = null;
                    break;

                default:
                    throw new ArgumentException();
            }

            parameter.Value = valueObject;
        }
    }
}