namespace DataCommander.Foundation.Data.PTypes
{
    using System;
    using System.Data.SqlTypes;

    /// <summary>
    /// 
    /// </summary>
    public static class PConvert
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlBoolean"></param>
        /// <returns></returns>
        public static PBoolean FromSqlBoolean( object sqlBoolean )
        {
            PBoolean sp;

            if (sqlBoolean == null)
            {
                sp = PBoolean.Empty;
            }
            else
            {
                var sql = (SqlBoolean) sqlBoolean;
                sp = sql;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlDateTime"></param>
        /// <returns></returns>
        public static PDateTime FromSqlDateTime( object sqlDateTime )
        {
            PDateTime sp;

            if (sqlDateTime == null)
            {
                sp = PDateTime.Empty;
            }
            else
            {
                var sql = (SqlDateTime) sqlDateTime;
                sp = sql;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlDecimal"></param>
        /// <returns></returns>
        public static PDecimal FromSqlDecimal( object sqlDecimal )
        {
            PDecimal sp;

            if (sqlDecimal == null)
            {
                sp = PDecimal.Empty;
            }
            else
            {
                var sql = (SqlDecimal) sqlDecimal;
                sp = sql;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlDouble"></param>
        /// <returns></returns>
        public static PDouble FromSqlDouble( object sqlDouble )
        {
            PDouble sp;

            if (sqlDouble == null)
            {
                sp = PDouble.Empty;
            }
            else
            {
                var sql = (SqlDouble) sqlDouble;
                sp = sql;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PInt16 FromSqlInt16( object value )
        {
            PInt16 sp;

            if (value == null)
            {
                sp = PInt16.Empty;
            }
            else
            {
                var sql = (SqlInt16) value;
                sp = sql;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PInt32 FromSqlInt32( object value )
        {
            PInt32 sp;

            if (value == null)
            {
                sp = PInt32.Empty;
            }
            else
            {
                var sql = (SqlInt32) value;
                sp = sql;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlMoney"></param>
        /// <returns></returns>
        public static PMoney FromSqlMoney( object sqlMoney )
        {
            PMoney sp;

            if (sqlMoney == null)
            {
                sp = PMoney.Empty;
            }
            else
            {
                var sql = (SqlMoney) sqlMoney;
                sp = sql;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public static PString FromSqlString( object sqlString )
        {
            PString sp;

            if (sqlString == null)
            {
                sp = PString.Empty;
            }
            else
            {
                var sql = (SqlString) sqlString;
                sp = sql;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static PBoolean ScalarToPBoolean( object scalar )
        {
            PBoolean sp;

            if (scalar == null)
            {
                sp = PBoolean.Empty;
            }
            else if (scalar == DBNull.Value)
            {
                sp = PBoolean.Null;
            }
            else
            {
                sp = (bool) scalar;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static PDateTime ScalarToPDateTime( object scalar )
        {
            PDateTime sp;

            if (scalar == null)
            {
                sp = PDateTime.Empty;
            }
            else if (scalar == DBNull.Value)
            {
                sp = PDateTime.Null;
            }
            else
            {
                sp = (DateTime) scalar;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static PDecimal ScalarToPDecimal( object scalar )
        {
            PDecimal sp;

            if (scalar == null)
            {
                sp = PDecimal.Empty;
            }
            else if (scalar == DBNull.Value)
            {
                sp = PDecimal.Null;
            }
            else
            {
                sp = (decimal) scalar;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static PDouble ScalarToPDouble( object scalar )
        {
            PDouble sp;

            if (scalar == null)
            {
                sp = PDouble.Empty;
            }
            else if (scalar == DBNull.Value)
            {
                sp = PDouble.Null;
            }
            else
            {
                sp = (double) scalar;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static PInt16 ScalarToPInt16( object scalar )
        {
            PInt16 sp;

            if (scalar == null)
            {
                sp = PInt16.Empty;
            }
            else if (scalar == DBNull.Value)
            {
                sp = PInt16.Null;
            }
            else
            {
                sp = (short) scalar;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static PInt32 ScalarToPInt32( object scalar )
        {
            PInt32 sp;

            if (scalar == null)
            {
                sp = PInt32.Empty;
            }
            else if (scalar == DBNull.Value)
            {
                sp = PInt32.Null;
            }
            else
            {
                sp = (int) scalar;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static PMoney ScalarToPMoney( object scalar )
        {
            PMoney sp;

            if (scalar == null)
            {
                sp = PMoney.Empty;
            }
            else if (scalar == DBNull.Value)
            {
                sp = PMoney.Null;
            }
            else
            {
                sp = (decimal) scalar;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static PString ScalarToPString( object scalar )
        {
            PString sp;

            if (scalar == null)
            {
                sp = PString.Empty;
            }
            else if (scalar == DBNull.Value)
            {
                sp = PString.Null;
            }
            else
            {
                sp = (string) scalar;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static PXml ScalarToPXml( object scalar )
        {
            PXml xml;

            if (scalar == null)
            {
                xml = PXml.Empty;
            }
            else if (scalar == DBNull.Value)
            {
                xml = PXml.Null;
            }
            else
            {
                xml = (SqlXml) scalar;
            }

            return xml;
        }
    }
}