using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Foundation.Xml;

namespace Foundation.Linq
{
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
        public static T[] ItemToArray<T>(this T item)
        {
            return new[] {item};
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<T> ItemAsEnumerable<T>(this T item)
        {
            return item.ItemToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void IfArgumentIs<TSource, TTarget>(this TSource source, Action<TTarget> action) where TTarget : class
        {
            if (source is TTarget)
            {
                var target = source as TTarget;
                action(target);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static bool IfAsNotNull<TSource, TTarget>(this TSource source, Action<TTarget> action) where TTarget : class
        {
            var target = source as TTarget;
            var selected = target != null;
            if (selected)
            {
                action(target);
            }
            return selected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool In<T>(this T item, params T[] collection)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(collection != null);
#endif
            return collection.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objA"></param>
        /// <param name="objB"></param>
        /// <returns></returns>
        public static bool ReferenceEquals<T>(this T objA, T objB) where T : class
        {
            return object.ReferenceEquals(objA, objB);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToLogXmlString(this object source)
        {
            string s;

            if (source != null)
            {
                try
                {
                    var xmlSerializer = new XmlSerializer(source.GetType());

                    using (var stringWriter = new StringWriter())
                    {
                        var xmlTextWriter =
                            new XmlTextWriter(stringWriter)
                            {
                                Formatting = Formatting.Indented,
                                Indentation = 2,
                                IndentChar = ' '
                            };

                        xmlSerializer.Serialize(xmlTextWriter, source);

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
        public static string SerializeToXmlString(this object source)
        {
            var type = source.GetType();
            var xmlSerializer = new XmlSerializer(type);

            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true
            };

            return xmlSerializer.SerializeToXmlString(settings, source);
        }
    }
}