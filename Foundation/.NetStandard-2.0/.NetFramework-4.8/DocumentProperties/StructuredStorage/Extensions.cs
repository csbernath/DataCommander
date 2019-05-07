using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.DocumentProperties.StructuredStorage
{
    internal static class Extensions
    {
        public static IEnumerable<STATPROPSETSTG> AsEnumerable( this IPropertySetStorage propertySetStorage )
        {
            Assert.IsTrue(propertySetStorage != null);

            propertySetStorage.Enum( out var enumStatPropSetStg );

            while (true)
            {
                var statPropSetStgArray = new STATPROPSETSTG[ 1 ];
                enumStatPropSetStg.Next( 1, statPropSetStgArray, out var fetched );

                if (fetched == 0)
                {
                    break;
                }

                yield return statPropSetStgArray[ 0 ];
            }
        }

        internal static IEnumerable<STATPROPSTG> AsEnumerable( this IPropertyStorage propertyStorage )
        {
            Assert.IsNotNull(propertyStorage);

            propertyStorage.Enum( out var enumStatPropStg );

            while (true)
            {
                var statPropStgArray = new STATPROPSTG[ 1 ];
                enumStatPropStg.Next( 1, statPropStgArray, out var fetched );

                if (fetched == 0)
                {
                    break;
                }

                yield return statPropStgArray[ 0 ];
            }
        }
    }
}