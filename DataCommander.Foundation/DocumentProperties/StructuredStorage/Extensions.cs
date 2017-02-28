namespace DataCommander.Foundation.DocumentProperties
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    internal static class Extensions
    {
        public static IEnumerable<STATPROPSETSTG> AsEnumerable( this IPropertySetStorage propertySetStorage )
        {
            Contract.Requires(propertySetStorage != null);

            IEnumSTATPROPSETSTG enumStatPropSetStg;
            propertySetStorage.Enum( out enumStatPropSetStg );

            while (true)
            {
                var statPropSetStgArray = new STATPROPSETSTG[ 1 ];
                UInt32 fetched;
                enumStatPropSetStg.Next( 1, statPropSetStgArray, out fetched );

                if (fetched == 0)
                {
                    break;
                }

                yield return statPropSetStgArray[ 0 ];
            }
        }

        internal static IEnumerable<STATPROPSTG> AsEnumerable( this IPropertyStorage propertyStorage )
        {
            Contract.Requires(propertyStorage != null);

            IEnumSTATPROPSTG enumStatPropStg;
            propertyStorage.Enum( out enumStatPropStg );

            while (true)
            {
                var statPropStgArray = new STATPROPSTG[ 1 ];
                UInt32 fetched;
                enumStatPropStg.Next( 1, statPropStgArray, out fetched );

                if (fetched == 0)
                {
                    break;
                }

                yield return statPropStgArray[ 0 ];
            }
        }
    }
}