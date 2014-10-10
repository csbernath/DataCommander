namespace DataCommander.Foundation.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using DataCommander.Foundation.Xml;

    /// <summary>
    /// 
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T[] ItemToArray<T>( this T item )
        {
            return new[] { item };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<T> ItemAsEnumerable<T>( this T item )
        {
            return item.ItemToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static Boolean In<T>( this T item, params T[] collection )
        {
            Contract.Requires<ArgumentNullException>( collection != null );
            return collection.Contains( item );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="t"></param>
        /// <param name="getValue"></param>
        /// <returns></returns>
        public static TResult GetValueOrDefault<T, TResult>( this T t, Func<T, TResult> getValue ) where T : class
        {
            TResult result;
            if (t != null)
            {
                result = getValue( t );
            }
            else
            {
                result = default( TResult );
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToLogXmlString( this Object source )
        {
            string s;

            if (source != null)
            {
                try
                {
                    var xmlSerializer = new XmlSerializer( source.GetType() );

                    using (var stringWriter = new StringWriter())
                    {
                        var xmlTextWriter =
                            new XmlTextWriter( stringWriter )
                            {
                                Formatting = Formatting.Indented,
                                Indentation = 2,
                                IndentChar = ' '
                            };

                        xmlSerializer.Serialize( xmlTextWriter, source );

                        s = stringWriter.ToString();
                    }
                }
                catch (Exception e)
                {
                    s = e.ToString();
                }
            }
            else
            {
                s = null;
            }

            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static String SerializeToXmlString( this Object source )
        {
            Type type = source.GetType();
            var xmlSerializer = new XmlSerializer( type );

            var settings =
                new XmlWriterSettings
                {
                    OmitXmlDeclaration = true
                };

            return xmlSerializer.SerializeToXmlString( settings, source );
        }
    }
}