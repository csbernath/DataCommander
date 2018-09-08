using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using Foundation.Assertions;

namespace Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public delegate int GetValues(object[] values);

    /// <summary>
    /// 
    /// </summary>
    public class MappedDataReader : DbDataReader
    {
        private readonly IDataReader _dataReader;
        private readonly GetValues _getValues;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="getValues"></param>
        public MappedDataReader(IDataReader dataReader, GetValues getValues)
        {
            Assert.IsNotNull(dataReader);
            Assert.IsNotNull(getValues);

            _dataReader = dataReader;
            _getValues = getValues;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Close()
        {
            _dataReader.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public override int Depth => _dataReader.Depth;

        /// <summary>
        /// 
        /// </summary>
        public override int FieldCount => _dataReader.FieldCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override bool GetBoolean(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override byte GetByte(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <param name="dataOffset"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferOffset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override char GetChar(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <param name="dataOffset"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferOffset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override string GetDataTypeName(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override DateTime GetDateTime(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override decimal GetDecimal(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override double GetDouble(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Type GetFieldType(int ordinal)
        {
            return _dataReader.GetFieldType(ordinal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override float GetFloat(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Guid GetGuid(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override short GetInt16(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override int GetInt32(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override long GetInt64(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override string GetName(int ordinal)
        {
            return _dataReader.GetName(ordinal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override int GetOrdinal(string name)
        {
            return _dataReader.GetOrdinal(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DataTable GetSchemaTable()
        {
            return _dataReader.GetSchemaTable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override string GetString(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override object GetValue(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public override int GetValues(object[] values)
        {
            return _getValues(values);
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool HasRows => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        public override bool IsClosed => _dataReader.IsClosed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override bool IsDBNull(int ordinal)
        {
            return _dataReader.IsDBNull(ordinal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool NextResult()
        {
            return _dataReader.NextResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Read()
        {
            return _dataReader.Read();
        }

        /// <summary>
        /// 
        /// </summary>
        public override int RecordsAffected => _dataReader.RecordsAffected;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object this[string name] => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override object this[int ordinal] => throw new NotImplementedException();
    }
}