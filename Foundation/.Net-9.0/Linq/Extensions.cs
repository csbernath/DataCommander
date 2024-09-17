using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Foundation.Linq;

public static class Extensions
{
    public static bool IfAsNotNull<TSource, TTarget>(this TSource source, Action<TTarget> action) where TTarget : class
    {
        TTarget target = source as TTarget;
        bool selected = target != null;
        if (selected)
            action(target);

        return selected;
    }

    public static bool In<T>(this T item, params T[] collection)
    {
        ArgumentNullException.ThrowIfNull(collection, nameof(collection));
        return collection.Contains(item);
    }

    public static T[] ItemToArray<T>(this T item) => [item];
    public static IEnumerable<T> ItemAsEnumerable<T>(this T item) => item.ItemToArray();

    public static string ToLogXmlString(this object source)
    {
        string logXmlString;
        if (source != null)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(source.GetType());

                using (StringWriter stringWriter = new StringWriter())
                {
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter)
                    {
                        Formatting = Formatting.Indented,
                        Indentation = 2,
                        IndentChar = ' '
                    };
                    xmlSerializer.Serialize(xmlTextWriter, source);
                    logXmlString = stringWriter.ToString();
                }
            }
            catch (Exception e)
            {
                logXmlString = e.ToString();
            }
        }
        else
            logXmlString = null;

        return logXmlString;
    }

    public static bool ReferenceEquals<T>(this T objA, T objB) where T : class => object.ReferenceEquals(objA, objB);
}