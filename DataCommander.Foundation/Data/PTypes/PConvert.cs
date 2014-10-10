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
        public static PBoolean FromSqlBoolean( Object sqlBoolean )
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
        public static PDateTime FromSqlDateTime( Object sqlDateTime )
        {
            PDateTime sp;

            if (sqlDateTime == null)
            {
                sp = PDateTime.Empty;
            }
            else
            {
                SqlDateTime sql = (SqlDateTime) sqlDateTime;
                sp = sql;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlDecimal"></param>
        /// <returns></returns>
        public static PDecimal FromSqlDecimal( Object sqlDecimal )
        {
            PDecimal sp;

            if (sqlDecimal == null)
            {
                sp = PDecimal.Empty;
            }
            else
            {
                SqlDecimal sql = (SqlDecimal) sqlDecimal;
                sp = sql;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlDouble"></param>
        /// <returns></returns>
        public static PDouble FromSqlDouble( Object sqlDouble )
        {
            PDouble sp;

            if (sqlDouble == null)
            {
                sp = PDouble.Empty;
            }
            else
            {
                SqlDouble sql = (SqlDouble) sqlDouble;
                sp = sql;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PInt16 FromSqlInt16( Object value )
        {
            PInt16 sp;

            if (value == null)
            {
                sp = PInt16.Empty;
            }
            else
            {
                SqlInt16 sql = (SqlInt16) value;
                sp = sql;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PInt32 FromSqlInt32( Object value )
        {
            PInt32 sp;

            if (value == null)
            {
                sp = PInt32.Empty;
            }
            else
            {
                SqlInt32 sql = (SqlInt32) value;
                sp = sql;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlMoney"></param>
        /// <returns></returns>
        public static PMoney FromSqlMoney( Object sqlMoney )
        {
            PMoney sp;

            if (sqlMoney == null)
            {
                sp = PMoney.Empty;
            }
            else
            {
                SqlMoney sql = (SqlMoney) sqlMoney;
                sp = sql;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public static PString FromSqlString( Object sqlString )
        {
            PString sp;

            if (sqlString == null)
            {
                sp = PString.Empty;
            }
            else
            {
                SqlString sql = (SqlString) sqlString;
                sp = sql;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static PBoolean ScalarToPBoolean( Object scalar )
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
                sp = (Boolean) scalar;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static PDateTime ScalarToPDateTime( Object scalar )
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
        public static PDecimal ScalarToPDecimal( Object scalar )
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
                sp = (Decimal) scalar;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static PDouble ScalarToPDouble( Object scalar )
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
                sp = (Double) scalar;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static PInt16 ScalarToPInt16( Object scalar )
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
                sp = (Int16) scalar;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static PInt32 ScalarToPInt32( Object scalar )
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
                sp = (Int32) scalar;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static PMoney ScalarToPMoney( Object scalar )
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
                sp = (Decimal) scalar;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static PString ScalarToPString( Object scalar )
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
                sp = (String) scalar;
            }

            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static PXml ScalarToPXml( Object scalar )
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