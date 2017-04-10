namespace DataCommander.Foundation.DocumentProperties
{
    using System.Collections.Generic;

    internal static class Extensions
    {
        public static IEnumerable<STATPROPSETSTG> AsEnumerable( this IPropertySetStorage propertySetStorage )
        {
#if CONTRACTS_FULL
            Contract.Requires(propertySetStorage != null);
#endif

            IEnumSTATPROPSETSTG enumStatPropSetStg;
            propertySetStorage.Enum( out enumStatPropSetStg );

            while (true)
            {
                var statPropSetStgArray = new STATPROPSETSTG[ 1 ];
                uint fetched;
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
#if CONTRACTS_FULL
            Contract.Requires(propertyStorage != null);
#endif

            IEnumSTATPROPSTG enumStatPropStg;
            propertyStorage.Enum( out enumStatPropStg );

            while (true)
            {
                var statPropStgArray = new STATPROPSTG[ 1 ];
                uint fetched;
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