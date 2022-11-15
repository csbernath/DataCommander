﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Foundation.Assertions;
using Foundation.Collections;

namespace Foundation.Configuration;

[DebuggerDisplay("Name = {Name}, Value = {Value}, Description = {Description}")]
public sealed class ConfigurationAttribute
{
    public ConfigurationAttribute(string name, object value, string description)
    {
        Name = name;
        Value = value;
        Description = description;
    }

    public string Name { get; }
    public object Value { get; set; }
    public string Description { get; }

    public T GetValue<T>()
    {
        Assert.IsInRange((Value == null && typeof(T).IsClass) || Value is T);

        var value = (T) Value;
        return value;
    }

    public ConfigurationAttribute Clone()
    {
        var clone = new ConfigurationAttribute(Name, Value, Description);
        return clone;
    }

    public void Write(TextWriter textWriter)
    {
        string typeName;
        Type type = null;

        if (Value != null)
        {
            type = Value.GetType();
            typeName = TypeNameCollection.GetTypeName(type);
        }
        else
        {
            typeName = "object";
        }

        textWriter.Write("  " + typeName + " " + Name + " = ");

        if (type != null)
        {
            if (type.IsArray)
            {
                var elementType = type.GetElementType();

                if (elementType == typeof(byte))
                {
                    var inArray = (byte[]) Value;
                    var base64 = System.Convert.ToBase64String(inArray);
                    textWriter.WriteLine(base64);
                }
                else
                {
                    var array = (Array) Value;

                    if (array.Length > 0)
                    {
                        textWriter.WriteLine();

                        var index = 0;

                        foreach (var arrayItem in array)
                        {
                            textWriter.Write("    [" + index + "] = ");
                            Write(arrayItem, textWriter);
                            index++;
                        }
                    }
                }
            }
            else
            {
                Write(Value, textWriter);
            }
        }
        else
        {
            Write(Value, textWriter);
        }
    }

    //[ContractVerification(false)]
    private static void Write(object attributeValue, TextWriter textWriter)
    {
        string attibuteValueString;

        if (attributeValue != null)
        {
            var type = attributeValue.GetType();
            var typeCode = Type.GetTypeCode(type);

            switch (typeCode)
            {
                case TypeCode.String:
                    attibuteValueString = "\"" + attributeValue + "\"";
                    break;

                case TypeCode.Object:
                {
                    if (type == typeof(TimeSpan))
                        attibuteValueString = attributeValue.ToString();
                    else
                    {
                        if (attributeValue is XmlNode xmlNode)
                            attibuteValueString = xmlNode.OuterXml;
                        else
                        {
                            xmlNode = XmlHelper.Serialize(attributeValue);
                            attibuteValueString = xmlNode.OuterXml;
                        }
                    }
                }
                    break;

                default:
                    attibuteValueString = attributeValue.ToString();
                    break;
            }
        }
        else
            attibuteValueString = "null";

        textWriter.WriteLine(attibuteValueString);
    }

    private static class XmlHelper
    {
        public static XmlElement Serialize(object obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            var type = obj.GetType();
            var xmlSerializer = new XmlSerializer(type);
            var stringWriter = new StringWriter(CultureInfo.InvariantCulture);
            xmlSerializer.Serialize(stringWriter, obj);

            var sb = stringWriter.GetStringBuilder();
            var s = sb.ToString();

            var xmlDocument = new XmlDocument();
            xmlDocument.InnerXml = s;
            var xmlElement = xmlDocument.DocumentElement;

            // removes unnecessary attributes generated by XmlSerializer
            xmlElement.Attributes.RemoveAll();

            return xmlElement;
        }
    }
}