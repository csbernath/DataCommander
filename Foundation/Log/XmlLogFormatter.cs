using System.IO;
using System.Xml;

namespace Foundation.Log
{
    internal sealed class XmlLogFormatter : ILogFormatter
    {
        private static void WriteElement(
            XmlWriter xmlWriter,
            string name,
            string value)
        {
            if (value != null)
            {
                xmlWriter.WriteElementString(name, value);
            }
        }

        private static void WriteTo(
            LogEntry entry,
            XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("logEntry");

            xmlWriter.WriteElementString("creationTime", XmlConvert.ToString(entry.CreationTime, XmlDateTimeSerializationMode.Local));
            xmlWriter.WriteElementString("logLevel", entry.LogLevel.ToString());
            WriteElement(xmlWriter, "hostName", entry.HostName);
            WriteElement(xmlWriter, "userName", entry.UserName);
            //TODO
            //MethodBase method = entry.Method;
            //Type type = method.DeclaringType;
            //string typeName = type.FullName;
            //xmlWriter.WriteElementString("type", typeName);
            //xmlWriter.WriteElementString("method", entry.Method.ToString());
            //xmlWriter.WriteElementString("message", entry.Message);

            //xmlWriter.WriteEndElement();
            //xmlWriter.WriteRaw(Environment.NewLine);
        }

        string ILogFormatter.Begin()
        {
            return "<logEntries>\r\n";
        }

        string ILogFormatter.Format(LogEntry entry)
        {
            var textWriter = new StringWriter();
            var xmlTextWriter = new XmlTextWriter(textWriter) { Formatting = Formatting.Indented, Indentation = 2, IndentChar = ' ' };
            WriteTo(entry, xmlTextWriter);
            return textWriter.ToString();
        }

        string ILogFormatter.End()
        {
            return "</logEntries>\r\n";
        }
    }
}